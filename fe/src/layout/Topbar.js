import { Toolbar } from "primereact/toolbar";
import { Button } from "primereact/button";

export default function Topbar({ title }) {
  return (
    <Toolbar
      className="border-none"
      start={<div className="text-lg font-bold">{title}</div>}
      end={
        <div className="flex gap-2">
          <Button icon="pi pi-bell" rounded text />
          <Button icon="pi pi-user" rounded text />
        </div>
      }
    />
  );
}
