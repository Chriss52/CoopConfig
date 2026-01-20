import type { IRouteDefinition } from "@/types";

export function isRouteActive(
  pathname: null | string,
  route: string | IRouteDefinition<string>,
): boolean {
  if (!pathname) return false;
  const routePath = typeof route === "string" ? route : route.path;
  if (pathname === routePath) return true;
  if (routePath === "/") return false;
  const segments = routePath.split("/").filter(Boolean);
  if (segments.length === 1) return false;
  if (segments.length === 2 && segments[1] && routePath.endsWith(segments[1])) {
    if (
      segments[1].toLowerCase() !== "base" &&
      !pathname.startsWith(routePath + "/")
    ) {
      return false;
    }
  }
  return pathname.startsWith(routePath + "/");
}
