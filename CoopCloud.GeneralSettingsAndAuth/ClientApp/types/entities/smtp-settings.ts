export interface ISmtpSettings {
  host: string;
  port: number;
  fromName: string;
  password: string;
  username: string;
  fromEmail: string;
  encryption: "ssl" | "tls" | "none";
}

export type TSmtpSettingsFormValues = ISmtpSettings;
