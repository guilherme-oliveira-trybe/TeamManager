'use client';

import { Home, Users, UserPlus, Key, Activity, Settings } from 'lucide-react';
import { useAuth } from '@/hooks/api/useAuth';
import { SidebarItem, SidebarSection } from './SidebarItem';

export function Sidebar() {
  const { isAdmin } = useAuth();

  return (
    <aside className="w-64 bg-zinc-900 border-r border-zinc-800 flex flex-col">
      {/* Logo/Brand */}
      <div className="p-6 border-b border-zinc-800">
        <h1 className="text-xl font-bold text-white">GFA Team Manager</h1>
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
