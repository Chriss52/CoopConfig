import { GeneralSettingsApi } from "@/api";
import { useQueryBuilder } from "@nubeteck/shadcn";

export const GENERAL_SETTINGS_QUERY_KEY = "GeneralSettings";
export const GENERAL_SETTINGS_ID = "1";

export const useGetQuery = () => {
  const api = new GeneralSettingsApi();
  return useQueryBuilder({
    queryFn: () => api.getAll(),
    queryKey: [GENERAL_SETTINGS_QUERY_KEY],
  });
};

export const useGetByIdQuery = (id: string) => {
  const api = new GeneralSettingsApi();
  return useQueryBuilder({
    options: { enabled: !!id },
    queryFn: () => api.getById(id),
    queryKey: [GENERAL_SETTINGS_QUERY_KEY, id],
  });
};

export const useGetValueQuery = (id: string = GENERAL_SETTINGS_ID) => {
  const api = new GeneralSettingsApi();
  return useQueryBuilder({
    queryFn: () => api.getById(id),
    queryKey: [GENERAL_SETTINGS_QUERY_KEY, id],
    options: {
      enabled: !!id,
      select: (data) => {
        if (!data?.value) return null;
        try {
          return JSON.parse(data.value);
        } catch (error) {
          console.error(
            `Error parsing GeneralSettings value for ID ${id}:`,
            error,
          );
          return null;
        }
      },
    },
  });
};
