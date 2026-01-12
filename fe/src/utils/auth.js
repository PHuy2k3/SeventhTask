export function isAuthed() {
  return !!localStorage.getItem("token");
}
