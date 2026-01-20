import type { IUser } from "@/types";

import BaseApi from "./base-api";

export default class UsersApi extends BaseApi {
  constructor() {
    super("users");
  }

  public async getAll() {
    const result = await this.axios.get<IUser[]>("");
    return result.data;
  }

  public async getById(id: string) {
    const result = await this.axios.get<IUser>(`/${id}`);
    return result.data;
  }
}
