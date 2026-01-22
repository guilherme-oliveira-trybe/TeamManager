import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import axios from 'axios';
import { useToast } from '../useToast';
import type {
  PasswordResetRequest,
  ApproveResetResponse,
  PasswordResetListResponse,
} from '@/types/password-reset';

export function usePendingResets() {
  return useQuery({
    queryKey: ['password-reset-requests'],
    queryFn: async () => {
      const response = await axios.get<PasswordResetListResponse>('/api/password-reset-requests');
      return response.data.data || [];
    },
    staleTime: 1000 * 60 * 1, // 1 minute
  });
}

export function useApproveReset() {
  const queryClient = useQueryClient();
  const toast = useToast();

  return useMutation({
    mutationFn: async (requestId: string) => {
      const response = await axios.post<ApproveResetResponse>(
        `/api/password-reset-requests/${requestId}/approve`
      );
      return response.data.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['password-reset-requests'] });
      toast.success('Solicitação aprovada! Senha temporária gerada.');
    },
    onError: (error) => {
      if (axios.isAxiosError(error)) {
        const message = error.response?.data?.errors?.[0] || 'Erro ao aprovar solicitação';
        toast.error(message);
      } else {
        toast.error('Erro ao aprovar solicitação');
      }
    },
  });
}
