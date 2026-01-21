'use client';

import { toast as toastify, type ToastOptions, type Id } from 'react-toastify';

/**
 * Default toast options
 */
const defaultOptions: ToastOptions = {
  position: 'top-right',
  autoClose: 3000,
  hideProgressBar: false,
  closeOnClick: true,
  pauseOnHover: true,
  draggable: true,
  theme: 'light',
};

/**
 * Custom hook for toast notifications
 * Provides consistent toast styling and behavior across the app
 */
export function useToast() {
  /**
   * Show success toast
   */
  const success = (message: string, options?: ToastOptions): Id => {
    return toastify.success(message, { ...defaultOptions, ...options });
  };

  /**
   * Show error toast
   */
  const error = (message: string, options?: ToastOptions): Id => {
    return toastify.error(message, { ...defaultOptions, ...options });
  };

  /**
   * Show info toast
   */
  const info = (message: string, options?: ToastOptions): Id => {
    return toastify.info(message, { ...defaultOptions, ...options });
  };

  /**
   * Show warning toast
   */
  const warning = (message: string, options?: ToastOptions): Id => {
    return toastify.warning(message, { ...defaultOptions, ...options });
  };

  /**
   * Show loading toast
   */
  const loading = (message: string, options?: ToastOptions): Id => {
    return toastify.loading(message, { ...defaultOptions, ...options });
  };

  /**
   * Update existing toast
   */
  const update = (
    toastId: Id,
    options: ToastOptions & { render?: string }
  ): void => {
    toastify.update(toastId, options);
  };

  /**
   * Dismiss toast by ID or all toasts
   */
  const dismiss = (toastId?: Id): void => {
    if (toastId) {
      toastify.dismiss(toastId);
    } else {
      toastify.dismiss();
    }
  };

  /**
   * Toast for promises with loading/success/error states
   */
  const promise = (
    promiseToResolve: Promise<any>,
    messages: {
      pending: string;
      success: string;
      error: string;
    }
  ) => {
    return toastify.promise(
      promiseToResolve,
      messages,
      defaultOptions
    );
  };

  return {
    success,
    error,
    info,
    warning,
    loading,
    update,
    dismiss,
    promise,
  };
}
