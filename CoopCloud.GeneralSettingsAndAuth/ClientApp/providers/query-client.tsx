import { AxiosError } from "axios";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
// import { PersistQueryClientProvider } from "@tanstack/react-query-persist-client";
// import { createAsyncStoragePersister } from "@tanstack/query-async-storage-persister";

declare module "@tanstack/react-query" {
  interface Register {
    defaultError: AxiosError;
  }
}

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 2,
      gcTime: 60 * 60 * 1000, // 1 hour.
      staleTime: 10 * 60 * 1000, // 10 min.
    },
  },
});

// const asyncStoragePersister = createAsyncStoragePersister({
//   storage: window.localStorage,
// });

const ReactQueryClientProvider = ({ children }: React.PropsWithChildren) => {
  return (
    <QueryClientProvider
      client={queryClient}
      // persistOptions={{ persister: asyncStoragePersister }}
    >
      <ReactQueryDevtools initialIsOpen={false} buttonPosition="bottom-right" />
      {children}
    </QueryClientProvider>
  );
};

export default ReactQueryClientProvider;
