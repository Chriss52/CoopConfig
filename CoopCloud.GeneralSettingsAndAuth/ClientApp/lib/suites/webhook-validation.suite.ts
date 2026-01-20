import type { TWebhookFormValues } from "@/types";

import { test, create, enforce } from "vest";

export const webhook = create((data: TWebhookFormValues) => {
  test("event", "El evento es requerido", () => {
    enforce(data.event).isNotEmpty();
  });

  test("category", "La categoría es requerida", () => {
    enforce(data.category).isNotEmpty();
  });

  test("url", "La URL es requerida", () => {
    enforce(data.url).isNotEmpty();
  });

  test("url", "La URL debe ser válida", () => {
    enforce(data.url).matches(/^https?:\/\/.+/);
  });

  test("method", "El método HTTP es requerido", () => {
    enforce(data.method).isNotEmpty();
  });

  test("statusId", "El estado es requerido", () => {
    enforce(data.statusId).isNumber();
    enforce(data.statusId).isNotBlank();
  });

  // Validate headers if present
  if (data.headers && data.headers.length > 0) {
    data.headers.forEach((header, index) => {
      test(`headers.${index}.field`, "El campo es requerido", () => {
        enforce(header.field).isNotEmpty();
      });

      test(`headers.${index}.value`, "El valor es requerido", () => {
        enforce(header.value).isNotEmpty();
      });
    });
  }
});
