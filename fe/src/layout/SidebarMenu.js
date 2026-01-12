import { PanelMenu } from "primereact/panelmenu";
import { useNavigate } from "react-router-dom";

export default function SidebarMenu() {
  const nav = useNavigate();

  const items = [
    {
      label: "Dashboard",
      icon: "pi pi-home",
      command: () => nav("/"),
    },
    {
      label: "OCR Import",
      icon: "pi pi-camera",
      command: () => nav("/ocr"),
    },
    {
      label: "Citizens",
      icon: "pi pi-users",
      command: () => nav("/citizens"),
    },
  ];

  return (
    <div className="p-3" style={{ width: 280 }}>
      <div className="mb-3">
        <div className="text-xl font-bold">Zootopia</div>
        <div className="text-sm text-500">Citizen Manager</div>
      </div>
      <PanelMenu model={items} />
    </div>
  );
}
