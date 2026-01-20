import type { IGeneralSettings } from "@/types";

import BaseApi from "./base-api";

export default class GeneralSettingsApi extends BaseApi {
  constructor() {
    super("configurations");
  }

  public async getAll() {
    const result = await this.axios.get<IGeneralSettings[]>("");
    return result.data;
  }

  public async getById(id: string) {
    const result = await this.axios.get(`/${id}`);
    return result.data;
  }

  public async update(data: Partial<IGeneralSettings> & { id: number }) {
    const result = await this.axios.put<IGeneralSettings>(`/${data.id}`, data);
    return result.data;
  }
}
