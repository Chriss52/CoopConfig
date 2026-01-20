import type { LucideIcon } from "lucide-react";

import React from "react";
import { buttonVariants } from "@nubeteck/shadcn";
import { CheckCircle, OctagonAlert } from "lucide-react";
import {
  AlertDialogTitle,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialog as ShadCNAlertDialog,
} from "@nubeteck/shadcn";

type ConfirmVariant = "default" | "success" | "destructive";

interface IConfirmOptions {
  body: string;
  title: string;
  icon?: LucideIcon;
  actionButton?: string;
  cancelButton?: string;
  variant?: ConfirmVariant;
  variantActionButton?: "default" | "success" | "destructive";
}

interface IConfirmState extends IConfirmOptions {
  open: boolean;
  resolve: null | ((value: boolean) => void);
}

const initialState: IConfirmState = {
  body: "",
  title: "",
  open: false,
  resolve: null,
  variant: "default",
  cancelButton: "Cancelar",
  actionButton: "Confirmar",
  variantActionButton: "default",
};

type ConfirmContextType = {
  confirm: (options: IConfirmOptions) => Promise<boolean>;
};

const ConfirmContext = React.createContext<null | ConfirmContextType>(null);

export const ConfirmProvider = ({ children }: React.PropsWithChildren) => {
  const [state, setState] = React.useState<IConfirmState>(initialState);

  const confirm = React.useCallback((options: IConfirmOptions) => {
    return new Promise<boolean>((resolve) => {
      setState({
        ...initialState,
        ...options,
        resolve,
        open: true,
      });
    });
  }, []);

  const handleClose = React.useCallback((result: boolean) => {
    setState((prev) => {
      prev.resolve?.(result);
      return { ...prev, open: false, resolve: null };
    });
  }, []);

  const handleOpenChange = React.useCallback(
    (open: boolean) => {
      if (!open) {
        handleClose(false);
      }
    },
    [handleClose],
  );

  const Icon = React.useMemo(() => {
    if (state.icon) return state.icon;
    if (state.variant === "destructive") return OctagonAlert;
    if (state.variant === "success") return CheckCircle;
    return null;
  }, [state.icon, state.variant]);

  const iconBgClass = React.useMemo(() => {
    if (state.variant === "destructive") return "bg-destructive/10";
    if (state.variant === "success") return "bg-green-100";
    return "bg-primary/10";
  }, [state.variant]);

  const iconColorClass = React.useMemo(() => {
    if (state.variant === "destructive") return "text-destructive";
    if (state.variant === "success") return "text-green-600";
    return "text-primary";
  }, [state.variant]);

  const contextHolder = (
    <ShadCNAlertDialog open={state.open} onOpenChange={handleOpenChange}>
      <AlertDialogContent>
        <AlertDialogHeader
          className={state.variant !== "default" ? "items-center" : undefined}
        >
          <AlertDialogTitle
            className={state.variant !== "default" ? "text-center" : undefined}
          >
            {Icon && (
              <div
                className={`mb-2 mx-auto flex h-14 w-14 items-center justify-center rounded-full ${iconBgClass}`}
              >
                <Icon className={`h-7 w-7 ${iconColorClass}`} />
              </div>
            )}
            {state.title}
          </AlertDialogTitle>
          <AlertDialogDescription
            className={`text-[15px] ${state.variant !== "default" ? "text-center" : ""}`}
          >
            {state.body}
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter
          className={
            state.variant !== "default" ? "mt-2 sm:justify-center" : undefined
          }
        >
          <AlertDialogCancel onClick={() => handleClose(false)}>
            {state.cancelButton}
          </AlertDialogCancel>
          <AlertDialogAction
            onClick={() => handleClose(true)}
            className={
              state.variantActionButton === "destructive"
                ? buttonVariants({ variant: "destructive" })
                : undefined
            }
          >
            {state.actionButton}
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </ShadCNAlertDialog>
  );

  return (
    <ConfirmContext.Provider value={{ confirm }}>
      {children}
      {contextHolder}
    </ConfirmContext.Provider>
  );
};

const useConfirm = () => {
  const context = React.useContext(ConfirmContext);
  if (!context) {
    throw new Error("useConfirm must be used within a ConfirmProvider");
  }
  return context;
};

export default useConfirm;
