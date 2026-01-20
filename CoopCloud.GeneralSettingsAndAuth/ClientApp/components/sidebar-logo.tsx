import { Avatar, AvatarImage, AvatarFallback } from "@nubeteck/shadcn";

export interface ISidebarLogoProps {
  title?: string;
}

const SidebarLogo = ({ title }: ISidebarLogoProps) => {
  return (
    <Avatar>
      <AvatarImage src="/images/logo.png" />
      <AvatarFallback>{title?.slice(0, 2)}</AvatarFallback>
    </Avatar>
  );
};

export default SidebarLogo;
