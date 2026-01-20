import React from "react";
import { toast } from "sonner";
import { CopyIcon, AlertCircleIcon } from "lucide-react";
import { Alert, AlertTitle, AlertDescription } from "@nubeteck/shadcn";
import {
  Button,
  Dialog,
  DialogTitle,
  DialogFooter,
  DialogHeader,
  DialogContent,
  DialogDescription,
} from "@nubeteck/shadcn";

interface ApiKeyDisplayDialogProps {
  onClose: () => void;
  apiKey: null | string;
}

export const ApiKeyDisplayDialog: React.FC<ApiKeyDisplayDialogProps> = ({
  apiKey,
  onClose,
}) => {
  const copyToClipboard = () => {
    if (apiKey) {
      navigator.clipboard.writeText(apiKey);
      toast.success("API Key copiada al portapapeles");
    }
  };

  return (
    <Dialog open={!!apiKey} onOpenChange={(open) => !open && onClose()}>
      <DialogContent className="sm:max-w-[500px]">
        <DialogHeader>
          <DialogTitle>API Key generada exitosamente</DialogTitle>
          <DialogDescription>
            Guarda esta llave en un lugar seguro.
          </DialogDescription>
        </DialogHeader>
        <div className="space-y-4">
          <Alert className="border-none bg-red-600/10 text-red-600 dark:bg-red-400/10 dark:text-red-400">
            <AlertCircleIcon />
            <AlertTitle>Información importante</AlertTitle>
            <AlertDescription className="text-red-600/80 dark:text-red-400/80">
              Esta es la única vez que podrás ver esta llave. Asegúrate de
              copiarla ahora, ya que no podrás recuperarla después.
            </AlertDescription>
          </Alert>
          <div className="rounded-md bg-muted p-4">
            <div className="flex items-center gap-2">
              <code className="flex-1 font-mono text-sm bg-background p-3 rounded border break-all">
                {apiKey}
              </code>
              <Button size="icon" variant="outline" onClick={copyToClipboard}>
                <CopyIcon className="h-4 w-4" />
              </Button>
            </div>
          </div>
        </div>
        <DialogFooter>
          <Button onClick={onClose}>Cerrar</Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

export default ApiKeyDisplayDialog;
