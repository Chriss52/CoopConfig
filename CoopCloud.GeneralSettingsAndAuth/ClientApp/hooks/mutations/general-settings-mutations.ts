import type { IGeneralSettings } from "@/types";

import { GeneralSettingsApi } from "@/api";
import { useMutationBuilder } from "@nubeteck/shadcn";

import { GENERAL_SETTINGS_QUERY_KEY } from "../queries/general-settings-queries";

export const useUpdateMutation = () => {
  const api = new GeneralSettingsApi();
  return useMutationBuilder<
    IGeneralSettings,
    Partial<IGeneralSettings> & { id: number }
  >({
    mutationFn: (data) => api.update(data),
    options: {
      onSuccess: async (data, variables, onMutateResult, context) => {
        if (data?.id) {
          context.client.invalidateQueries({
            queryKey: [GENERAL_SETTINGS_QUERY_KEY, data.id.toString()],
          });
        }
      },
    },
  });
};
