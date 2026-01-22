import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';
import { jwtDecode } from 'jwt-decode';

interface JwtPayload {
  nameid: string;      // ClaimTypes.NameIdentifier
  unique_name: string; // ClaimTypes.Name
  email: string;       // ClaimTypes.Email
  cpf: string;         // custom claim
  role: string;        // ClaimTypes.Role (Admin, Athlete, Coach, Staff)
  unit?: string;       // custom claim (opcional)
  position?: string;   // custom claim (opcional)
}

/**
 * Proxy (formerly Middleware) for route protection
 * Next.js 16+ uses proxy.ts instead of middleware.ts
 * Checks for authentication cookie and redirects if necessary
 * Extracts user data from JWT for Server Components
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

  // Decode JWT and add user data to headers for Server Components
  const response = NextResponse.next();
  
  if (token?.value) {
    try {
      const decoded = jwtDecode<JwtPayload>(token.value);
      
      // Add user data to headers
      response.headers.set('x-user-id', decoded.nameid);
      response.headers.set('x-user-name', decoded.unique_name || decoded.email);
      response.headers.set('x-user-email', decoded.email);
      response.headers.set('x-user-role', decoded.role);
      response.headers.set('x-user-cpf', decoded.cpf);
      
      if (decoded.unit) {
        response.headers.set('x-user-unit', decoded.unit);
      }
      
      if (decoded.position) {
        response.headers.set('x-user-position', decoded.position);
      }
    } catch (error) {
      // If JWT decode fails, continue without headers
      console.error('Error decoding JWT in proxy:', error);
    }
  }

  return response;
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
