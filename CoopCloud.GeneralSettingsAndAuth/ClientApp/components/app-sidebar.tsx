import type { IMenuItem } from "@/types";

import React from "react";
import { ChevronRight } from "lucide-react";
import { isRouteActive } from "@/lib/utils";
import { Link, useLocation } from "react-router";
import {
  cn,
  Sidebar,
  useSidebar,
  SidebarMenu,
  SidebarGroup,
  SidebarFooter,
  SidebarHeader,
  SidebarContent,
  SidebarMenuSub,
  SidebarMenuItem,
  SidebarGroupLabel,
  SidebarMenuButton,
  SidebarMenuSubItem,
  SidebarMenuSubButton,
} from "@nubeteck/shadcn";

import BranchSwitcher from "./branch-switcher";

export interface IAppSidebarProps {
  title: string;
  className?: string;
  items: IMenuItem[];
  secondaryItem?: IMenuItem;
}

const AppSidebar = ({
  items,
  title,
  className,
  secondaryItem,
}: IAppSidebarProps) => {
  const { state } = useSidebar();
  const { pathname } = useLocation();

  const isCollapsed = state === "collapsed";

  const [openSubmenus, setOpenSubmenus] = React.useState<
    Record<string, boolean>
  >(() => {
    const initialState: Record<string, boolean> = {};
    items.forEach((item) => {
      if (item.children) {
        const isOnSubmenuPage = item.basePath
          ? pathname?.startsWith(item.basePath)
          : item.children.some((child) => pathname === child.href);
        if (isOnSubmenuPage) {
          initialState[item.id] = true;
        }
      }
    });
    return initialState;
  });

  const toggleSubmenu = (id: string, e?: React.MouseEvent) => {
    if (e) e.preventDefault();
    setOpenSubmenus((prev) => ({
      ...prev,
      [id]: !prev[id],
    }));
  };

  const isSubmenuOpen = (id: string): boolean => !!openSubmenus[id];

  const getMenuItemClassName = (itemPath: string) => {
    const isActive = isRouteActive(pathname, itemPath);

    return cn(
      "transition-colors duration-150",
      isActive
        ? "bg-accent text-accent-foreground"
        : "hover:bg-accent/50 text-foreground",
    );
  };

  const RegularMenuItem = ({ item }: { item: IMenuItem }) => {
    const isActive = isRouteActive(pathname, item.href);
    const Icon = item.icon;

    return (
      <SidebarMenuItem>
        <SidebarMenuButton
          asChild
          isActive={isActive}
          className={getMenuItemClassName(item.href)}
          tooltip={isCollapsed ? item.label : undefined}
        >
          <Link to={item.href} className="group">
            <Icon className="w-4 h-4" />
            <span>{item.label}</span>
          </Link>
        </SidebarMenuButton>
      </SidebarMenuItem>
    );
  };

  const SubMenuItem = ({ item }: { item: IMenuItem }) => {
    const isActive = pathname === item.href;
    const Icon = item.icon;

    return (
      <SidebarMenuSubItem>
        <SidebarMenuSubButton
          asChild
          isActive={isActive}
          className={getMenuItemClassName(item.href)}
        >
          <Link to={item.href} className="group">
            <Icon className="w-4 h-4" />
            <span>{item.label}</span>
          </Link>
        </SidebarMenuSubButton>
      </SidebarMenuSubItem>
    );
  };

  const SubmenuItem = ({ item }: { item: IMenuItem }) => {
    const isActive = item.basePath
      ? pathname?.startsWith(item.basePath)
      : item.children?.some((child) => pathname === child.href) || false;
    const isOpen = isSubmenuOpen(item.id);
    const Icon = item.icon;

    const handleToggle = (e: React.MouseEvent) => {
      e.preventDefault();
      toggleSubmenu(item.id);
    };

    return (
      <SidebarMenuItem className="relative">
        <SidebarMenuButton
          isActive={isActive}
          tooltip={isCollapsed ? item.label : undefined}
          onClick={isCollapsed ? undefined : handleToggle}
          className={cn(
            getMenuItemClassName(item.href),
            isOpen && !isCollapsed && "mb-1",
            !isCollapsed && "cursor-pointer",
          )}
        >
          <div className="flex justify-between items-center w-full">
            <div className="flex items-center gap-2">
              <Icon className="w-4 h-4 shrink-0" />
              <span>{item.label}</span>
            </div>
            {!isCollapsed && (
              <ChevronRight
                className={cn(
                  "h-4 w-4 shrink-0 transition-transform duration-200",
                  isOpen && "rotate-90",
                )}
              />
            )}
          </div>
        </SidebarMenuButton>
        {(isOpen || isCollapsed) && item.children && (
          <SidebarMenuSub>
            {item.children.map((subItem) => (
              <SubMenuItem item={subItem} key={subItem.id} />
            ))}
          </SidebarMenuSub>
        )}
      </SidebarMenuItem>
    );
  };

  return (
    <Sidebar collapsible="icon" className={cn(className)}>
      <SidebarHeader
        className={cn(
          "border-sidebar-border flex justify-center border-b h-12",
          !isCollapsed ? "h-14 p-0" : "h-12",
        )}
      >
        <BranchSwitcher />
      </SidebarHeader>
      <SidebarContent className="pb-2">
        <SidebarGroup>
          {title && <SidebarGroupLabel>{title}</SidebarGroupLabel>}
          <SidebarMenu>
            {items.map((item) =>
              item.children ? (
                <SubmenuItem item={item} key={item.id} />
              ) : (
                <RegularMenuItem item={item} key={item.id} />
              ),
            )}
          </SidebarMenu>
        </SidebarGroup>
      </SidebarContent>
      <SidebarFooter>
        {secondaryItem && <RegularMenuItem item={secondaryItem} />}
      </SidebarFooter>
    </Sidebar>
  );
};

export default AppSidebar;
