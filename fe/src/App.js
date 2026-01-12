import React from "react";
import { BrowserRouter, Routes, Route } from "react-router-dom";

import RequireAuth from "./components/RequireAuth";
import LoginPage from "./pages/LoginPage";
import AppShell from "./layout/AppShell";

import DashboardPage from "./pages/DashboardPage";
import OcrUpload from "./pages/OcrUpload";
import CitizensPage from "./pages/CitizenPage";
import CitizenDetailPage from "./pages/CitizenDetailPage";

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />

        <Route
          path="/*"
          element={
            <RequireAuth>
              <AppShell>
                <Routes>
                  <Route path="/" element={<DashboardPage />} />
                  <Route path="/ocr" element={<OcrUpload />} />
                  <Route path="/citizens" element={<CitizensPage />} />
                  <Route path="/citizens/:id" element={<CitizenDetailPage />} />
                </Routes>
              </AppShell>
            </RequireAuth>
          }
        />
      </Routes>
    </BrowserRouter>
  );
}
