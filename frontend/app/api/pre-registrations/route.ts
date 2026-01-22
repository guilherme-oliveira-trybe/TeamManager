import { NextRequest, NextResponse } from 'next/server';
import axios from 'axios';

export async function GET(request: NextRequest) {
  try {
    const authToken = request.cookies.get('auth_token')?.value;

    if (!authToken) {
      return NextResponse.json(
        { errors: ['Não autorizado'] },
        { status: 401 }
      );
    }

    const backendUrl = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000';
    
    const response = await axios.get(
      `${backendUrl}/api/pre-registrations`,
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
        { errors: error.response?.data?.errors || ['Erro ao buscar pré-cadastros'] },
        { status: error.response?.status || 500 }
      );
    }

    return NextResponse.json(
      { errors: ['Erro interno do servidor'] },
      { status: 500 }
    );
  }
}

export async function POST(request: NextRequest) {
  try {
    const authToken = request.cookies.get('auth_token')?.value;

    if (!authToken) {
      return NextResponse.json(
        { errors: ['Não autorizado'] },
        { status: 401 }
      );
    }

    const body = await request.json();
    const backendUrl = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000';
    
    const response = await axios.post(
      `${backendUrl}/api/pre-registrations`,
      body,
      {
        headers: {
          Authorization: `Bearer ${authToken}`,
          'Content-Type': 'application/json',
        },
      }
    );

    return NextResponse.json(response.data);
  } catch (error) {
    if (axios.isAxiosError(error)) {
      return NextResponse.json(
        { errors: error.response?.data?.errors || ['Erro ao criar pré-cadastro'] },
        { status: error.response?.status || 500 }
      );
    }

    return NextResponse.json(
      { errors: ['Erro interno do servidor'] },
      { status: 500 }
    );
  }
}
