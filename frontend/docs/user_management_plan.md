# 📋 Plano de Implementação: Gestão de Usuários + Dashboard

**Data:** 21/01/2026  
**Objetivo:** Design e planejamento da interface de gestão administrativa

---

## 🎯 Contexto do Backend (Análise)

### **Endpoints Identificados**

#### **1. Gestão de Usuários (`/api/users`)**

**Listar Usuários:**
- `GET /api/users/status/{status}` 🔐 AdminOnly
  - Status: 1=PendingRegistration, 2=AwaitingActivation, 3=Active, 4=Rejected, 5=Inactive
  - Retorna lista de usuários filtrados por status

**Ativar/Desativar:**
- `POST /api/users/{id}/activate` 🔐 AdminOnly
  - Ativa um usuário (status → Active)
  - Envia adminId automaticamente via ClaimsPrincipal
  
- `POST /api/users/{id}/deactivate` 🔐 AdminOnly
  - Desativa um usuário (status → Inactive)

**Consultar Usuário:**
- `GET /api/users/{id}` 🔐 Authenticated (com validação de acesso)
- `GET /api/users/cpf/{cpf}` 🔐 AdminOnly

**Atualizar:**
- `PUT /api/users/{id}` 🔐 Authenticated (com validação de acesso)
- `PATCH /api/users/{id}/position` 🔐 AdminOnly

**Deletar:**
- `DELETE /api/users/{id}` 🔐 AdminOnly

---

#### **2. Pré-Cadastro (`/api/pre-registrations`)**

**Criar:**
- `POST /api/pre-registrations` 🔐 AdminOnly
  - Cria um pré-cadastro e gera código de ativação
  - Retorna: id, cpf, profile, unit, position, activationCode

**Consultar:**
- `GET /api/pre-registrations/{id}` 🔐 AdminOnly
- `GET /api/pre-registrations/cpf/{cpf}` 🔐 AdminOnly

**Regenerar Código:**
- `POST /api/pre-registrations/{id}/regenerate` 🔐 AdminOnly
  - Gera novo código de ativação se o anterior expirou/perdeu

---

#### **3. Reset de Senha (`/api/auth`)**

**Listar Pendentes:**
- `GET /api/auth/password-reset-requests/pending` 🔐 AdminOnly
  - Retorna solicitações PENDENTES de reset de senha

**Aprovar:**
- `POST /api/auth/password-reset-requests/{requestId}/approve` 🔐 AdminOnly
  - Aprova solicitação e gera senha temporária
  - Retorna: userFullName, approvedByName, temporaryPassword, expirationDate

---

## 🎨 Proposta de Design - Dashboard

### **Estrutura de Navegação**

```
┌─────────────────────────────────────────────────┐
│  🏠 GFA Team Manager                [ Profile ] │
├─────────────────────────────────────────────────┤
│                                                 │
│  SIDEBAR              │  MAIN CONTENT          │
│  ┌──────────┐         │                        │
│  │ 📊 Visão Geral     │  [Dynamic Content]     │
│  │                    │                        │
│  │ 👥 Gestão          │                        │
│  │  ├ Usuários        │                        │
│  │  ├ Pré-Cadastro    │                        │
│  │  └ Reset Senha     │                        │
│  │                    │                        │
│  │ ⚽ Time             │                        │
│  │  ├ Jogadores       │                        │
│  │  ├ Staff           │                        │
│  │  └ Departamentos   │                        │
│  │                    │                        │
│  │ 🏃 Atividades      │                        │
│  │                    │                        │
│  │ ⚙️ Configurações   │                        │
│  └──────────────────┘ │                        │
└─────────────────────────────────────────────────┘
```

---

### **1. Página: Visão Geral (Dashboard Home)**

**URL:** `/dashboard`

**Componentes:**

