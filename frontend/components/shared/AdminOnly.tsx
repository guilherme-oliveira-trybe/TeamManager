'use client';

import { useAuth } from '@/hooks/api/useAuth';

interface AdminOnlyProps {
  children: React.ReactNode;
}

export function AdminOnly({ children }: AdminOnlyProps) {
  const { isAdmin } = useAuth();
  
  if (!isAdmin) return null;
  
  return <>{children}</>;
}
