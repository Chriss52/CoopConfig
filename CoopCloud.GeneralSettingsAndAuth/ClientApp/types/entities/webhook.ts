export interface IWebhookHeader {
  field: string;
  value: string;
}

export interface IWebhook {
  id: string;
  url: string;
  event: string;
  category: string;
  statusId: number;
  headers?: IWebhookHeader[];
  method: "GET" | "PUT" | "POST" | "PATCH" | "DELETE";
}

export type TWebhookFormValues = Omit<IWebhook, "id">;
