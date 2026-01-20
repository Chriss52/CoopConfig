import {
  BellIcon,
  ClockIcon,
  UsersIcon,
  ShieldIcon,
  type LucideIcon,
  AlertTriangleIcon,
} from "lucide-react";
import {
  Badge,
  Button,
  DropdownMenu,
  DropdownMenuItem,
  DropdownMenuContent,
  DropdownMenuTrigger,
  DropdownMenuSeparator,
} from "@nubeteck/shadcn";

type AlertType = "info" | "warning" | "success" | "critical";

interface IAlert {
  id: string;
  time: string;
  title: string;
  type: AlertType;
  icon: LucideIcon;
  subtitle: string;
}

const NotificationHeader = () => {
  const getAlertColors = (type: AlertType) => {
    const colorMap = {
      info: {
        bgColor: "bg-blue-100",
        iconColor: "text-blue-600",
      },
      critical: {
        bgColor: "bg-red-100",
        iconColor: "text-red-600",
      },
      success: {
        bgColor: "bg-green-100",
        iconColor: "text-green-600",
      },
      warning: {
        bgColor: "bg-yellow-100",
        iconColor: "text-yellow-600",
      },
    };
    return colorMap[type];
  };

  const notifications: IAlert[] = [
    {
      type: "critical",
      icon: AlertTriangleIcon,
      time: "Hace 15 minutos",
      id: "suspicious-transaction",
      title: "Transacción sospechosa detectada",
      subtitle: "Juan Pérez - Retiro de RD$ 150,000",
    },
    {
      type: "warning",
      icon: ShieldIcon,
      id: "kyc-expired",
      time: "Hace 2 horas",
      title: "Documento KYC vencido",
      subtitle: "María González - Actualizar identificación",
    },
    {
      type: "info",
      icon: ClockIcon,
      id: "monthly-report",
      time: "Hace 4 horas",
      title: "Reporte mensual pendiente",
      subtitle: "Informe PLD/FT - Vence mañana",
    },
    {
      icon: UsersIcon,
      type: "success",
      id: "new-member",
      time: "Hace 6 horas",
      title: "Nuevo cliente registrado",
      subtitle: "Carlos Martínez - Aprobación pendiente",
    },
  ];

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button
          size="icon"
          variant="ghost"
          aria-label="Aplicaciones"
          className="size-7 relative"
        >
          <BellIcon />
          <span className="sr-only">Toggle Notifications</span>
          <Badge className="absolute top-0 right-0 h-3 px-0 min-w-3 rounded-full tabular-nums">
            {notifications.length}
          </Badge>
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent
        forceMount
        align="end"
        sideOffset={8}
        className="w-80"
      >
        <div className="p-3 border-b">
          <h4 className="text-sm font-semibold">Notificaciones</h4>
        </div>
        <div>
          {notifications.map((notification, index) => {
            const colors = getAlertColors(notification.type);
            return (
              <DropdownMenuItem
                key={notification.id}
                className={`p-3 cursor-pointer hover:bg-gray-50 ${
                  !(index === notifications.length - 1)
                    ? "border-b border-gray-100"
                    : ""
                }`}
              >
                <div className="flex items-start gap-3 w-full">
                  <div
                    className={`flex-shrink-0 w-8 h-8 ${colors.bgColor} rounded-full flex items-center justify-center`}
                  >
                    <notification.icon
                      className={`h-4 w-4 ${colors.iconColor}`}
                    />
                  </div>
                  <div className="flex-1 min-w-0">
                    <p className="text-sm font-medium">{notification.title}</p>
                    <p className="text-xs text-gray-500 mt-1">
                      {notification.subtitle}
                    </p>
                    <p className="text-xs text-gray-400 mt-1">
                      {notification.time}
                    </p>
                  </div>
                </div>
              </DropdownMenuItem>
            );
          })}
        </div>
        <DropdownMenuSeparator />
        <Button size="sm" variant="ghost" className="w-full">
          Ver todas las alertas
        </Button>
      </DropdownMenuContent>
    </DropdownMenu>
  );
};

export default NotificationHeader;
