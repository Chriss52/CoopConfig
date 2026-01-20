import React from "react";
import SidebarLogo from "@/components/sidebar-logo";
import { Card, CardTitle, CardHeader, CardDescription } from "@nubeteck/shadcn";
import {
  UsersIcon,
  BanknoteIcon,
  SettingsIcon,
  CreditCardIcon,
} from "lucide-react";

interface ModuleCard {
  id: string;
  url: string;
  title: string;
  description: string;
  icon: React.ComponentType<{ className?: string }>;
}

const MODULES: ModuleCard[] = [
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
  {
    id: "settings",
    url: "/settings",
    icon: SettingsIcon,
    title: "Configuraciones Generales",
    description: "Configuración y administración del sistema",
  },
];

const NavigatePage = () => {
  return (
    <div className="flex min-h-svh flex-col items-center justify-center bg-muted/40 p-6">
      <div className="w-full max-w-6xl space-y-8">
        <div className="flex flex-col items-center gap-4 text-center">
          <div className="flex items-center gap-3 font-medium">
            <SidebarLogo />
            <span className="text-2xl font-bold">Coop Cloud</span>
          </div>
          <div className="space-y-2">
            <h1 className="text-3xl font-bold tracking-tight">
              Selecciona un módulo
            </h1>
            <p className="text-muted-foreground">
              Elige el módulo que deseas acceder
            </p>
          </div>
        </div>
        <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
          {MODULES.map((module) => {
            const Icon = module.icon;
            const isExternal = module.url.startsWith("http");
            return (
              <a
                key={module.id}
                href={module.url}
                rel="noopener noreferrer"
                {...(isExternal && { target: "_blank" })}
              >
                <Card className="group transition-all hover:shadow-lg hover:border-primary/50">
                  <CardHeader className="pb-4">
                    <div className="mb-4 flex h-12 w-12 items-center justify-center rounded-lg bg-primary/10 text-primary transition-colors group-hover:bg-primary group-hover:text-primary-foreground">
                      <Icon className="h-6 w-6" />
                    </div>
                    <CardTitle className="text-xl">{module.title}</CardTitle>
                    <CardDescription className="text-sm">
                      {module.description}
                    </CardDescription>
                  </CardHeader>
                </Card>
              </a>
            );
          })}
        </div>
        <div className="text-center text-sm text-muted-foreground">
          <p>
            ¿Necesitas ayuda?{" "}
            <a
              href="mailto:soporte@coopcloud.com"
              className="underline underline-offset-4 hover:text-foreground"
            >
              Contáctanos
            </a>
          </p>
        </div>
      </div>
    </div>
  );
};

export default NavigatePage;
