import type { IRouteDefinition } from "@/types";

import { v4 as uuidv4 } from "uuid";

const createRoute = <P extends string>(
  path: P,
  label: string,
): IRouteDefinition<P> => ({ path, label, id: uuidv4() });

export const ROUTES = {
  SETTINGS: createRoute("/settings", "Configuraciones"),
  WEBHOOKS: createRoute("/settings/webhooks", "Webhooks"),
  API_KEYS: createRoute("/settings/api-keys", "API Keys"),
  SMTP: createRoute("/settings/smtp", "Correo electrónico"),
};