```typescript
// Cards de Estatísticas
┌─────────────────────────────────────────────────┐
│  📊 ESTATÍSTICAS RÁPIDAS                        │
├─────────────────────────────────────────────────┤
│                                                 │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐      │
│  │ 👥 125   │  │ ⏳ 8     │  │ 🔔 3     │      │
│  │ Usuários │  │ Aguard.  │  │ Pendentes│      │
│  │ Ativos   │  │ Ativação │  │ Resetar  │      │
│  └──────────┘  └──────────┘  └──────────┘      │
│                                                 │
├─────────────────────────────────────────────────┤
│  📋 AÇÕES RÁPIDAS                               │
│  • Novo pré-cadastro                            │
│  • Aprovar solicitações                         │
│  • Ver usuários pendentes                       │
└─────────────────────────────────────────────────┘

// Tabela de Atividades Recentes
┌─────────────────────────────────────────────────┐
│  🔔 ATIVIDADES RECENTES                         │
├─────────────────────────────────────────────────┤
│  • João Silva completou cadastro - 2h atrás    │
│  • Maria Santos solicitou reset - 4h atrás     │
│  • Pedro Costa ativado por Admin - 1d atrás    │
└─────────────────────────────────────────────────┘
```

**Features:**
- Cards com contadores dinâmicos
- Gráfico de usuários por status (opcional)
- Lista de ações recentes
- Atalhos para páginas mais usadas

---

### **2. Página: Gestão de Usuários**

**URL:** `/dashboard/users`

**Layout:**

```typescript
┌─────────────────────────────────────────────────┐
│  👥 GESTÃO DE USUÁRIOS                          │
├─────────────────────────────────────────────────┤
│                                                 │
│  [Filtros]                                      │
│  Status: [Todos ▼] Busca: [___________] 🔍     │
│                                                 │
│  [Tabs]                                         │
│  ┌──────────┬──────────┬──────────┬──────────┐ │
│  │ Todos    │Aguardando│ Ativos   │ Inativos │ │
│  │  (125)   │  (8)     │  (112)   │  (5)     │ │
│  └──────────┴──────────┴──────────┴──────────┘ │
│                                                 │
│  [Tabela]                                       │
│  ┌──────┬────────────┬─────────┬──────┬─────┐  │
│  │ Nome │ CPF        │ Status  │ Perfil│Ações│  │
│  ├──────┼────────────┼─────────┼──────┼─────┤  │
│  │ João │***.*64     │🟡Aguard.│Player │ ⋮   │  │
│  │ Maria│***.*23     │🟢Ativo  │Admin  │ ⋮   │  │
│  │ Pedro│***.*87     │🔴Inativo│Staff  │ ⋮   │  │
│  └──────┴────────────┴─────────┴──────┴─────┘  │
│                                                 │
│  [Paginação]  1 2 3 ... 10                     │
└─────────────────────────────────────────────────┘
```

**Ações por Usuário:**

```typescript
// Menu dropdown (⋮)
┌─────────────────────┐
│ 👁️ Ver Detalhes     │
│ ✏️ Editar          │
│ ─────────────────── │
│ ✅ Ativar          │  // Se status != Active
│ ⏸️ Desativar       │  // Se status == Active
│ ─────────────────── │
│ 🗑️ Excluir         │
└─────────────────────┘
```

**Modal de Detalhes:**

```typescript
┌─────────────────────────────────────┐
│  👤 DETALHES DO USUÁRIO        [ X ]│
├─────────────────────────────────────┤
│                                     │
│  Nome: João Silva Santos            │
│  CPF: 611.203.190-64                │
│  Email: joao.silva@email.com        │
│  Telefone: (11) 98765-4321          │
│  Data Nasc: 15/05/2000              │
│  Peso: 75kg | Altura: 1.80m         │
│                                     │
│  Status: 🟡 Aguardando Ativação     │
│  Perfil: Jogador                    │
│  Unidade: Ataque                    │
│  Posição: Atacante                  │
│                                     │
│  📞 Contato de Emergência:          │
│  Nome: Maria Silva                  │
│  Telefone: (11) 98765-0000          │
│                                     │
│  Cadastrado em: 20/01/2026          │
│  Última atividade: 21/01/2026       │
│                                     │
│  [Ativar Usuário]  [Editar]         │
└─────────────────────────────────────┘
```

**Confirmação de Ativação/Desativação:**

```typescript
┌─────────────────────────────────────┐
│  ⚠️ CONFIRMAR AÇÃO            [ X ] │
├─────────────────────────────────────┤
│                                     │
│  Deseja ativar o usuário            │
│  "João Silva Santos"?               │
│                                     │
│  Esta ação permitirá que o usuário  │
│  acesse o sistema.                  │
│                                     │
│  [Cancelar]       [✅ Confirmar]    │
└─────────────────────────────────────┘
```

---

### **3. Página: Pré-Cadastro**

**URL:** `/dashboard/pre-registrations`

**Layout:**

