import React from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { Button } from "primereact/button";
import { logout } from "../api/auth.api";


export default function AppTopbar() {
  const { pathname } = useLocation();
  const nav = useNavigate();

  function onLogout() {
    logout();
    nav("/login", { replace: true });
  }

  return (
    <header className="topbar">
      <div className="topbar-actions">
        <Button
          label="Logout"
          icon="pi pi-sign-out"
          severity="secondary"
          onClick={onLogout}
        />
      </div>
    </header>
  );
}
