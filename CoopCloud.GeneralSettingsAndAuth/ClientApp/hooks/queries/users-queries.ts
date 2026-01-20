import { UsersApi } from "@/api";
import { useQueryBuilder } from "@nubeteck/shadcn";

export const USERS_QUERY_KEY = "Users";

export const useGetQuery = () => {
  return useQueryBuilder({
    queryKey: [USERS_QUERY_KEY],
    // queryFn: () => api.getAll(),
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    queryFn: async () => [] as any[],
  });
};

export const useGetByIdQuery = (id: string) => {
  const api = new UsersApi();
  return useQueryBuilder({
    options: { enabled: !!id },
    queryFn: () => api.getById(id),
    queryKey: [USERS_QUERY_KEY, id],
  });
};

export const useGetSelectQuery = () => {
  const query = useGetQuery();
  return {
    isUsersPending: query.isPending,
    users:
      query.data?.map((user) => ({
        id: user.id,
        value: user.id,
        label: user.fullName,
        role: user.roles?.[0]?.name || "",
      })) ?? [],
  };
};
