import SidebarMenu from "./SidebarMenu";
import Topbar from "./Topbar";

export default function AppLayout({ title, children }) {
  return (
    <div className="flex" style={{ minHeight: "100vh" }}>
      <aside
        style={{
          borderRight: "1px solid rgba(255,255,255,0.08)",
          background: "rgba(0,0,0,0.15)",
        }}
      >
        <SidebarMenu />
      </aside>

      <main className="flex-1">
        <div
          className="p-3"
          style={{ borderBottom: "1px solid rgba(255,255,255,0.08)" }}
        >
          <Topbar title={title} />
        </div>
        <div className="p-3">{children}</div>
      </main>
    </div>
  );
}
