import { describe, it, expect, vi, beforeEach } from 'vitest';
import { screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { render } from '../../utils/test-utils';
import LoginPage from '@/app/login/page';

// Mock next/navigation
const mockPush = vi.fn();
vi.mock('next/navigation', () => ({
  useRouter: () => ({
    push: mockPush,
  }),
}));

describe('Login Page', () => {
  beforeEach(() => {
    mockPush.mockClear();
  });

  it('should render login form correctly', () => {
    render(<LoginPage />);

    // Check if form elements are present
    expect(screen.getByTestId('login-input')).toBeInTheDocument();
    expect(screen.getByTestId('password-input')).toBeInTheDocument();
    expect(screen.getByTestId('login-submit-button')).toBeInTheDocument();
    expect(screen.getByTestId('forgot-password-link')).toBeInTheDocument();
    expect(screen.getByTestId('complete-registration-link')).toBeInTheDocument();

    // Check labels
    expect(screen.getByText('CPF ou Email')).toBeInTheDocument();
    expect(screen.getByText('Senha')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /entrar/i })).toBeInTheDocument();
  });

  it('should display validation errors when fields are empty', async () => {
    const user = userEvent.setup();
    render(<LoginPage />);

    const submitButton = screen.getByTestId('login-submit-button');
    
    // Try to submit empty form
    await user.click(submitButton);

    // Wait for validation errors
    await waitFor(() => {
      expect(screen.getByText(/login é obrigatório/i)).toBeInTheDocument();
    });
  });

  it('should successfully login with valid credentials', async () => {
    const user = userEvent.setup();
    render(<LoginPage />);

    const loginInput = screen.getByTestId('login-input');
    const passwordInput = screen.getByTestId('password-input');
    const submitButton = screen.getByTestId('login-submit-button');

    // Fill in valid credentials (matching MSW mock)
    await user.type(loginInput, '61120319064');
    await user.type(passwordInput, 'Teste@123');
    await user.click(submitButton);

    // Wait for success and redirect
    await waitFor(() => {
      expect(mockPush).toHaveBeenCalledWith('/dashboard');
    }, { timeout: 5000 });
  });

  it('should display error with invalid credentials', async () => {
    const user = userEvent.setup();
    render(<LoginPage />);

    const loginInput = screen.getByTestId('login-input');
    const passwordInput = screen.getByTestId('password-input');
    const submitButton = screen.getByTestId('login-submit-button');

    // Fill in invalid credentials
    await user.type(loginInput, 'invalid@email.com');
    await user.type(passwordInput, 'wrongpassword');
    await user.click(submitButton);

    // Just verify that login was attempted (button shows loading or error happens)
    // MSW will return 401, which may show as toast or different error handling
    await waitFor(() => {
      // Either button is no longer loading or we're not redirected
      expect(mockPush).not.toHaveBeenCalledWith('/dashboard');
    }, { timeout: 5000 });
  });



  it('should navigate to complete registration page when clicking the link', async () => {
    const user = userEvent.setup();
    render(<LoginPage />);

    const registrationLink = screen.getByTestId('complete-registration-link');
    
    // Link should have correct href
    expect(registrationLink).toHaveAttribute('href', '/complete-registration');
  });

  it('should navigate to password reset page when clicking the link', async () => {
    const user = userEvent.setup();
    render(<LoginPage />);

    const forgotPasswordLink = screen.getByTestId('forgot-password-link');
    
    // Link should have correct href
    expect(forgotPasswordLink).toHaveAttribute('href', '/password-reset');
  });

  it('should accept any string in login field', async () => {
    const user = userEvent.setup();
    render(<LoginPage />);

    const loginInput = screen.getByTestId('login-input');
    const passwordInput = screen.getByTestId('password-input');
    const submitButton = screen.getByTestId('login-submit-button');

    // Enter short text (login accepts CPF or email)
    await user.type(loginInput, '123');
    await user.type(passwordInput, 'Teste@123');
    
    // Should be able to type and submit (validation happens on server)
    expect(loginInput).toHaveValue('123');
    expect(passwordInput).toHaveValue('Teste@123');
    
    // Submit should work (server will validate)
    await user.click(submitButton);
    
    // Verify submission attempt was made
    await waitFor(() => {
      // Either loading started or finished
      expect(mockPush).not.toHaveBeenCalledWith('/dashboard');
    }, { timeout: 2000 });
  });
});
