import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';

/**
 * Proxy (formerly Middleware) for route protection
 * Next.js 16+ uses proxy.ts instead of middleware.ts
 * Checks for authentication cookie and redirects if necessary
 */
export function proxy(request: NextRequest) {
  const token = request.cookies.get('auth_token');
  const { pathname } = request.nextUrl;

  // Protected routes - require authentication
  const protectedRoutes = [
    '/dashboard',
    '/admin',
    '/settings',
    '/departments',
    '/sectors',
    '/staff',
    '/users',
  ];
  
  const isProtected = protectedRoutes.some(route => pathname.startsWith(route));

  // If trying to access protected route without auth, redirect to login
  if (isProtected && !token) {
    const loginUrl = new URL('/login', request.url);
    loginUrl.searchParams.set('redirect', pathname);
    return NextResponse.redirect(loginUrl);
  }

  // Auth routes - redirect to dashboard if already logged in
  const authRoutes = [
    '/login',
    '/complete-registration',
    '/password-reset',
  ];
  
  const isAuthRoute = authRoutes.some(route => pathname.startsWith(route));

  // If already authenticated and trying to access auth pages, redirect to dashboard
  if (isAuthRoute && token) {
    return NextResponse.redirect(new URL('/dashboard', request.url));
  }

  return NextResponse.next();
}

/**
 * Matcher configuration
 * Specify which routes this middleware should run on
 */
export const config = {
  matcher: [
    /*
     * Match all request paths except:
     * - api routes
     * - _next/static (static files)
     * - _next/image (image optimization)
     * - favicon.ico, manifest.json, etc.
     */
    '/((?!api|_next/static|_next/image|favicon.ico|manifest.json|icons|sw.js|workbox-).*)',
  ],
};
