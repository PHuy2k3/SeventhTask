import { useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";

import AppLayout from "../layout/AppLayout";
import { Toast } from "primereact/toast";
import { InputText } from "primereact/inputtext";
import { DataTable } from "primereact/datatable";
import { Column } from "primereact/column";
import { Button } from "primereact/button";
import { Card } from "primereact/card";
import { Avatar } from "primereact/avatar";
import { Chip } from "primereact/chip";
import { Badge } from "primereact/badge";

import { listCitizens, exportCitizensExcel } from "../api/citizens.api";

function fmtDob(dob) {
  if (!dob) return "";
  const dt = new Date(dob);
  if (Number.isNaN(dt.getTime())) return String(dob);
  return dt.toLocaleDateString("vi-VN");
}

export default function CitizensList() {
  const toast = useRef(null);
  const navigate = useNavigate();

  const [q, setQ] = useState("");
  const [rows, setRows] = useState([]);
  const [loading, setLoading] = useState(false);

  async function load() {
    setLoading(true);
    try {
      const data = await listCitizens(q);
      setRows(Array.isArray(data) ? data : []);
    } catch (err) {
      toast.current?.show({
        severity: "error",
        summary: "Load lỗi",
        detail: String(err?.message || err),
      });
    } finally {
      setLoading(false);
    }
  }

  async function onExportExcel() {
    try {
      const blob = await exportCitizensExcel(q || "");
      const url = window.URL.createObjectURL(blob);

      const a = document.createElement("a");
      a.href = url;
      a.download = `citizens_${new Date()
        .toISOString()
        .slice(0, 19)
        .replace(/[:T]/g, "-")}.xlsx`;
      document.body.appendChild(a);
      a.click();
      a.remove();

      window.URL.revokeObjectURL(url);
    } catch (err) {
      toast.current?.show({
        severity: "error",
        summary: "Export lỗi",
        detail: String(err?.message || err),
      });
    }
  }

  function onViewDetail(row) {
    if (!row?.id) return;
    navigate(`/citizens/${row.id}`);
  }

  const actionBody = (row) => (
    <Button
      icon="pi pi-eye"
      rounded
      text
      severity="info"
      tooltip="Xem chi tiết"
      tooltipOptions={{ position: "top" }}
      onClick={() => onViewDetail(row)}
    />
  );

  const nameBody = (row) => (
    <div className="flex items-center gap-3">
      <Avatar 
        label={row.fullName?.charAt(0)?.toUpperCase() || '?'} 
        size="normal" 
        shape="circle"
        className="bg-primary text-primary-inverse"
      />
      <div>
        <div className="font-medium text-900">{row.fullName || "--"}</div>
        <small className="text-500">ID: {row.id}</small>
      </div>
    </div>
  );

  const dobBody = (row) => <div className="font-medium">{fmtDob(row.dateOfBirth)}</div>;

  const nationalIdBody = (row) => (
    <Chip 
      label={row.nationalId || "N/A"} 
      severity={row.nationalId ? "info" : "secondary"}
      size="small"
      className="font-mono"
    />
  );

  const addressBody = (row) => (
    <div className="line-height-3 truncate-text" title={row.addressText || ""}>
      {row.addressText || "--"}
    </div>
  );

  useEffect(() => {
    load();
  }, []);

  return (
    <AppLayout title="Danh sách công dân">
      <Toast ref={toast} position="top-right" />

      <div className="flex flex-column gap-6 p-4 md:p-6">
        <div className="flex justify-content-between align-items-center flex-wrap gap-3">
          <div className="flex align-items-center gap-2">
            <i className="pi pi-users text-primary text-4xl" />
            <div>
              <h1 className="m-0 text-3xl font-bold text-900">Công dân</h1>
              <Badge 
                value={rows.length} 
                severity="info" 
                size="xlarge" 
                className="ml-2"
              />
            </div>
          </div>
        </div>

        <Card className="shadow-6 border-round-xl overflow-hidden">
          <div className="p-6 pb-4">
            <div className="flex flex-column sm:flex-row gap-3 align-items-stretch sm:align-items-end">
              <span className="p-input-icon-left flex-1">
                <i className="pi pi-search" />
                <InputText
                  value={q}
                  onChange={(e) => setQ(e.target.value)}
                  placeholder="Tìm theo tên hoặc CCCD..."
                  className="w-full h-3rem"
                />
              </span>

              <Button 
                icon="pi pi-refresh" 
                label="Tải lại" 
                onClick={load} 
                loading={loading}
                severity="secondary"
                outlined
                size="small"
              />

              <Button
                icon="pi pi-file-excel"
                label="Excel"
                severity="success"
                size="small"
                onClick={onExportExcel}
              />
            </div>
          </div>

          <DataTable
            value={rows}
            loading={loading}
            stripedRows
            showGridlines="surface-border"
            paginator
            rows={10}
            rowsPerPageOptions={[10, 20, 50]}
            paginatorTemplate="RowsPerPageDropdown FirstPageLink PrevPageLink CurrentPageReport NextPageLink LastPageLink"
            currentPageReportTemplate="{first}-{last} của {totalRecords}"
            emptyMessage={
              <div className="flex flex-column align-items-center p-6">
                <i className="pi pi-users text-5xl text-400 mb-4" />
                <h3 className="text-900 mb-2">Không tìm thấy</h3>
                <p className="text-600 mb-4">Thử thay đổi từ khóa tìm kiếm</p>
              </div>
            }
            sortField="fullName"
            sortOrder={1}
            className="p-datatable-hoverable-rows p-datatable-sm"
            rowHover
          >
            <Column 
              header="Thông tin" 
              body={nameBody} 
              style={{ minWidth: '280px' }}
            />
            <Column 
              header="ID/CCCD" 
              body={nationalIdBody}
              style={{ minWidth: '140px' }}
            />
            <Column 
              header="Ngày sinh" 
              body={dobBody}
              style={{ minWidth: '130px' }}
            />
            <Column 
              header="Địa chỉ" 
              body={addressBody}
              style={{ minWidth: '300px', flexGrow: 1 }}
            />
            <Column 
              header="" 
              body={actionBody} 
              style={{ minWidth: '70px', maxWidth: '70px' }}
              alignHeader="center"
            />
          </DataTable>
        </Card>
      </div>
    </AppLayout>
  );
}
