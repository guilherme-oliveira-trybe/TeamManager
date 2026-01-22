'use client';

import { useState } from 'react';
import { X } from 'lucide-react';
import { useCreatePreRegistration } from '@/hooks/api/usePreRegistrations';
import type { CreatePreRegistrationRequest } from '@/types/pre-registration';

interface CreatePreRegModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: (data: any) => void;
}

const profileOptions = [
  { value: 3, label: 'Jogador' },
  { value: 1, label: 'Administrador' },
];

const unitOptions = [
  { value: 1, label: 'Offense (Ataque)' },
  { value: 2, label: 'Defense (Defesa)' },
];

const positionOptions = [
  // Offense
  { value: 1, label: 'QB (Quarterback)', unit: 1 },
  { value: 2, label: 'RB (Running Back)', unit: 1 },
  { value: 3, label: 'WR (Wide Receiver)', unit: 1 },
  { value: 4, label: 'OL (Offensive Line)', unit: 1 },
  // Defense
  { value: 5, label: 'DL (Defensive Line)', unit: 2 },
  { value: 6, label: 'LB (Linebacker)', unit: 2 },
  { value: 7, label: 'DB (Defensive Back)', unit: 2 },
];

export function CreatePreRegModal({ isOpen, onClose, onSuccess }: CreatePreRegModalProps) {
  const [formData, setFormData] = useState<CreatePreRegistrationRequest>({
    cpf: '',
    profile: 3, // Default to Athlete
    unit: undefined,
    position: undefined,
  });

  const createMutation = useCreatePreRegistration();

  if (!isOpen) return null;

  const isAthlete = formData.profile === 3;
  const availablePositions = formData.unit
    ? positionOptions.filter((p) => p.unit === formData.unit)
    : [];

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    // Validate CPF
    const cpfDigits = formData.cpf.replace(/\D/g, '');
    if (cpfDigits.length !== 11) {
      return;
    }

    // Validate athlete fields
    if (isAthlete && (!formData.unit || !formData.position)) {
      return;
    }

    const dataToSend = {
      cpf: cpfDigits,
      profile: formData.profile,
      unit: isAthlete ? formData.unit : undefined,
      position: isAthlete ? formData.position : undefined,
    };

    createMutation.mutate(dataToSend, {
      onSuccess: (data) => {
        onSuccess(data);
        handleClose();
      },
    });
  };

  const handleClose = () => {
    setFormData({
      cpf: '',
      profile: 3,
      unit: undefined,
      position: undefined,
    });
    onClose();
  };

  const formatCPF = (value: string) => {
    const digits = value.replace(/\D/g, '').slice(0, 11);
    return digits
      .replace(/(\d{3})(\d)/, '$1.$2')
      .replace(/(\d{3})(\d)/, '$1.$2')
      .replace(/(\d{3})(\d{1,2})$/, '$1-$2');
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      {/* Backdrop */}
      <div
        className="absolute inset-0 bg-black/50 backdrop-blur-sm"
        onClick={handleClose}
      />

      {/* Modal */}
      <div className="relative bg-zinc-900 border border-zinc-800 rounded-lg shadow-xl w-full max-w-md mx-4">
        <form onSubmit={handleSubmit} className="p-6">
          {/* Header */}
          <div className="flex items-center justify-between mb-6">
            <h3 className="text-xl font-semibold text-white">✨ Novo Pré-Cadastro</h3>
            <button
              type="button"
              onClick={handleClose}
              className="p-2 hover:bg-zinc-800 rounded-lg transition-colors"
            >
              <X className="h-5 w-5 text-zinc-400" />
            </button>
          </div>

          {/* Form */}
          <div className="space-y-4">
            {/* CPF */}
            <div>
              <label className="block text-sm font-medium text-zinc-300 mb-2">
                CPF *
              </label>
              <input
                type="text"
                value={formData.cpf}
                onChange={(e) =>
                  setFormData({ ...formData, cpf: formatCPF(e.target.value) })
                }
                placeholder="000.000.000-00"
                className="w-full px-3 py-2 bg-zinc-800 border border-zinc-700 rounded-lg text-white placeholder-zinc-500 focus:outline-none focus:border-blue-500"
                required
              />
            </div>

            {/* Profile */}
            <div>
              <label className="block text-sm font-medium text-zinc-300 mb-2">
                Perfil *
              </label>
              <div className="space-y-2">
                {profileOptions.map((option) => (
                  <label key={option.value} className="flex items-center gap-2 cursor-pointer">
                    <input
                      type="radio"
                      name="profile"
                      value={option.value}
                      checked={formData.profile === option.value}
                      onChange={(e) =>
                        setFormData({
                          ...formData,
                          profile: Number(e.target.value),
                          unit: undefined,
                          position: undefined,
                        })
                      }
                      className="text-blue-600 focus:ring-blue-500"
                    />
                    <span className="text-zinc-300">{option.label}</span>
                  </label>
                ))}
              </div>
            </div>

            {/* Unit - Only for Athletes */}
            {isAthlete && (
              <div>
                <label className="block text-sm font-medium text-zinc-300 mb-2">
                  Unidade *
                </label>
                <select
                  value={formData.unit || ''}
                  onChange={(e) =>
                    setFormData({
                      ...formData,
                      unit: e.target.value ? Number(e.target.value) : undefined,
                      position: undefined,
                    })
                  }
                  className="w-full px-3 py-2 bg-zinc-800 border border-zinc-700 rounded-lg text-white focus:outline-none focus:border-blue-500"
                  required={isAthlete}
                >
                  <option value="">Selecione...</option>
                  {unitOptions.map((option) => (
                    <option key={option.value} value={option.value}>
                      {option.label}
                    </option>
                  ))}
                </select>
              </div>
            )}

            {/* Position - Only for Athletes with Unit selected */}
            {isAthlete && formData.unit && (
              <div>
                <label className="block text-sm font-medium text-zinc-300 mb-2">
                  Posição (obrigatório para jogador)
                </label>
                <select
                  value={formData.position || ''}
                  onChange={(e) =>
                    setFormData({
                      ...formData,
                      position: e.target.value ? Number(e.target.value) : undefined,
                    })
                  }
                  className="w-full px-3 py-2 bg-zinc-800 border border-zinc-700 rounded-lg text-white focus:outline-none focus:border-blue-500"
                  required={isAthlete}
                >
                  <option value="">Selecione...</option>
                  {availablePositions.map((option) => (
                    <option key={option.value} value={option.value}>
                      {option.label}
                    </option>
                  ))}
                </select>
              </div>
            )}
          </div>

          {/* Footer */}
          <div className="flex gap-3 mt-6">
            <button
              type="button"
              onClick={handleClose}
              className="flex-1 px-4 py-2 bg-zinc-800 hover:bg-zinc-700 text-white rounded-lg transition-colors"
              disabled={createMutation.isPending}
            >
              Cancelar
            </button>
            <button
              type="submit"
              className="flex-1 px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg transition-colors disabled:opacity-50"
              disabled={createMutation.isPending}
            >
              {createMutation.isPending ? 'Criando...' : '✨ Criar'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
