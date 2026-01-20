import type { ColumnDef } from "@tanstack/react-table";
import type { IWebhook, TWebhookFormValues } from "@/types";

import React from "react";
import { toast } from "sonner";
import { Statuses } from "@/core";
import { StatusBadge, WebhookFormDialog } from "@/components";
import { PlayIcon, PlusIcon, Edit2Icon, TrashIcon } from "lucide-react";
import {
  Button,
  PageHeader,
  renderEmpty,
  useTableState,
  AdvancedDataTable,
} from "@nubeteck/shadcn";

const MOCK_WEBHOOKS: IWebhook[] = [
  {
    id: "1",
    method: "POST",
    category: "Préstamos",
    event: "loan.created",
    statusId: Statuses.ACTIVE,
    url: "https://api.negocio.com/webhooks/loans",
  },
  {
    id: "2",
    method: "POST",
    category: "Pagos",
    event: "payment.received",
    statusId: Statuses.INACTIVE,
    url: "https://erp.interno.com/sync",
  },
];

const AVAILABLE_EVENTS = [
  { value: "loan.created", label: "Préstamo Creado" },
  { value: "loan.updated", label: "Préstamo Actualizado" },
  { value: "loan.approved", label: "Préstamo Aprobado" },
  { label: "Pago Recibido", value: "payment.received" },
  { label: "Pago Vencido", value: "payment.overdue" },
  { label: "Cliente Nuevo", value: "customer.created" },
  { value: "customer.updated", label: "Cliente Actualizado" },
];

const WebhooksPage = () => {
  const {
    setSearch,
    tableState,
    setFilters,
    setSorting,
    setPagination,
    setJoinOperator,
  } = useTableState<IWebhook>([{ id: "id", desc: true }]);

  const [webhooks, setWebhooks] = React.useState<IWebhook[]>(MOCK_WEBHOOKS);
  const [isDialogOpen, setIsDialogOpen] = React.useState(false);
  const [editingWebhook, setEditingWebhook] = React.useState<null | IWebhook>(
    null,
  );

  const handleSave = (data: TWebhookFormValues) => {
    if (editingWebhook) {
      setWebhooks(
        webhooks.map((w) =>
          w.id === editingWebhook.id ? { ...w, ...data } : w,
        ),
      );
      toast.success("Webhook actualizado");
    } else {
      const newWebhook: IWebhook = {
        ...data,
        id: Math.random().toString(),
      };
      setWebhooks([...webhooks, newWebhook]);
      toast.success("Webhook creado");
    }
    setEditingWebhook(null);
  };

  const handleEdit = (webhook: IWebhook) => {
    setEditingWebhook(webhook);
    setIsDialogOpen(true);
  };

  const handleDelete = (webhook: IWebhook) => {
    setWebhooks(webhooks.filter((w) => w.id !== webhook.id));
    toast.success("Webhook eliminado");
  };

  const handleTest = (webhook: IWebhook) => {
    toast.promise(new Promise((resolve) => setTimeout(resolve, 1000)), {
      error: "Error al enviar evento",
      loading: "Enviando evento de prueba...",
      success: `Evento de prueba enviado a ${webhook.url} (200 OK)`,
    });
  };

  const columns = React.useMemo<ColumnDef<IWebhook>[]>(
    () => [
      {
        id: "event",
        header: "Evento",
        accessorKey: "event",
        enableColumnFilter: true,
        meta: { variant: "text", label: "Evento" },
        cell: ({ row }) => {
          const event = row.getValue("event");
          return (
            AVAILABLE_EVENTS.find((e) => e.value === event)?.label || event
          );
        },
      },
      {
        id: "category",
        header: "Categoria",
        accessorKey: "category",
        enableColumnFilter: true,
        meta: { variant: "text", label: "Categoria" },
        cell: ({ row }) => renderEmpty(row.getValue("category")),
      },
      {
        id: "url",
        header: "URL",
        accessorKey: "url",
        enableColumnFilter: true,
        meta: { label: "URL", variant: "text" },
        cell: ({ row }) => renderEmpty(row.getValue("url")),
      },
      {
        id: "method",
        header: "Método",
        accessorKey: "method",
        enableColumnFilter: true,
        meta: { variant: "text", label: "Método" },
        cell: ({ row }) => renderEmpty(row.getValue("method")),
      },
      {
        id: "statusId",
        header: "Estado",
        enableSorting: false,
        accessorKey: "statusId",
        enableColumnFilter: true,
        cell: ({ row }) => <StatusBadge value={row.getValue("statusId")} />,
        meta: {
          options: [],
          label: "Estado",
          variant: "select",
        },
      },
    ],
    [],
  );

  return (
    <>
      <PageHeader
        title="Webhooks"
        subtitle="Configura webhooks para notificar a sistemas externos sobre eventos."
        actions={
          <Button
            onClick={() => {
              setEditingWebhook(null);
              setIsDialogOpen(true);
            }}
          >
            <PlusIcon className="h-4 w-4" />
            Agregar Webhook
          </Button>
        }
      />
      <AdvancedDataTable
        // {...pagination}
        data={webhooks}
        columns={columns}
        onSearch={setSearch}
        onSorting={setSorting}
        // refreshing={refreshing}
        getRowId={(row) => row.id}
        sorting={tableState.sorting}
        onPagination={setPagination}
        // isPending={isPeriodsPending}
        onAdvancedFilters={setFilters}
        searchValue={tableState.search}
        // onRefresh={() => refetchPeriods()}
        advancedFilters={tableState.filters}
        joinOperator={tableState.joinOperator}
        onJoinOperatorChange={setJoinOperator}
        actions={[
          { icon: PlayIcon, label: "Probar", onClick: handleTest },
          { icon: Edit2Icon, label: "Editar", onClick: handleEdit },
          { icon: TrashIcon, label: "Eliminar", onClick: handleDelete },
        ]}
      />
      <WebhookFormDialog
        visible={isDialogOpen}
        onSubmitSuccess={handleSave}
        onHide={() => setIsDialogOpen(false)}
        webhook={editingWebhook || undefined}
      />
    </>
  );
};

export default WebhooksPage;
