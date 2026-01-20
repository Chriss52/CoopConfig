import { ROUTES } from "@/core/routes";
import AppLayout from "@/components/app-layout";
import AppSidebar from "@/components/app-sidebar";
import { KeyIcon, MailIcon, WebhookIcon, Settings2Icon } from "lucide-react";

const MainLayout = () => {
  return (
    <AppLayout
      sidebar={
        <AppSidebar
          className="border-r bg-card"
          title="Configuraciones generales"
          // secondaryItem={{
          //   id: "back-to-home",
          //   icon: ArrowLeftIcon,
          //   label: "Volver atrás",
          //   href: ROUTES.DASHBOARD.path,
          // }}
          items={[
            {
              icon: Settings2Icon,
              id: ROUTES.SETTINGS.id,
              href: ROUTES.SETTINGS.path,
              label: ROUTES.SETTINGS.label,
            },
            {
              icon: MailIcon,
              id: ROUTES.SMTP.id,
              href: ROUTES.SMTP.path,
              label: ROUTES.SMTP.label,
            },
            {
              icon: WebhookIcon,
              id: ROUTES.WEBHOOKS.id,
              href: ROUTES.WEBHOOKS.path,
              label: ROUTES.WEBHOOKS.label,
            },
            {
              icon: KeyIcon,
              id: ROUTES.API_KEYS.id,
              href: ROUTES.API_KEYS.path,
              label: ROUTES.API_KEYS.label,
            },
          ]}
        />
      }
    />
  );
};

export default MainLayout;
