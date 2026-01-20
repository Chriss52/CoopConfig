import type { TWebhookFormValues } from "@/types";

import React from "react";
import { Statuses } from "@/core";
import { TrashIcon } from "lucide-react";
import { WebhookSuite } from "@/lib/suites";
import { vestResolver } from "@hookform/resolvers/vest";
import { useForm, useFieldArray } from "react-hook-form";
import {
  Label,
  Button,
  FormGrid,
  FormDialog,
  FormInputText,
  SwitchSection,
  FormInputSelect,
  type IFormDialogProps,
} from "@nubeteck/shadcn";

export interface IWebhookFormDialogProps extends Pick<
  IFormDialogProps<TWebhookFormValues>,
  "onHide" | "visible"
> {
  webhook?: TWebhookFormValues & { id: string };
  onSubmitSuccess: (data: TWebhookFormValues) => void | Promise<void>;
}

const AVAILABLE_EVENTS = [
  { value: "loan.created", label: "Préstamo Creado" },
  { value: "loan.updated", label: "Préstamo Actualizado" },
  { value: "loan.approved", label: "Préstamo Aprobado" },
  { label: "Pago Recibido", value: "payment.received" },
  { label: "Pago Vencido", value: "payment.overdue" },
  { label: "Cliente Nuevo", value: "customer.created" },
  { value: "customer.updated", label: "Cliente Actualizado" },
];

const AVAILABLE_CATEGORIES = [
  { label: "Préstamos", value: "Préstamos" },
  { label: "Pagos", value: "Pagos" },
  { label: "Clientes", value: "Clientes" },
  { label: "Notificaciones", value: "Notificaciones" },
  { label: "Reportes", value: "Reportes" },
];

const AVAILABLE_METHODS = [
  { label: "GET", value: "GET" },
  { label: "POST", value: "POST" },
  { label: "PUT", value: "PUT" },
  { label: "PATCH", value: "PATCH" },
  { label: "DELETE", value: "DELETE" },
];

const defaultValues: TWebhookFormValues = {
  url: "",
  event: "",
  headers: [],
  category: "",
  method: "POST",
  statusId: Statuses.ACTIVE,
};

const WebhookFormDialog = ({
  onHide,
  webhook,
  onSubmitSuccess,
  ...props
}: IWebhookFormDialogProps) => {
  const form = useForm<TWebhookFormValues>({
    defaultValues: webhook || defaultValues,
    resolver: vestResolver(WebhookSuite.webhook),
  });

  const { reset, control } = form;

  const { append, fields, remove } = useFieldArray({
    control,
    name: "headers",
  });

  React.useEffect(() => {
    if (webhook) {
      reset(webhook);
    }
  }, [reset, webhook]);

  const handleReset = React.useCallback(() => {
    reset(defaultValues);
  }, [reset]);

  return (
    <FormDialog
      {...props}
      modal
      form={form}
      className="min-w-2xl"
      title={webhook ? "Editar webhook" : "Nuevo webhook"}
      submitLabel={webhook ? "Guardar cambios" : "Crear webhook"}
      subtitle="Configura los detalles del endpoint de notificación."
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
        <FormGrid gap={3} cols={2}>
          <FormInputSelect
            name="event"
            label="Evento"
            options={AVAILABLE_EVENTS}
            placeholder="Seleccionar evento"
          />
          <FormInputSelect
            name="category"
            label="Categoría"
            options={AVAILABLE_CATEGORIES}
            placeholder="Seleccionar categoría"
          />
          <FormInputSelect
            name="method"
            label="Método HTTP"
            options={AVAILABLE_METHODS}
            placeholder="Seleccionar método"
          />
        </FormGrid>
        <FormInputText name="url" label="URL" placeholder="https://..." />
        <SwitchSection
          label="Activo"
          name="isActive"
          description="El webhook estará habilitado para recibir eventos"
        />
        <div className="space-y-3">
          <div className="flex items-center justify-between">
            <Label>Headers</Label>
            <Button
              size="sm"
              type="button"
              variant="outline"
              onClick={() => append({ field: "", value: "" })}
            >
              Agregar Header
            </Button>
          </div>
          {fields.length > 0 && (
            <div className="space-y-2">
              {fields.map((field, index) => (
                <div key={field.id} className="flex items-end gap-2">
                  <FormInputText
                    label=""
                    fieldClassname="flex-1"
                    placeholder="Content-Type"
                    name={`headers.${index}.field`}
                  />
                  <FormInputText
                    label=""
                    fieldClassname="flex-1"
                    placeholder="application/json"
                    name={`headers.${index}.value`}
                  />
                  <Button
                    size="icon"
                    type="button"
                    variant="ghost"
                    onClick={() => remove(index)}
                  >
                    <TrashIcon className="h-4 w-4" />
                  </Button>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </FormDialog>
  );
};

export default WebhookFormDialog;
