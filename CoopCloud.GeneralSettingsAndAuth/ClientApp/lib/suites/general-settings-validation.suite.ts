import { test, create, enforce } from "vest";

export type GeneralSettingsFormValues = {
  dayCountBaseId: string;
  roundingRuleId: string;
  transactionCutoffTime?: string;
  shouldAccrueInterestOnHolidays: boolean;
};

export const generalSettings = create((data: GeneralSettingsFormValues) => {
  test("dayCountBaseId", "Debe seleccionar una base de días", () => {
    enforce(data.dayCountBaseId).isNotBlank();
  });

  test("roundingRuleId", "Debe seleccionar una regla de redondeo", () => {
    enforce(data.roundingRuleId).isNotBlank();
  });

  // transactionCutoffTime is optional in Zod schema, so no strict enforcement here unless needed.
  // shouldAccrueInterestOnHolidays is boolean, usually doesn't need 'NotBlank' enforcement.
});
