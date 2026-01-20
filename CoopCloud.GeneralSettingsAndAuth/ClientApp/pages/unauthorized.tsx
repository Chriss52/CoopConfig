import { Button } from "@nubeteck/shadcn";
import { Link, useNavigate } from "react-router";
import { Home, ArrowLeft, AlertTriangleIcon } from "lucide-react";

const UnauthorizedPage = () => {
  const navigate = useNavigate();
  return (
    <div className="flex min-h-screen items-center justify-center bg-background px-4">
      <div className="w-full max-w-md space-y-8 text-center">
        <div className="flex justify-center">
          <div className="rounded-full bg-destructive/10 p-6">
            <AlertTriangleIcon className="h-16 w-16 text-destructive" />
          </div>
        </div>
        <div className="space-y-3">
          <h1 className="text-4xl font-bold tracking-tight">403</h1>
          <h2 className="text-2xl font-semibold">Acceso Denegado</h2>
          <p className="text-muted-foreground">
            No tienes los permisos necesarios para acceder a esta página.
          </p>
        </div>
        <div className="flex flex-col gap-3 pt-6 sm:flex-row sm:justify-center">
          <Button size="lg" variant="outline" onClick={() => navigate(-1)}>
            <ArrowLeft className="h-4 w-4" />
            Volver atrás
          </Button>
          <Button asChild size="lg">
            <Link to="/">
              <Home className="h-4 w-4" />
              Ir al inicio
            </Link>
          </Button>
        </div>
        <div className="pt-6 text-sm text-muted-foreground">
          <p>
            Si crees que esto es un error, contacta con el administrador del
            sistema.
          </p>
        </div>
      </div>
    </div>
  );
};

export default UnauthorizedPage;
