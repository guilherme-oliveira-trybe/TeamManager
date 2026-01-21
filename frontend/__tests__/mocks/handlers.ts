import { http, HttpResponse } from 'msw';

const API_URL = 'http://localhost:3000';

/**
 * MSW Request Handlers for API mocking
 * These handlers intercept API calls made during tests
 */
export const handlers = [
  // Login endpoint
  http.post(`${API_URL}/api/auth/login`, async ({ request }): Promise<any> => {
    const body = await request.json() as { login: string; password: string };
    
    // Simulate successful login with normal password
    if (body.login === '61120319064' && body.password === 'Teste@123') {
      return HttpResponse.json(
        { success: true, requiresPasswordChange: false },
        { status: 200 }
      );
    }
    
    // Simulate successful login with temporary password
    if (body.login === '61120319064' && body.password === 'TEMP_PASS_123') {
      return HttpResponse.json(
        { success: true, requiresPasswordChange: true },
        { status: 200 }
      );
    }
    
    // Simulate blocked login when APPROVED reset request exists
    if (body.login === 'blocked@user.com' && body.password === 'OldPassword@123') {
      return HttpResponse.json(
        { errors: ['Uma solicitação de reset de senha foi aprovada. Por favor, utilize a senha temporária enviada.'] },
        { status: 400 }
      );
    }
    
    // Simulate invalid credentials
    return HttpResponse.json(
      { errors: ['Credenciais inválidas'] },
      { status: 401 }
    );
  }),

  // Complete Registration endpoint
  http.post(`${API_URL}/api/users/complete-registration`, async ({ request }): Promise<any> => {
    const body = await request.json() as any;
    
    // Validate CPF (mock validation)
    if (body.cpf && body.activationCode) {
      return HttpResponse.json({
        isSuccess: true,
        data: { id: 'mock-user-id', email: body.email },
        errors: [],
      }, { status: 201 });
    }
    
    // Simulate validation error
    return HttpResponse.json({
      isSuccess: false,
      data: null,
      errors: ['Código de ativação inválido'],
    }, { status: 400 });
  }),

  // Password Reset Request endpoint
  http.post(`${API_URL}/api/auth/request-password-reset`, async ({ request }): Promise<any> => {
    const body = await request.json() as { cpf: string; email: string };
    
    // Simulate successful password reset request
    if (body.cpf && body.email) {
      return HttpResponse.json({
        isSuccess: true,
        data: null,
        errors: [],
      }, { status: 200 });
    }
    
    // Simulate error
    return HttpResponse.json({
      isSuccess: false,
      data: null,
      errors: ['CPF ou email não encontrado'],
    }, { status: 404 });
  }),

  // Change Password endpoint
  http.post(`${API_URL}/api/auth/change-password`, async ({ request }): Promise<any> => {
    const body = await request.json() as { currentPassword: string; newPassword: string; confirmNewPassword: string };
    const authHeader = request.headers.get('cookie');
    
    // Simulate unauthorized (no token)
    if (!authHeader || !authHeader.includes('auth_token')) {
      return HttpResponse.json(
        { errors: ['Não autorizado'] },
        { status: 401 }
      );
    }
    
    // Simulate passwords don't match
    if (body.newPassword !== body.confirmNewPassword) {
      return HttpResponse.json({
        isSuccess: false,
        data: null,
        errors: ['As senhas não coincidem'],
      }, { status: 400 });
    }
    
    // Simulate password too short
    if (body.newPassword.length < 8) {
      return HttpResponse.json({
        isSuccess: false,
        data: null,
        errors: ['A senha deve ter no mínimo 8 caracteres'],
      }, { status: 400 });
    }
    
    // Simulate success with temporary password
    if (body.currentPassword === 'TEMP_PASS_123') {
      return HttpResponse.json({
        isSuccess: true,
        data: null,
        errors: [],
      }, { status: 200 });
    }
    
    // Simulate success with normal password
    if (body.currentPassword === 'Teste@123') {
      return HttpResponse.json({
        isSuccess: true,
        data: null,
        errors: [],
      }, { status: 200 });
    }
    
    // Simulate invalid current password
    return HttpResponse.json({
      isSuccess: false,
      data: null,
      errors: ['Senha atual incorreta'],
    }, { status: 400 });
  }),

  // Logout endpoint
  http.post(`${API_URL}/api/auth/logout`, async (): Promise<any> => {
    return HttpResponse.json({ success: true }, { status: 200 });
  }),

  // Backend pre-registration validation (if called directly)
  http.post(`${API_URL}/api/pre-registrations/validate`, async ({ request }): Promise<any> => {
    const body = await request.json() as { cpf: string; activationCode: string };
    
    if (body.cpf === '61120319064' && body.activationCode === 'ELUIP9IA') {
      return HttpResponse.json({
        isSuccess: true,
        data: {
          id: '4b9d3120-5f36-493c-bfca-d03b1f7faf33',
          cpf: '61120319064',
          profile: 3,
          unit: 'Defense',
          position: 'DL',
        },
        errors: [],
      });
    }
    
    return HttpResponse.json({
      isSuccess: false,
      data: null,
      errors: ['Pré-cadastro não encontrado ou já utilizado'],
    }, { status: 404 });
  }),
];
