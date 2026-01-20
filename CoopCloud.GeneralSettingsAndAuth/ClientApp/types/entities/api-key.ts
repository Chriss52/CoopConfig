export interface IApiKey {
  id: string;
  name: string;
  createdAt: Date;
  statusId: number;
  description?: string;
}

export type TApiKeyFormValues = Omit<IApiKey, "id" | "createdAt">;
