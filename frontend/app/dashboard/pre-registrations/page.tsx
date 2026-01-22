'use client';

import { useState } from 'react';
import { UserPlus, Copy, RefreshCw } from 'lucide-react';
import { LoadingSpinnerInline } from '@/components/shared/LoadingSpinner';
import { usePreRegistrations, useRegenerateActivationCode } from '@/hooks/api/usePreRegistrations';
import { CreatePreRegModal } from '@/components/dashboard/CreatePreRegModal';
import { PreRegSuccessModal } from '@/components/dashboard/PreRegSuccessModal';
import { ConfirmationModal } from '@/components/shared/ConfirmationModal';
import { EmptyState } from '@/components/shared/EmptyState';
import { ActionMenu, ActionMenuItem } from '@/components/shared/ActionMenu';
import type { PreRegistration } from '@/types/pre-registration';

export default function PreRegistrationsPage() {
  const [createModalOpen, setCreateModalOpen] = useState(false);
  const [successModalData, setSuccessModalData] = useState<PreRegistration | null>(null);
  const [regenerateConfirm, setRegenerateConfirm] = useState<{
    isOpen: boolean;
    id: string | null;
    cpf: string | null;
  }>({ isOpen: false, id: null, cpf: null });

  const { data: preRegs = [], isLoading } = usePreRegistrations();
  const regenerateMutation = useRegenerateActivationCode();

  const handleCreateSuccess = (data: PreRegistration) => {
    setSuccessModalData(data);
  };

  const handleCopy = (code: string) => {
    navigator.clipboard.writeText(code);
  };

  const handleRegenerate = (id: string, cpf: string) => {
    setRegenerateConfirm({ isOpen: true, id, cpf });
  };

  const confirmRegenerate = () => {
    if (regenerateConfirm.id) {
      regenerateMutation.mutate(regenerateConfirm.id, {
        onSuccess: (data) => {
          setRegenerateConfirm({ isOpen: false, id: null, cpf: null });
          if (data) {
            setSuccessModalData(data);
          }
        },
      });
    }
  };

  const getActionItems = (preReg: PreRegistration): ActionMenuItem[] => {
    const items: ActionMenuItem[] = [
      {
        label: 'Copiar Código',
        icon: <Copy className="h-4 w-4" />,
        onClick: () => handleCopy(preReg.activationCode),
      },
    ];

    if (!preReg.isUsed) {
      items.push({ type: 'divider' });
      items.push({
        label: 'Regenerar Código',
        icon: <RefreshCw className="h-4 w-4" />,
        onClick: () => handleRegenerate(preReg.id, preReg.cpf),
        variant: 'warning',
      });
    }

    return items;
  };

  const formatCPF = (cpf: string) => {
    return cpf.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
  };

  const getDaysRemaining = (expirationDate: string) => {
    const expires = new Date(expirationDate);
    const now = new Date();
    const diffTime = expires.getTime() - now.getTime();
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return diffDays;
  };

  const profileLabels: Record<number, string> = {
    1: 'Admin',
    3: 'Jogador',
  };

  return (
    <div className="p-8">
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-3xl font-bold text-white mb-2">Pré-Cadastros</h1>
          <p className="text-zinc-400">Gerencie códigos de ativação</p>
        </div>
        <button
          onClick={() => setCreateModalOpen(true)}
          className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg transition-colors flex items-center gap-2"
        >
          <UserPlus className="h-5 w-5" />
          Novo Pré-Cadastro
        </button>
      </div>

      {isLoading ? (
        <LoadingSpinnerInline />
      ) : preRegs.length === 0 ? (
        <EmptyState
          icon={UserPlus}
          title="Nenhum pré-cadastro encontrado"
          description="Crie um novo pré-cadastro para gerar um código de ativação."
          action={{
            label: 'Criar Pré-Cadastro',
            onClick: () => setCreateModalOpen(true),
          }}
        />
      ) : (
        <div className="bg-zinc-900 border border-zinc-800 rounded-lg"> {/* Removed overflow-hidden */}
          <table className="w-full">
            <thead className="bg-zinc-800/50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-zinc-400 uppercase rounded-tl-lg"> {/* Added radius */}
                  CPF
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-zinc-400 uppercase">
                  Perfil
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-zinc-400 uppercase">
                  Código
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-zinc-400 uppercase">
                  Status
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-zinc-400 uppercase">
                  Expira em
                </th>
                <th className="px-6 py-3 text-right text-xs font-medium text-zinc-400 uppercase rounded-tr-lg"> {/* Added radius */}
                  Ações
                </th>
              </tr>
            </thead>
            <tbody className="divide-y divide-zinc-800">
              {preRegs.map((preReg) => {
                const daysRemaining = getDaysRemaining(preReg.expirationDate);
                const isExpired = daysRemaining < 0;
                const isExpiringSoon = daysRemaining <= 2 && daysRemaining >= 0;

                return (
                  <tr key={preReg.id} className="hover:bg-zinc-800/30">
                    <td className="px-6 py-4 text-sm text-white">
                      {formatCPF(preReg.cpf)}
                    </td>
                    <td className="px-6 py-4 text-sm text-zinc-400">
                      {profileLabels[preReg.profile] || 'Desconhecido'}
                    </td>
                    <td className="px-6 py-4">
                      <code className="text-sm font-mono text-blue-400">
                        {preReg.activationCode}
                      </code>
                    </td>
                    <td className="px-6 py-4">
                      {preReg.isUsed ? (
                        <span className="px-2 py-1 text-xs font-medium bg-green-500/10 text-green-500 border border-green-500/20 rounded">
                          ✅ Usado
                        </span>
                      ) : isExpired ? (
                        <span className="px-2 py-1 text-xs font-medium bg-red-500/10 text-red-500 border border-red-500/20 rounded">
                          ⏱️ Expirado
                        </span>
                      ) : (
                        <span className="px-2 py-1 text-xs font-medium bg-amber-500/10 text-amber-500 border border-amber-500/20 rounded">
                          ⏳ Pendente
                        </span>
                      )}
                    </td>
                    <td className="px-6 py-4 text-sm">
                      <span
                        className={
                          isExpired
                            ? 'text-red-400'
                            : isExpiringSoon
                            ? 'text-amber-400'
                            : 'text-zinc-400'
                        }
                      >
                        {isExpired
                          ? `Expirado há ${Math.abs(daysRemaining)} dia(s)`
                          : `${daysRemaining} dia(s)`}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-right">
                      <ActionMenu items={getActionItems(preReg)} />
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      )}

      {/* Modals */}
      <CreatePreRegModal
        isOpen={createModalOpen}
        onClose={() => setCreateModalOpen(false)}
        onSuccess={handleCreateSuccess}
      />

      <PreRegSuccessModal
        isOpen={!!successModalData}
        onClose={() => setSuccessModalData(null)}
        preRegistration={successModalData}
      />

      <ConfirmationModal
        isOpen={regenerateConfirm.isOpen}
        onClose={() => setRegenerateConfirm({ isOpen: false, id: null, cpf: null })}
        onConfirm={confirmRegenerate}
        title="Regenerar Código"
        message={`Deseja regenerar o código de ativação para o CPF ${
          regenerateConfirm.cpf ? formatCPF(regenerateConfirm.cpf) : ''
        }? O código atual será invalidado.`}
        variant="warning"
        confirmText="Regenerar"
        isLoading={regenerateMutation.isPending}
      />
    </div>
  );
}
