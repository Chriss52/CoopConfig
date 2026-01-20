import type { ILoginRequest, ILoginResponse } from "@/api/auth-api";

import { AuthApi } from "@/api";
import { useNavigate } from "react-router";
import { useMutationBuilder } from "@nubeteck/shadcn";

export const useLoginMutation = () => {
  const navigate = useNavigate();
  return useMutationBuilder<ILoginResponse, ILoginRequest>({
    mutationFn: (data) => AuthApi.login(data),
    options: {
      onSuccess: (data) => {
        localStorage.setItem("token", data.token.token);
        navigate("/");
      },
    },
  });
};
