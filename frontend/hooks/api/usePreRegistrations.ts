import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import axios from 'axios';
import { useToast } from '../useToast';
import type {
  PreRegistration,
  CreatePreRegistrationRequest,
  PreRegistrationResponse,
  PreRegistrationListResponse,
} from '@/types/pre-registration';

export function usePreRegistrations() {
  return useQuery({
    queryKey: ['pre-registrations'],
    queryFn: async () => {
      const response = await axios.get<PreRegistrationListResponse>('/api/pre-registrations');
      return response.data.data || [];
    },
    staleTime: 1000 * 60 * 2, // 2 minutes
  });
}

export function useCreatePreRegistration() {
  const queryClient = useQueryClient();
  const toast = useToast();

  return useMutation({
    mutationFn: async (data: CreatePreRegistrationRequest) => {
      const response = await axios.post<PreRegistrationResponse>('/api/pre-registrations', data);
      return response.data.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['pre-registrations'] });
      toast.success('Pré-cadastro criado com sucesso!');
    },
    onError: (error) => {
      if (axios.isAxiosError(error)) {
        const message = error.response?.data?.errors?.[0] || 'Erro ao criar pré-cadastro';
        toast.error(message);
      } else {
        toast.error('Erro ao criar pré-cadastro');
      }
    },
  });
}

export function useRegenerateActivationCode() {
  const queryClient = useQueryClient();
  const toast = useToast();

  return useMutation({
    mutationFn: async (id: string) => {
      const response = await axios.post<PreRegistrationResponse>(
        `/api/pre-registrations/${id}/regenerate`
      );
      return response.data.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['pre-registrations'] });
      toast.success('Código regenerado com sucesso!');
    },
    onError: (error) => {
      if (axios.isAxiosError(error)) {
        const message = error.response?.data?.errors?.[0] || 'Erro ao regenerar código';
        toast.error(message);
      } else {
        toast.error('Erro ao regenerar código');
      }
    },
  });
}
