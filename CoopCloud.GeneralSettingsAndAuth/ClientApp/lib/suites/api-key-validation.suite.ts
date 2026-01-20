import type { TApiKeyFormValues } from "@/types";

import { test, create, enforce } from "vest";

export const apiKey = create((data: TApiKeyFormValues) => {
  test("name", "El nombre es requerido", () => {
    enforce(data.name).isNotEmpty();
  });

  test("name", "El nombre debe tener al menos 3 caracteres", () => {
    enforce(data.name).longerThanOrEquals(3);
  });

  test("statusId", "El estado es requerido", () => {
    enforce(data.statusId).isNumber();
    enforce(data.statusId).isNotBlank();
  });
});
