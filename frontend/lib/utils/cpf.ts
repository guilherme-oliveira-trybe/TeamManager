/**
 * CPF Validation and Formatting Utilities
 * Aligned with backend validation (IsValidCpf extension)
 */

/**
 * Validates a Brazilian CPF number using checksum algorithm
 * @param cpf - CPF string (can be formatted or unformatted)
 * @returns true if CPF is valid, false otherwise
 */
export function isValidCPF(cpf: string): boolean {
  if (!cpf) return false;

  // Remove all non-digit characters
  const cleanCpf = cpf.replace(/\D/g, '');
  
  // CPF must have exactly 11 digits
  if (cleanCpf.length !== 11) return false;
  
  // Reject known invalid CPFs (all digits the same)
  if (/^(\d)\1{10}$/.test(cleanCpf)) return false;

  // Validate first check digit
  let sum = 0;
  for (let i = 0; i < 9; i++) {
    sum += parseInt(cleanCpf.charAt(i)) * (10 - i);
  }
  let digit = 11 - (sum % 11);
  if (digit > 9) digit = 0;
  if (digit !== parseInt(cleanCpf.charAt(9))) return false;

  // Validate second check digit
  sum = 0;
  for (let i = 0; i < 10; i++) {
    sum += parseInt(cleanCpf.charAt(i)) * (11 - i);
  }
  digit = 11 - (sum % 11);
  if (digit > 9) digit = 0;
  if (digit !== parseInt(cleanCpf.charAt(10))) return false;

  return true;
}

/**
 * Formats a CPF number to XXX.XXX.XXX-XX pattern
 * @param cpf - Unformatted CPF string
 * @returns Formatted CPF or original string if invalid
 */
export function formatCPF(cpf: string): string {
  const cleaned = cpf.replace(/\D/g, '');
  
  if (cleaned.length !== 11) return cpf;
  
  return cleaned.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
}

/**
 * Removes all formatting from CPF, leaving only digits
 * @param cpf - Formatted or unformatted CPF
 * @returns CPF with only digits
 */
export function cleanCPF(cpf: string): string {
  return cpf.replace(/\D/g, '');
}

/**
 * Validates and formats CPF in one step
 * @param cpf - CPF string
 * @returns Formatted CPF if valid, empty string if invalid
 */
export function validateAndFormatCPF(cpf: string): string {
  if (!isValidCPF(cpf)) return '';
  return formatCPF(cpf);
}
