import type { AxiosInstance, CreateAxiosDefaults } from "axios";

import Axios from "axios";

const SERVER_URL = process.env.VITE_NUBETECK_SERVER_URL;

if (!SERVER_URL) {
  throw new Error("Please add your Server URL to environment variables.");
}

export type AxiosOptions = Omit<CreateAxiosDefaults, "baseUrl" | "headers"> & {
  namespace?: string;
};

export default class BaseApi {
  protected axios!: AxiosInstance;

  constructor(namespace: string, options?: AxiosOptions) {
    const instance = Axios.create({
      ...options,
      headers: { "Content-Type": "application/json" },
      baseURL: namespace ? `${SERVER_URL}/${namespace}/` : SERVER_URL,
    });
    instance.interceptors.request.use((config) => {
      const token = localStorage.getItem("token");
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    });
    instance.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response) {
          const { data } = error.response;
          if (data?.detail) {
            return Promise.reject(data?.detail);
          }
          return Promise.reject(data);
        }
        return Promise.reject(error.message || "Network Error");
      },
    );
    this.axios = instance;
  }
}
