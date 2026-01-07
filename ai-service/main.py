from fastapi import FastAPI, File, UploadFile
from fastapi.responses import JSONResponse
import numpy as np
import cv2
import re
import easyocr

app = FastAPI(title="Zootopia OCR Service")
reader = easyocr.Reader(["vi", "en"], gpu=False)

def preprocess(img_bgr: np.ndarray) -> np.ndarray:
    gray = cv2.cvtColor(img_bgr, cv2.COLOR_BGR2GRAY)
    gray = cv2.bilateralFilter(gray, 9, 75, 75)
    thr = cv2.adaptiveThreshold(gray, 255, cv2.ADAPTIVE_THRESH_GAUSSIAN_C,
                                cv2.THRESH_BINARY, 31, 7)
    return thr

def extract_fields(raw: str) -> dict:
    text = re.sub(r"\s+", " ", raw).strip()

    m_id = re.search(r"\b(\d{12}|\d{10}|\d{9})\b", text)
    national_id = m_id.group(1) if m_id else None

    m_dob = re.search(r"\b(\d{1,2}[\/\-]\d{1,2}[\/\-]\d{4})\b", text)
    dob = m_dob.group(1) if m_dob else None

    m_name = re.search(r"(Họ và tên|Họ tên)\s*[:\-]?\s*([A-ZÀ-Ỵ\s]{5,})", text, re.IGNORECASE)
    full_name = m_name.group(2).strip() if m_name else None
    if full_name:
        full_name = re.sub(r"\s{2,}", " ", full_name)

    return {
        "fullName": full_name,
        "nationalId": national_id,
        "dob": dob,
        "addressText": None
    }

@app.post("/extract")
async def extract(file: UploadFile = File(...)):
    if file.content_type not in ["image/jpeg", "image/png", "image/webp"]:
        return JSONResponse(status_code=400, content={"error": "Only jpg/png/webp"})

    data = await file.read()
    arr = np.frombuffer(data, np.uint8)
    img = cv2.imdecode(arr, cv2.IMREAD_COLOR)
    if img is None:
        return JSONResponse(status_code=400, content={"error": "Invalid image"})

    pre = preprocess(img)
    results = reader.readtext(pre)

    texts = [r[1] for r in results]
    confs = [float(r[2]) for r in results] if results else []
    raw_text = " ".join(texts).strip()
    confidence = sum(confs) / len(confs) if confs else 0.0

    fields = extract_fields(raw_text)

    return {
        **fields,
        "confidence": round(confidence, 4),
        "rawText": raw_text,
        "lines": [{"text": r[1], "confidence": float(r[2])} for r in results]
    }