```typescript
┌─────────────────────────────────────────────────┐
│  📝 PRÉ-CADASTRO DE USUÁRIOS                    │
├─────────────────────────────────────────────────┤
│                                                 │
│  [+ Novo Pré-Cadastro]                          │
│                                                 │
│  [Filtros]                                      │
│  Status: [Todos ▼] Busca CPF: [___________] 🔍 │
│                                                 │
│  [Tabela]                                       │
│  ┌─────────┬────────┬────────┬────────┬──────┐ │
│  │ CPF     │ Perfil │ Código │ Expira │ Ações│ │
│  ├─────────┼────────┼────────┼────────┼──────┤ │
│  │***.*64  │ Player │ ABCD123│ 5 dias │  ⋮   │ │
│  │***.*23  │ Admin  │ USADA  │   -    │  👁️  │ │
│  │***.*87  │ Staff  │ EXPIRED│   -    │  🔄  │ │
│  └─────────┴────────┴────────┴────────┴──────┘ │
└─────────────────────────────────────────────────┘
```

**Modal: Novo Pré-Cadastro**

```typescript
┌─────────────────────────────────────┐
│  ✨ NOVO PRÉ-CADASTRO         [ X ]│
├─────────────────────────────────────┤
│                                     │
│  CPF *                              │
│  [___.___.___-__]                   │
│                                     │
│  Perfil *                           │
│  ○ Jogador                          │
│  ○ Administrador                    │
│                                     │
│  Unidade *                          │
│  [Selecione... ▼]                   │
│  • Offense (Ataque)                 │
│  • Defense (Defesa)                 │
│                                     │
│  Posição (obrigatório para jogador)│
│  [Selecione... ▼]                   │
│  --- Offense ---                    │
│  • QB (Quarterback)                 │
│  • RB (Running Back)                │
│  • WR (Wide Receiver)               │
│  • OL (Offensive Line)              │
│  --- Defense ---                    │
│  • DL (Defensive Line)              │
│  • LB (Linebacker)                  │
│  • DB (Defensive Back)              │
│                                     │
│  [Cancelar]    [✨ Criar]           │
└─────────────────────────────────────┘
```

**Sucesso:**

```typescript
┌─────────────────────────────────────┐
│  ✅ PRÉ-CADASTRO CRIADO        [ X ]│
├─────────────────────────────────────┤
│                                     │
│  CPF: 611.203.190-64                │
│  Código de Ativação:                │
│                                     │
│   ┌────────────────────┐            │
│   │   ABCD1234EFGH     │  📋 Copiar │
│   └────────────────────┘            │
│                                     │
│  Validade: 7 dias                   │
│                                     │
│  ⚠️ Importante: Envie este código   │
│  para o usuário. Ele será usado     │
│  para completar o cadastro.         │
│                                     │
│  [Fechar]                           │
└─────────────────────────────────────┘
```

---

### **4. Página: Solicitações de Reset de Senha**

**URL:** `/dashboard/password-resets`

**Layout:**

```typescript
┌─────────────────────────────────────────────────┐
│  🔑 SOLICITAÇÕES DE RESET DE SENHA              │
├─────────────────────────────────────────────────┤
│                                                 │
│  [Tabs]                                         │
│  ┌──────────┬──────────┐                       │
│  │ Penden ES│ Histórico│                       │
│  │  (3)     │  (45)    │                       │
│  └──────────┴──────────┘                       │
│                                                 │
│  [Tabela - Pendentes]                           │
│  ┌────────────┬───────────┬──────────┬──────┐  │
│  │ Usuário    │ Solicitado│ CPF      │ Ação │  │
│  ├────────────┼───────────┼──────────┼──────┤  │
│  │ João Silva │ Há 2h     │***.*64   │[✅]  │  │
│  │ Maria S.   │ Há 4h     │***.*23   │[✅]  │  │
│  │ Pedro C.   │ Há 1d     │***.*87   │[✅]  │  │
│  └────────────┴───────────┴──────────┴──────┘  │
│                                                 │
│  💡 Ao aprovar, uma senha temporária será       │
│     gerada automaticamente.                     │
└─────────────────────────────────────────────────┘
```

**Modal: Aprovar Reset**

