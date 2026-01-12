import React from "react";
import AppSidebar from "../components/AppSidebar";
import AppTopbar from "./AppTopbar";

export default function AppShell({ children }) {
  return (
    <div className="shell">
      <div className="shell-main">
        <AppTopbar />
        <div className="shell-content">{children}</div>
      </div>
    </div>
  );
}
