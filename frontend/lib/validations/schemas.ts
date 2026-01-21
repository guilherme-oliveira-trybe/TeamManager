/**
 * Zod Validation Schemas
 * 100% aligned with backend FluentValidation validators
 */

import * as z from 'zod';
import { isValidCPF } from '@/lib/utils/cpf';

// ============================================
// HELPERS
// ============================================

/**
 * CPF validation schema (matches backend IsValidCpf)
 */
const cpfSchema = z.string()
  .min(1, 'CPF é obrigatório')
  .refine((cpf) => isValidCPF(cpf), { message: 'CPF inválido' });

/**
 * Birth date validation with minimum age (matches backend BeValidAge)
 */
const birthDateSchema = z.date({
  message: 'Data de nascimento é obrigatória',
}).refine((date) => {
  const age = Math.floor((Date.now() - date.getTime()) / (365.25 * 24 * 60 * 60 * 1000));
  return age >= 13;
}, { message: 'Usuário deve ter pelo menos 13 anos' });

// ============================================
// AUTH SCHEMAS
// ============================================

/**
 * Login Schema
 * Backend: LoginRequestValidator
 */
export const loginSchema = z.object({
  login: z.string()
    .min(1, 'Login é obrigatório')
    .refine((login) => {
      // Must be either valid email OR valid CPF
      return login.includes('@') || isValidCPF(login);
    }, { message: 'Login deve ser um email válido ou CPF com 11 dígitos' }),
  password: z.string()
    .min(1, 'Senha é obrigatória')
    .min(8, 'Senha deve ter no mínimo 8 caracteres'),
});

export type LoginFormData = z.infer<typeof loginSchema>;

// ============================================
// USER SCHEMAS
// ============================================

/**
 * Complete Registration Schema
 * Backend: CompleteRegistrationRequestValidator
 */
export const completeRegistrationSchema = z.object({
  // Validation fields
  cpf: cpfSchema,
  activationCode: z.string()
    .min(1, 'Código de ativação é obrigatório')
    .length(8, 'Código deve ter 8 caracteres')
    .transform(val => val.toUpperCase()),
  
  // Personal data
  fullName: z.string()
    .min(1, 'Nome completo é obrigatório')
    .min(3, 'Nome deve ter no mínimo 3 caracteres')
    .max(200, 'Nome deve ter no máximo 200 caracteres'),
  birthDate: birthDateSchema,
  weight: z.number({ message: 'Peso é obrigatório' })
    .gt(0, 'Peso deve ser maior que zero')
    .lt(300, 'Peso deve ser menor que 300kg'),
  height: z.number({ message: 'Altura é obrigatória' })
    .gt(0, 'Altura deve ser maior que zero')
    .lt(250, 'Altura deve ser menor que 250cm'),
  phone: z.string()
    .min(1, 'Telefone é obrigatório')
    .min(11, 'Telefone inválido'),
  email: z.string()
    .min(1, 'Email é obrigatório')
    .email('Email inválido'),
  
  // Security
  password: z.string()
    .min(1, 'Senha é obrigatória')
    .min(8, 'Senha deve ter no mínimo 8 caracteres')
    .regex(/[A-Z]/, 'Senha deve conter pelo menos uma letra maiúscula')
    .regex(/[0-9]/, 'Senha deve conter pelo menos um número'),
  confirmPassword: z.string()
    .min(1, 'Confirmação de senha é obrigatória'),
  
  // Emergency contact
  emergencyContactName: z.string()
    .min(1, 'Nome do contato de emergência é obrigatório')
    .min(3, 'Nome do contato deve ter no mínimo 3 caracteres'),
  emergencyContactPhone: z.string()
    .min(1, 'Telefone do contato de emergência é obrigatório')
    .min(11, 'Telefone do contato inválido'),
}).refine((data) => data.password === data.confirmPassword, {
  message: 'As senhas não coincidem',
  path: ['confirmPassword'],
});

export type CompleteRegistrationFormData = z.infer<typeof completeRegistrationSchema>;

/**
 * Password Reset Request Schema
 * Backend: RequestPasswordResetRequestValidator
 */
export const passwordResetSchema = z.object({
  cpf: cpfSchema,
  email: z.string()
    .min(1, 'Email é obrigatório')
    .email('Email inválido'),
});

export type PasswordResetFormData = z.infer<typeof passwordResetSchema>;

/**
 * Change Password Schema
 * Backend: ChangePasswordRequestValidator
 */
export const changePasswordSchema = z.object({
  currentPassword: z.string()
    .min(1, 'Senha atual é obrigatória'),
  newPassword: z.string()
    .min(1, 'Nova senha é obrigatória')
    .min(8, 'Senha deve ter no mínimo 8 caracteres')
    .regex(/[A-Z]/, 'Senha deve conter pelo menos uma letra maiúscula')
    .regex(/[0-9]/, 'Senha deve conter pelo menos um número'),
  confirmNewPassword: z.string()
    .min(1, 'Confirmação de senha é obrigatória'),
}).refine((data) => data.newPassword === data.confirmNewPassword, {
  message: 'As senhas não coincidem',
  path: ['confirmNewPassword'],
});

export type ChangePasswordFormData = z.infer<typeof changePasswordSchema>;
