import React from "react";
import { NavLink } from "react-router-dom";

export default function AppSidebar() {
  const itemClass = ({ isActive }) =>
    "sb-item" + (isActive ? " active" : "");

  return (
    <aside className="sb">
      <div className="sb-brand">
        <div className="sb-title">Zootopia</div>
        <div className="sb-sub">Citizen Manager</div>
      </div>

      <nav className="sb-nav">
        <NavLink to="/" end className={itemClass}>
          <i className="pi pi-home" />
          <span>Dashboard</span>
        </NavLink>

        <NavLink to="/ocr" className={itemClass}>
          <i className="pi pi-camera" />
          <span>OCR Import</span>
        </NavLink>

        <NavLink to="/citizens" className={itemClass}>
          <i className="pi pi-users" />
          <span>Citizens</span>
        </NavLink>
      </nav>
    </aside>
  );
}