```typescript
┌─────────────────────────────────────┐
│  ✅ APROVAR RESET DE SENHA    [ X ] │
├─────────────────────────────────────┤
│                                     │
│  Usuário: João Silva Santos         │
│  CPF: 611.203.190-64                │
│  Email: joao.silva@email.com        │
│                                     │
│  ⚠️ Uma senha temporária será       │
│  gerada e o usuário precisará       │
│  alterá-la no primeiro login.       │
│                                     │
│  Validade: 24 horas                 │
│                                     │
│  [Cancelar]    [✅ Aprovar]         │
└─────────────────────────────────────┘
```

**Sucesso:**

```typescript
┌─────────────────────────────────────┐
│  ✅ SENHA TEMPORÁRIA GERADA    [ X ]│
├─────────────────────────────────────┤
│                                     │
│  Usuário: João Silva Santos         │
│  Senha Temporária:                  │
│                                     │
│   ┌────────────────────┐            │
│   │   TempPass@2026    │  📋 Copiar │
│   └────────────────────┘            │
│                                     │
│  Validade: 24 horas                 │
│  Expira em: 22/01/2026 20:25        │
│                                     │
│  ⚠️ Envie esta senha ao usuário.    │
│  O usuário DEVE alterar no 1º login.│
│                                     │
│  [Fechar]                           │
└─────────────────────────────────────┘
```

---

## 🎯 Componentes Reutilizáveis Sugeridos

### **1. DataTable Component**

```typescript
<DataTable
  columns={[
    { key: 'name', label: 'Nome', sortable: true },
    { key: 'cpf', label: 'CPF', render: (value) => maskCPF(value) },
    { key: 'status', label: 'Status', render: (value) => <StatusBadge status={value} /> },
    { key: 'actions', label: 'Ações', render: (row) => <ActionMenu row={row} /> },
  ]}
  data={users}
  onSort={handleSort}
  pagination={{
    currentPage: 1,
    totalPages: 10,
    onPageChange: handlePageChange
  }}
/>
```

**Features:**
- Sorting
- Pagination
- Custom render functions
- Loading states
- Empty states
- Responsive (mobile → cards, desktop → table)

---

### **2. StatusBadge Component**

```typescript
<StatusBadge status="AwaitingActivation" />
// Renders: 🟡 Aguardando Ativação

<StatusBadge status="Active" />
// Renders: 🟢 Ativo

<StatusBadge status="Inactive" />
// Renders: 🔴 Inativo
```

**Variants:**
- PendingRegistration: 🔵 Pendente
- AwaitingActivation: 🟡 Aguardando
- Active: 🟢 Ativo
- Rejected: ⚫ Rejeitado
- Inactive: 🔴 Inativo

---

### **3. ConfirmationModal Component**

```typescript
<ConfirmationModal
  title="Confirmar Ação"
  message="Deseja ativar este usuário?"
  variant="warning" // success, danger, warning, info
  confirmText="Confirmar"
  cancelText="Cancelar"
  onConfirm={handleConfirm}
  onCancel={handleCancel}
  isOpen={isOpen}
/>
```

---

### **4. ActionMenu Component**

```typescript
<ActionMenu
  items={[
    { label: 'Ver Detalhes', icon: Eye, onClick: handleView },
    { label: 'Editar', icon: Edit, onClick: handleEdit },
    { type: 'divider' },
    { label: 'Ativar', icon: Check, onClick: handleActivate, variant: 'success' },
    { label: 'Desativar', icon: Pause, onClick: handleDeactivate, variant: 'warning' },
    { type: 'divider' },
    { label: 'Excluir', icon: Trash, onClick: handleDelete, variant: 'danger' },
  ]}
/>
```

---

### **5. Sidebar Component**

```typescript
<Sidebar>
  <SidebarSection title="Gestão">
    <SidebarItem
      icon={Users}
      label="Usuários"
      href="/dashboard/users"
      badge={pendingCount}
    />
    <SidebarItem
      icon={UserPlus}
      label="Pré-Cadastro"
      href="/dashboard/pre-registrations"
    />
    <SidebarItem
      icon={Key}
      label="Reset Senha"
      href="/dashboard/password-resets"
      badge={pendingResets}
    />
  </SidebarSection>
</Sidebar>
```

---

### **6. StatsCard Component**

```typescript
<StatsCard
  title="Usuários Ativos"
  value={125}
  icon={Users}
  trend={{ value: 12, direction: 'up' }}
  color="green"
/>
```

---

## 🗂️ Estrutura de Arquivos Sugerida

