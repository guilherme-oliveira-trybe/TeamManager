/**
 * API Response Types
 * Aligned with backend response structure
 */

/**
 * Base API Response structure from backend
 */
export interface BaseResponse<T = unknown> {
  isSuccess: boolean;
  data?: T;
  errors: string[];
}

/**
 * Operation Response (no data, just success/error)
 */
export interface OperationResponse {
  isSuccess: boolean;
  errors: string[];
}

// ============================================
// AUTH TYPES
// ============================================

/**
 * Login Response
 */
export interface LoginResponse {
  token: string;
  expiresAt: string;
}

/**
 * User Profile enum (matches backend)
 */
export enum UserProfile {
  Player = 'Player',
  Staff = 'Staff',
  Admin = 'Admin',
}

/**
 * User Status enum (matches backend)
 */
export enum UserStatus {
  AwaitingActivation = 'AwaitingActivation',
  Active = 'Active',
  Inactive = 'Inactive',
}

/**
 * Unit enum (matches backend)
 */
export enum Unit {
  MainTeam = 'MainTeam',
  UnderTeam = 'UnderTeam',
}

// ============================================
// USER TYPES
// ============================================

/**
 * User Response from backend
 */
export interface UserResponse {
  id: string;
  cpf: string;
  fullName: string;
  email: string;
  phone: string;
  birthDate: string;
  weight: number;
  height: number;
  profile: UserProfile;
  unit?: Unit;
  position?: string;
  status: UserStatus;
  emergencyContactName?: string;
  emergencyContactPhone?: string;
  createdAt: string;
  updatedAt?: string;
}

// ============================================
// DEPARTMENT TYPES
// ============================================

/**
 * Department Response from backend
 */
export interface DepartmentResponse {
  id: string;
  name: string;
  description?: string;
  sectorsCount: number;
  staffMembersCount: number;
  createdAt: string;
  updatedAt?: string;
}

/**
 * Department with Sectors
 */
export interface DepartmentWithSectorsResponse extends DepartmentResponse {
  sectors: SectorResponse[];
}

// ============================================
// SECTOR TYPES
// ============================================

/**
 * Sector Response from backend
 */
export interface SectorResponse {
  id: string;
  name: string;
  description?: string;
  departmentId: string;
  departmentName: string;
  staffMembersCount: number;
  createdAt: string;
  updatedAt?: string;
}

/**
 * Sector with Staff Members
 */
export interface SectorWithStaffResponse extends SectorResponse {
  staffMembers: StaffMemberResponse[];
}

// ============================================
// STAFF MEMBER TYPES
// ============================================

/**
 * Staff Member Response from backend
 */
export interface StaffMemberResponse {
  id: string;
  fullName: string;
  cpf: string;
  email: string;
  phone: string;
  position: string;
  sectorId: string;
  sectorName: string;
  departmentName: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

// ============================================
// PRE-REGISTRATION TYPES
// ============================================

/**
 * Pre-Registration Response from backend
 */
export interface PreRegistrationResponse {
  id: string;
  cpf: string;
  activationCode: string;
  profile: UserProfile;
  unit?: string;
  position?: string;
  expirationDate: string;
  isUsed: boolean;
  usedAt?: string;
  createdAt: string;
}

// ============================================
// PAGINATION TYPES
// ============================================

/**
 * Paginated Response (for future use)
 */
export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}
