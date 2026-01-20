export interface IUser {
  id: string;
  email: string;
  createdAt: Date;
  username: string;
  fullName: string;
  isActive: boolean;
  lastLoginAt: Date;
  phoneNumber: string;
  roles: [{ id: string; name: string }];
}
