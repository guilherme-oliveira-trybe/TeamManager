'use client';

import { useState } from 'react';
import { useCompleteRegistrationForm } from '@/hooks/forms/useCompleteRegistrationForm';
import { useCompleteRegistration } from '@/hooks/api/useUser';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import Link from 'next/link';
import { ArrowLeft, ArrowRight } from 'lucide-react';

export default function CompleteRegistrationPage() {
  const [step, setStep] = useState(1);
  const form = useCompleteRegistrationForm();
  const { mutate: completeRegistration, isPending } = useCompleteRegistration();

  const onSubmit = form.handleSubmit((data) => {
    completeRegistration(data);
  });

  const nextStep = async () => {
    const fields = getStepFields(step);
    const isValid = await form.trigger(fields);
    if (isValid) setStep(step + 1);
  };

  const prevStep = () => setStep(step - 1);

  const getStepFields = (currentStep: number) => {
    switch (currentStep) {
      case 1:
        return ['cpf', 'activationCode'] as const;
      case 2:
        return ['fullName', 'birthDate', 'weight', 'height', 'phone', 'email'] as const;
      case 3:
        return ['password', 'confirmPassword', 'emergencyContactName', 'emergencyContactPhone'] as const;
      default:
        return [];
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-zinc-950 via-zinc-900 to-zinc-950 p-4">
      <Card className="w-full max-w-2xl border-zinc-800 bg-zinc-900/50 backdrop-blur">
        <CardHeader>
          <div className="flex items-center justify-between mb-4">
            <Link href="/login">
              <Button
                variant="ghost"
                size="icon"
                className="text-zinc-400 hover:text-white hover:bg-zinc-800"
              >
                <ArrowLeft className="h-5 w-5" />
              </Button>
            </Link>
            <div className="flex gap-2">
              {[1, 2, 3].map((s) => (
                <div
                  key={s}
                  data-testid="step-indicator"
                  className={`h-2 w-12 rounded-full ${
                    s === step ? 'bg-[#FFD700]' : s < step ? 'bg-[#FFD700]/50' : 'bg-zinc-700'
                  }`}
                />
              ))}
            </div>
          </div>
          <CardTitle className="text-2xl font-bold text-white">
            Complete seu Cadastro
          </CardTitle>
          <CardDescription className="text-zinc-400" data-testid="step-title">
            {step === 1 && 'Passo 1: Validação'}
            {step === 2 && 'Passo 2: Dados Pessoais'}
            {step === 3 && 'Passo 3: Senha e Contato de Emergência'}
          </CardDescription>
        </CardHeader>
        <CardContent>
          <Form {...form}>
            <form onSubmit={onSubmit} className="space-y-4">
              {/* Step 1: Validation */}
              {step === 1 && (
                <div className="space-y-4">
                  <FormField
                    control={form.control}
                    name="cpf"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel className="text-white">CPF</FormLabel>
                        <FormControl>
                          <Input
                            {...field}
                            data-testid="registration-cpf-input"
                            placeholder="000.000.000-00"
                            className="bg-zinc-800 border-zinc-700 text-white placeholder:text-zinc-500"
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="activationCode"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel className="text-white">Código de Ativação</FormLabel>
                        <FormControl>
                          <Input
                            {...field}
                            data-testid="registration-code-input"
                            placeholder="XXXXXXXX"
                            maxLength={8}
                            className="bg-zinc-800 border-zinc-700 text-white placeholder:text-zinc-500 uppercase"
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>
              )}

              {/* Step 2: Personal Data */}
              {step === 2 && (
                <div className="space-y-4">
                  <FormField
                    control={form.control}
                    name="fullName"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel className="text-white">Nome Completo</FormLabel>
                        <FormControl>
                          <Input
                            {...field}
                            data-testid="registration-name-input"
                            placeholder="Seu nome completo"
                            className="bg-zinc-800 border-zinc-700 text-white placeholder:text-zinc-500"
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                    <FormField
                      control={form.control}
                      name="birthDate"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel className="text-white">Data de Nascimento</FormLabel>
                          <FormControl>
                            <Input
                              type="date"
                              data-testid="registration-birthdate-input"
                              value={field.value ? new Date(field.value).toISOString().split('T')[0] : ''}
                              onChange={(e) => field.onChange(e.target.value ? new Date(e.target.value) : undefined)}
                              className="bg-zinc-800 border-zinc-700 text-white"
                            />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                    <FormField
                      control={form.control}
                      name="weight"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel className="text-white">Peso (kg)</FormLabel>
                          <FormControl>
                            <Input
                              type="number"
                              {...field}
                              data-testid="registration-weight-input"
                              onChange={(e) => field.onChange(e.target.valueAsNumber)}
                              className="bg-zinc-800 border-zinc-700 text-white"
                            />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                    <FormField
                      control={form.control}
                      name="height"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel className="text-white">Altura (cm)</FormLabel>
                          <FormControl>
                            <Input
                              type="number"
                              {...field}
                              data-testid="registration-height-input"
                              onChange={(e) => field.onChange(e.target.valueAsNumber)}
                              className="bg-zinc-800 border-zinc-700 text-white"
                            />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                  </div>
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <FormField
                      control={form.control}
                      name="phone"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel className="text-white">Telefone</FormLabel>
                          <FormControl>
                            <Input
                              {...field}
                              data-testid="registration-phone-input"
                              placeholder="(11) 98765-4321"
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
                              data-testid="registration-email-input"
                              type="email"
                              placeholder="seu@email.com"
                              className="bg-zinc-800 border-zinc-700 text-white placeholder:text-zinc-500"
                            />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                  </div>
                </div>
              )}

              {/* Step 3: Password & Emergency Contact */}
              {step === 3 && (
                <div className="space-y-4">
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <FormField
                      control={form.control}
                      name="password"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel className="text-white">Senha</FormLabel>
                          <FormControl>
                            <Input
                              {...field}
                              data-testid="registration-password-input"
                              type="password"
                              placeholder="Mínimo 8 caracteres"
                              className="bg-zinc-800 border-zinc-700 text-white placeholder:text-zinc-500"
                            />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                    <FormField
                      control={form.control}
                      name="confirmPassword"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel className="text-white">Confirmar Senha</FormLabel>
                          <FormControl>
                            <Input
                              {...field}
                              data-testid="registration-confirm-password-input"
                              type="password"
                              placeholder="Repita a senha"
                              className="bg-zinc-800 border-zinc-700 text-white placeholder:text-zinc-500"
                            />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                  </div>
                  <div className="border-t border-zinc-700 pt-4 mt-4">
                    <h3 className="text-white font-semibold mb-4">Contato de Emergência</h3>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      <FormField
                        control={form.control}
                        name="emergencyContactName"
                        render={({ field }) => (
                          <FormItem>
                            <FormLabel className="text-white">Nome</FormLabel>
                            <FormControl>
                              <Input
                                {...field}
                                data-testid="emergency-name-input"
                                placeholder="Nome do contato"
                                className="bg-zinc-800 border-zinc-700 text-white placeholder:text-zinc-500"
                              />
                            </FormControl>
                            <FormMessage />
                          </FormItem>
                        )}
                      />
                      <FormField
                        control={form.control}
                        name="emergencyContactPhone"
                        render={({ field }) => (
                          <FormItem>
                            <FormLabel className="text-white">Telefone</FormLabel>
                            <FormControl>
                              <Input
                                {...field}
                                data-testid="emergency-phone-input"
                                placeholder="(11) 98765-4321"
                                className="bg-zinc-800 border-zinc-700 text-white placeholder:text-zinc-500"
                              />
                            </FormControl>
                            <FormMessage />
                          </FormItem>
                        )}
                      />
                    </div>
                  </div>
                </div>
              )}

              {/* Navigation Buttons */}
              <div className="flex gap-2 pt-4">
                {step > 1 && (
                  <Button
                    type="button"
                    variant="outline"
                    onClick={prevStep}
                    data-testid={`step${step}-back-button`}
                    className="flex-1 border-zinc-700 text-white hover:bg-zinc-800"
                    disabled={isPending}
                  >
                    <ArrowLeft className="mr-2 h-4 w-4" />
                    Voltar
                  </Button>
                )}
                {step < 3 && (
                  <Button
                    type="button"
                    onClick={nextStep}
                    data-testid={`step${step}-next-button`}
                    className="flex-1 bg-[#FFD700] hover:bg-[#FFC700] text-black font-semibold"
                  >
                    Próximo
                    <ArrowRight className="ml-2 h-4 w-4" />
                  </Button>
                )}
                {step === 3 && (
                  <Button
                    type="submit"
                    data-testid="registration-submit-button"
                    className="flex-1 bg-[#FFD700] hover:bg-[#FFC700] text-black font-semibold"
                    disabled={isPending}
                  >
                    {isPending ? 'Finalizando...' : 'Finalizar Cadastro'}
                  </Button>
                )}
              </div>
            </form>
          </Form>
        </CardContent>
      </Card>
    </div>
  );
}
