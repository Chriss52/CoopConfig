import React from "react";
import { Separator, SidebarTrigger } from "@nubeteck/shadcn";

import NavUser from "./nav-user";
import AppsHeader from "./apps-header";
import NotificationHeader from "./notification-header";

const AppHeader = () => {
  const handleLogout = () => {
    // eslint-disable-next-line no-console
    console.log("Cerrando sesión...");
    // window.location.href = '/login';
  };

  // Ctrl+Q
  React.useEffect(() => {
    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.ctrlKey && event.key === "q") {
        event.preventDefault();
        handleLogout();
      }
    };
    document.addEventListener("keydown", handleKeyDown);
    return () => {
      document.removeEventListener("keydown", handleKeyDown);
    };
  }, []);

  return (
    <header className="flex border-b h-14 shrink-0 items-center gap-2 transition-[width,height] ease-linear group-has-data-[collapsible=icon]/sidebar-wrapper:h-12">
      <div className="flex flex-1 h-14 items-center justify-between px-6">
        <div className="flex items-center gap-2">
          <SidebarTrigger className="-ml-1" />
          <Separator
            orientation="vertical"
            className="mr-2 data-[orientation=vertical]:h-6"
          />
          {/* <DynamicBreadcrumb /> */}
        </div>
        <div className="flex items-center gap-2">
          <AppsHeader />
          <NotificationHeader />
          <Separator
            orientation="vertical"
            className="mx-1 data-[orientation=vertical]:h-6"
          />
          <NavUser />
        </div>
      </div>
    </header>
  );
};

export default AppHeader;
