export interface PreRegistration {
  id: string;
  cpf: string;
  profile: number;
  unit?: number;
  position?: number;
  activationCode: string;
  expiresAt: string;
  isUsed: boolean;
  createdAt: string;
}

export interface CreatePreRegistrationRequest {
  cpf: string;
  profile: number;
  unit?: number;
  position?: number;
}

export interface PreRegistrationResponse {
  isSuccess: boolean;
  data: PreRegistration | null;
  errors: string[];
}

export interface PreRegistrationListResponse {
  isSuccess: boolean;
  data: PreRegistration[] | null;
  errors: string[];
}
