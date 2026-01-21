# 🏆 GFA Team Manager - Backend API

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=c-sharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Entity Framework](https://img.shields.io/badge/EF%20Core-8.0-512BD4)](https://docs.microsoft.com/en-us/ef/core/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15+-336791?logo=postgresql)](https://www.postgresql.org/)

API RESTful para gerenciamento de equipes de futebol americano, incluindo gestão de atletas, atividades, pré-cadastros e autenticação.

---

## 📋 Índice

- [Arquitetura](#-arquitetura)
- [Tecnologias](#-tecnologias)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Começando](#-começando)
- [Configuração](#-configuração)
- [Endpoints](#-endpoints-principais)
- [Autenticação](#-autenticação)
- [Domínio](#-domínio)
- [Testes](#-testes)
- [Migrations](#-migrations)
- [Contribuindo](#-contribuindo)

---

## 🏗️ Arquitetura

O projeto segue os princípios de **Clean Architecture** e **Domain-Driven Design (DDD)**:

```
┌─────────────────────────────────────┐
│          API Layer                  │
│  (Controllers/Endpoints, Middleware)│
├─────────────────────────────────────┤
│      Application Layer              │
│  (Services, DTOs, Validators)       │
├─────────────────────────────────────┤
│         Domain Layer                │
│  (Entities, Enums, Interfaces)      │
├─────────────────────────────────────┤
│     Infrastructure Layer            │
│  (Repositories, DbContext, Data)    │
└─────────────────────────────────────┘
```

### Camadas:

- **API (`GFATeamManager.Api`)**: Endpoints REST usando Minimal APIs
- **Application (`GFATeamManager.Application`)**: Lógica de negócio e serviços
- **Domain (`GFATeamManager.Domain`)**: Entidades e regras de domínio
- **Infrastructure (`GFATeamManager.Infrastructure`)**: Acesso a dados e integrações

---

## 🛠️ Tecnologias

### Core:
- **.NET 8.0** - Framework principal
- **C# 12** - Linguagem
- **ASP.NET Core** - Web framework
- **Minimal APIs** - Endpoints REST simplificados

### Dados:
- **Entity Framework Core 8** - ORM
- **PostgreSQL 15+** - Banco de dados
- **Npgsql** - Provider PostgreSQL

### Autenticação & Segurança:
- **JWT (JSON Web Tokens)** - Autenticação stateless
- **BCrypt.Net** - Hashing de senhas
- **Rate Limiting** - Proteção contra ataques

### Validação:
- **FluentValidation** - Validação de DTOs e requisições

### Testes:
- **xUnit** - Framework de testes
- **Moq** - Mocking
- **FluentAssertions** - Assertions expressivas

---

## 📁 Estrutura do Projeto

```
backend/
├── src/
│   ├── GFATeamManager.Api/
│   │   ├── Endpoints/          # Minimal API endpoints
│   │   ├── Extensions/         # Extension methods
│   │   ├── Middleware/         # Custom middleware
│   │   └── Program.cs          # Entry point
│   │
│   ├── GFATeamManager.Application/
│   │   ├── DTOs/               # Data Transfer Objects
│   │   ├── Services/           # Business logic
│   │   └── Validators/         # FluentValidation validators
│   │
│   ├── GFATeamManager.Domain/
│   │   ├── Entities/           # Domain entities
│   │   ├── Enums/              # Enumerations
│   │   └── Interfaces/         # Repository interfaces
│   │
│   └── GFATeamManager.Infrastructure/
│       ├── Data/
│       │   ├── Context/        # DbContext
│       │   ├── Configurations/ # EF configurations
│       │   ├── Migrations/     # Database migrations
│       │   └── Repositories/   # Repository implementations
│       └── Services/           # External services
│
└── tests/
    ├── GFATeamManager.Domain.Tests/
    ├── GFATeamManager.Application.Tests/
    └── GFATeamManager.Infrastructure.Tests/
```

---

## 🚀 Começando

### Pré-requisitos:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 15+](https://www.postgresql.org/download/)
- IDE recomendada: [JetBrains Rider](https://www.jetbrains.com/rider/) ou [Visual Studio 2022](https://visualstudio.microsoft.com/)

### Instalação:

1. **Clone o repositório:**
```bash
git clone https://github.com/your-org/GFATeamManager.git
cd GFATeamManager/backend
```

2. **Restaure as dependências:**
```bash
dotnet restore
```

3. **Configure o banco de dados:**

Crie um banco PostgreSQL:
```sql
CREATE DATABASE gfateammanager;
```

4. **Configure as variáveis de ambiente:**

Crie `.env` ou configure `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=gfateammanager;Username=postgres;Password=yourpassword"
  },
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-minimum-32-characters",
    "Issuer": "GFATeamManager",
    "Audience": "GFATeamManager",
    "ExpirationHours": 8
  }
}
```

5. **Execute as migrations:**
```bash
dotnet ef database update --project src/GFATeamManager.Infrastructure
```

6. **Execute a aplicação:**
```bash
dotnet run --project src/GFATeamManager.Api
```

A API estará disponível em: `https://localhost:5000`

---

## ⚙️ Configuração

### Variáveis de Ambiente:

| Variável | Descrição | Padrão |
|----------|-----------|--------|
| `ASPNETCORE_ENVIRONMENT` | Ambiente (Development/Staging/Production) | Development |
| `ConnectionStrings__DefaultConnection` | String de conexão PostgreSQL | - |
| `JwtSettings__SecretKey` | Chave secreta JWT (mín. 32 chars) | - |
| `JwtSettings__ExpirationHours` | Tempo de expiração do token | 8 |

### Rate Limiting:

A API possui limitação de requisições configurada:

- **Login:** 5 requisições / minuto
- **Auth (autenticado):** 100 requisições / minuto
- **Admin:** 200 requisições / minuto

---

## 📡 Endpoints Principais

### Autenticação:

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| POST | `/api/auth/login` | Login do usuário | ❌ |
| POST | `/api/auth/request-password-reset` | Solicitar reset de senha | ❌ |
| POST | `/api/auth/change-password` | Alterar senha | ✅ |
| GET | `/api/auth/password-reset-requests/pending` | Listar solicitações pendentes | ✅ Admin |
| POST | `/api/auth/password-reset-requests/{id}/approve` | Aprovar solicitação | ✅ Admin |

### Usuários:

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/users` | Listar todos os usuários | ✅ Admin |
| GET | `/api/users/me` | Dados do usuário logado | ✅ |
| POST | `/api/users/complete-registration` | Completar cadastro | ❌ |
| PUT | `/api/users/{id}/activate` | Ativar usuário | ✅ Admin |
| PUT | `/api/users/{id}/reject` | Rejeitar usuário | ✅ Admin |

### Pré-Cadastros:

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/pre-registrations` | Listar pré-cadastros | ✅ Admin |
| POST | `/api/pre-registrations` | Criar pré-cadastro | ✅ Admin |
| POST | `/api/pre-registrations/validate` | Validar código de ativação | ❌ |
| DELETE | `/api/pre-registrations/{id}` | Deletar pré-cadastro | ✅ Admin |

### Atividades:

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/activities` | Listar atividades | ✅ Staff |
| POST | `/api/activities` | Criar atividade | ✅ Staff |
| GET | `/api/activities/my` | Minhas atividades | ✅ |
| POST | `/api/activities/{id}/items` | Adicionar item | ✅ Staff |
| PUT | `/api/activities/{id}/items/{itemId}` | Atualizar item | ✅ Staff |

📖 **[Documentação completa da API](./api_documentation_password_reset.md)**

---

## 🔐 Autenticação

### Fluxo de Autenticação:

1. **Login:**
```http
POST /api/auth/login
Content-Type: application/json

{
  "login": "user@email.com",
  "password": "Password123"
}
```

2. **Resposta:**
```json
{
  "isSuccess": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "expiresAt": "2026-01-22T06:00:00Z"
  }
}
```

3. **Uso do Token:**
```http
GET /api/users/me
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

### Password Reset (Híbrido):

O sistema implementa lógica híbrida de reset de senha:

- **PENDING:** Auto-cancelamento ao fazer login com senha normal
- **APPROVED:** Bloqueia login, força uso da senha temporária

[Ver documentação detalhada](./api_documentation_password_reset.md)

---

## 📦 Domínio

### Entidades Principais:

#### **User**
```csharp
public class User : BaseEntity
{
    public string Cpf { get; set; }
    public string PasswordHash { get; set; }
    public ProfileType Profile { get; set; }      // Admin, Staff, Athlete
    public UserStatus Status { get; set; }          // Active, Inactive, etc.
    public string FullName { get; set; }
    public DateTime BirthDate { get; set; }
    public decimal Weight { get; set; }
    public int Height { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public EmergencyContact? EmergencyContact { get; set; }
    public bool RequiresPasswordChange { get; set; }
}
```

#### **PreRegistration**
```csharp
public class PreRegistration : BaseEntity
{
    public string Cpf { get; set; }
    public string FullName { get; set; }
    public string ActivationCode { get; set; }    // 8 chars uppercase
    public ProfileType Profile { get; set; }
    public PlayerUnit? Unit { get; set; }
    public PlayerPosition? Position { get; set; }
    public bool IsUsed { get; set; }
}
```

#### **Activity**
```csharp
public class Activity : BaseEntity
{
    public ActivityType Type { get; set; }         // Training, Game, Meeting, etc.
    public DateTime ScheduledDate { get; set; }
    public string Description { get; set; }
    public PlayerUnit? TargetUnit { get; set; }
    public List<ActivityItem> Items { get; set; }  // Individual player data
}
```

### Enums:

- `ProfileType`: Admin, Staff, Athlete
- `UserStatus`: PendingRegistration, AwaitingActivation, Active, Inactive, Rejected
- `ActivityType`: Training, Game, Meeting, Evaluation
- `PlayerUnit`: Offense, Defense, SpecialTeams
- `PlayerPosition`: QB, RB, WR, TE, OL, DL, LB, DB, K, P

---

## 🧪 Testes

### Executar todos os testes:

```bash
dotnet test
```

### Executar testes de um projeto específico:

```bash
dotnet test tests/GFATeamManager.Application.Tests
```

### Executar com cobertura:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Estatísticas de Testes:

```
✅ Domain Tests:        30+ testes
✅ Application Tests:   50+ testes
✅ Infrastructure Tests: 20+ testes
───────────────────────────────────
Total:                  100+ testes
Cobertura:              ~85%
```

### Estrutura de Testes:

```csharp
// Exemplo: AuthServiceTests.cs
[Fact]
public async Task LoginAsync_ShouldSoftDeletePendingRequest_WhenLoggingInWithNormalPassword()
{
    // Arrange
    var user = CreateTestUser();
    var pendingRequest = CreatePendingRequest(user.Id);
    
    // Act
    var result = await _authService.LoginAsync(loginRequest);
    
    // Assert
    result.IsSuccess.Should().BeTrue();
    pendingRequest.IsDeleted.Should().BeTrue();
}
```

---

## 🗄️ Migrations

### Criar nova migration:

```bash
dotnet ef migrations add MigrationName --project src/GFATeamManager.Infrastructure --startup-project src/GFATeamManager.Api
```

### Aplicar migrations:

```bash
dotnet ef database update --project src/GFATeamManager.Infrastructure --startup-project src/GFATeamManager.Api
```

### Reverter migration:

```bash
dotnet ef database update PreviousMigrationName --project src/GFATeamManager.Infrastructure --startup-project src/GFATeamManager.Api
```

### Gerar script SQL:

```bash
dotnet ef migrations script --project src/GFATeamManager.Infrastructure --output migration.sql
```

---

## 🏗️ Build & Deploy

### Build:

```bash
dotnet build --configuration Release
```

### Publish:

```bash
dotnet publish src/GFATeamManager.Api --configuration Release --output ./publish
```

### Docker:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/GFATeamManager.Api/GFATeamManager.Api.csproj", "src/GFATeamManager.Api/"]
RUN dotnet restore "src/GFATeamManager.Api/GFATeamManager.Api.csproj"
COPY . .
WORKDIR "/src/src/GFATeamManager.Api"
RUN dotnet build "GFATeamManager.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GFATeamManager.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GFATeamManager.Api.dll"]
```

---

## 👥 Contribuindo

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'feat: add amazing feature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

### Convenções de Commit:

Seguimos [Conventional Commits](https://www.conventionalcommits.org/):

- `feat:` Nova funcionalidade
- `fix:` Correção de bug
- `docs:` Documentação
- `test:` Testes
- `refactor:` Refatoração
- `chore:` Manutenção

---

## 📄 Licença

Este projeto é proprietário e de uso interno.

---

## 📞 Suporte

Para questões e suporte:
- **Email**: dev@gfateam.com
- **Documentação**: [Wiki do Projeto](./docs)
- **Issues**: [GitHub Issues](https://github.com/your-org/GFATeamManager/issues)

---

**Feito com ❤️ pelo time GFA**
