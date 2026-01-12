import AppLayout from "../layout/AppLayout";
import { Card } from "primereact/card";

export default function Dashboard() {
  return (
    <AppLayout title="Dashboard">
     <div className="grid">
          <div className="col-12 md:col-4">
            <Card title="Today Imports" subTitle="OCR">
              <div className="kpi">12</div>
              <div className="muted">+3 so với hôm qua</div>
            </Card>
          </div>
    
          <div className="col-12 md:col-4">
            <Card title="Citizens" subTitle="Database">
              <div className="kpi">1,284</div>
              <div className="muted">Tổng công dân</div>
            </Card>
          </div>
    
          <div className="col-12 md:col-4">
            <Card title="OCR Quality" subTitle="Avg confidence">
              <div className="kpi">0.86</div>
              <div className="muted">Tự động highlight field thấp</div>
            </Card>
          </div>
    
          <div className="col-12">
            <Card title="Quick actions">
              <div className="muted">
                Mở OCR Import để nhập CCCD, hoặc Citizens để quản lý danh sách.
              </div>
            </Card>
          </div>
        </div>
    </AppLayout>
  );
}
