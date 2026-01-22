'use client';

import { useState } from 'react';
import { Key, CheckCircle } from 'lucide-react';
import { useAdminOnly } from '@/hooks/useAdminOnly';
import { usePendingResets, useApproveReset } from '@/hooks/api/usePasswordResets';
import { TempPasswordModal } from '@/components/dashboard/TempPasswordModal';
import { ConfirmationModal } from '@/components/shared/ConfirmationModal';
import { EmptyState } from '@/components/shared/EmptyState';
import { ActionMenu, ActionMenuItem } from '@/components/shared/ActionMenu';

interface TempPasswordData {
  temporaryPassword: string;
  expiresAt: string;
  userName?: string;
}

export default function PasswordResetsPage() {
  const { isAdmin, isLoading: authLoading } = useAdminOnly();
  const [tempPasswordData, setTempPasswordData] = useState<TempPasswordData | null>(null);
  const [approveConfirm, setApproveConfirm] = useState<{
    isOpen: boolean;
    id: string | null;
    userName: string | null;
  }>({ isOpen: false, id: null, userName: null });

  const { data: resets = [], isLoading } = usePendingResets();
  const approveMutation = useApproveReset();

  if (authLoading || !isAdmin) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-zinc-400">Carregando...</div>
      </div>
    );
  }

  const handleApprove = (id: string, userName: string) => {
    setApproveConfirm({ isOpen: true, id, userName });
  };

  const confirmApprove = () => {
    if (approveConfirm.id) {
      approveMutation.mutate(approveConfirm.id, {
        onSuccess: (data) => {
          setApproveConfirm({ isOpen: false, id: null, userName: null });
          if (data) {
            setTempPasswordData({
              temporaryPassword: data.temporaryPassword,
              expiresAt: data.expiresAt,
              userName: approveConfirm.userName || undefined,
            });
          }
        },
      });
    }
  };

  const getActionItems = (reset: any): ActionMenuItem[] => {
    return [
      {
        label: 'Aprovar',
        icon: <CheckCircle className="h-4 w-4" />,
        onClick: () => handleApprove(reset.id, reset.userName),
        variant: 'success',
      },
    ];
  };

  const formatCPF = (cpf: string) => {
    return cpf.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
  };

  const formatDate = (date: string) => {
    return new Date(date).toLocaleString('pt-BR');
  };

  const getTimeSinceRequest = (date: string) => {
    const requestDate = new Date(date);
    const now = new Date();
    const diffMs = now.getTime() - requestDate.getTime();
    const diffHours = Math.floor(diffMs / (1000 * 60 * 60));
    const diffDays = Math.floor(diffHours / 24);

    if (diffDays > 0) {
      return `Há ${diffDays} dia(s)`;
    } else if (diffHours > 0) {
      return `Há ${diffHours} hora(s)`;
    } else {
      const diffMins = Math.floor(diffMs / (1000 * 60));
      return `Há ${diffMins} minuto(s)`;
    }
  };

  return (
    <div className="p-8">
      <div className="mb-6">
        <h1 className="text-3xl font-bold text-white mb-2">
          Solicitações de Reset de Senha
        </h1>
        <p className="text-zinc-400">
          Aprove solicitações pendentes e gere senhas temporárias
        </p>
      </div>

      {/* Stats */}
      <div className="mb-6 bg-zinc-900 border border-zinc-800 rounded-lg p-4">
        <div className="flex items-center gap-4">
          <div>
            <p className="text-2xl font-bold text-white">{resets.length}</p>
            <p className="text-sm text-zinc-400">Solicitações Pendentes</p>
          </div>
        </div>
      </div>

      {/* Table */}
      {isLoading ? (
        <div className="text-center text-zinc-400 py-12">Carregando...</div>
      ) : resets.length === 0 ? (
        <EmptyState
          icon={Key}
          title="Nenhuma solicitação pendente"
          description="Quando usuários solicitarem reset de senha, aparecerão aqui para aprovação."
        />
      ) : (
        <div className="bg-zinc-900 border border-zinc-800 rounded-lg overflow-hidden">
          <table className="w-full">
            <thead className="bg-zinc-800/50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-zinc-400 uppercase">
                  Usuário
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-zinc-400 uppercase">
                  CPF
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-zinc-400 uppercase">
                  Email
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-zinc-400 uppercase">
                  Solicitado em
                </th>
                <th className="px-6 py-3 text-right text-xs font-medium text-zinc-400 uppercase">
                  Ações
                </th>
              </tr>
            </thead>
            <tbody className="divide-y divide-zinc-800">
              {resets.map((reset) => (
                <tr key={reset.id} className="hover:bg-zinc-800/30">
                  <td className="px-6 py-4 text-sm text-white">{reset.userName}</td>
                  <td className="px-6 py-4 text-sm text-zinc-400">
                    {formatCPF(reset.userCpf)}
                  </td>
                  <td className="px-6 py-4 text-sm text-zinc-400">{reset.userEmail}</td>
                  <td className="px-6 py-4 text-sm text-zinc-400">
                    <div>{formatDate(reset.requestedAt)}</div>
                    <div className="text-xs text-zinc-500">
                      {getTimeSinceRequest(reset.requestedAt)}
                    </div>
                  </td>
                  <td className="px-6 py-4 text-right">
                    <ActionMenu items={getActionItems(reset)} />
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* Modals */}
      <ConfirmationModal
        isOpen={approveConfirm.isOpen}
        onClose={() => setApproveConfirm({ isOpen: false, id: null, userName: null })}
        onConfirm={confirmApprove}
        title="Aprovar Solicitação"
        message={`Deseja aprovar a solicitação de reset de senha para "${approveConfirm.userName}"? Uma senha temporária será gerada.`}
        variant="success"
        confirmText="Aprovar"
        isLoading={approveMutation.isPending}
      />

      <TempPasswordModal
        isOpen={!!tempPasswordData}
        onClose={() => setTempPasswordData(null)}
        data={tempPasswordData}
      />
    </div>
  );
}
