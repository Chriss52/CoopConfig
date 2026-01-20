import type { ColumnDef } from "@tanstack/react-table";
import type { IApiKey, TApiKeyFormValues } from "@/types";

import React from "react";
import dayjs from "dayjs";
import { toast } from "sonner";
import { Statuses } from "@/core";
import { useConfirm } from "@/hooks";
import { StatusBadge } from "@/components";
import { PlusIcon, PowerIcon, RefreshCwIcon } from "lucide-react";
import { ApiKeyFormDialog, ApiKeyDisplayDialog } from "@/components/dialogs";
import {
  Button,
  PageHeader,
  renderEmpty,
  useTableState,
  AdvancedDataTable,
} from "@nubeteck/shadcn";

const MOCK_KEYS: IApiKey[] = [
  {
    id: "1",
    statusId: Statuses.ACTIVE,
    name: "Servidor de Producción",
    createdAt: new Date("2024-01-15T10:00:00"),
    description: "Llave para el servidor de producción",
  },
  {
    id: "2",
    statusId: Statuses.ACTIVE,
    name: "Entorno de Pruebas",
    createdAt: new Date("2024-02-01T09:00:00"),
  },
];

const ApiKeysPage = () => {
  const { confirm } = useConfirm();

  const {
    setSearch,
    setFilters,
    setSorting,
    tableState,
    setPagination,
    setJoinOperator,
  } = useTableState<IApiKey>([{ desc: true, id: "createdAt" }]);

  const [apiKeys, setApiKeys] = React.useState<IApiKey[]>(MOCK_KEYS);
  const [createdKey, setCreatedKey] = React.useState<null | string>(null);
  const [isFormOpen, setIsFormOpen] = React.useState(false);

  const handleCreate = (data: TApiKeyFormValues) => {
    const newKey: IApiKey = {
      ...data,
      createdAt: new Date(),
      id: Math.random().toString(),
    };
    setApiKeys([newKey, ...apiKeys]);

    // Simulate generating a full secret key
    const generatedKey = `sk_live_${Math.random().toString(36).substring(7)}`;
    setCreatedKey(generatedKey);
    toast.success("API Key generada exitosamente");
  };

  const handleDeactivate = (apiKey: IApiKey) => {
    setApiKeys(
      apiKeys.map((k) =>
        k.id === apiKey.id ? { ...k, statusId: Statuses.INACTIVE } : k,
      ),
    );
    toast.success("API Key desactivada");
  };

  const handleRegenerate = async (apiKey: IApiKey) => {
    const confirmed = await confirm({
      variant: "destructive",
      cancelButton: "Cancelar",
      actionButton: "Regenerar",
      title: "Regenerar API Key",
      body: `¿Estás seguro de que deseas regenerar la API Key "${apiKey.name}"? La llave anterior dejará de funcionar inmediatamente.`,
    });

    if (confirmed) {
      const generatedKey = `sk_live_${Math.random().toString(36).substring(7)}`;
      setCreatedKey(generatedKey);
      toast.success("API Key regenerada exitosamente");
    }
  };

  const columns = React.useMemo<ColumnDef<IApiKey>[]>(
    () => [
      {
        id: "name",
        header: "Nombre",
        accessorKey: "name",
        enableColumnFilter: true,
        meta: { label: "Nombre", variant: "text" },
        cell: ({ row }) => renderEmpty(row.getValue("name")),
      },
      {
        id: "description",
        header: "Descripción",
        enableColumnFilter: true,
        accessorKey: "description",
        meta: { variant: "text", label: "Descripción" },
        cell: ({ row }) => renderEmpty(row.getValue("description")),
      },
      {
        id: "statusId",
        header: "Estado",
        enableSorting: false,
        accessorKey: "statusId",
        enableColumnFilter: true,
        meta: { options: [], label: "Estado", variant: "select" },
        cell: ({ row }) => <StatusBadge value={row.getValue("statusId")} />,
      },
      {
        id: "createdAt",
        accessorKey: "createdAt",
        enableColumnFilter: true,
        header: "Fecha de creación",
        meta: { variant: "date", label: "Fecha de creación" },
        cell: ({ row }) => dayjs(row.getValue("createdAt")).format("L"),
      },
    ],
    [],
  );

  return (
    <>
      <PageHeader
        title="API Keys"
        subtitle="Gestiona las llaves de acceso para la API pública."
        actions={
          <Button onClick={() => setIsFormOpen(true)}>
            <PlusIcon className="h-4 w-4" />
            Nueva API Key
          </Button>
        }
      />
      <AdvancedDataTable
        data={apiKeys}
        columns={columns}
        onSearch={setSearch}
        onSorting={setSorting}
        getRowId={(row) => row.id}
        onPagination={setPagination}
        sorting={tableState.sorting}
        onAdvancedFilters={setFilters}
        searchValue={tableState.search}
        advancedFilters={tableState.filters}
        joinOperator={tableState.joinOperator}
        onJoinOperatorChange={setJoinOperator}
        actions={[
          {
            icon: PowerIcon,
            label: "Desactivar",
            onClick: handleDeactivate,
          },
          {
            icon: RefreshCwIcon,
            label: "Regenerar token",
            onClick: handleRegenerate,
          },
        ]}
      />
      <ApiKeyFormDialog
        visible={isFormOpen}
        onSubmitSuccess={handleCreate}
        onHide={() => setIsFormOpen(false)}
      />
      <ApiKeyDisplayDialog
        apiKey={createdKey}
        onClose={() => setCreatedKey(null)}
      />
    </>
  );
};

export default ApiKeysPage;
