'use client';

import { AlertTriangle } from 'lucide-react';
import { cn } from '@/lib/utils';

interface ConfirmationModalProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void;
  title: string;
  message: string;
  variant?: 'warning' | 'danger' | 'success' | 'info';
  confirmText?: string;
  cancelText?: string;
  isLoading?: boolean;
}

const variantStyles = {
  warning: {
    icon: 'text-amber-500',
    button: 'bg-amber-600 hover:bg-amber-700',
  },
  danger: {
    icon: 'text-red-500',
    button: 'bg-red-600 hover:bg-red-700',
  },
  success: {
    icon: 'text-green-500',
    button: 'bg-green-600 hover:bg-green-700',
  },
  info: {
    icon: 'text-blue-500',
    button: 'bg-blue-600 hover:bg-blue-700',
  },
};

export function ConfirmationModal({
  isOpen,
  onClose,
  onConfirm,
  title,
  message,
  variant = 'warning',
  confirmText = 'Confirmar',
  cancelText = 'Cancelar',
  isLoading = false,
}: ConfirmationModalProps) {
  if (!isOpen) return null;

  const styles = variantStyles[variant];

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
          <div className="flex items-start gap-4">
            <div className={cn('p-2 rounded-full bg-zinc-800', styles.icon)}>
              <AlertTriangle className="h-6 w-6" />
            </div>
            <div className="flex-1">
              <h3 className="text-lg font-semibold text-white mb-2">
                {title}
              </h3>
              <p className="text-zinc-400 text-sm">{message}</p>
            </div>
          </div>

          <div className="flex gap-3 mt-6">
            <button
              onClick={onClose}
              disabled={isLoading}
              className="flex-1 px-4 py-2 bg-zinc-800 hover:bg-zinc-700 text-white rounded-lg transition-colors disabled:opacity-50"
            >
              {cancelText}
            </button>
            <button
              onClick={onConfirm}
              disabled={isLoading}
              className={cn(
                'flex-1 px-4 py-2 text-white rounded-lg transition-colors disabled:opacity-50',
                styles.button
              )}
            >
              {isLoading ? 'Aguarde...' : confirmText}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
