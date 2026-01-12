import { http } from "./http";

export async function login(username, password) {
  const res = await http.post("/api/auth/login", { username, password });
  return res.data; // { token, role }
}

export function logout() {
  localStorage.removeItem("token");
}
