import { useEffect, useRef, useState } from "react";
import AppLayout from "../layout/AppLayout";
import { Toast } from "primereact/toast";
import { InputText } from "primereact/inputtext";
import { DataTable } from "primereact/datatable";
import { Column } from "primereact/column";
import { Button } from "primereact/button";
import { listCitizens } from "../api/citizens.api";
import { Card } from "primereact/card";

export default function CitizensList() {
  const toast = useRef(null);
  const [q, setQ] = useState("");
  const [rows, setRows] = useState([]);
  const [loading, setLoading] = useState(false);

  async function load() {
    setLoading(true);
    try {
      const data = await listCitizens(q);
      setRows(Array.isArray(data) ? data : []);
    } catch (err) {
      toast.current.show({ severity: "error", summary: "Load lỗi", detail: String(err?.message || err) });
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return (
    <AppLayout title="Citizens">
      <Toast ref={toast} />

      <Card>
        <div className="flex gap-2 align-items-center mb-3">
          <span className="p-input-icon-left flex-1">
            <i className="pi pi-search" />
            <InputText
              value={q}
              onChange={(e) => setQ(e.target.value)}
              placeholder="Tìm theo tên hoặc CCCD..."
              className="w-full"
            />
          </span>
          <Button icon="pi pi-refresh" label="Tải lại" onClick={load} />
        </div>

        <DataTable value={rows} loading={loading} paginator rows={10} rowsPerPageOptions={[10, 20, 50]}>
          <Column field="id" header="ID" style={{ width: 90 }} />
          <Column field="fullName" header="Họ tên" />
          <Column field="nationalId" header="CCCD/ID" />
          <Column field="dateOfBirth" header="Ngày sinh" />
          <Column field="addressText" header="Địa chỉ" />
        </DataTable>
      </Card>
    </AppLayout>
  );
}
