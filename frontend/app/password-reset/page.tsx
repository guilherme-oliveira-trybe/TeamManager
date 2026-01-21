'use client';

import { usePasswordResetForm } from '@/hooks/forms/usePasswordResetForm';
import { useRequestPasswordReset } from '@/hooks/api/useUser';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import Link from 'next/link';
import { ArrowLeft } from 'lucide-react';

export default function PasswordResetPage() {
  const form = usePasswordResetForm();
  const { mutate: requestReset, isPending } = useRequestPasswordReset();

  const onSubmit = form.handleSubmit((data) => {
    requestReset(data);
  });

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-zinc-950 via-zinc-900 to-zinc-950 p-4">
      <Card className="w-full max-w-md border-zinc-800 bg-zinc-900/50 backdrop-blur">
        <CardHeader>
          <div className="flex items-center gap-2 mb-4">
            <Link href="/login">
              <Button
                variant="ghost"
                size="icon"
                data-testid="reset-back-button"
                className="text-zinc-400 hover:text-white hover:bg-zinc-800"
              >
                <ArrowLeft className="h-5 w-5" />
              </Button>
            </Link>
          </div>
          <CardTitle className="text-2xl font-bold text-white">
            Recuperar Senha
          </CardTitle>
          <CardDescription className="text-zinc-400">
            Solicite uma nova senha. Um administrador irá aprovar sua solicitação.
          </CardDescription>
        </CardHeader>
        <CardContent>
          <Form {...form}>
            <form onSubmit={onSubmit} className="space-y-4">
              <FormField
                control={form.control}
                name="cpf"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="text-white">CPF</FormLabel>
                    <FormControl>
                      <Input
                        {...field}
                        data-testid="reset-cpf-input"
                        placeholder="000.000.000-00"
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
                name="email"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="text-white">Email</FormLabel>
                    <FormControl>
                      <Input
                        {...field}
                        data-testid="reset-email-input"
                        type="email"
                        placeholder="seu@email.com"
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
                data-testid="reset-submit-button"
                className="w-full bg-[#FFD700] hover:bg-[#FFC700] text-black font-semibold"
                disabled={isPending}
              >
                {isPending ? 'Enviando...' : 'Solicitar Nova Senha'}
              </Button>
            </form>
          </Form>
        </CardContent>
      </Card>
    </div>
  );
}
