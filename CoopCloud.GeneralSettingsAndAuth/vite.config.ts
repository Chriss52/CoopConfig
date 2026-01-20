import tailwindcss from "@tailwindcss/vite";
import { loadEnv, defineConfig } from "vite";
import tsconfigPaths from "vite-tsconfig-paths";
import { reactRouter } from "@react-router/dev/vite";

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), "");
  return {
    plugins: [tailwindcss(), reactRouter(), tsconfigPaths()],
    define: {
      "process.env.VITE_NUBETECK_SERVER_URL": JSON.stringify(
        env.VITE_NUBETECK_SERVER_URL,
      ),
    },
    server: {
      proxy: {
        "/api": {
          secure: false,
          changeOrigin: true,
          target: env.VITE_NUBETECK_SERVER_URL,
        },
      },
    },
  };
});
