export interface PasswordResetRequest {
  id: string;
  userId: string;
  userName: string;
  userEmail: string;
  userCpf: string;
  status: number; // 1=Pending, 2=Approved, 3=Rejected
  requestedAt: string;
  approvedAt?: string;
  approvedBy?: string;
  expiresAt?: string;
}

export interface ApproveResetResponse {
  isSuccess: boolean;
  data: {
    temporaryPassword: string;
    expiresAt: string;
  } | null;
  errors: string[];
}

export interface PasswordResetListResponse {
  isSuccess: boolean;
  data: PasswordResetRequest[] | null;
  errors: string[];
}
