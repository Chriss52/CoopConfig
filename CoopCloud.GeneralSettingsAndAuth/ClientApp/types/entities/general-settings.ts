export interface IGeneralSettings {
  id: number;
  key: string;
  field: string;
  value: string;
  rules: string;
  isAdmin: boolean;
  dataTypeId: number;
  categoryId: number;
  description: string;
  isRequired: boolean;
  defaultValue: string;
  sourceId: null | number;
}
