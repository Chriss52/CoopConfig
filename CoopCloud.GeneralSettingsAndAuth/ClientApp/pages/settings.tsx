import React from "react";
import { toast } from "sonner";
import { useConfirm } from "@/hooks";
import { useForm } from "react-hook-form";
import { GeneralSettingsSuite } from "@/lib/suites";
import { SaveIcon, RotateCcwIcon } from "lucide-react";
import { vestResolver } from "@hookform/resolvers/vest";
import { type GeneralSettingsFormValues } from "@/lib/suites/general-settings-validation.suite";
import {
  Card,
  Button,
  FormGrid,
  CustomForm,
  PageHeader,
  CardContent,
  SwitchSection,
  FormInputTimer,
  FormInputSelect,
} from "@nubeteck/shadcn";

const SettingsPage = () => {
  const { confirm } = useConfirm();

  const form = useForm<GeneralSettingsFormValues>({
    resolver: vestResolver(GeneralSettingsSuite.generalSettings),
    defaultValues: {
      dayCountBaseId: "",
      roundingRuleId: "",
      transactionCutoffTime: "",
      shouldAccrueInterestOnHolidays: false,
    },
  });

  const {
    reset,
    formState: { isDirty },
  } = form;

  const handleSubmit = async (data: GeneralSettingsFormValues) => {
    const confirmed = await confirm({
      variant: "destructive",
      cancelButton: "Cancelar",
      actionButton: "Guardar cambios",
      title: "Confirmar cambios globales",
      body: "Está a punto de modificar los parámetros generales del sistema. Estos cambios afectarán globalmente a todos los cálculos, operaciones y transacciones. ¿Desea continuar?",
    });

    if (confirmed) {
      try {
        // Aquí iría la lógica para guardar los datos
        // eslint-disable-next-line no-console
        console.log("Datos a guardar:", data);
        toast.success("Cambios guardados exitosamente");
        reset(data);
      } catch (error) {
        toast.error("Error al guardar cambios");
        console.error("Settings update error:", error);
      }
    }
  };

  const handleReset = async () => {
    const confirmed = await confirm({
      cancelButton: "Volver",
      variant: "destructive",
      title: "Deshacer cambios",
      actionButton: "Deshacer cambios",
      body: "¿Estás seguro de que quieres descartar los cambios no guardados y volver a los valores originales?",
    });

    if (confirmed) {
      reset();
    }
  };

  return (
    <CustomForm form={form} onSubmit={handleSubmit} className="mb-4 space-y-4">
      <PageHeader
        title="Parámetros generales del sistema"
        subtitle="Configura los parámetros generales que afectan a toda la cooperativa"
        actions={
          <div className="flex items-center gap-2">
            <Button
              type="button"
              variant="outline"
              disabled={!isDirty}
              onClick={handleReset}
            >
              <RotateCcwIcon className="mr-2 h-4 w-4" />
              Cancelar
            </Button>
            <Button type="submit" disabled={!isDirty}>
              <SaveIcon className="mr-2 h-4 w-4" />
              Guardar cambios
            </Button>
          </div>
        }
      />
      <Card className="shadow-none">
        <CardContent className="pt-6">
          <FormGrid cols={2}>
            <FormInputSelect
              name="dayCountBaseId"
              label="Bases del dia del año"
              placeholder="Seleccionar base"
              options={[
                { value: "1", label: "30/360" },
                { value: "2", label: "Actual/360" },
                { value: "3", label: "Actual/365" },
              ]}
            />
            <FormInputTimer
              name="transactionCutoffTime"
              placeholder="Seleccionar la hora"
              label="Hora de corte de transacciones"
              description="Las transacciones realizadas después de esta hora se procesarán al siguiente día hábil"
            />
            <FormInputSelect
              name="roundingRuleId"
              label="Método de redondeo"
              placeholder="Seleccionar método"
              description="Método de redondeo aplicado a intereses y cuotas"
              options={[
                { value: "1", label: "Redondeo estándar" },
                { value: "2", label: "Redondeo hacia arriba" },
                { value: "3", label: "Redondeo hacia abajo" },
              ]}
            />
            <SwitchSection
              name="shouldAccrueInterestOnHolidays"
              label="Devengar intereses en días feriados"
              description="Si se activa, los días feriados generarán intereses normalmente"
            />
          </FormGrid>
        </CardContent>
      </Card>
    </CustomForm>
  );
};

export default SettingsPage;
