import { Outlet } from "react-router";
import { SidebarInset, SidebarProvider } from "@nubeteck/shadcn";

import AppHeader from "./app-header";

export interface AppLayoutProps {
  sidebar?: React.ReactNode;
}

const AppLayout = ({ sidebar }: AppLayoutProps) => {
  return (
    <SidebarProvider>
      {sidebar}
      <SidebarInset className="overflow-hidden">
        <AppHeader />
        <main className="flex-1 overflow-y-auto overflow-x-hidden p-6 min-w-0">
          <div className="max-w-full">
            <Outlet />
          </div>
        </main>
      </SidebarInset>
    </SidebarProvider>
  );
};

export default AppLayout;
