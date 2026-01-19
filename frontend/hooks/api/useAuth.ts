/**
 * React Query hooks for API calls
 * Handles authentication-related API requests
 */

'use client';

import { useMutation } from '@tanstack/react-query';
import { useRouter } from 'next/navigation';
import axios from 'axios';
import { useToast } from '@/hooks/useToast';
import type { LoginFormData } from '@/lib/validations/schemas';

/**
 * Hook for login mutation
 * Calls Next.js API route (which sets httpOnly cookie)
 */
export function useLogin() {
  const router = useRouter();
  const toast = useToast();

  return useMutation({
    mutationFn: async (data: LoginFormData) => {
      const response = await axios.post('/api/auth/login', data);
      return response.data;
    },
    onSuccess: () => {
      toast.success('Login realizado com sucesso!');
      router.push('/dashboard');
      router.refresh(); // Refresh to update proxy/middleware
    },
    onError: (error) => {
      if (axios.isAxiosError(error)) {
        const message = error.response?.data?.errors?.[0] || 'Login ou senha inválidos';
        toast.error(message);
      } else {
        toast.error('Erro ao fazer login');
      }
    },
  });
}

/**
 * Hook for logout mutation
 */
export function useLogout() {
  const router = useRouter();
  const toast = useToast();

  return useMutation({
    mutationFn: async () => {
      const response = await axios.post('/api/auth/logout');
      return response.data;
    },
    onSuccess: () => {
      toast.success('Logout realizado com sucesso!');
      router.push('/login');
      router.refresh();
    },
    onError: () => {
      toast.error('Erro ao fazer logout');
    },
  });
}
