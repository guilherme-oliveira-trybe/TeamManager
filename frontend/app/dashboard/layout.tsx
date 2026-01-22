import { redirect } from 'next/navigation';
import { Sidebar } from '@/components/dashboard/Sidebar';
import { getCurrentUserFromHeaders } from '@/lib/auth/server';

export default async function DashboardLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  // Server Component - dados disponíveis instantaneamente via proxy
  const user = await getCurrentUserFromHeaders();
  
  if (!user) {
    redirect('/login');
  }
  
  return (
    <div className="min-h-screen flex bg-zinc-950">
      <Sidebar user={user} />
      <main className="flex-1 overflow-y-auto">
        {children}
      </main>
    </div>
  );
}
