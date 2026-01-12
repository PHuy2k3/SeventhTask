import { useRef, useState } from "react";
import AppLayout from "../layout/AppLayout";
import { Toast } from "primereact/toast";
import { FileUpload } from "primereact/fileupload";
import { Card } from "primereact/card";
import { InputText } from "primereact/inputtext";
import { Button } from "primereact/button";
import { Divider } from "primereact/divider";

import { extractFromImage } from "../api/ai.api";
import { createCitizen } from "../api/citizens.api";

// ----------------- helpers -----------------
function ddmmyyyyToIso(s) {
  const m = /^(\d{1,2})\/(\d{1,2})\/(\d{4})$/.exec((s || "").trim());
  if (!m) return (s || "").trim();
  const dd = m[1].padStart(2, "0");
  const mm = m[2].padStart(2, "0");
  const yyyy = m[3];
  return `${yyyy}-${mm}-${dd}`;
}

function isLabelValue(v) {
  const s = (v || "").trim().toLowerCase();
  return s === "" || s === "name" || s === "name:" || s === "họ và tên" || s === "ho va ten";
}

function getLineAfter(lines, includesText) {
  if (!Array.isArray(lines)) return "";
  const key = includesText.toLowerCase();
  const i = lines.findIndex((x) => (x.text || "").toLowerCase().includes(key));
  if (i >= 0 && lines[i + 1]?.text) return String(lines[i + 1].text).trim();
  return "";
}

function findLineContains(lines, includesText) {
  if (!Array.isArray(lines)) return "";
  const key = includesText.toLowerCase();
  const hit = lines.find((x) => (x.text || "").toLowerCase().includes(key));
  return hit ? String(hit.text).trim() : "";
}

// Regex lấy value sau nhãn (fallback nếu lines không ổn)
function pickAfterLabel(raw, labelRegex) {
  if (!raw) return "";
  const r = new RegExp(`(?:${labelRegex})\\s*[:：]?\\s*([^\\n\\r]{3,80})`, "i");
  const m = raw.match(r);
  return (m?.[1] || "").trim();
}

function normalizeOriginText(s) {
  return (s || "")
    .replace(/^Place of Origin\s*:\s*/i, "")
    .replace(/^Quê quán\s*:\s*/i, "")
    .replace(/^Que quan\s*:\s*/i, "")
    .trim();
}

