/**
 * React Hook Form custom hooks for authentication forms
 */

'use client';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { loginSchema, type LoginFormData } from '@/lib/validations/schemas';

/**
 * Hook for login form
 * Provides form state, validation, and submission handling
 */
export function useLoginForm() {
  return useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      login: '',
      password: '',
    },
    mode: 'onBlur', // Validate on blur for better UX
  });
}
