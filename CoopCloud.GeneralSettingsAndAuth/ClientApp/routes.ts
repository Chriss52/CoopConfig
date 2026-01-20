import {
  route,
  index,
  layout,
  prefix,
  type RouteConfig,
} from "@react-router/dev/routes";

export default [
  route("*", "./pages/not-found.tsx"),
  route("login", "./pages/login.tsx"),
  index("./pages/navigate.tsx"),
  layout("./layouts/main-layout.tsx", [
    ...prefix("settings", [
      index("./pages/settings.tsx"),
      route("webhooks", "./pages/webhooks.tsx"),
      route("api-keys", "./pages/api-keys.tsx"),
      route("smtp", "./pages/smtp-settings.tsx"),
    ]),
  ]),
] satisfies RouteConfig;
