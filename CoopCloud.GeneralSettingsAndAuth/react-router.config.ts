import type { Config } from "@react-router/dev/config";
import path from "path";
import * as fs from "fs/promises";

export default {
  ssr: false,
  appDirectory: "ClientApp",
  buildEnd: async ({ reactRouterConfig: { buildDirectory } }) => {
    const clientDir = path.join(buildDirectory, "client");
    const wwwrootDir = path.join(process.cwd(), "wwwroot");
    try {
      await fs.mkdir(wwwrootDir, { recursive: true });
      const items = await fs.readdir(clientDir);
      for (const item of items) {
        const srcPath = path.join(clientDir, item);
        const destPath = path.join(wwwrootDir, item);
        try {
          await fs.rm(destPath, { force: true, recursive: true });
        } catch (error) {
          // Ignorar si no existe
        }
        const stat = await fs.stat(srcPath);
        if (stat.isDirectory()) {
          await fs.cp(srcPath, destPath, { recursive: true });
        } else {
          await fs.copyFile(srcPath, destPath);
        }
      }
      console.log("Content successfully copied from client to wwwroot");
    } catch (error) {
      console.error("Error copying files:", error);
    }
  },
} satisfies Config;
