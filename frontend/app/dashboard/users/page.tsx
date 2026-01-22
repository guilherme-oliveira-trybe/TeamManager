'use client';

import { UserCheck, UserX, MoreVertical } from 'lucide-react';
import { LoadingSpinnerInline } from '@/components/shared/LoadingSpinner';
import { EmptyState } from '@/components/shared/EmptyState';
import { useUsers, useActivateUser, useDeactivateUser } from '@/hooks/api/useUsers';
import { ActionMenu, ActionMenuItem } from '@/components/shared/ActionMenu';
import { StatusBadge } from '@/components/shared/StatusBadge';
import { User, UserStatus } from '@/types/user';

export default function UsersPage() {
  const { data: users = [], isLoading } = useUsers();
  const activateMutation = useActivateUser();
  const deactivateMutation = useDeactivateUser();

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

  return (
    <div className="p-8">
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-3xl font-bold text-white mb-2">Usuários</h1>
          <p className="text-zinc-400">Gerencie os usuários do sistema</p>
        </div>
      </div>

      {isLoading ? (
        <LoadingSpinnerInline />
      ) : users.length === 0 ? (
        <EmptyState
          icon={UserCheck}
          title="Nenhum usuário encontrado"
          description="Não há usuários cadastrados no momento."
        />
      ) : (
        <div className="bg-zinc-900 border border-zinc-800 rounded-lg overflow-hidden">
          <table className="w-full">
            <thead className="bg-zinc-800/50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-zinc-400 uppercase">
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
                <th className="px-6 py-3 text-right text-xs font-medium text-zinc-400 uppercase">
                  Ações
                </th>
              </tr>
            </thead>
            <tbody className="divide-y divide-zinc-800">
              {users.map((user) => (
                <tr key={user.id} className="hover:bg-zinc-800/30">
                  <td className="px-6 py-4 text-sm text-white">
                    <div className="flex flex-col">
                      <span className="font-medium">{user.name}</span>
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
