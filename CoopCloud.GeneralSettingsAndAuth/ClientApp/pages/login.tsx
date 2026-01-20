import type { ILoginRequest } from "@/api/auth-api";

import React from "react";
import { toast } from "sonner";
import { Mutations } from "@/hooks";
import { useForm } from "react-hook-form";
import { Loader2, LogInIcon } from "lucide-react";
import SidebarLogo from "@/components/sidebar-logo";
import {
  Card,
  Button,
  CardTitle,
  CardHeader,
  CustomForm,
  CardContent,
  FormInputText,
  CardDescription,
  FormInputPassword,
} from "@nubeteck/shadcn";

const LoginPage = () => {
  const form = useForm<ILoginRequest>({
    defaultValues: {
      email: "",
      password: "",
    },
  });

  const { isPending, mutateAsync: login } = Mutations.Auth.useLoginMutation();

  const handleSubmit = async (data: ILoginRequest) => {
    try {
      await login(data);
      toast.success("Inicio de sesión exitoso");
    } catch (error) {
      toast.error("Error al iniciar sesión. Verifica tus credenciales.");
    }
  };

  return (
    <div className="bg-muted flex min-h-svh flex-col items-center justify-center gap-6 p-6 md:p-10">
      <div className="flex w-full max-w-sm flex-col gap-6">
        <a href="#" className="flex items-center gap-3 self-center font-medium">
          <SidebarLogo />
          Coop Cloud
        </a>
        <div className="flex flex-col gap-6">
          <Card className="w-full max-w-md">
            <CardHeader className="text-center">
              <CardTitle className="text-xl">Bienvenido de nuevo</CardTitle>
              <CardDescription>
                Ingresa tu correo electrónico y contraseña para acceder
              </CardDescription>
            </CardHeader>
            <CardContent>
              <CustomForm form={form} onSubmit={handleSubmit}>
                <div className="space-y-4">
                  <FormInputText
                    required
                    name="email"
                    label="Correo Electrónico"
                    placeholder="correo@ejemplo.com"
                  />
                  <FormInputPassword
                    required
                    name="password"
                    label="Contraseña"
                    placeholder="••••••••"
                  />
                  <Button type="submit" className="w-full" disabled={isPending}>
                    {isPending ? (
                      <>
                        <Loader2 className="h-4 w-4 animate-spin" />
                        Iniciando sesión...
                      </>
                    ) : (
                      <>
                        <LogInIcon className="h-4 w-4" />
                        Iniciar Sesión
                      </>
                    )}
                  </Button>
                </div>
              </CustomForm>
            </CardContent>
          </Card>
          <div className="text-center text-sm text-muted-foreground [&_a]:underline [&_a]:underline-offset-4 hover:[&_a]:text-foreground">
            ¿No tienes cuenta?{" "}
            <a href="mailto:soporte@coopcloud.com">Contáctanos</a>
          </div>
        </div>
      </div>
    </div>
  );
};

export default LoginPage;