```
frontend/
├── app/
│   ├── dashboard/
│   │   ├── layout.tsx                    # Layout com sidebar
│   │   ├── page.tsx                      # Dashboard home
│   │   ├── users/
│   │   │   ├── page.tsx                  # Lista de usuários
│   │   │   ├── [id]/
│   │   │   │   └── page.tsx              # Detalhes do usuário  
│   │   │   └── components/
│   │   │       ├── UserTable.tsx
│   │   │       ├── UserModal.tsx
│   │   │       └── UserFilters.tsx
│   │   ├── pre-registrations/
│   │   │   ├── page.tsx
│   │   │   └── components/
│   │   │       ├── PreRegTable.tsx
│   │   │       └── CreatePreRegModal.tsx
│   │   └── password-resets/
│   │       ├── page.tsx
│   │       └── components/
│   │           ├── ResetRequestsTable.tsx
│   │           └── ApproveResetModal.tsx
│   │
├── components/
│   ├── dashboard/
│   │   ├── Sidebar.tsx
│   │   ├── DashboardHeader.tsx
│   │   └── StatsCard.tsx
│   ├── shared/
│   │   ├── DataTable.tsx
│   │   ├── StatusBadge.tsx
│   │   ├── ActionMenu.tsx
│   │   ├── ConfirmationModal.tsx
│   │   └── EmptyState.tsx
│   │
├── hooks/
│   ├── api/
│   │   ├── useUsers.ts
│   │   ├── usePreRegistrations.ts
│   │   └── usePasswordResets.ts
│   │
├── types/
│   ├── user.ts
│   ├── preRegistration.ts
│   └── passwordReset.ts
│
└── lib/
    ├── utils/
    │   ├── cpf.ts                        # Já existe
    │   ├── date.ts
    │   └── status.ts
    └── constants/
        └── userStatus.ts
```

---

## 🔐 Permissões e Controle de Acesso por Role

### **Análise de Roles do Backend:**

**ProfileType Enum:**
```csharp
public enum ProfileType
{
    Admin = 1,      // Acesso total
    Coach = 2,      // Técnico (futuro)
    Athlete = 3,    // Jogador (read-only)
    Staff = 4       // Equipe técnica (futuro)
}
```

**Token JWT Claims:**
```csharp
- ClaimTypes.NameIdentifier → userId (Guid)
- ClaimTypes.Email → email
- ClaimTypes.Role → "Admin" | "Athlete" | "Coach" | "Staff"
- "unit" → PlayerUnit (Ataque, Defesa, etc)
- "position" → PlayerPosition (Atacante, Zagueiro, etc)
```

**Como Extrair no Frontend:**
```typescript
// O token já vem decodificado no cookie httpOnly
// Precisamos criar endpoint GET /api/auth/me para retornar user info

interface CurrentUser {
  id: string;
  email: string;
  role: 'Admin' | 'Athlete' | 'Coach' | 'Staff';
  unit?: string;  // Para Athlete
  position?: string;  // Para Athlete
}
```

---

### **Matriz de Permissões por Endpoint:**

#### **Gestão de Usuários:**

| Endpoint | Admin | Athlete | Descrição |
|----------|-------|---------|------------|
| `GET /users/status/{status}` | ✅ | ❌ | Listar usuários |
| `GET /users/{id}` | ✅ | ✅* | Ver próprio perfil |
| `POST /users/{id}/activate` | ✅ | ❌ | Ativar usuário |
| `POST /users/{id}/deactivate` | ✅ | ❌ | Desativar usuário |
| `PUT /users/{id}` | ✅ | ✅* | Editar (próprio) |
| `DELETE /users/{id}` | ✅ | ❌ | Excluir usuário |

*Com validação `CanAccessUser()` - apenas próprio ID

#### **Pré-Cadastro:**

| Endpoint | Admin | Athlete |
|----------|-------|----------|
| `POST /pre-registrations` | ✅ | ❌ |
| `GET /pre-registrations/{id}` | ✅ | ❌ |
| `GET /pre-registrations/cpf/{cpf}` | ✅ | ❌ |
| `POST /pre-registrations/{id}/regenerate` | ✅ | ❌ |

#### **Reset de Senha:**

| Endpoint | Admin | Athlete |
|----------|-------|----------|
| `GET /auth/password-reset-requests/pending` | ✅ | ❌ |
| `POST /auth/password-reset-requests/{id}/approve` | ✅ | ❌ |
| `POST /auth/request-password-reset` | ✅ | ✅ | Público |

