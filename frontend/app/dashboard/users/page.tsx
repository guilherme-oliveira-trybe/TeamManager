'use client';

import { useState, useCallback } from 'react';
import { UserCheck, UserX } from 'lucide-react';
import { LoadingSpinnerInline } from '@/components/shared/LoadingSpinner';
import { EmptyState } from '@/components/shared/EmptyState';
import { useUsers, useActivateUser, useDeactivateUser } from '@/hooks/api/useUsers';
import { ActionMenu, ActionMenuItem } from '@/components/shared/ActionMenu';
import { StatusBadge } from '@/components/shared/StatusBadge';
import { User, UserStatus } from '@/types/user';
import { Pagination } from '@/components/shared/Pagination';
import { SearchInput } from '@/components/shared/SearchInput';

export default function UsersPage() {
  const [page, setPage] = useState(1);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedStatus, setSelectedStatus] = useState<number | undefined>(undefined);
  
  const { data, isLoading, isPlaceholderData } = useUsers({
    page,
    pageSize: 10,
    searchTerm,
    status: selectedStatus
  });

  const users = data?.data || [];
  const pagination = data?.pagination;

  const activateMutation = useActivateUser();
  const deactivateMutation = useDeactivateUser();

  const handleSearch = useCallback((term: string) => {
    setSearchTerm(term);
    setPage(1);
  }, []);

  const handleStatusChange = useCallback((status: number | undefined) => {
    setSelectedStatus(status);
    setPage(1);
  }, []);

  const handleActivate = useCallback((userId: string) => {
    activateMutation.mutate(userId);
  }, [activateMutation]);

  const handleDeactivate = useCallback((userId: string) => {
    deactivateMutation.mutate(userId);
  }, [deactivateMutation]);

  const formatCPF = (cpf: string) => {
    return cpf.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
  };

  const getActionItems = (user: User): ActionMenuItem[] => {
    const items: ActionMenuItem[] = [];

    if (user.status === UserStatus.PendingRegistration || user.status === UserStatus.AwaitingActivation || user.status === UserStatus.Inactive) {
      items.push({
        label: 'Ativar Usuário',
        icon: <UserCheck className="h-4 w-4" />,
        onClick: () => handleActivate(user.id),
      });
    }

    if (user.status === UserStatus.Active) {
      items.push({
        label: 'Desativar',
        icon: <UserX className="h-4 w-4" />,
        onClick: () => handleDeactivate(user.id),
        variant: 'danger',
      });
    }

    return items;
  };

  const profileLabels: Record<number, string> = {
    1: 'Admin',
    3: 'Jogador',
  };

  const statusTabs = [
    { label: 'Todos', value: undefined },
    { label: 'Aguardando', value: UserStatus.AwaitingActivation },
    { label: 'Ativos', value: UserStatus.Active },
    { label: 'Inativos', value: UserStatus.Inactive },
  ];

  return (
    <div className="p-8">
      <div className="mb-6">
        <h1 className="text-3xl font-bold text-white mb-2">Usuários</h1>
        <p className="text-zinc-400">Gerencie os usuários do sistema</p>
      </div>

       {/* Status Tabs and Search */}
       <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4 mb-6">
        <div className="flex gap-2 border-b border-zinc-800 w-full md:w-auto overflow-x-auto">
          {statusTabs.map((tab) => (
            <button
              key={tab.label}
              onClick={() => handleStatusChange(tab.value)}
              className={`px-4 py-2 font-medium border-b-2 transition-colors whitespace-nowrap ${
                selectedStatus === tab.value
                  ? 'border-amber-500 text-white'
                  : 'border-transparent text-zinc-400 hover:text-white'
              }`}
            >
              {tab.label}
            </button>
          ))}
        </div>
        
        <div className="w-full md:w-64">
           <SearchInput onSearch={handleSearch} placeholder="Buscar usuários..." />
        </div>
      </div>

      {isLoading ? (
        <LoadingSpinnerInline />
      ) : users.length === 0 ? (
        <EmptyState
          icon={UserCheck}
          title="Nenhum usuário encontrado"
          description={searchTerm ? "Nenhum resultado para sua busca." : "Não há usuários com este status no momento."}
        />
      ) : (
        <div className={`bg-zinc-900 border border-zinc-800 rounded-lg flex flex-col ${isPlaceholderData ? 'opacity-70' : ''}`}> 
          {/* Removed overflow-hidden for dropdown visibility */}
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-zinc-800/50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-zinc-400 uppercase rounded-tl-lg">
                    Nome
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-zinc-400 uppercase">
                    CPF
                  </th>
                   <th className="px-6 py-3 text-left text-xs font-medium text-zinc-400 uppercase">
                    Perfil
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-zinc-400 uppercase">
                    Status
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-zinc-400 uppercase rounded-tr-lg">
                    Ações
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-zinc-800">
                {users.map((user: User) => (
                  <tr key={user.id} className="hover:bg-zinc-800/30">
                    <td className="px-6 py-4 text-sm text-white">
                      <div className="flex flex-col">
                        <span className="font-medium">{user.fullName || user.name}</span>
                        <span className="text-xs text-zinc-500">{user.email}</span>
                      </div>
                    </td>
                    <td className="px-6 py-4 text-sm text-zinc-400">
                      {formatCPF(user.cpf)}
                    </td>
                    <td className="px-6 py-4 text-sm text-zinc-400">
                      {profileLabels[user.profile] || 'Desconhecido'}
                    </td>
                    <td className="px-6 py-4 text-sm">
                      <StatusBadge status={user.status} />
                    </td>
                    <td className="px-6 py-4 text-right">
                      <ActionMenu items={getActionItems(user)} />
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          
          {/* Pagination Controls */}
          {pagination && (
             <Pagination 
               currentPage={pagination.CurrentPage}
               totalPages={pagination.TotalPages}
               onPageChange={setPage}
               hasNext={pagination.HasNext}
               hasPrevious={pagination.HasPrevious}
               totalCount={pagination.TotalCount}
             />
          )}
        </div>
      )}
    </div>
  );
}
