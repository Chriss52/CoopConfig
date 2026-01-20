import type { LucideIcon } from "lucide-react";

export interface IMenuItem {
  id: string;
  href: string;
  label: string;
  icon: LucideIcon;
  basePath?: string;
  children?: IMenuItem[];
}

export interface IRouteDefinition<P extends string = string> {
  readonly path: P;
  readonly id: string;
  readonly label: string;
}

export type TPaginated<T> = {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
};
