import { NextRequest, NextResponse } from 'next/server';
import axios from 'axios';

export async function POST(
  request: NextRequest,
  context: { params: Promise<{ id: string }> }
) {
  try {
    const authToken = request.cookies.get('auth_token')?.value;

    if (!authToken) {
      return NextResponse.json(
        { errors: ['Não autorizado'] },
        { status: 401 }
      );
    }

    const { id } = await context.params;
    const backendUrl = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5268';
    
    const response = await axios.post(
      `${backendUrl}/api/auth/password-reset-requests/${id}/approve`,
      {},
      {
        headers: {
          Authorization: `Bearer ${authToken}`,
        },
      }
    );

    return NextResponse.json(response.data);
  } catch (error) {
    if (axios.isAxiosError(error)) {
      return NextResponse.json(
        { errors: error.response?.data?.errors || ['Erro ao aprovar solicitação'] },
        { status: error.response?.status || 500 }
      );
    }

    return NextResponse.json(
      { errors: ['Erro interno do servidor'] },
      { status: 500 }
    );
  }
}
