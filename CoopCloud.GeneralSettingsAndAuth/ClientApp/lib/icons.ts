import type { LucideIcon } from "lucide-react";

import * as LucideIcons from "lucide-react";

/**
 * Busca y retorna un icono de lucide-react basándose en el nombre proporcionado
 * @param iconName - El nombre del icono en PascalCase (ej: "Home", "User", "Settings")
 * @returns El componente del icono de Lucide o undefined si no se encuentra
 */
export const findLucideIcon = (iconName: string): undefined | LucideIcon => {
  const pascalCaseName = iconName.charAt(0).toUpperCase() + iconName.slice(1);
  const icon = (LucideIcons as Record<string, unknown>)[pascalCaseName];
  if (typeof icon === "object") {
    return icon as LucideIcon;
  }
  return undefined;
};

/**
 * Crea un icono de lucide-react basándose en el nombre proporcionado
 * @param iconName - El nombre del icono en PascalCase (ej: "Home", "User", "Settings")
 * @returns El componente del icono de Lucide o un icono por defecto si no se encuentra
 */
export const createIcon = (iconName: string): LucideIcon => {
  const icon = findLucideIcon(iconName);
  if (!icon) {
    console.warn(
      `Icon "${iconName}" not found in lucide-react. Using default icon.`,
    );
    return LucideIcons.HelpCircle;
  }
  return icon;
};
