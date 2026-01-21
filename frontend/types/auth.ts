/**
 * Auth-related types
 */

export interface AuthUser {
  id: string;
  email: string;
  fullName: string;
  profile: string;
  cpf: string;
}

export interface AuthState {
  user: AuthUser | null;
  isAuthenticated: boolean;
  isLoading: boolean;
}
