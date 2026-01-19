/**
 * React Hook Form custom hook for password reset request
 */

'use client';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { 
  passwordResetSchema, 
  type PasswordResetFormData 
} from '@/lib/validations/schemas';

/**
 * Hook for password reset request form
 */
export function usePasswordResetForm() {
  return useForm<PasswordResetFormData>({
    resolver: zodResolver(passwordResetSchema),
    defaultValues: {
      cpf: '',
      email: '',
    },
    mode: 'onBlur',
  });
}
