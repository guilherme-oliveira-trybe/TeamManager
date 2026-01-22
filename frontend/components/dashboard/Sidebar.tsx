'use client';

import { Home, Users, UserPlus, Key, Activity, Settings } from 'lucide-react';
import { SidebarItem, SidebarSection } from './SidebarItem';
import type { CurrentUser } from '@/types/user';

interface SidebarProps {
  user: CurrentUser;
}

export function Sidebar({ user }: SidebarProps) {
  const isAdmin = user.role === 'Admin';

  return (
    <aside className="w-64 bg-zinc-900 border-r border-zinc-800 flex flex-col">
      {/* Logo/Brand */}
      <div className="p-6 border-b border-zinc-800">
        <h1 className="text-xl font-bold text-white">GFA Team Manager</h1>
        <p className="text-xs text-zinc-400 mt-1">{user.email}</p>
      </div>

      {/* Navigation */}
      <nav className="flex-1 p-4 space-y-6 overflow-y-auto">
        {/* Dashboard Home - Everyone sees */}
        <SidebarSection title="Geral">
          <SidebarItem
            href="/dashboard"
            icon={Home}
            label="Visão Geral"
          />
        </SidebarSection>

        {/* Management - Admin Only */}
        {isAdmin && (
          <SidebarSection title="Gestão">
            <SidebarItem
              href="/dashboard/users"
              icon={Users}
              label="Usuários"
            />
            <SidebarItem
              href="/dashboard/pre-registrations"
              icon={UserPlus}
              label="Pré-Cadastro"
            />
            <SidebarItem
              href="/dashboard/password-resets"
              icon={Key}
              label="Reset Senha"
            />
          </SidebarSection>
        )}

        {/* Activities - Everyone sees but different behavior */}
        <SidebarSection title="Time">
          <SidebarItem
            href="/dashboard/activities"
            icon={Activity}
            label="Atividades"
          />
        </SidebarSection>

        {/* Settings - Everyone sees */}
        <SidebarSection title="Conta">
          <SidebarItem
            href="/dashboard/settings"
            icon={Settings}
            label="Configurações"
          />
        </SidebarSection>
      </nav>
    </aside>
  );
}
