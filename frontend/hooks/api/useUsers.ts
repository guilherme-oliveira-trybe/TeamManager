import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import axios from 'axios';
import { useToast } from '../useToast';
import type { User, UserListResponse } from '@/types/user';
import type { PaginationMetadata, PaginatedResult } from '@/types/pagination';

interface UserQueryParams {
  page?: number;
  pageSize?: number;
  searchTerm?: string;
  status?: number;
}

export function useUsers(params?: UserQueryParams) {
  return useQuery({
    queryKey: ['users', params],
    queryFn: async (): Promise<PaginatedResult<User>> => {
      const { page = 1, pageSize = 10, searchTerm = '', status } = params || {};
      
      const queryParams = new URLSearchParams();
      queryParams.append('pageNumber', page.toString());
      queryParams.append('pageSize', pageSize.toString());
      if (searchTerm) queryParams.append('searchTerm', searchTerm);
      if (status !== undefined) queryParams.append('status', status.toString());

      const response = await axios.get<UserListResponse>(`/api/users?${queryParams.toString()}`);

      const paginationHeader = response.headers['x-pagination'];
      const pagination: PaginationMetadata = paginationHeader 
        ? JSON.parse(paginationHeader)
        : { 
            TotalCount: 0, 
            PageSize: pageSize, 
            CurrentPage: page, 
            TotalPages: 0, 
            HasNext: false, 
            HasPrevious: false 
          };

      return {
        data: response.data.data || [],
        pagination
      };
    },
    staleTime: 1000 * 60 * 2, // 2 minutes
    placeholderData: (previousData) => previousData, // Keep previous data while fetching new page
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
