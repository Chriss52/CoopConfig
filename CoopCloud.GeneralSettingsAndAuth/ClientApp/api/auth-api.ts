import BaseApi from "./base-api";

export interface ILoginRequest {
  email: string;
  password: string;
}

export interface ILoginResponse {
  token: { token: string };
}

class AuthApi extends BaseApi {
  constructor() {
    super("auth");
  }

  async login(data: ILoginRequest): Promise<ILoginResponse> {
    const response = await this.axios.post<ILoginResponse>("login", data);
    return response.data;
  }
}

export default new AuthApi();
