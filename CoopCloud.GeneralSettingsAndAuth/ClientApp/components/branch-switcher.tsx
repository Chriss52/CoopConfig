import React from "react";
import { StoreIcon, ChevronsUpDownIcon } from "lucide-react";
import {
  Skeleton,
  useSidebar,
  SidebarMenu,
  DropdownMenu,
  SidebarMenuItem,
  DropdownMenuItem,
  DropdownMenuLabel,
  SidebarMenuButton,
  DropdownMenuContent,
  DropdownMenuTrigger,
} from "@nubeteck/shadcn";

import SidebarLogo from "./sidebar-logo";

const branches = [
  {
    name: "Sucursal Principal",
    description: "Calle 123, Ciudad, País",
  },
];

const BranchSwitcher = () => {
  const { isMobile } = useSidebar();

  const [activeBranch, setActiveBranch] = React.useState<
    null | (typeof branches)[0]
  >(null);

  const isUserPending = false;
  const title = "Coop Casa";

  return (
    <SidebarMenu>
      <SidebarMenuItem>
        <DropdownMenu>
          <DropdownMenuTrigger asChild className="cursor-pointer w-full px-4">
            <SidebarMenuButton
              size="lg"
              disabled={isUserPending}
              className="rounded-none data-[state=open]:bg-sidebar-accent data-[state=open]:text-sidebar-accent-foreground"
            >
              {isUserPending ? (
                <>
                  <div className="flex aspect-square items-center justify-center">
                    <Skeleton className="h-9 w-9 rounded-lg" />
                  </div>
                  <div className="grid flex-1 gap-1 text-left text-sm leading-tight">
                    <Skeleton className="h-3 w-[100px]" />
                    <Skeleton className="h-3 w-[150px]" />
                  </div>
                </>
              ) : (
                <>
                  <div className="flex aspect-square size-8 items-center justify-center">
                    <SidebarLogo title={title} />
                  </div>
                  <div className="grid flex-1 text-left text-sm leading-tight">
                    <span className="text-sm font-semibold truncate leading-tight">
                      {title}
                    </span>
                    <span className="truncate text-xs">
                      {activeBranch?.description || "Selecciona una sucursal"}
                    </span>
                  </div>
                  <ChevronsUpDownIcon className="ml-auto" />
                </>
              )}
            </SidebarMenuButton>
          </DropdownMenuTrigger>
          <DropdownMenuContent
            align="start"
            sideOffset={4}
            side={isMobile ? "bottom" : "right"}
            className="w-(--radix-dropdown-menu-trigger-width) min-w-56 rounded-lg"
          >
            <DropdownMenuLabel className="text-muted-foreground text-xs">
              Sucursales
            </DropdownMenuLabel>
            {branches?.map((branch) => (
              <DropdownMenuItem
                key={branch.name}
                className="gap-2 p-2"
                onClick={() => setActiveBranch(branch)}
              >
                <div className="flex size-6 items-center justify-center rounded-md border">
                  <StoreIcon className="size-3.5 shrink-0" />
                </div>
                {branch.name}
                {/* <DropdownMenuShortcut>⌘{index + 1}</DropdownMenuShortcut> */}
              </DropdownMenuItem>
            ))}
          </DropdownMenuContent>
        </DropdownMenu>
      </SidebarMenuItem>
    </SidebarMenu>
  );
};

export default BranchSwitcher;
