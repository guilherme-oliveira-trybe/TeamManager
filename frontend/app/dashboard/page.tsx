'use client';

import { useAuth } from '@/hooks/api/useAuth';

export default function DashboardPage() {
  const { user, isAdmin, isAthlete, isLoading } = useAuth();

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-zinc-400">Carregando...</div>
      </div>
    );
  }

  if (isAdmin) {
    return (
      <div className="p-8">
        <h1 className="text-3xl font-bold text-white mb-6">
          Dashboard Administrativo
        </h1>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          {/* Stats cards will go here */}
          <div className="bg-zinc-900 border border-zinc-800 rounded-lg p-6">
            <h3 className="text-zinc-400 text-sm font-medium">Usuários Ativos</h3>
            <p className="text-3xl font-bold text-white mt-2">--</p>
          </div>
          <div className="bg-zinc-900 border border-zinc-800 rounded-lg p-6">
            <h3 className="text-zinc-400 text-sm font-medium">Aguardando Ativação</h3>
            <p className="text-3xl font-bold text-white mt-2">--</p>
          </div>
          <div className="bg-zinc-900 border border-zinc-800 rounded-lg p-6">
            <h3 className="text-zinc-400 text-sm font-medium">Solicitações Pendentes</h3>
            <p className="text-3xl font-bold text-white mt-2">--</p>
          </div>
        </div>
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
