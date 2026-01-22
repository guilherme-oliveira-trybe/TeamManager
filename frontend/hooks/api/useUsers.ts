import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import axios from 'axios';
import { useToast } from '../useToast';
import type { User, UserListResponse } from '@/types/user';

export function useUsers(status?: number) {
  return useQuery({
    queryKey: ['users', status],
    queryFn: async () => {
      const url = status 
        ? `/api/users/status/${status}`
        : '/api/users';
      const response = await axios.get<UserListResponse>(url);
      return response.data.data || [];
    },
    staleTime: 1000 * 60 * 2, // 2 minutes
  });
}

export function useActivateUser() {
  const queryClient = useQueryClient();
  const toast = useToast();

  return useMutation({
    mutationFn: async (userId: string) => {
      const response = await axios.post(`/api/users/${userId}/activate`);
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
      toast.success('Usuário ativado com sucesso!');
    },
    onError: (error) => {
      if (axios.isAxiosError(error)) {
        const message = error.response?.data?.errors?.[0] || 'Erro ao ativar usuário';
        toast.error(message);
      } else {
        toast.error('Erro ao ativar usuário');
      }
    },
  });
}

export function useDeactivateUser() {
  const queryClient = useQueryClient();
  const toast = useToast();

  return useMutation({
    mutationFn: async (userId: string) => {
      const response = await axios.post(`/api/users/${userId}/deactivate`);
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] });
      toast.success('Usuário desativado com sucesso!');
    },
    onError: (error) => {
      if (axios.isAxiosError(error)) {
        const message = error.response?.data?.errors?.[0] || 'Erro ao desativar usuário';
        toast.error(message);
      } else {
        toast.error('Erro ao desativar usuário');
      }
    },
  });
}
