export type UserRole = 'Admin' | 'Athlete' | 'Coach' | 'Staff';

export interface CurrentUser {
  id: string;
  email: string;
  role: UserRole;
  profile: number;
  unit?: number;
  position?: number;
}

export enum UserStatus {
  PendingRegistration = 1,
  AwaitingActivation = 2,
  Active = 3,
  Rejected = 4,
  Inactive = 5,
}

export interface User {
  id: string;
  name: string;
  cpf: string;
  email: string;
  phone?: string;
  birthDate: string;
  weight?: number;
  height?: number;
  status: UserStatus;
  profile: number;
  unit?: string;
  position?: string;
  emergencyContactName?: string;
  emergencyContactPhone?: string;
  createdAt: string;
  updatedAt: string;
}

export interface UpdateUserRequest {
  name?: string;
  email?: string;
  phone?: string;
  weight?: number;
  height?: number;
  emergencyContactName?: string;
  emergencyContactPhone?: string;
}
