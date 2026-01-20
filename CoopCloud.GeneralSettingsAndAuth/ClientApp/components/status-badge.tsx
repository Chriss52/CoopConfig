import { Badge } from "@nubeteck/shadcn";
import {
  DRAFT,
  ACTIVE,
  FROZEN,
  CLOSED,
  BLOCKED,
  OPENING,
  INACTIVE,
  SUSPENDED,
  CANCELLED,
  EMBARGOED,
  REGULARIZING,
} from "@/core/statuses";

export type StatusValue = number;

interface StatusBadgeProps {
  className?: string;
  value: StatusValue;
}

const StatusBadge: React.FC<StatusBadgeProps> = ({ value, className = "" }) => {
  const statusConfig: Record<
    number,
    {
      className: string;
      label: string;
      variant: "default" | "outline" | "secondary" | "destructive";
    }
  > = {
    [BLOCKED]: {
      label: "Bloqueado",
      variant: "destructive",
      className: "bg-red-100 text-red-800 dark:bg-red-900/40 dark:text-red-400",
    },
    [INACTIVE]: {
      label: "Inactivo",
      variant: "secondary",
      className:
        "bg-gray-100 text-gray-800 dark:bg-gray-800 dark:text-gray-300",
    },
    [FROZEN]: {
      label: "Congelado",
      variant: "secondary",
      className:
        "bg-blue-100 text-blue-800 dark:bg-blue-900/40 dark:text-blue-400",
    },
    [OPENING]: {
      label: "En Apertura",
      variant: "secondary",
      className:
        "bg-cyan-100 text-cyan-800 dark:bg-cyan-900/40 dark:text-cyan-400",
    },
    [EMBARGOED]: {
      label: "Embargado",
      variant: "destructive",
      className:
        "bg-purple-100 text-purple-800 dark:bg-purple-900/40 dark:text-purple-400",
    },
    [SUSPENDED]: {
      label: "Suspendido",
      variant: "destructive",
      className:
        "bg-orange-100 text-orange-800 dark:bg-orange-900/40 dark:text-orange-400",
    },
    [REGULARIZING]: {
      variant: "secondary",
      label: "Regularizando",
      className:
        "bg-yellow-100 text-yellow-800 dark:bg-yellow-900/40 dark:text-yellow-400",
    },
    [CANCELLED]: {
      label: "Cancelado",
      variant: "outline",
      className:
        "bg-red-50 text-red-700 border-red-200 dark:bg-red-900/20 dark:text-red-400 dark:border-red-900/50",
    },
    [CLOSED]: {
      label: "Cerrado",
      variant: "outline",
      className:
        "bg-slate-50 text-slate-700 border-slate-200 dark:bg-slate-900/40 dark:text-slate-400 dark:border-slate-800",
    },
    [ACTIVE]: {
      label: "Activo",
      variant: "default",
      className:
        "bg-green-100 text-green-800 dark:bg-green-900/40 dark:text-green-400 hover:bg-green-100 dark:hover:bg-green-900/40",
    },
    [DRAFT]: {
      label: "Borrador",
      variant: "outline",
      className:
        "bg-indigo-50 text-indigo-700 border-indigo-200 dark:bg-indigo-900/20 dark:text-indigo-400 dark:border-indigo-900/50",
    },
  };

  const config = statusConfig[value] || {
    className: "",
    label: "Desconocido",
    variant: "secondary" as const,
  };

  return (
    <Badge
      variant={config.variant}
      className={`${config.className} ${className}`}
    >
      {config.label}
    </Badge>
  );
};

export default StatusBadge;
