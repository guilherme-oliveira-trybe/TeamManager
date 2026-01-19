import { NextResponse } from 'next/server';
import { cookies } from 'next/headers';

/**
 * POST /api/auth/logout
 * Clears authentication cookie
 */
export async function POST() {
  try {
    // Clear auth cookie
    const cookieStore = await cookies();
    cookieStore.delete('auth_token');

    return NextResponse.json({ success: true });
  } catch (error) {
    return NextResponse.json(
      { errors: ['Erro ao fazer logout'] },
      { status: 500 }
    );
  }
}
