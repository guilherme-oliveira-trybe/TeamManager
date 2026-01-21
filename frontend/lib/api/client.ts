/**
 * Axios API Client Configuration
 * Handles authentication, error handling, and base configuration
 */

import axios, { type AxiosError } from 'axios';

const API_URL = process.env.NEXT_PUBLIC_BACKEND_URL || 'http://localhost:5000';

/**
 * Base Axios client instance
 */
export const apiClient = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true, // Important for httpOnly cookies
});

/**
 * Request interceptor
 * Add any request interceptors here (e.g., auth token from cookies)
 */
apiClient.interceptors.request.use(
  (config) => {
    // Cookies are automatically sent with withCredentials: true
    // No need to manually add auth header
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

/**
 * Response interceptor
 * Handle global errors and response formatting
 */
apiClient.interceptors.response.use(
  (response) => {
    return response;
  },
  (error: AxiosError) => {
    // Handle 401 Unauthorized (token expired or invalid)
    if (error.response?.status === 401) {
      // Redirect to login
      if (typeof window !== 'undefined') {
        window.location.href = '/login';
      }
    }

    // Handle 403 Forbidden (insufficient permissions)
    if (error.response?.status === 403) {
      console.error('Access denied');
    }

    // Handle network errors
    if (!error.response) {
      console.error('Network error - servidor offline ou sem conexão');
    }

    return Promise.reject(error);
  }
);

/**
 * Helper to extract error messages from API responses
 */
export function getApiErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    const responseData = error.response?.data as any;
    
    // Backend returns errors as: { isSuccess: false, errors: string[] }
    if (responseData?.errors && Array.isArray(responseData.errors)) {
      return responseData.errors[0] || 'Erro desconhecido';
    }
    
    // Fallback messages
    if (error.response?.status === 401) {
      return 'Não autorizado. Faça login novamente.';
    }
    
    if (error.response?.status === 403) {
      return 'Você não tem permissão para realizar esta ação.';
    }
    
    if (error.response?.status === 404) {
      return 'Recurso não encontrado.';
    }
    
    if (error.response?.status === 500) {
      return 'Erro interno do servidor. Tente novamente mais tarde.';
    }
    
    return error.message || 'Erro ao conectar com servidor';
  }
  
  return 'Erro desconhecido';
}