// ----------------- component -----------------
export default function OcrUpload() {
  const toast = useRef(null);
  const [loading, setLoading] = useState(false);

  const [ocr, setOcr] = useState(null);
  const [form, setForm] = useState({
    fullName: "",
    nationalId: "",
    dateOfBirth: "",
    addressText: "",
  });

  async function onUploadCustom(e) {
    const file = e.files?.[0];
    if (!file) return;

    setLoading(true);
    try {
      const data = await extractFromImage(file);
      setOcr(data);

      // DEBUG: xem JSON OCR
      console.log("OCR response =", data);

      // --- parse name ---
      // Ưu tiên: dòng sau "Name:" hoặc "Họ và tên"
      const nameFromLines =
        getLineAfter(data.lines, "name:") ||
        getLineAfter(data.lines, "họ và tên") ||
        getLineAfter(data.lines, "ho va ten");

      // Fallback: regex từ rawText
      const raw = data.rawText || "";
      const nameFromRaw = pickAfterLabel(raw, "Họ và tên|Name");

      const bestName = !isLabelValue(data.fullName)
        ? (data.fullName || "").trim()
        : (nameFromLines || nameFromRaw || "").trim();

      // --- parse origin/address ---
      const originLine =
        findLineContains(data.lines, "place of origin") ||
        findLineContains(data.lines, "quê quán") ||
        findLineContains(data.lines, "que quan");

      const originFromLines = normalizeOriginText(originLine);
      const originFromRaw = normalizeOriginText(pickAfterLabel(raw, "Place of Origin|Quê quán|Que quan"));

      const bestAddress =
        (data.addressText ?? "").trim() ||
        originFromLines ||
        originFromRaw ||
        "";

      // --- parse id ---
      // data.nationalId OK rồi, nhưng nếu OCR lỡ trả "FX-202345678" thì có thể giữ nguyên.
      const bestNationalId = (data.nationalId || "").trim();

      // --- dob ---
      const bestDob = ddmmyyyyToIso(data.dob || "");

      setForm({
        fullName: bestName,
        nationalId: bestNationalId,
        dateOfBirth: bestDob,
        addressText: bestAddress,
      });

      toast.current?.show({
        severity: "success",
        summary: "OCR OK",
        detail: "Đã điền dữ liệu vào form. Kiểm tra lại rồi bấm Lưu.",
      });
    } catch (err) {
      toast.current?.show({
        severity: "error",
        summary: "OCR lỗi",
        detail: String(err?.message || err),
        life: 8000,
      });
    } finally {
      setLoading(false);
    }
  }

  async function onSave() {
    setLoading(true);
    try {
      const payload = {
        fullName: (form.fullName || "").trim(),
        nationalId: (form.nationalId || "").trim(),
        // ✅ rỗng thì null, không gửi ""
        dateOfBirth: form.dateOfBirth ? ddmmyyyyToIso(form.dateOfBirth) : null,
        addressText: form.addressText ? form.addressText.trim() : null,
      };

      console.log("POST /api/citizens payload =", payload);

      await createCitizen(payload);

      toast.current?.show({
        severity: "success",
        summary: "Đã lưu",
        detail: "Citizen saved vào SQL Server.",
      });
    } catch (err) {
      const detail = err?.response?.data
        ? JSON.stringify(err.response.data)
        : String(err?.message || err);

      toast.current?.show({
        severity: "error",
        summary: "Lưu lỗi",
        detail,
        life: 10000,
      });
    } finally {
      setLoading(false);
    }
  }

  const canSave =
    !loading &&
    (form.fullName || "").trim().length > 0 &&
    (form.nationalId || "").trim().length > 0;

  return (
    <AppLayout title="OCR Import">
      <Toast ref={toast} />

      <div className="grid">
        <div className="col-12 md:col-6">
          <Card title="1) Upload ảnh">
            <FileUpload
              mode="advanced"
              accept="image/*"
              maxFileSize={10_000_000}
              customUpload
              uploadHandler={onUploadCustom}
              chooseLabel="Chọn ảnh"
              uploadLabel={loading ? "Đang xử lý..." : "Nhận diện"}
              cancelLabel="Huỷ"
              disabled={loading}
            />

            {ocr && (
              <div className="mt-3 text-sm text-500">
                Confidence: <b>{ocr.confidence ?? 0}</b>
              </div>
            )}

            {ocr?.rawText && (
              <div className="mt-3">
                <div className="text-sm text-500 mb-2">Raw OCR</div>
                <pre style={{ whiteSpace: "pre-wrap", margin: 0, fontSize: 12 }}>
                  {ocr.rawText}
                </pre>
              </div>
            )}
          </Card>
        </div>

        <div className="col-12 md:col-6">
          <Card title="2) Kiểm tra & lưu">
            <div className="flex flex-column gap-3">
              <InputText
                value={form.fullName}
                onChange={(e) => setForm((p) => ({ ...p, fullName: e.target.value }))}
                placeholder="Họ tên"
                className="w-full"
              />

              <InputText
                value={form.nationalId}
                onChange={(e) => setForm((p) => ({ ...p, nationalId: e.target.value }))}
                placeholder="CCCD / ID"
                className="w-full"
              />

              <InputText
                value={form.dateOfBirth}
                onChange={(e) => setForm((p) => ({ ...p, dateOfBirth: e.target.value }))}
                placeholder="Ngày sinh (yyyy-MM-dd)"
                className="w-full"
              />

              <InputText
                value={form.addressText}
                onChange={(e) => setForm((p) => ({ ...p, addressText: e.target.value }))}
                placeholder="Địa chỉ / Quê quán"
                className="w-full"
              />

              <Divider />

              <Button
                label={loading ? "Đang lưu..." : "Lưu công dân"}
                icon="pi pi-save"
                onClick={onSave}
                disabled={!canSave}
              />
            </div>
          </Card>
        </div>
      </div>
    </AppLayout>
  );
}
