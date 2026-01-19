'use client';

import { useLoginForm } from '@/hooks/forms/useLoginForm';
import { useLogin } from '@/hooks/api/useAuth';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import Link from 'next/link';
import Image from 'next/image';

export default function LoginPage() {
  const form = useLoginForm();
  const { mutate: login, isPending } = useLogin();

  const onSubmit = form.handleSubmit((data) => {
    login(data);
  });

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-zinc-950 via-zinc-900 to-zinc-950 p-4">
      <Card className="w-full max-w-md border-zinc-800 bg-zinc-900/50 backdrop-blur">
        <CardHeader className="space-y-4">
          <div className="flex justify-center">
            <div className="relative w-24 h-24">
              <Image
                src="/logo-galo.jpg"
                alt="Galo Futebol Americano"
                fill
                className="object-contain"
                priority
              />
            </div>
          </div>
          <div className="text-center">
            <CardTitle className="text-2xl font-bold text-white">
              Portal Galo FA
            </CardTitle>
            <CardDescription className="text-zinc-400">
              Entre com suas credenciais
            </CardDescription>
          </div>
        </CardHeader>
        <CardContent>
          <Form {...form}>
            <form onSubmit={onSubmit} className="space-y-4">
              <FormField
                control={form.control}
                name="login"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="text-white">CPF ou Email</FormLabel>
                    <FormControl>
                      <Input
                        {...field}
                        placeholder="Digite seu CPF ou email"
                        disabled={isPending}
                        className="bg-zinc-800 border-zinc-700 text-white placeholder:text-zinc-500"
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="password"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="text-white">Senha</FormLabel>
                    <FormControl>
                      <Input
                        {...field}
                        type="password"
                        placeholder="Digite sua senha"
                        disabled={isPending}
                        className="bg-zinc-800 border-zinc-700 text-white placeholder:text-zinc-500"
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <Button
                type="submit"
                className="w-full bg-[#FFD700] hover:bg-[#FFC700] text-black font-semibold"
                disabled={isPending}
              >
                {isPending ? 'Entrando...' : 'Entrar'}
              </Button>
            </form>
          </Form>

          <div className="mt-6 space-y-2 text-center text-sm">
            <Link
              href="/password-reset"
              className="block text-zinc-400 hover:text-[#FFD700] transition-colors"
            >
              Esqueceu sua senha?
            </Link>
            <Link
              href="/complete-registration"
              className="block text-zinc-400 hover:text-[#FFD700] transition-colors"
            >
              Primeiro acesso? Complete seu cadastro
            </Link>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
