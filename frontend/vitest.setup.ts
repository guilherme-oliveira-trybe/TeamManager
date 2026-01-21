import '@testing-library/jest-dom';
import { cleanup } from '@testing-library/react';
import { afterEach, beforeAll, afterAll } from 'vitest';
import { server } from './__tests__/mocks/server';

// Setup MSW server before all tests
beforeAll(() => {
  server.listen({
    onUnhandledRequest: 'warn', // Warn about unhandled requests instead of erroring
  });
});

// Clean up after each test
afterEach(() => {
  cleanup(); // Clean up React components
  server.resetHandlers(); // Reset MSW handlers to default
});

// Close MSW server after all tests
afterAll(() => {
  server.close();
});
