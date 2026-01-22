'use client';

import { useMutation } from '@tanstack/react-query';
import { useRouter } from 'next/navigation';
import axios from 'axios';
import { useToast } from '@/hooks/useToast';
import type { LoginFormData, ChangePasswordFormData } from '@/lib/validations/schemas';

const BACKEND_URL = process.env.NEXT_PUBLIC_BACKEND_URL || 'http://localhost:5000';

export function useLogin() {
  const router = useRouter();
  const toast = useToast();

  return useMutation({
    mutationFn: async (data: LoginFormData) => {
      const response = await axios.post('/api/auth/login', data);
      return response.data;
    },
    onSuccess: (data) => {
      toast.success('Login realizado com sucesso!');
      
      if (data?.requiresPasswordChange) {
        router.push('/change-password');
        toast.info('Por favor, altere sua senha temporária.');
      } else {
        router.push('/dashboard');
      }
      
      router.refresh();
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

export function useChangePassword() {
  const router = useRouter();
  const toast = useToast();

  return useMutation({
    mutationFn: async (data: ChangePasswordFormData) => {
      const response = await axios.post('/api/auth/change-password', data);
      return response.data;
    },
    onSuccess: () => {
      toast.success('Senha alterada com sucesso!');
      router.push('/dashboard');
      router.refresh();
    },
    onError: (error) => {
      if (axios.isAxiosError(error)) {
        const message = error.response?.data?.errors?.[0] || 'Erro ao alterar senha';
        toast.error(message);
      } else {
        toast.error('Erro ao alterar senha');
      }
    },
  });
}

export function useCurrentUser() {
  return useQuery({
    queryKey: ['currentUser'],
    queryFn: async () => {
      const response = await axios.get('/api/auth/me');
      return response.data;
    },
    staleTime: 1000 * 60 * 5, // 5 minutes
    retry: false,
  });
}

export function useAuth() {
  const { data: user, isLoading } = useCurrentUser();
  
  return {
    user,
    isLoading,
    isAdmin: user?.role === 'Admin',
    isAthlete: user?.role === 'Athlete',
    isAuthenticated: !!user,
  };
}
