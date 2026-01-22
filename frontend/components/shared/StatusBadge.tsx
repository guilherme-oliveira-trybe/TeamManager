import { UserStatus } from '@/types/user';
import { cn } from '@/lib/utils';

interface StatusBadgeProps {
  status: UserStatus;
  className?: string;
}

const statusConfig = {
  [UserStatus.PendingRegistration]: {
    label: 'Pendente',
    color: 'bg-blue-500/10 text-blue-500 border-blue-500/20',
    icon: '🔵',
  },
  [UserStatus.AwaitingActivation]: {
    label: 'Aguardando',
    color: 'bg-amber-500/10 text-amber-500 border-amber-500/20',
    icon: '🟡',
  },
  [UserStatus.Active]: {
    label: 'Ativo',
    color: 'bg-green-500/10 text-green-500 border-green-500/20',
    icon: '🟢',
  },
  [UserStatus.Rejected]: {
    label: 'Rejeitado',
    color: 'bg-gray-500/10 text-gray-500 border-gray-500/20',
    icon: '⚫',
  },
  [UserStatus.Inactive]: {
    label: 'Inativo',
    color: 'bg-red-500/10 text-red-500 border-red-500/20',
    icon: '🔴',
  },
};

export function StatusBadge({ status, className }: StatusBadgeProps) {
  const config = statusConfig[status];

  return (
    <span
      className={cn(
        'inline-flex items-center gap-1.5 px-2.5 py-1 rounded-md text-xs font-medium border',
        config.color,
        className
      )}
    >
      <span>{config.icon}</span>
      <span>{config.label}</span>
    </span>
  );
}
