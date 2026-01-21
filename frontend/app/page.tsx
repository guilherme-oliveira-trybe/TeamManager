import { redirect } from 'next/navigation';

/**
 * Root page - redirects to login
 * This is the entry point of the application
 */
export default function Home() {
  redirect('/login');
}
