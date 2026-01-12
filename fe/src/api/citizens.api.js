import { http } from "./http";

export async function listCitizens(q) {
  const res = await http.get("/api/citizens", { params: { q } });
  return res.data;
}

export async function createCitizen(payload) {
  const res = await http.post("/api/citizens", payload);
  return res.data;
}
export async function exportCitizensExcel(q = "") {
  const res = await http.get(`/api/citizens/export?q=${encodeURIComponent(q)}`, {
    responseType: "blob",
  });
  return res.data; // Blob
}
export async function getCitizenById(id) {
  const res = await http.get(`/api/citizens/${id}`);
  return res.data;
}
export async function updateCitizen(id, payload) {
  const res = await http.put(`/api/citizens/${id}`, payload);
  return res.data;
}