#### **Atividades:**

| Endpoint | Admin | Athlete |
|----------|-------|----------|
| `POST /activities` | ✅ | ❌ | Criar atividade |
| `GET /activities` | ✅ | ✅ | Listar (filtrado) |
| `GET /activities/{id}` | ✅ | ✅ | Ver detalhes (filtrado) |
| `PUT /activities/{id}` | ✅ | ❌ | Editar |
| `DELETE /activities/{id}` | ✅ | ❌ | Excluir |
| `POST /activities/{id}/items` | ✅ | ❌ | Adicionar item |
| `PUT /activities/{id}/items/{itemId}` | ✅ | ❌ | Editar item |
| `DELETE /activities/{id}/items/{itemId}` | ✅ | ❌ | Excluir item |

**Nota:** Athlete vê apenas atividades da sua `unit` e `position`

---

### **Comportamento da UI por Role:**

#### **🔴 Admin (Acesso Completo):**

**Sidebar visível:**
```typescript
✅ 📊 Visão Geral (Dashboard)
✅ 👥 Gestão
   ✅ Usuários
   ✅ Pré-Cadastro  
   ✅ Reset Senha
✅ ⚽ Time
   ✅ Jogadores (futuro)
   ✅ Staff (futuro)
   ✅ Departamentos
✅ 🏃 Atividades
   ✅ Criar atividade
   ✅ Editar atividade
   ✅ Excluir atividade
✅ ⚙️ Configurações
```

**Ações disponíveis:**
- Criar, editar, excluir tudo
- Ativar/desativar usuários
- Aprovar reset de senha
- Gerenciar pré-cadastros

---

#### **🔵 Athlete (Read-Only + Próprio Perfil):**

**Sidebar visível:**
```typescript
✅ 📊 Visão Geral (Dashboard simplificado)
   - Minhas estatísticas
   - Próximas atividades
❌ 👥 Gestão (OCULTA COMPLETAMENTE)
❌ ⚽ Time (OCULTA)
✅ 🏃 Atividades
   ✅ Ver minhas atividades (somente unidade/posição)
   ❌ Criar atividade (botão não aparece)
   ❌ Editar atividade
   ❌ Excluir atividade
✅ ⚙️ Configurações
   ✅ Meu Perfil (editar próprios dados)
   ✅ Alterar Senha
```

**Ações disponíveis:**
- Ver próprio perfil
- Editar próprios dados (nome, telefone, email, etc)
- Alterar própria senha
- Ver atividades da sua unidade/posição (read-only)
- Ver dashboard com suas estatísticas pessoais

**Ações BLOQUEADAS:**
- Tudo relacionado a gestão de outros usuários
- Criar/editar/excluir atividades
- Aprovar solicitações
- Criar pré-cadastros

---

### **Implementação de Verificação de Role:**

#### **1. Hook de Autorização:**

```typescript
// hooks/useAuth.ts
export const useAuth = () => {
  const [user, setUser] = useState<CurrentUser | null>(null);
  
  useEffect(() => {
    // Buscar user info do backend
    fetch('/api/auth/me')
      .then(res => res.json())
      .then(data => setUser(data));
  }, []);
  
  const isAdmin = user?.role === 'Admin';
  const isAthlete = user?.role === 'Athlete';
  
  return { user, isAdmin, isAthlete };
};
```

#### **2. Proteção de Rotas (middleware):**

```typescript
// middleware.ts
export function middleware(request: NextRequest) {
  const path = request.nextUrl.pathname;
  
  // Rotas que requerem Admin
  const adminOnlyRoutes = [
    '/dashboard/users',
    '/dashboard/pre-registrations',
    '/dashboard/password-resets',
  ];
  
  if (adminOnlyRoutes.some(route => path.startsWith(route))) {
    // Verificar se é admin (via token ou session)
    const role = getUserRoleFromToken(request);
    if (role !== 'Admin') {
      return NextResponse.redirect(new URL('/dashboard', request.url));
    }
  }
  
  return NextResponse.next();
}
```

#### **3. Componente de Proteção:**

```typescript
// components/AdminOnly.tsx
export const AdminOnly = ({ children }: { children: React.ReactNode }) => {
  const { isAdmin } = useAuth();
  
  if (!isAdmin) return null;
  
  return <>{children}</>;
};

// Uso:
<AdminOnly>
  <SidebarItem label="Gestão de Usuários" href="/dashboard/users" />
</AdminOnly>
```

