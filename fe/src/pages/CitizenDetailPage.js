import React, { useEffect, useRef, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Card } from "primereact/card";
import { Button } from "primereact/button";
import { Toast } from "primereact/toast";
import { ProgressSpinner } from "primereact/progressspinner";
import { Tag } from "primereact/tag";
import { Avatar } from "primereact/avatar";
import { Divider } from "primereact/divider";
import { Dialog } from "primereact/dialog";
import { InputText } from "primereact/inputtext";

import { getCitizenById, updateCitizen } from "../api/citizens.api";

function fmtDob(dob) {
  if (!dob) return "";
  const dt = new Date(dob);
  if (Number.isNaN(dt.getTime())) return String(dob);
  return dt.toLocaleDateString("vi-VN");
}

function toDateInputValue(d) {
  if (!d) return "";
  const dt = new Date(d);
  if (Number.isNaN(dt.getTime())) return "";
  const yyyy = dt.getFullYear();
  const mm = String(dt.getMonth() + 1).padStart(2, "0");
  const dd = String(dt.getDate()).padStart(2, "0");
  return `${yyyy}-${mm}-${dd}`;
}

export default function CitizenDetailPage() {
  const { id } = useParams();
  const nav = useNavigate();
  const toast = useRef(null);

  const [loading, setLoading] = useState(true);
  const [citizen, setCitizen] = useState(null);

  // ✅ dialog edit
  const [showEdit, setShowEdit] = useState(false);
  const [saving, setSaving] = useState(false);
  const [form, setForm] = useState({
    fullName: "",
    nationalId: "",
    dateOfBirth: "",
    addressText: "",
  });

  useEffect(() => {
    let mounted = true;

    async function load() {
      setLoading(true);
      try {
        const data = await getCitizenById(id);
        if (mounted) {
          setCitizen(data);
          setForm({
            fullName: data?.fullName || "",
            nationalId: data?.nationalId || "",
            dateOfBirth: toDateInputValue(data?.dateOfBirth),
            addressText: data?.addressText || "",
          });
        }
      } catch (err) {
        const msg =
          err?.response?.status === 404
            ? "Không tìm thấy công dân"
            : "Lỗi tải dữ liệu";
        toast.current?.show({ severity: "error", summary: "Lỗi", detail: msg, life: 4000 });
      } finally {
        if (mounted) setLoading(false);
      }
    }

    load();
    return () => (mounted = false);
  }, [id]);

  const avatarFallback = citizen?.fullName ? citizen.fullName.charAt(0).toUpperCase() : "?";

  function onOpenEdit() {
    if (!citizen) return;
    // nạp lại từ citizen (tránh trường hợp user sửa dở)
    setForm({
      fullName: citizen?.fullName || "",
      nationalId: citizen?.nationalId || "",
      dateOfBirth: toDateInputValue(citizen?.dateOfBirth),
      addressText: citizen?.addressText || "",
    });
    setShowEdit(true);
  }

  async function onSaveEdit() {
    if (!form.fullName.trim()) {
      toast.current?.show({ severity: "warn", summary: "Thiếu dữ liệu", detail: "Vui lòng nhập họ tên", life: 3000 });
      return;
    }
    if (!form.nationalId.trim()) {
      toast.current?.show({ severity: "warn", summary: "Thiếu dữ liệu", detail: "Vui lòng nhập CCCD/ID", life: 3000 });
      return;
    }

    setSaving(true);
    try {
      const payload = {
        fullName: form.fullName.trim(),
        nationalId: form.nationalId.trim(),
        // gửi ISO yyyy-MM-dd (an toàn)
        dateOfBirth: form.dateOfBirth ? form.dateOfBirth : null,
        addressText: form.addressText?.trim() || null,
      };

      await updateCitizen(id, payload);

      // cập nhật UI ngay (khỏi chờ reload)
      setCitizen((prev) => ({
        ...prev,
        fullName: payload.fullName,
        nationalId: payload.nationalId,
        dateOfBirth: payload.dateOfBirth,
        addressText: payload.addressText,
      }));

      setShowEdit(false);
      toast.current?.show({ severity: "success", summary: "Thành công", detail: "Đã cập nhật thông tin", life: 3000 });
    } catch (err) {
      const msg = err?.response?.data?.message || "Cập nhật thất bại";
      toast.current?.show({ severity: "error", summary: "Lỗi", detail: msg, life: 5000 });
    } finally {
      setSaving(false);
    }
  }

  async function onCopy() {
    if (!citizen) return;
    const text = [
      `Họ tên: ${citizen.fullName || ""}`,
      `CCCD/ID: ${citizen.nationalId || ""}`,
      `Ngày sinh: ${fmtDob(citizen.dateOfBirth)}`,
      `Địa chỉ: ${citizen.addressText || ""}`,
      `Mã nội bộ: ${citizen.id}`,
    ].join("\n");

    try {
      await navigator.clipboard.writeText(text);
      toast.current?.show({ severity: "success", summary: "Đã sao chép", detail: "Copy vào clipboard", life: 2000 });
    } catch {
      toast.current?.show({ severity: "warn", summary: "Không copy được", detail: "Trình duyệt chặn clipboard", life: 3000 });
    }
  }

  const header = (
    <div className="flex flex-column align-items-center gap-3 p-4">
      <Avatar
        label={avatarFallback}
        size="xlarge"
        shape="circle"
        className="bg-primary text-white shadow-4"
        style={{ width: "120px", height: "120px", fontSize: "2.5rem" }}
      />
      <div className="text-center">
        <div className="text-2xl font-bold text-900 mb-1">{citizen?.fullName || "(Chưa có tên)"}</div>
        <Tag value={`ID: ${citizen?.nationalId || "N/A"}`} severity="info" rounded />
      </div>
    </div>
  );

  const footer = (
    <div className="flex justify-content-between">
      <Button
        icon="pi pi-copy"
        label="Sao chép"
        severity="secondary"
        size="small"
        onClick={onCopy}
        disabled={!citizen}
      />
      <Button
        icon="pi pi-pencil"
        label="Chỉnh sửa"
        size="small"
        onClick={onOpenEdit}
        disabled={!citizen}
      />
    </div>
  );

  return (
    <div className="page surface-ground min-h-screen py-8 px-4">
      <Toast ref={toast} />

      <div className="flex align-items-center justify-content-between mb-6">
        <div className="flex align-items-center gap-3">
          <Button icon="pi pi-arrow-left" label="Quay lại" severity="secondary" onClick={() => nav(-1)} />
          <div className="text-3xl font-bold text-900">Chi tiết công dân</div>
        </div>
      </div>

      {loading ? (
        <div className="flex justify-content-center align-items-center min-h-40rem">
          <ProgressSpinner style={{ width: "50px", height: "50px" }} />
        </div>
      ) : citizen ? (
        <div className="flex justify-content-center">
          <Card
            header={header}
            footer={footer}
            className="w-full lg:w-8 shadow-8 border-round-2xl"
            style={{ maxWidth: "600px" }}
          >
            <div className="p-0">
              <Divider />
              <div className="grid p-6">
                <div className="col-12">
                  <div className="field-row flex flex-column gap-2 mb-4">
                    <label className="font-semibold text-900">Ngày sinh</label>
                    <div className="text-700">{fmtDob(citizen.dateOfBirth)}</div>
                  </div>
                </div>
                <div className="col-12">
                  <div className="field-row flex flex-column gap-2 mb-4">
                    <label className="font-semibold text-900">Địa chỉ</label>
                    <div className="text-700">{citizen.addressText || "Chưa cập nhật"}</div>
                  </div>
                </div>
                <div className="col-12">
                  <div className="field-row flex flex-column gap-2">
                    <label className="font-semibold text-900">Mã nội bộ</label>
                    <div className="text-700 font-mono bg-surface-50 p-3 border-round border-1 border-surface-200">
                      {citizen.id}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </Card>
        </div>
      ) : (
        <Card className="w-full lg:w-6 text-center py-8 shadow-4">
          <i className="pi pi-user text-5xl text-500 mb-4" />
          <h3 className="text-900 mb-2">Không tìm thấy thông tin</h3>
          <p className="text-600 mb-4">Công dân không tồn tại hoặc đã bị xóa.</p>
          <Button label="Tải lại" icon="pi pi-refresh" onClick={() => window.location.reload()} />
        </Card>
      )}

      {/* ✅ Dialog chỉnh sửa */}
      <Dialog
        header="Chỉnh sửa công dân"
        visible={showEdit}
        style={{ width: "min(720px, 95vw)" }}
        onHide={() => setShowEdit(false)}
        footer={
          <div className="flex justify-content-end gap-2">
            <Button label="Huỷ" severity="secondary" onClick={() => setShowEdit(false)} disabled={saving} />
            <Button label="Lưu" icon="pi pi-check" onClick={onSaveEdit} loading={saving} />
          </div>
        }
      >
        <div className="grid">
          <div className="col-12 md:col-6">
            <label className="block mb-2 font-semibold">Họ tên</label>
            <InputText
              value={form.fullName}
              onChange={(e) => setForm((s) => ({ ...s, fullName: e.target.value }))}
              className="w-full"
              autoFocus
            />
          </div>

          <div className="col-12 md:col-6">
            <label className="block mb-2 font-semibold">CCCD / ID</label>
            <InputText
              value={form.nationalId}
              onChange={(e) => setForm((s) => ({ ...s, nationalId: e.target.value }))}
              className="w-full"
            />
          </div>

          <div className="col-12 md:col-6">
            <label className="block mb-2 font-semibold">Ngày sinh</label>
            <InputText
              type="date"
              value={form.dateOfBirth}
              onChange={(e) => setForm((s) => ({ ...s, dateOfBirth: e.target.value }))}
              className="w-full"
            />
          </div>

          <div className="col-12 md:col-6">
            <label className="block mb-2 font-semibold">Địa chỉ</label>
            <InputText
              value={form.addressText}
              onChange={(e) => setForm((s) => ({ ...s, addressText: e.target.value }))}
              className="w-full"
            />
          </div>
        </div>
      </Dialog>
    </div>
  );
}
