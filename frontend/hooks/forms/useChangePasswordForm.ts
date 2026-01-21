/**
 * React Hook Form custom hook for change password
 */

'use client';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { 
  changePasswordSchema, 
  type ChangePasswordFormData 
} from '@/lib/validations/schemas';

/**
 * Hook for change password form
 */
export function useChangePasswordForm() {
  return useForm<ChangePasswordFormData>({
    resolver: zodResolver(changePasswordSchema),
    defaultValues: {
      currentPassword: '',
      newPassword: '',
      confirmNewPassword: '',
    },
    mode: 'onBlur',
  });
}
