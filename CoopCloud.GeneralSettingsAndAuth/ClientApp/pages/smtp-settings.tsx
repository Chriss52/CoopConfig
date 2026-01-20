import type { TSmtpSettingsFormValues } from "@/types";

import React from "react";
import { toast } from "sonner";
import { useForm } from "react-hook-form";
import { SmtpSettingsSuite } from "@/lib/suites";
import { vestResolver } from "@hookform/resolvers/vest";
import { SaveIcon, SendIcon, RotateCcwIcon } from "lucide-react";
import {
  Card,
  Button,
  FormGrid,
  CardTitle,
  CustomForm,
  PageHeader,
  CardHeader,
  CardContent,
  FormInputText,
  SeparatorText,
  FormInputSelect,
  CardDescription,
  FormInputPassword,
} from "@nubeteck/shadcn";

const MOCK_SMTP_SETTINGS: TSmtpSettingsFormValues = {
  port: 587,
  encryption: "tls",
  password: "********",
  host: "smtp.gmail.com",
  fromName: "Cooperativa",
  username: "noreply@cooperativa.com",
  fromEmail: "noreply@cooperativa.com",
};

const SmtpSettingsPage = () => {
  const [isTesting, setIsTesting] = React.useState(false);

  const form = useForm<TSmtpSettingsFormValues>({
    defaultValues: MOCK_SMTP_SETTINGS,
    resolver: vestResolver(SmtpSettingsSuite.smtpSettings),
  });
  const {
    reset,
    formState: { isDirty },
  } = form;

  const onSubmit = (data: TSmtpSettingsFormValues) => {
    // eslint-disable-next-line no-console
    console.log("SMTP Settings:", data);
    toast.success("Configuración SMTP guardada exitosamente");
    reset(data);
  };

  const handleReset = () => {
    reset();
    toast.info("Cambios descartados");
  };

  const handleTestConnection = async () => {
    setIsTesting(true);
    try {
      // Simulate API call
      await new Promise((resolve) => setTimeout(resolve, 2000));
      toast.success("Conexión SMTP exitosa. Correo de prueba enviado.");
    } catch (error) {
      toast.error("Error al conectar con el servidor SMTP");
    } finally {
      setIsTesting(false);
    }
  };

  return (
    <CustomForm form={form} onSubmit={onSubmit} className="space-y-4">
      <PageHeader
        title="Configuración de correo"
        subtitle="Configura el servidor SMTP para el envío de correos electrónicos"
        actions={
          <div className="flex items-center gap-2">
            <Button
              type="button"
              variant="outline"
              disabled={!isDirty}
              onClick={handleReset}
            >
              <RotateCcwIcon className="h-4 w-4" />
              Cancelar
            </Button>
            <Button
              type="button"
              variant="outline"
              disabled={isTesting}
              onClick={handleTestConnection}
            >
              <SendIcon className="h-4 w-4" />
              {isTesting ? "Probando..." : "Probar conexión"}
            </Button>
            <Button type="submit" disabled={!isDirty}>
              <SaveIcon className="h-4 w-4" />
              Guardar cambios
            </Button>
          </div>
        }
      />
      <Card className="shadow-none">
        <CardHeader>
          <CardTitle>Configuración de correo</CardTitle>
          <CardDescription>
            Configura el servidor SMTP para el envío de correos electrónicos
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-6">
            <FormGrid cols={2}>
              <FormInputText
                name="host"
                label="Host del servidor"
                placeholder="smtp.gmail.com"
              />
              <FormInputText name="port" label="Puerto" placeholder="587" />
              <FormInputSelect
                name="encryption"
                label="Encriptación"
                placeholder="Seleccionar encriptación"
                options={[
                  { value: "none", label: "Ninguna" },
                  { label: "SSL", value: "ssl" },
                  { label: "TLS", value: "tls" },
                ]}
              />
            </FormGrid>
            <SeparatorText text="Autenticación" />
            <FormGrid cols={2}>
              <FormInputText
                label="Usuario"
                name="username"
                placeholder="usuario@ejemplo.com"
              />
              <FormInputPassword
                name="password"
                label="Contraseña"
                placeholder="••••••••"
              />
            </FormGrid>
            <SeparatorText text="Información del remitente" />
            <FormGrid cols={2}>
              <FormInputText
                name="fromEmail"
                label="Correo remitente"
                placeholder="noreply@cooperativa.com"
              />
              <FormInputText
                name="fromName"
                label="Nombre remitente"
                placeholder="Cooperativa"
              />
            </FormGrid>
          </div>
        </CardContent>
      </Card>
    </CustomForm>
  );
};

export default SmtpSettingsPage;
