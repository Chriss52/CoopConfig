import clsx from "clsx";
import React from "react";
import {
  LogOutIcon,
  PaletteIcon,
  KeyboardIcon,
  FileTextIcon,
  HelpCircleIcon,
  ChevronDownIcon,
  UserRoundPenIcon,
} from "lucide-react";
import {
  Avatar,
  Skeleton,
  SidebarMenu,
  AvatarImage,
  DropdownMenu,
  AvatarFallback,
  SidebarMenuItem,
  DropdownMenuItem,
  SidebarMenuButton,
  DropdownMenuGroup,
  DropdownMenuLabel,
  DropdownMenuContent,
  DropdownMenuTrigger,
  DropdownMenuSeparator,
} from "@nubeteck/shadcn";

const user = {
  firstName: "Loren",
  lastName: "Lizardo",
  role: "Administrador",
  email: "loren.lizardo@example.com",
};

const NavUser = () => {
  const isUserPending = false; // Simula el estado de carga del usuario

  const handleLogout = async () => {
    localStorage.removeItem("token");
    // router.replace("/login");
  };

  const renderUserInfoSection = React.useCallback((isSub: boolean = false) => {
    return (
      <>
        <Avatar
          className={clsx("rounded-lg", {
            "h-9 w-9": isSub,
            "h-8 w-8": !isSub,
          })}
        >
          <AvatarImage alt="@shadcn" src="https://github.com/shadcn.png" />
          <AvatarFallback className="rounded-lg">
            {user?.firstName?.charAt(0)}
            {user?.lastName?.charAt(0)}
          </AvatarFallback>
        </Avatar>
        <div className="grid flex-1 text-left leading-tight">
          <span className="truncate font-regular">
            {user?.firstName}
            {user?.lastName && ` ${user?.lastName}`}
          </span>
          {isSub && <span className="truncate text-xs">{user?.email}</span>}
          <span
            className={clsx("truncate text-xs text-primary font-bold", {
              "mt-1": isSub,
            })}
          >
            {user?.role}
          </span>
        </div>
      </>
    );
  }, []);

  return (
    <SidebarMenu>
      <SidebarMenuItem>
        <DropdownMenu>
          <DropdownMenuTrigger asChild className="w-48">
            <SidebarMenuButton
              size="lg"
              disabled={isUserPending}
              className="cursor-pointer data-[state=open]:bg-sidebar-accent data-[state=open]:text-sidebar-accent-foreground"
            >
              {isUserPending ? (
                <>
                  <Skeleton className="h-8 w-8 rounded-lg" />
                  <div className="grid flex-1 gap-1 text-left text-sm leading-tight">
                    <Skeleton className="h-3 w-[150px]" />
                    <Skeleton className="h-3 w-[100px]" />
                  </div>
                </>
              ) : (
                <>
                  {renderUserInfoSection()}
                  <ChevronDownIcon className="ml-auto size-4" />
                </>
              )}
            </SidebarMenuButton>
          </DropdownMenuTrigger>
          <DropdownMenuContent
            align="end"
            side="bottom"
            sideOffset={4}
            className="w-(--radix-dropdown-menu-trigger-width) min-w-56 rounded-lg"
          >
            <DropdownMenuLabel className="p-0 font-normal">
              <div className="flex items-center gap-2 px-1 py-1.5 text-left text-sm">
                {renderUserInfoSection(true)}
              </div>
            </DropdownMenuLabel>
            <DropdownMenuSeparator />
            <DropdownMenuGroup>
              <DropdownMenuItem>
                <UserRoundPenIcon />
                Perfil
              </DropdownMenuItem>
              <DropdownMenuItem>
                <KeyboardIcon />
                Atajos
              </DropdownMenuItem>
              <DropdownMenuItem>
                <PaletteIcon />
                Tema
              </DropdownMenuItem>
            </DropdownMenuGroup>
            <DropdownMenuSeparator />
            <DropdownMenuGroup>
              <DropdownMenuItem>
                <FileTextIcon />
                Documentación
              </DropdownMenuItem>
              <DropdownMenuItem>
                <HelpCircleIcon />
                Ayuda
              </DropdownMenuItem>
            </DropdownMenuGroup>
            <DropdownMenuSeparator />
            <DropdownMenuItem onClick={handleLogout}>
              <LogOutIcon />
              Cerrar sesión
            </DropdownMenuItem>
            <DropdownMenuSeparator />
            <DropdownMenuItem className="text-xs" onClick={handleLogout}>
              {/* <LogOutIcon /> */}
              Términos y condiciones
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </SidebarMenuItem>
    </SidebarMenu>
  );
};

export default NavUser;
