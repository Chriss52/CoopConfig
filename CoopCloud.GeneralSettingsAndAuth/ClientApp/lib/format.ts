export function formatCurrency(
  value: null | number | undefined,
  options: Intl.NumberFormatOptions = {},
) {
  if (value === null || value === undefined) {
    return "N/A";
  }

  const { currency = "DOP", ...rest } = options;

  return value.toLocaleString("es-DO", {
    currency,
    style: "currency",
    minimumFractionDigits: 2,
    ...rest,
  });
}
