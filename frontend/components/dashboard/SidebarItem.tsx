'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { LucideIcon } from 'lucide-react';
import { cn } from '@/lib/utils';

interface SidebarItemProps {
  href: string;
  icon: LucideIcon;
  label: string;
  badge?: number;
}

export function SidebarItem({ href, icon: Icon, label, badge }: SidebarItemProps) {
  const pathname = usePathname();
  const isActive = pathname === href || pathname?.startsWith(`${href}/`);

  return (
    <Link
      href={href}
      className={cn(
        'flex items-center gap-3 px-3 py-2 rounded-lg transition-colors',
        'hover:bg-zinc-800',
        isActive ? 'bg-zinc-800 text-white' : 'text-zinc-400'
      )}
    >
      <Icon className="h-5 w-5" />
      <span className="flex-1">{label}</span>
      {badge !== undefined && badge > 0 && (
        <span className="px-2 py-0.5 text-xs font-medium bg-blue-600 text-white rounded-full">
          {badge}
        </span>
      )}
    </Link>
  );
}

interface SidebarSectionProps {
  title: string;
  children: React.ReactNode;
}

export function SidebarSection({ title, children }: SidebarSectionProps) {
  return (
    <div className="space-y-2">
      <h3 className="px-3 text-xs font-semibold text-zinc-500 uppercase tracking-wider">
        {title}
      </h3>
      <div className="space-y-1">{children}</div>
    </div>
  );
}
