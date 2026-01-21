/**
 * React Query hooks for user-related API calls
 */

'use client';

import { useMutation } from '@tanstack/react-query';
import { useRouter } from 'next/navigation';
import { useToast } from '@/hooks/useToast';
import { apiClient, getApiErrorMessage } from '@/lib/api/client';
import type { 
  CompleteRegistrationFormData,
  PasswordResetFormData 
} from '@/lib/validations/schemas';
import type { BaseResponse, UserResponse } from '@/types/api';

const BACKEND_URL = process.env.NEXT_PUBLIC_BACKEND_URL || 'http://localhost:5000';

/**
 * Hook for complete registration mutation
 * Registers a new user with pre-registration code
 */
export function useCompleteRegistration() {
  const router = useRouter();
  const toast = useToast();

  return useMutation({
    mutationFn: async (data: CompleteRegistrationFormData) => {
      const cleanCpf = data.cpf.replace(/\D/g, '');
      
      const payload = {
        ...data,
        cpf: cleanCpf,
        birthDate: data.birthDate.toISOString(),
      };

      const response = await apiClient.post<BaseResponse<UserResponse>>(
        `${BACKEND_URL}/api/users/complete-registration`,
        payload
      );

      return response.data;
    },
    onSuccess: () => {
      toast.success('Cadastro realizado com sucesso! Faça login para continuar.');
      router.push('/login');
    },
    onError: (error) => {
      const message = getApiErrorMessage(error);
      toast.error(message);
    },
  });
}

/**
 * Hook for password reset request mutation
 * Sends request to reset password
 */
export function useRequestPasswordReset() {
  const router = useRouter();
  const toast = useToast();

  return useMutation({
    mutationFn: async (data: PasswordResetFormData) => {
      const cleanCpf = data.cpf.replace(/\D/g, '');
      
      const payload = {
        cpf: cleanCpf,
        email: data.email,
      };

      const response = await apiClient.post(
        `${BACKEND_URL}/api/auth/request-password-reset`,
        payload
      );

      return response.data;
    },
    onSuccess: () => {
      toast.success(
        'Solicitação enviada com sucesso! Aguarde aprovação do administrador.',
        { autoClose: 5000 }
      );
      router.push('/login');
    },
    onError: (error) => {
      const message = getApiErrorMessage(error);
      toast.error(message);
    },
  });
}
