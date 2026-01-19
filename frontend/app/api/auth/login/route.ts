import { NextRequest, NextResponse } from 'next/server';
import { cookies } from 'next/headers';
import axios from 'axios';

const BACKEND_URL = process.env.BACKEND_URL || 'http://localhost:5000';

interface LoginResponse {
  isSuccess: boolean;
  data?: {
    token: string;
    expiresAt: string;
  };
  errors: string[];
}

/**
 * POST /api/auth/login
 * Handles user login and sets httpOnly cookie
 */
export async function POST(request: NextRequest) {
  try {
    const body = await request.json();

    // Call backend API
    const { data } = await axios.post<LoginResponse>(
      `${BACKEND_URL}/api/auth/login`,
      body
    );

    if (data.isSuccess && data.data) {
      // Set httpOnly cookie with JWT token
      const cookieStore = await cookies();
      cookieStore.set({
        name: 'auth_token',
        value: data.data.token,
        httpOnly: true,
        secure: process.env.NODE_ENV === 'production',
        sameSite: 'lax',
        maxAge: 60 * 60 * 8, // 8 hours
        path: '/',
      });

      return NextResponse.json({ success: true });
    }

    return NextResponse.json(data, { status: 401 });
  } catch (error) {
    if (axios.isAxiosError(error) && error.response) {
      return NextResponse.json(error.response.data, { 
        status: error.response.status 
      });
    }
    
    return NextResponse.json(
      { errors: ['Erro ao conectar com servidor'] },
      { status: 500 }
    );
  }
}
