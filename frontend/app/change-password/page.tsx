'use client';

import { useChangePasswordForm } from '@/hooks/forms/useChangePasswordForm';
import { useChangePassword } from '@/hooks/api/useAuth';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';

export default function ChangePasswordPage() {
  const form = useChangePasswordForm();
  const { mutate: changePassword, isPending } = useChangePassword();

  const onSubmit = form.handleSubmit((data) => {
    changePassword(data);
  });

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-zinc-950 via-zinc-900 to-zinc-950 p-4">
      <Card className="w-full max-w-md border-zinc-800 bg-zinc-900/50 backdrop-blur">
        <CardHeader className="space-y-2">
          <CardTitle className="text-2xl font-bold text-white">
            Alterar Senha
          </CardTitle>
          <CardDescription className="text-zinc-400">
            Por favor, altere sua senha temporária por uma senha permanente
          </CardDescription>
        </CardHeader>
        <CardContent>
          <Form {...form}>
            <form onSubmit={onSubmit} className="space-y-4">
              <FormField
                control={form.control}
                name="currentPassword"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="text-white">Senha Atual (Temporária)</FormLabel>
                    <FormControl>
                      <Input
                        {...field}
                        type="password"
                        placeholder="Digite a senha temporária"
                        disabled={isPending}
                        data-testid="current-password-input"
                        className="bg-zinc-800 border-zinc-700 text-white placeholder:text-zinc-500"
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="newPassword"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="text-white">Nova Senha</FormLabel>
                    <FormControl>
                      <Input
                        {...field}
                        type="password"
                        placeholder="Digite a nova senha"
                        disabled={isPending}
                        data-testid="new-password-input"
                        className="bg-zinc-800 border-zinc-700 text-white placeholder:text-zinc-500"
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="confirmNewPassword"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="text-white">Confirmar Nova Senha</FormLabel>
                    <FormControl>
                      <Input
                        {...field}
                        type="password"
                        placeholder="Confirme a nova senha"
                        disabled={isPending}
                        data-testid="confirm-password-input"
                        className="bg-zinc-800 border-zinc-700 text-white placeholder:text-zinc-500"
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <Button
                type="submit"
                data-testid="change-password-submit"
                className="w-full bg-gradient-to-r from-blue-600 to-blue-500 hover:from-blue-700 hover:to-blue-600"
                disabled={isPending}
              >
                {isPending ? 'Alterando...' : 'Alterar Senha'}
              </Button>
            </form>
          </Form>
        </CardContent>
      </Card>
    </div>
  );
}
