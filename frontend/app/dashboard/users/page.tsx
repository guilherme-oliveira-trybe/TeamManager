'use client';

import { useState } from 'react';
import { Users, Eye, CheckCircle, PauseCircle, Trash } from 'lucide-react';
import { useAdminOnly } from '@/hooks/useAdminOnly';
import { useUsers, useActivateUser, useDeactivateUser } from '@/hooks/api/useUsers';
import { StatusBadge } from '@/components/shared/StatusBadge';
import { ActionMenu, ActionMenuItem } from '@/components/shared/ActionMenu';
import { ConfirmationModal } from '@/components/shared/ConfirmationModal';
import { EmptyState } from '@/components/shared/EmptyState';
import { UserStatus } from '@/types/user';

export default function UsersPage() {
  const { isAdmin, isLoading: authLoading } = useAdminOnly();
  const [selectedStatus, setSelectedStatus] = useState<number | undefined>(undefined);
  const [confirmModal, setConfirmModal] = useState<{
    isOpen: boolean;
    type: 'activate' | 'deactivate' | null;
    userId: string | null;
    userName: string | null;
  }>({
    isOpen: false,
    type: null,
    userId: null,
    userName: null,
  });

  const { data: users = [], isLoading, error } = useUsers(selectedStatus);
  const activateMutation = useActivateUser();
  const deactivateMutation = useDeactivateUser();

  if (authLoading || !isAdmin) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-zinc-400">Carregando...</div>
      </div>
    );
  }

  const handleActivate = (userId: string, userName: string) => {
    setConfirmModal({
      isOpen: true,
      type: 'activate',
      userId,
      userName,
    });
  };

  const handleDeactivate = (userId: string, userName: string) => {
    setConfirmModal({
      isOpen: true,
      type: 'deactivate',
      userId,
      userName,
    });
  };

  const handleConfirm = () => {
    if (!confirmModal.userId) return;

    if (confirmModal.type === 'activate') {
      activateMutation.mutate(confirmModal.userId, {
        onSuccess: () => {
          setConfirmModal({ isOpen: false, type: null, userId: null, userName: null });
        },
      });
    } else if (confirmModal.type === 'deactivate') {
      deactivateMutation.mutate(confirmModal.userId, {
        onSuccess: () => {
          setConfirmModal({ isOpen: false, type: null, userId: null, userName: null });
        },
      });
    }
  };

  const getActionItems = (user: any): ActionMenuItem[] => {
    const items: ActionMenuItem[] = [
      {
        label: 'Ver Detalhes',
        icon: <Eye className="h-4 w-4" />,
        onClick: () => console.log('View', user.id),
      },
    ];

    if (user.status === UserStatus.AwaitingActivation) {
      items.push({ type: 'divider' });
      items.push({
        label: 'Ativar',
        icon: <CheckCircle className="h-4 w-4" />,
        onClick: () => handleActivate(user.id, user.name),
        variant: 'success',
      });
    }

    if (user.status === UserStatus.Active) {
      items.push({ type: 'divider' });
      items.push({
        label: 'Desativar',
        icon: <PauseCircle className="h-4 w-4" />,
        onClick: () => handleDeactivate(user.id, user.name),
        variant: 'warning',
      });
    }

    return items;
  };

  const statusTabs = [
    { label: 'Todos', value: undefined, count: users.length },
    { label: 'Aguardando', value: UserStatus.AwaitingActivation },
    { label: 'Ativos', value: UserStatus.Active },
    { label: 'Inativos', value: UserStatus.Inactive },
  ];

  return (
    <div className="p-8">
      <div className="mb-6">
        <h1 className="text-3xl font-bold text-white mb-2">Gestão de Usuários</h1>
        <p className="text-zinc-400">Gerencie usuários do sistema</p>
      </div>

      {/* Status Tabs */}
      <div className="flex gap-2 mb-6 border-b border-zinc-800">
        {statusTabs.map((tab) => (
          <button
            key={tab.label}
            onClick={() => setSelectedStatus(tab.value)}
            className={`px-4 py-2 font-medium border-b-2 transition-colors ${
              selectedStatus === tab.value
                ? 'border-blue-500 text-white'
                : 'border-transparent text-zinc-400 hover:text-white'
            }`}
          >
            {tab.label}
            {tab.count !== undefined && (
              <span className="ml-2 text-xs bg-zinc-800 px-2 py-0.5 rounded-full">
                {tab.count}
              </span>
            )}
          </button>
        ))}
      </div>

      {/* Users Table */}
      {isLoading ? (
        <div className="text-center text-zinc-400 py-12">Carregando...</div>
      ) : error ? (
        <div className="text-center text-red-400 py-12">Erro ao carregar usuários</div>
      ) : users.length === 0 ? (
        <EmptyState
          icon={Users}
          title="Nenhum usuário encontrado"
          description="Não há usuários neste status no momento."
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
                  Email
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
              {users.map((user: any) => (
                <tr key={user.id} className="hover:bg-zinc-800/30">
                  <td className="px-6 py-4 text-sm text-white">{user.name}</td>
                  <td className="px-6 py-4 text-sm text-zinc-400">
                    {user.cpf.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '***.$2.***-**')}
                  </td>
                  <td className="px-6 py-4 text-sm text-zinc-400">{user.email}</td>
                  <td className="px-6 py-4">
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

      {/* Confirmation Modal */}
      <ConfirmationModal
        isOpen={confirmModal.isOpen}
        onClose={() => setConfirmModal({ isOpen: false, type: null, userId: null, userName: null })}
        onConfirm={handleConfirm}
        title={confirmModal.type === 'activate' ? 'Confirmar Ativação' : 'Confirmar Desativação'}
        message={
          confirmModal.type === 'activate'
            ? `Deseja ativar o usuário "${confirmModal.userName}"? Esta ação permitirá que o usuário acesse o sistema.`
            : `Deseja desativar o usuário "${confirmModal.userName}"? O usuário não poderá mais acessar o sistema.`
        }
        variant={confirmModal.type === 'activate' ? 'success' : 'warning'}
        confirmText={confirmModal.type === 'activate' ? 'Ativar' : 'Desativar'}
        isLoading={activateMutation.isPending || deactivateMutation.isPending}
      />
    </div>
  );
}
