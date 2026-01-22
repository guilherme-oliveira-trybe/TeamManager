export interface User {
  id: string;
  name: string;
  cpf: string;
  email: string;
  phone?: string;
  birthDate: string;
  weight?: number;
  height?: number;
  status: number;
  profile: number;
  unit?: number;
  position?: number;
  emergencyContactName?: string;
  emergencyContactPhone?: string;
  createdAt: string;
  updatedAt: string;
}

export interface UserListResponse {
  isSuccess: boolean;
  data: User[] | null;
  errors: string[];
}
