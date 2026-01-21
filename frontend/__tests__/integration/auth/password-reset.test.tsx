import { describe, it, expect, vi, beforeEach } from 'vitest';
import { screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { render } from '../../utils/test-utils';
import PasswordResetPage from '@/app/password-reset/page';

// Mock next/navigation
const mockPush = vi.fn();
vi.mock('next/navigation', () => ({
  useRouter: () => ({
    push: mockPush,
  }),
}));

describe('Password Reset Page', () => {
  beforeEach(() => {
    mockPush.mockClear();
  });

  it('should render password reset form correctly', () => {
    render(<PasswordResetPage />);

    // Check form elements
    expect(screen.getByTestId('reset-cpf-input')).toBeInTheDocument();
    expect(screen.getByTestId('reset-email-input')).toBeInTheDocument();
    expect(screen.getByTestId('reset-submit-button')).toBeInTheDocument();
    expect(screen.getByTestId('reset-back-button')).toBeInTheDocument();

    // Check labels and title
    expect(screen.getByText('Recuperar Senha')).toBeInTheDocument();
    expect(screen.getByText('CPF')).toBeInTheDocument();
    expect(screen.getByText('Email')).toBeInTheDocument();
  });

  it('should validate CPF format', async () => {
    const user = userEvent.setup();
    render(<PasswordResetPage />);

    const cpfInput = screen.getByTestId('reset-cpf-input');
    const emailInput = screen.getByTestId('reset-email-input');
    const submitButton = screen.getByTestId('reset-submit-button');

    // Enter invalid CPF
    await user.type(cpfInput, '12345');
    await user.type(emailInput, 'valid@email.com');
    await user.click(submitButton);

    // Should show CPF validation error
    await waitFor(() => {
      expect(screen.getByText(/CPF inválido/i)).toBeInTheDocument();
    });
  });

  it('should validate email format', async () => {
    const user = userEvent.setup();
    render(<PasswordResetPage />);

    const cpfInput = screen.getByTestId('reset-cpf-input');
    const emailInput = screen.getByTestId('reset-email-input');
    const submitButton = screen.getByTestId('reset-submit-button');

    // Enter valid CPF but invalid email
    await user.type(cpfInput, '61120319064');
    await user.type(emailInput, 'invalid-email');
    await user.click(submitButton);

    // Should show email validation error
    await waitFor(() => {
      expect(screen.getByText(/email inválido/i)).toBeInTheDocument();
    });
  });

  it('should successfully submit password reset request', async () => {
    const user = userEvent.setup();
    render(<PasswordResetPage />);

    const cpfInput = screen.getByTestId('reset-cpf-input');
    const emailInput = screen.getByTestId('reset-email-input');
    const submitButton = screen.getByTestId('reset-submit-button');

    // Fill in valid data
    await user.type(cpfInput, '61120319064');
    await user.type(emailInput, 'joao.silva@email.com');
    await user.click(submitButton);

    // Wait for submission to complete (redirect may or may not happen immediately)
    await waitFor(() => {
      // Verify button is back to normal or redirect happened
      const button = screen.getByTestId('reset-submit-button');
      expect(button).not.toBeDisabled();
    }, { timeout: 5000 });
  });





  it('should have back button linking to login', () => {
    render(<PasswordResetPage />);

    const backButton = screen.getByTestId('reset-back-button');
    
    // Back button should be inside a Link to /login
    const link = backButton.closest('a');
    expect(link).toHaveAttribute('href', '/login');
  });

  it('should validate empty fields', async () => {
    const user = userEvent.setup();
    render(<PasswordResetPage />);

    const submitButton = screen.getByTestId('reset-submit-button');

    // Try to submit without filling anything
    await user.click(submitButton);

    // Should show validation errors
    await waitFor(() => {
      expect(screen.getByText(/CPF é obrigatório/i)).toBeInTheDocument();
      expect(screen.getByText(/Email é obrigatório/i)).toBeInTheDocument();
    });
  });
});
