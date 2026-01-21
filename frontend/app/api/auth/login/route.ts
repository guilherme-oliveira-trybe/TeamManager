import { NextRequest, NextResponse } from 'next/server';
import { cookies } from 'next/headers';
import axios from 'axios';

const BACKEND_URL = process.env.BACKEND_URL || 'http://localhost:5000';

interface LoginResponse {
  isSuccess: boolean;
  data?: {
    token: string;
    expiresAt: string;
    requiresPasswordChange: boolean;
  };
  errors: string[];
}

export async function POST(request: NextRequest) {
  try {
    const body = await request.json();

    const { data } = await axios.post<LoginResponse>(
      `${BACKEND_URL}/api/auth/login`,
      body
    );

    if (data.isSuccess && data.data) {
      const cookieStore = await cookies();
      cookieStore.set({
        name: 'auth_token',
        value: data.data.token,
        httpOnly: true,
        secure: process.env.NODE_ENV === 'production',
        sameSite: 'lax',
        maxAge: 60 * 60 * 8,
        path: '/',
      });

      return NextResponse.json({ 
        success: true,
        requiresPasswordChange: data.data.requiresPasswordChange 
      });
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
