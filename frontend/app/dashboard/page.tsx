'use client';

import { LoadingSpinner } from '@/components/shared/LoadingSpinner';
import { useAuth } from '@/hooks/api/useAuth';
import { useUsers } from '@/hooks/api/useUsers';
import { usePreRegistrations } from '@/hooks/api/usePreRegistrations';
import { usePendingResets } from '@/hooks/api/usePasswordResets';
import { UserStatus, User } from '@/types/user';

export default function DashboardPage() {
  const { user, isAdmin, isAthlete, isLoading: authLoading } = useAuth();
  
  // Fetch data for admin stats
  const { data: usersResult } = useUsers();
  const users: User[] = usersResult?.data || [];
  const { data: preRegs = [] } = usePreRegistrations();
  const { data: pendingResets = [] } = usePendingResets();

  const isLoading = authLoading;

  if (isLoading) {
    return <LoadingSpinner />;
  }

  // Calculate stats
  const activeUsersCount = users.filter(u => u.status === UserStatus.Active).length;
  const waitingUsersCount = users.filter(u => u.status === UserStatus.AwaitingActivation).length;
  const pendingRequestsCount = pendingResets.length;

  if (isAdmin) {
    return (
      <div className="p-8">
        <h1 className="text-3xl font-bold text-white mb-6">
          Dashboard Administrativo
        </h1>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          {/* Active Users */}
          <div className="bg-zinc-900 border border-zinc-800 rounded-lg p-6 hover:border-zinc-700 transition-colors">
            <h3 className="text-zinc-400 text-sm font-medium uppercase tracking-wider">Usuários Ativos</h3>
            <p className="text-3xl font-bold text-white mt-2">{activeUsersCount}</p>
          </div>
          
          {/* Awaiting Activation */}
          <div className="bg-zinc-900 border border-zinc-800 rounded-lg p-6 hover:border-zinc-700 transition-colors">
            <h3 className="text-zinc-400 text-sm font-medium uppercase tracking-wider">Aguardando Ativação</h3>
            <div className="flex items-end gap-2 mt-2">
              <p className="text-3xl font-bold text-white">{waitingUsersCount}</p>
              {waitingUsersCount > 0 && (
                <span className="text-sm text-amber-500 mb-1 font-medium">
                  • Pendentes
                </span>
              )}
            </div>
          </div>

          {/* Pending Password Resets */}
          <div className="bg-zinc-900 border border-zinc-800 rounded-lg p-6 hover:border-zinc-700 transition-colors">
            <h3 className="text-zinc-400 text-sm font-medium uppercase tracking-wider">Solicitações de Senha</h3>
            <div className="flex items-end gap-2 mt-2">
              <p className="text-3xl font-bold text-white">{pendingRequestsCount}</p>
              {pendingRequestsCount > 0 && (
                <span className="text-sm text-amber-500 mb-1 font-medium">
                  • Requer atenção
                </span>
              )}
            </div>
          </div>
        </div>

        {/* Quick Actions / Recent Activity could go here later */}
      </div>
    );
  }

  if (isAthlete) {
    return (
      <div className="p-8">
        <h1 className="text-3xl font-bold text-white mb-6">
          Bem-vindo, {user?.email}
        </h1>
        <div className="bg-zinc-900 border border-zinc-800 rounded-lg p-6">
          <p className="text-zinc-400">
            Dashboard do atleta em desenvolvimento...
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="p-8">
      <h1 className="text-3xl font-bold text-white mb-6">Dashboard</h1>
      <div className="bg-zinc-900 border border-zinc-800 rounded-lg p-6">
        <p className="text-zinc-400">Bem-vindo ao sistema!</p>
      </div>
    </div>
  );
}
