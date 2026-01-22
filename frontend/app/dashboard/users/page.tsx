'use client';

import { useState } from 'react';

import { UserCheck, UserX, MoreVertical } from 'lucide-react';
import { LoadingSpinnerInline } from '@/components/shared/LoadingSpinner';
import { EmptyState } from '@/components/shared/EmptyState';
import { useUsers, useActivateUser, useDeactivateUser } from '@/hooks/api/useUsers';
import { ActionMenu, ActionMenuItem } from '@/components/shared/ActionMenu';
import { StatusBadge } from '@/components/shared/StatusBadge';
import { User, UserStatus } from '@/types/user';

export default function UsersPage() {
  const [selectedStatus, setSelectedStatus] = useState<number | undefined>(undefined);
  // Fetch ALL users (ignore status param in query key/fn for now if possible, or just pass undefined)
  const { data: allUsers = [], isLoading } = useUsers(); 
  const activateMutation = useActivateUser();
  const deactivateMutation = useDeactivateUser();

  // Calculate counts safely
  const counts = {
    all: allUsers.length,
    awaiting: allUsers.filter((u: any) => u.status === UserStatus.AwaitingActivation).length,
    active: allUsers.filter((u: any) => u.status === UserStatus.Active).length,
    inactive: allUsers.filter((u: any) => u.status === UserStatus.Inactive).length,
  };

  // Filter users based on selection
  const filteredUsers = selectedStatus === undefined
    ? allUsers
    : allUsers.filter((u: any) => u.status === selectedStatus);

  const handleActivate = (userId: string) => {
    activateMutation.mutate(userId);
  };

  const handleDeactivate = (userId: string) => {
    deactivateMutation.mutate(userId);
  };

  const formatCPF = (cpf: string) => {
    return cpf.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
  };

  const getActionItems = (user: User): ActionMenuItem[] => {
    const items: ActionMenuItem[] = [];

    // Se o usuário estiver Pendente ou Aguardando Aprovação, pode ativar
    if (user.status === UserStatus.PendingRegistration || user.status === UserStatus.AwaitingActivation || user.status === UserStatus.Inactive) {
      items.push({
        label: 'Ativar Usuário',
        icon: <UserCheck className="h-4 w-4" />,
        onClick: () => handleActivate(user.id),
      });
    }

    // Se estiver Ativo, pode desativar
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
    { label: 'Todos', value: undefined, count: counts.all },
    { label: 'Aguardando', value: UserStatus.AwaitingActivation, count: counts.awaiting },
    { label: 'Ativos', value: UserStatus.Active, count: counts.active },
    { label: 'Inativos', value: UserStatus.Inactive, count: counts.inactive },
  ];

  return (
    <div className="p-8">
      <div className="mb-6">
        <h1 className="text-3xl font-bold text-white mb-2">Usuários</h1>
        <p className="text-zinc-400">Gerencie os usuários do sistema</p>
      </div>

       {/* Status Tabs */}
       <div className="flex gap-2 mb-6 border-b border-zinc-800">
        {statusTabs.map((tab) => (
          <button
            key={tab.label}
            onClick={() => setSelectedStatus(tab.value)}
            className={`px-4 py-2 font-medium border-b-2 transition-colors flex items-center gap-2 ${
              selectedStatus === tab.value
                ? 'border-amber-500 text-white'
                : 'border-transparent text-zinc-400 hover:text-white'
            }`}
          >
            {tab.label}
            <span className={`text-xs px-2 py-0.5 rounded-full ${
              selectedStatus === tab.value 
                ? 'bg-amber-500/10 text-amber-500' 
                : 'bg-zinc-800 text-zinc-400'
            }`}>
              {tab.count}
            </span>
          </button>
        ))}
      </div>

      {isLoading ? (
        <LoadingSpinnerInline />
      ) : filteredUsers.length === 0 ? (
        <EmptyState
          icon={UserCheck}
          title="Nenhum usuário encontrado"
          description="Não há usuários com este status no momento."
        />
      ) : (
        <div className="bg-zinc-900 border border-zinc-800 rounded-lg"> {/* Removed overflow-hidden for dropdown visibility */}
          <table className="w-full">
            <thead className="bg-zinc-800/50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-zinc-400 uppercase rounded-tl-lg"> {/* Added radius */}
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
                <th className="px-6 py-3 text-right text-xs font-medium text-zinc-400 uppercase rounded-tr-lg"> {/* Added radius */}
                  Ações
                </th>
              </tr>
            </thead>
            <tbody className="divide-y divide-zinc-800">
              {filteredUsers.map((user: any) => (
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
      )}
    </div>
  );
}
