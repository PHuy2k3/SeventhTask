import React, { useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Card } from "primereact/card";
import { InputText } from "primereact/inputtext";
import { Password } from "primereact/password";
import { Button } from "primereact/button";
import { Toast } from "primereact/toast";
import { login } from "../api/auth.api";

export default function LoginPage() {
  const toast = useRef(null);
  const nav = useNavigate();

  const [loading, setLoading] = useState(false);
  const [username, setUsername] = useState("admin");
  const [password, setPassword] = useState("");

  async function onSubmit(e) {
    e.preventDefault();
    setLoading(true);
    try {
      const data = await login(username.trim(), password);
      localStorage.setItem("token", data.token);

      toast.current?.show({
        severity: "success",
        summary: "Đăng nhập thành công",
        detail: "Chào Admin Zootopia!",
      });

      nav("/", { replace: true });
    } catch (err) {
      const msg =
        err?.response?.data?.message ||
        (err?.response?.status === 401 ? "Sai tài khoản hoặc mật khẩu" : "Lỗi server");

      toast.current?.show({
        severity: "error",
        summary: "Đăng nhập thất bại",
        detail: msg,
        life: 6000,
      });
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="auth-page">
      <Toast ref={toast} />

      <Card className="auth-card" title="Zootopia Admin Login">
        <form onSubmit={onSubmit} className="flex flex-column gap-3">
          <span className="p-input-icon-left">
            <i className="pi pi-user" />
            <InputText
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              placeholder="Username"
              autoComplete="username"
              disabled={loading}
              className="w-full"
            />
          </span>

          <Password
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="Password"
            toggleMask
            feedback={false}
            disabled={loading}
            inputClassName="w-full"
          />

          <Button
            type="submit"
            label={loading ? "Đang đăng nhập..." : "Đăng nhập"}
            icon="pi pi-sign-in"
            loading={loading}
          />

          <div className="text-sm text-500">
            *Không có đăng ký. Chỉ dành cho Admin.
          </div>
        </form>
      </Card>
    </div>
  );
}
