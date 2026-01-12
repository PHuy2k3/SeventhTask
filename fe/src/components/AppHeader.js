import React from "react";
import { useNavigate } from "react-router-dom";
import { Button } from "primereact/button";
import { logout } from "../api/auth.api";

export default function AppHeader() {
  const nav = useNavigate();

  function onLogout() {
    logout();
    nav("/login", { replace: true });
  }

  return (
    <div className="app-header">
      <div className="brand">
        <span className="dot" />
        <span className="title">Zootopia Citizen Admin</span>
      </div>

      <div className="actions">
        <Button
          label="Logout"
          icon="pi pi-sign-out"
          severity="secondary"
          onClick={onLogout}
        />
      </div>
    </div>
  );
}
