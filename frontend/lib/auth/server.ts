import { headers } from 'next/headers';
import type { CurrentUser } from '@/types/user';

export async function getCurrentUserFromHeaders(): Promise<CurrentUser | null> {
  const headersList = await headers();
  
  const id = headersList.get('x-user-id');
  const email = headersList.get('x-user-email');
  const role = headersList.get('x-user-role');
  
  if (!id || !email || !role) {
    return null;
  }
  
  // Mapeia Profile enum do backend para número
  const profileMap: Record<string, number> = {
    'Admin': 1,
    'Coach': 2,
    'Athlete': 3,
    'Staff': 4,
  };
  
  return {
    id,
    email,
    role: role as any,
    profile: profileMap[role] || 3,
    unit: headersList.get('x-user-unit') ? parseInt(headersList.get('x-user-unit')!) : undefined,
    position: headersList.get('x-user-position') ? parseInt(headersList.get('x-user-position')!) : undefined,
  };
}
