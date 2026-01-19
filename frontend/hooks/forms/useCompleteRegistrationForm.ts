/**
 * React Hook Form custom hook for complete registration
 */

'use client';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { 
  completeRegistrationSchema, 
  type CompleteRegistrationFormData 
} from '@/lib/validations/schemas';

/**
 * Hook for complete registration form
 * Multi-step form with full validation
 */
export function useCompleteRegistrationForm() {
  return useForm<CompleteRegistrationFormData>({
    resolver: zodResolver(completeRegistrationSchema),
    defaultValues: {
      cpf: '',
      activationCode: '',
      fullName: '',
      birthDate: undefined,
      weight: undefined,
      height: undefined,
      phone: '',
      email: '',
      password: '',
      confirmPassword: '',
      emergencyContactName: '',
      emergencyContactPhone: '',
    },
    mode: 'onBlur',
  });
}
