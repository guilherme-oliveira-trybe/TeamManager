'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAuth } from './api/useAuth';
import { useToast } from './useToast';

export function useAdminOnly() {
  const { user, isAdmin, isLoading } = useAuth();
  const router = useRouter();
  const toast = useToast();
  
  useEffect(() => {
    if (!isLoading && user && !isAdmin) {
      router.push('/dashboard');
      toast.error('Acesso negado. Área restrita a administradores.');
    }
  }, [user, isAdmin, isLoading, router, toast]);
  
  return { isAdmin, isLoading };
}
