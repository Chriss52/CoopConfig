import type { TSmtpSettingsFormValues } from "@/types";

import { test, create, enforce } from "vest";

export const smtpSettings = create((data: TSmtpSettingsFormValues) => {
  test("host", "El host es requerido", () => {
    enforce(data.host).isNotEmpty();
  });

  test("port", "El puerto es requerido", () => {
    enforce(data.port).isNumber();
    enforce(data.port).greaterThan(0);
  });

  test("port", "El puerto debe estar entre 1 y 65535", () => {
    enforce(data.port).lessThanOrEquals(65535);
  });

  test("username", "El usuario es requerido", () => {
    enforce(data.username).isNotEmpty();
  });

  test("password", "La contraseña es requerida", () => {
    enforce(data.password).isNotEmpty();
  });

  test("fromEmail", "El correo remitente es requerido", () => {
    enforce(data.fromEmail).isNotEmpty();
  });

  test("fromEmail", "Debe ser un correo válido", () => {
    enforce(data.fromEmail).matches(/^[^\s@]+@[^\s@]+\.[^\s@]+$/);
  });

  test("fromName", "El nombre remitente es requerido", () => {
    enforce(data.fromName).isNotEmpty();
  });

  test("encryption", "El tipo de encriptación es requerido", () => {
    enforce(data.encryption).isNotEmpty();
  });
});
