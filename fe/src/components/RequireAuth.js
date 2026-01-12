import React from "react";
import { Navigate } from "react-router-dom";
import { isAuthed } from "../utils/auth";

export default function RequireAuth({ children }) {
  if (!isAuthed()) return <Navigate to="/login" replace />;
  return children;
}