#### **4. Hook para Páginas Admin:**

```typescript
// hooks/useAdminOnly.ts
export const useAdminOnly = () => {
  const { user, isAdmin } = useAuth();
  const router = useRouter();
  
  useEffect(() => {
    if (user && !isAdmin) {
      router.push('/dashboard');
      toast.error('Acesso negado. Área restrita a administradores.');
    }
  }, [user, isAdmin, router]);
  
  return isAdmin;
};

// Uso em página:
function UsersPage() {
  const isAdmin = useAdminOnly();
  
  if (!isAdmin) return <LoadingSpinner />;
  
  return <UserManagementTable />;
}
```

---

### **Sidebar Condicional:**

```typescript
// components/dashboard/Sidebar.tsx
function Sidebar() {
  const { isAdmin } = useAuth();
  
  return (
    <aside>
      {/* Todos veem */}
      <SidebarItem icon={Home} label="Visão Geral" href="/dashboard" />
      
      {/* Apenas Admin */}
      {isAdmin && (
        <SidebarSection title="Gestão">
          <SidebarItem icon={Users} label="Usuários" href="/dashboard/users" />
          <SidebarItem icon={UserPlus} label="Pré-Cadastro" href="/dashboard/pre-registrations" />
          <SidebarItem icon={Key} label="Reset Senha" href="/dashboard/password-resets" />
        </SidebarSection>
      )}
      
      {/* Todos veem - mas comportamento diferente */}
      <SidebarSection title="Atividades">
        <SidebarItem icon={Activity} label="Atividades" href="/dashboard/activities" />
      </SidebarSection>
      
      {/* Todos veem */}
      <SidebarSection title="Conta">
        <SidebarItem icon={Settings} label="Configurações" href="/dashboard/settings" />
      </SidebarSection>
    </aside>
  );
}
```

---

### **Dashboard Home Condicional:**

```typescript
// app/dashboard/page.tsx
function DashboardPage() {
  const { isAdmin, isAthlete } = useAuth();
  
  if (isAdmin) {
    return (
      <>
        <StatsCards /> {/* Todos usuários, pendentes, etc */}
        <QuickActions /> {/* Criar pré-cadastro, aprovar resets */}
        <RecentActivity /> {/* Log de ações do sistema */}
      </>
    );
  }
  
  if (isAthlete) {
    return (
      <>
        <AthleteStats /> {/* Minhas estatísticas pessoais */}
        <MyUpcomingActivities /> {/* Próximas atividades */}
        <MyPerformance /> {/* Gráficos de performance */}
      </>
    );
  }
  
  return <GenericDashboard />;
}
```

---

### **Endpoint Necessário no Backend:**

**Adicionar em `AuthEndpoints.cs`:**

```csharp
group.MapGet("/me", async (
    ClaimsPrincipal user,
    IUserService service) =>
{
    var userId = user.GetUserId();
    var result = await service.GetByIdAsync(userId);
    
    return result.IsSuccess 
        ? Results.Ok(new {
            id = userId,
            email = user.GetUserEmail(),
            role = user.GetUserRole(),
            profile = user.GetUserProfile(),
            unit = user.GetUserUnit(),
            position = user.GetUserPosition()
        })
        : Results.NotFound();
})
.WithName("GetCurrentUser")
.RequireAuthorization()
.RequireRateLimiting("authenticated");
```

---

## 📱 Responsividade

### **Breakpoints:**

```typescript
// Mobile: < 768px
- Sidebar colapsável (hamburger menu)
- Tabelas → Cards verticais
- Modals full-screen

// Tablet: 768px - 1024px
- Sidebar fixo lateral (ícone + texto reduzido)
- Tabelas com scroll horizontal

// Desktop: > 1024px
- Sidebar fixo completo
- Tabelas full-width
- Modals centralizados
```

---

## 🎨 Design System Sugerido

### **Cores:**

```typescript
const theme = {
  status: {
    pending: '#3B82F6',      // Blue
    awaiting: '#F59E0B',     // Amber
    active: '#10B981',       // Green
    rejected: '#6B7280',     // Gray
    inactive: '#EF4444',     // Red
  },
  admin: {
    primary: '#3B82F6',      // Blue
    secondary: '#8B5CF6',    // Purple
    accent: '#EC4899',       // Pink
  }
};
```

