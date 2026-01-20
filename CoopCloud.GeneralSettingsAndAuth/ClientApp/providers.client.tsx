import React from "react";
import { Toaster, setDateLocale } from "@nubeteck/shadcn";
import {
  ThemeProvider,
  ConfirmProvider,
  ReactQueryClientProvider,
} from "@/providers";

setDateLocale("es");

const AppProviders = ({ children }: React.PropsWithChildren) => {
  return (
    <ThemeProvider defaultTheme="light" storageKey="vite-ui-theme">
      <ReactQueryClientProvider>
        <ConfirmProvider>{children}</ConfirmProvider>
      </ReactQueryClientProvider>
      <Toaster richColors position="top-center" />
    </ThemeProvider>
  );
};

export default AppProviders;
