import { Link } from "react-router";
import {
  GripIcon,
  UsersIcon,
  BanknoteIcon,
  CreditCardIcon,
} from "lucide-react";
import {
  Button,
  DropdownMenu,
  DropdownMenuItem,
  DropdownMenuContent,
  DropdownMenuTrigger,
} from "@nubeteck/shadcn";

const AppsHeader = () => {
  const apps = [
    {
      id: "members",
      icon: UsersIcon,
      title: "Gestión de socios",
      description: "Registro y administración",
      url: "https://coopcloud-members.onlinedemo.do",
    },
    {
      id: "savings",
      icon: BanknoteIcon,
      title: "Aportaciones y Ahorros",
      url: "https://coopcloud-shares.onlinedemo.do",
      description: "Gestión de aportaciones, cuentas y plazos fijos",
    },
    {
      id: "loans",
      title: "Préstamos",
      icon: CreditCardIcon,
      url: "https://coopcloud-loans.onlinedemo.do",
      description: "Sistema integral de créditos y financiamiento",
    },
  ];

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button
          size="icon"
          variant="ghost"
          className="size-7"
          aria-label="Aplicaciones"
        >
          <GripIcon />
          <span className="sr-only">Toggle Apps</span>
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent
        forceMount
        align="center"
        sideOffset={8}
        className="w-80"
      >
        <div className="p-2 border-b">
          <h4 className="text-sm font-semibold text-gray-900 dark:text-gray-100">
            Explorar
          </h4>
        </div>
        <div className="p-1">
          <div className="space-y-1">
            {apps.map((app) => {
              const IconComponent = app.icon;
              return (
                <DropdownMenuItem asChild key={app.id}>
                  <Link
                    to={app.url}
                    className="p-2 cursor-pointer hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors"
                  >
                    <div className="flex items-center gap-3 w-full">
                      <div className="w-8 h-8 rounded-full bg-primary flex items-center justify-center flex-shrink-0">
                        <IconComponent size={15} className="text-white" />
                      </div>
                      <div className="flex flex-col min-w-0 flex-1">
                        <span className="text-gray-900 dark:text-gray-100 text-sm font-medium">
                          {app.title}
                        </span>
                        <span className="text-gray-500 dark:text-gray-400 text-xs">
                          {app.description}
                        </span>
                      </div>
                    </div>
                  </Link>
                </DropdownMenuItem>
              );
            })}
          </div>
        </div>
      </DropdownMenuContent>
    </DropdownMenu>
  );
};

export default AppsHeader;
