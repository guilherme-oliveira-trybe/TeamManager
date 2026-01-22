'use client';

import { useState } from 'react';
import { X, Copy, Check } from 'lucide-react';
import type { PreRegistration } from '@/types/pre-registration';

interface PreRegSuccessModalProps {
  isOpen: boolean;
  onClose: () => void;
  preRegistration: PreRegistration | null;
}

export function PreRegSuccessModal({
  isOpen,
  onClose,
  preRegistration,
}: PreRegSuccessModalProps) {
  const [copied, setCopied] = useState(false);

  if (!isOpen || !preRegistration) return null;

  const copyToClipboard = () => {
    navigator.clipboard.writeText(preRegistration.activationCode);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  const formatCPF = (cpf: string) => {
    return cpf.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
  };

  const getExpirationDays = () => {
    const expiresAt = new Date(preRegistration.expiresAt);
    const now = new Date();
    const diffTime = expiresAt.getTime() - now.getTime();
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return diffDays;
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      {/* Backdrop */}
      <div
        className="absolute inset-0 bg-black/50 backdrop-blur-sm"
        onClick={onClose}
      />

      {/* Modal */}
      <div className="relative bg-zinc-900 border border-zinc-800 rounded-lg shadow-xl w-full max-w-md mx-4">
        <div className="p-6">
          {/* Header */}
          <div className="flex items-center justify-between mb-6">
            <div className="flex items-center gap-2">
              <div className="p-2 bg-green-500/10 rounded-lg">
                <Check className="h-6 w-6 text-green-500" />
              </div>
              <h3 className="text-xl font-semibold text-white">
                Pré-Cadastro Criado
              </h3>
            </div>
            <button
              onClick={onClose}
              className="p-2 hover:bg-zinc-800 rounded-lg transition-colors"
            >
              <X className="h-5 w-5 text-zinc-400" />
            </button>
          </div>

          {/* Content */}
          <div className="space-y-4">
            <div>
              <p className="text-sm text-zinc-400 mb-1">CPF:</p>
              <p className="text-white font-medium">{formatCPF(preRegistration.cpf)}</p>
            </div>

            <div>
              <p className="text-sm text-zinc-400 mb-2">Código de Ativação:</p>
              <div className="flex items-center gap-2 p-3 bg-zinc-800 rounded-lg border border-zinc-700">
                <code className="flex-1 text-lg font-mono text-white">
                  {preRegistration.activationCode}
                </code>
                <button
                  onClick={copyToClipboard}
                  className="p-2 hover:bg-zinc-700 rounded transition-colors"
                  title="Copiar código"
                >
                  {copied ? (
                    <Check className="h-5 w-5 text-green-500" />
                  ) : (
                    <Copy className="h-5 w-5 text-zinc-400" />
                  )}
                </button>
              </div>
            </div>

            <div className="p-3 bg-amber-500/10 border border-amber-500/20 rounded-lg">
              <p className="text-sm text-amber-500">
                ⚠️ <strong>Importante:</strong> Envie este código para o usuário. 
                Ele terá <strong>{getExpirationDays()} dias</strong> para completar o cadastro.
              </p>
            </div>
          </div>

          {/* Footer */}
          <div className="mt-6">
            <button
              onClick={onClose}
              className="w-full px-4 py-2 bg-zinc-800 hover:bg-zinc-700 text-white rounded-lg transition-colors"
            >
              Fechar
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
