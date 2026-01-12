import { http } from "./http";

export async function extractFromImage(file) {
  const fd = new FormData();
  fd.append("file", file);

  const res = await http.post("/api/ai/extract", fd, {
    headers: { "Content-Type": "multipart/form-data" },
  });

  return res.data;
}