### **Ícones (lucide-react):**

```typescript
import {
  Users,           // Usuários
  UserPlus,        // Pré-cadastro
  Key,             // Reset senha
  CheckCircle,     // Ativar
  PauseCircle,     // Desativar
  Eye,             // Ver
  Edit,            // Editar
  Trash,           // Excluir
  Copy,            // Copiar
  Mail,            // Enviar email
  MoreVertical,    // Menu ações
  Filter,          // Filtros
  Search,          // Busca
} from 'lucide-react';
```

---

## 🚀 Fluxos de Usuário

### **Fluxo 1: Admin cria pré-cadastro**

```
1. Admin → /dashboard/pre-registrations
2. Click "Novo Pré-Cadastro"
3. Preenche CPF, Perfil, Setor, Posição
4. Click "Criar"
5. Sistema gera código (ex: ELUIP9IA)
6. Modal mostra código com opção de copiar
7. Admin envia código ao usuário (email/whatsapp)
```

### **Fluxo 2: Usuário completa cadastro** (já implementado)

```
1. Usuário → /complete-registration
2. Preenche CPF + Código
3. Preenche dados pessoais
4. Preenche senha + emergência
5. Submete
6. Status → AwaitingActivation
```

### **Fluxo 3: Admin ativa usuário**

```
1. Admin → /dashboard/users
2. Tab "Aguardando" (mostra lista)
3. Click ⋮ → "Ativar" no usuário
4. Modal de confirmação
5. Click "Confirmar"
6. Status → Active
7. Usuário pode fazer login
```

### **Fluxo 4: Admin aprova reset de senha**

```
1. Admin → /dashboard/password-resets
2. Tab "Pendentes" (mostra solicitações)
3. Click "✅ Aprovar"
4. Modal de confirmação
5. Click "Aprovar"
6. Sistema gera senha temporária
7. Modal mostra senha com opção copiar
8. Admin envia senha ao usuário
9. Usuário faz login → Redireciona /change-password
```

---

## ✅ Checklist de Implementação (Sugestão)

### **Fase 1: Setup Básico**
- [ ] Criar layout de dashboard com sidebar
- [ ] Implementar componentes base (DataTable, StatusBadge, etc)
- [ ] Criar hooks de API (useUsers, usePreRegistrations, usePasswordResets)
- [ ] Configurar middleware de autorização

### **Fase 2: Gestão de Usuários**
- [ ] Página de listagem de usuários
- [ ] Filtros e busca
- [ ] Modal de detalhes
- [ ] Ação de ativar/desativar
- [ ] Ação de excluir

### **Fase 3: Pré-Cadastro**
- [ ] Página de listagem
- [ ] Modal de criar pré-cadastro
- [ ] Integração com API
- [ ] Modal de sucesso com código
- [ ] Regenerar código

### **Fase 4: Reset de Senha**
- [ ] Página de solicitações pendentes
- [ ] Aprovar solicitação
- [ ] Modal com senha temporária
- [ ] Histórico de resets

### **Fase 5: Dashboard Home**
- [ ] Cards de estatísticas
- [ ] Atividades recentes
- [ ] Ações rápidas

### **Fase 6: Testes**
- [ ] Testes de integração das páginas
- [ ] Testes de componentes
- [ ] Testes de permissões

---

## 💡 Observações Importantes

### **1. UX/UI:**
- Use feedback visual claro (toasts, loading states)
- Confirmações para ações destrutivas
- Empty states informativos
- Skeleton loaders durante carregamento

### **2. Segurança:**
- Validar permissões no frontend E backend
- Não mostrar CPF completo em listas
- Logs de ações administrativas
- Rate limiting para ações críticas (já existe no backend)

### **3. Acessibilidade:**
- Usar semantic HTML
- Navegação por teclado
- ARIA labels apropriados
- Contraste adequado

### **4. Performance:**
- Pagination server-side
- Debounce em buscas
- Lazy loading de modais
- Cache de queries (React Query)

---

## 🎯 Próximos Passos

1. **Revisar este plano** com o usuário
2. **Ajustar** conforme feedback
3. **Priorizar** funcionalidades
4. **Criar task.md** detalhado
5. **Iniciar implementação** fase por fase

---

**Documento criado para análise e discussão.**  
**Nenhum código foi implementado conforme solicitado.** ✅
