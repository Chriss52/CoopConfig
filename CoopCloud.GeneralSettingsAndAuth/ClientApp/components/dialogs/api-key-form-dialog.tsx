import type { TApiKeyFormValues } from "@/types";

import React from "react";
import { Statuses } from "@/core";
import { useForm } from "react-hook-form";
import { ApiKeySuite } from "@/lib/suites";
import { vestResolver } from "@hookform/resolvers/vest";
import {
  FormDialog,
  FormInputText,
  FormInputTextArea,
  type IFormDialogProps,
} from "@nubeteck/shadcn";

export interface IApiKeyFormDialogProps extends Pick<
  IFormDialogProps<TApiKeyFormValues>,
  "onHide" | "visible"
> {
  onSubmitSuccess: (data: TApiKeyFormValues) => void | Promise<void>;
}

const defaultValues: TApiKeyFormValues = {
  name: "",
  description: "",
  statusId: Statuses.ACTIVE,
};

const ApiKeyFormDialog = ({
  onHide,
  onSubmitSuccess,
  ...props
}: IApiKeyFormDialogProps) => {
  const form = useForm<TApiKeyFormValues>({
    defaultValues,
    resolver: vestResolver(ApiKeySuite.apiKey),
  });

  const { reset } = form;

  const handleReset = React.useCallback(() => {
    reset(defaultValues);
  }, [reset]);

  return (
    <FormDialog
      {...props}
      modal
      form={form}
      noFixedHeight
      submitLabel="Generar API Key"
      title="Generar nueva API Key"
      subtitle="Esta llave permitirá acceso administrativo a la API."
      onHide={() => {
        handleReset();
        onHide?.();
      }}
      onSubmit={(values) => {
        onSubmitSuccess?.(values);
        handleReset();
        onHide?.();
      }}
    >
      <div className="space-y-4">
        <FormInputText
          name="name"
          label="Nombre de la llave"
          placeholder="Ej: Integración Facturación"
        />
        <FormInputTextArea
          rows={3}
          name="description"
          label="Descripción"
          placeholder="Descripción opcional de la llave..."
        />
      </div>
    </FormDialog>
  );
};

export default ApiKeyFormDialog;
