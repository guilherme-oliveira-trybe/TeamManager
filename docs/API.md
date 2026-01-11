# Documentação da API

## Visão Geral

A API do GFA Team Manager segue o estilo **REST** utilizando **Minimal APIs** do .NET. Todos os endpoints retornam JSON e seguem convenções HTTP padrão.

**Base URL**: `https://localhost:5001/api`

**Swagger UI**: `https://localhost:5001/swagger`

## Autenticação

A API utiliza **JWT (JSON Web Tokens)** para autenticação.

### Obter Token

Faça login para receber um token JWT:

```http
POST /api/auth/login
```

O token deve ser incluído no header `Authorization` de todas as requisições autenticadas:

```
Authorization: Bearer {seu_token_jwt}
```

### Níveis de Acesso

- **Anônimo**: Endpoints públicos (login, registro, request password reset)
- **Autenticado**: Qualquer usuário logado
- **AdminOnly**: Apenas administradores

## Rate Limiting

A API implementa rate limiting para proteger contra abuso:

- **Public** (login, registro): 10 requisições por minuto
- **Authenticated**: 30 requisições por minuto
- **Admin**: 60 requisições por minuto

## Respostas Padrão

### Sucesso com Dados

```json
{
  "isSuccess": true,
  "data": { ... },
  "message": null
}
```

### Sucesso sem Dados

```json
{
  "isSuccess": true,
  "message": "Operação realizada com sucesso"
}
```

### Erro

```json
{
  "isSuccess": false,
  "data": null,
  "message": "Descrição do erro"
}
```

### Erro de Validação

```json
{
  "isSuccess": false,
  "data": null,
  "message": "Erro de validação",
  "errors": {
    "Campo": ["Mensagem de erro 1", "Mensagem de erro 2"]
  }
}
```

## Endpoints

### Authentication

#### POST /api/auth/login

Autentica um usuário e retorna token JWT.

**Acesso**: Anônimo  
**Rate Limit**: login (10/min)

**Request Body**:
```json
{
  "login": "12345678901",  // CPF ou email
  "password": "SuaSenha123"
}
```

**Response** (200 OK):
```json
{
  "isSuccess": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresAt": "2026-01-11T16:35:15Z"
  },
  "message": null
}
```

**Errors**:
- `401 Unauthorized`: Login ou senha inválidos
- `400 Bad Request`: Usuário não está ativo

---

#### POST /api/auth/change-password

Altera a senha do usuário autenticado.

**Acesso**: Autenticado  
**Rate Limit**: authenticated (30/min)

**Request Body**:
```json
{
  "currentPassword": "SenhaAtual123",
  "newPassword": "NovaSenha123",
  "confirmNewPassword": "NovaSenha123"
}
```

**Response** (200 OK):
```json
{
  "isSuccess": true,
  "message": "Operação realizada com sucesso"
}
```

**Errors**:
- `400 Bad Request`: Senha atual incorreta ou senhas não coincidem
- `401 Unauthorized`: Token inválido ou expirado

---

#### POST /api/auth/request-password-reset

Solicita reset de senha (requer aprovação de admin).

**Acesso**: Anônimo  
**Rate Limit**: login (10/min)

**Request Body**:
```json
{
  "cpf": "12345678901",
  "email": "usuario@email.com"
}
```

**Response** (200 OK):
```json
{
  "message": "Se os dados estiverem corretos, sua solicitação foi enviada ao administrador."
}
```

**Nota**: Sempre retorna sucesso para evitar enumeração de usuários.

---

#### GET /api/auth/password-reset-requests/pending

Lista solicitações de reset de senha pendentes.

**Acesso**: AdminOnly  
**Rate Limit**: admin (60/min)

**Response** (200 OK):
```json
{
  "isSuccess": true,
  "data": [
    {
      "userFullName": "João Silva",
      "approvedByName": null,
      "temporaryPassword": null,
      "expirationDate": null,
      "isUsed": false
    }
  ],
  "message": null
}
```

---

#### POST /api/auth/password-reset-requests/{requestId}/approve

Aprova uma solicitação de reset e gera senha temporária.

**Acesso**: AdminOnly  
**Rate Limit**: admin (60/min)

**Path Parameters**:
- `requestId` (UUID): ID da solicitação

**Response** (200 OK):
```json
{
  "isSuccess": true,
  "data": {
    "userFullName": "João Silva",
    "approvedByName": "Admin User",
    "temporaryPassword": "TempPass123",
    "expirationDate": "2026-01-12T08:35:15Z",
    "isUsed": false
  },
  "message": null
}
```

---

### Pre-Registrations

#### POST /api/pre-registrations

Cria um pré-cadastro com código de ativação.

**Acesso**: AdminOnly  
**Rate Limit**: admin (60/min)

**Request Body**:
```json
{
  "cpf": "12345678901",
  "profile": 3  // 1=Admin, 2=Coach, 3=Athlete, 4=Staff
}
```

**Response** (201 Created):
```json
{
  "isSuccess": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "cpf": "12345678901",
    "activationCode": "A1B2C3D4",
    "profile": 3,
    "expirationDate": "2026-01-18T08:35:15Z",
    "isUsed": false,
    "usedAt": null,
    "createdAt": "2026-01-11T08:35:15Z"
  },
  "message": null
}
```

**Errors**:
- `400 Bad Request`: CPF inválido ou já cadastrado

---

#### GET /api/pre-registrations/{id}

Busca pré-cadastro por ID.

**Acesso**: AdminOnly  
**Rate Limit**: admin (60/min)

**Path Parameters**:
- `id` (UUID): ID do pré-cadastro

**Response** (200 OK):
```json
{
  "isSuccess": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "cpf": "12345678901",
    "activationCode": "A1B2C3D4",
    "profile": 3,
    "expirationDate": "2026-01-18T08:35:15Z",
    "isUsed": false,
    "usedAt": null,
    "createdAt": "2026-01-11T08:35:15Z"
  },
  "message": null
}
```

---

#### GET /api/pre-registrations/cpf/{cpf}

Busca pré-cadastros válidos por CPF.

**Acesso**: AdminOnly  
**Rate Limit**: admin (60/min)

**Path Parameters**:
- `cpf` (string): CPF do usuário

**Response** (200 OK):
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "cpf": "12345678901",
      "activationCode": "A1B2C3D4",
      "profile": 3,
      "expirationDate": "2026-01-18T08:35:15Z",
      "isUsed": false,
      "usedAt": null,
      "createdAt": "2026-01-11T08:35:15Z"
    }
  ],
  "message": null
}
```

---

#### POST /api/pre-registrations/{id}/regenerate

Regenera código de ativação (se não usado).

**Acesso**: AdminOnly  
**Rate Limit**: admin (60/min)

**Path Parameters**:
- `id` (UUID): ID do pré-cadastro

**Response** (200 OK):
```json
{
  "isSuccess": true,
  "message": "Operação realizada com sucesso"
}
```

---

### Users

#### POST /api/users/complete-registration

Completa o cadastro usando código de ativação.

**Acesso**: Anônimo  
**Rate Limit**: public (10/min)

**Request Body**:
```json
{
  "cpf": "12345678901",
  "activationCode": "A1B2C3D4",
  "password": "SenhaSegura123",
  "confirmPassword": "SenhaSegura123",
  "fullName": "João Silva",
  "birthDate": "1995-05-15",
  "weight": 85.5,
  "height": 180,
  "phone": "11987654321",
  "email": "joao.silva@email.com",
  "emergencyContactName": "Maria Silva",
  "emergencyContactPhone": "11912345678"
}
```

**Response** (201 Created):
```json
{
  "isSuccess": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "cpf": "12345678901",
    "fullName": "João Silva",
    "email": "joao.silva@email.com",
    "phone": "11987654321",
    "birthDate": "1995-05-15",
    "weight": 85.5,
    "height": 180,
    "profile": 3,
    "status": 2,  // AwaitingActivation
    "createdAt": "2026-01-11T08:35:15Z",
    "activatedAt": null,
    "emergencyContact": {
      "name": "Maria Silva",
      "phone": "11912345678"
    },
    "requiresPasswordChange": false
  },
  "message": null
}
```

**Errors**:
- `400 Bad Request`: Código inválido, expirado, ou email já cadastrado

---

#### GET /api/users/{id}

Busca usuário por ID.

**Acesso**: Autenticado (próprio usuário ou admin)  
**Rate Limit**: authenticated (30/min)

**Path Parameters**:
- `id` (UUID): ID do usuário

**Response** (200 OK):
```json
{
  "isSuccess": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "cpf": "12345678901",
    "fullName": "João Silva",
    "email": "joao.silva@email.com",
    "phone": "11987654321",
    "birthDate": "1995-05-15",
    "weight": 85.5,
    "height": 180,
    "profile": 3,
    "status": 3,
    "createdAt": "2026-01-11T08:35:15Z",
    "activatedAt": "2026-01-11T09:00:00Z",
    "emergencyContact": {
      "name": "Maria Silva",
      "phone": "11912345678"
    },
    "requiresPasswordChange": false
  },
  "message": null
}
```

**Errors**:
- `403 Forbidden`: Usuário não tem permissão de acessar
- `404 Not Found`: Usuário não encontrado

---

#### GET /api/users/cpf/{cpf}

Busca usuário por CPF.

**Acesso**: AdminOnly  
**Rate Limit**: authenticated (30/min)

**Path Parameters**:
- `cpf` (string): CPF do usuário

**Response**: Igual ao GET por ID

---

#### GET /api/users/status/{status}

Lista usuários por status.

**Acesso**: AdminOnly  
**Rate Limit**: admin (60/min)

**Path Parameters**:
- `status` (int): 1=PendingRegistration, 2=AwaitingActivation, 3=Active, 4=Rejected, 5=Inactive

**Response** (200 OK):
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "fullName": "João Silva",
      "email": "joao.silva@email.com",
      ...
    }
  ],
  "message": null
}
```

---

#### PUT /api/users/{id}

Atualiza informações do usuário.

**Acesso**: Autenticado (próprio usuário ou admin)  
**Rate Limit**: authenticated (30/min)

**Path Parameters**:
- `id` (UUID): ID do usuário

**Request Body**:
```json
{
  "fullName": "João Silva Santos",
  "birthDate": "1995-05-15",
  "weight": 87.0,
  "height": 180,
  "phone": "11987654321",
  "email": "joao.silva@email.com",
  "emergencyContactName": "Maria Silva",
  "emergencyContactPhone": "11912345678"
}
```

**Response** (200 OK): Retorna dados atualizados do usuário

**Errors**:
- `400 Bad Request`: Email já cadastrado por outro usuário
- `403 Forbidden`: Sem permissão

---

#### POST /api/users/{id}/activate

Ativa um usuário (muda status para Active).

**Acesso**: AdminOnly  
**Rate Limit**: admin (60/min)

**Path Parameters**:
- `id` (UUID): ID do usuário

**Response** (200 OK):
```json
{
  "isSuccess": true,
  "message": "Operação realizada com sucesso"
}
```

**Errors**:
- `400 Bad Request`: Usuário não está aguardando ativação

---

#### POST /api/users/{id}/deactivate

Desativa um usuário (muda status para Inactive).

**Acesso**: AdminOnly  
**Rate Limit**: admin (60/min)

**Path Parameters**:
- `id` (UUID): ID do usuário

**Response** (200 OK):
```json
{
  "isSuccess": true,
  "message": "Operação realizada com sucesso"
}
```

---

#### DELETE /api/users/{id}

Deleta um usuário (soft delete).

**Acesso**: AdminOnly  
**Rate Limit**: admin (60/min)

**Path Parameters**:
- `id` (UUID): ID do usuário

**Response** (204 No Content)

---

## Códigos de Status HTTP

- `200 OK`: Requisição bem-sucedida
- `201 Created`: Recurso criado com sucesso
- `204 No Content`: Operação bem-sucedida sem corpo de resposta
- `400 Bad Request`: Erro de validação ou regra de negócio
- `401 Unauthorized`: Não autenticado ou token inválido
- `403 Forbidden`: Autenticado mas sem permissão
- `404 Not Found`: Recurso não encontrado
- `429 Too Many Requests`: Rate limit excedido
- `500 Internal Server Error`: Erro interno do servidor

## Validações

### CPF
- Deve conter 11 dígitos numéricos
- Validação de dígitos verificadores
- Formato aceito: apenas números ou formatado (###.###.###-##)

### Senha
- Mínimo 8 caracteres
- Deve conter letras e números (recomendado)

### Email
- Formato válido de email
- Único no sistema

### Outros Campos
- **FullName**: Máximo 200 caracteres
- **Phone**: Máximo 20 caracteres
- **Weight**: Decimal com 2 casas (máx 999.99)
- **Height**: Inteiro (em centímetros)

## Exemplo de Fluxo Completo

### 1. Admin cria pré-cadastro

```bash
POST /api/pre-registrations
Authorization: Bearer {admin_token}

{
  "cpf": "12345678901",
  "profile": 3
}

# Response: activationCode = "A1B2C3D4"
```

### 2. Usuário completa cadastro

```bash
POST /api/users/complete-registration

{
  "cpf": "12345678901",
  "activationCode": "A1B2C3D4",
  "password": "MinhaSenh@123",
  "confirmPassword": "MinhaSenh@123",
  "fullName": "João Silva",
  ...
}

# Status agora é AwaitingActivation
```

### 3. Admin ativa usuário

```bash
POST /api/users/{userId}/activate
Authorization: Bearer {admin_token}

# Status agora é Active
```

### 4. Usuário faz login

```bash
POST /api/auth/login

{
  "login": "12345678901",
  "password": "MinhaSenh@123"
}

# Response: token JWT
```

### 5. Usuário acessa seus dados

```bash
GET /api/users/{userId}
Authorization: Bearer {user_token}
```

## Ambientes

### Development
- URL: `https://localhost:5001`
- Swagger: Habilitado
- CORS: Permitido para qualquer origem

### Production
- URL: Configurar conforme deploy
- Swagger: Desabilitado (recomendado)
- CORS: Configurar origens específicas
- HTTPS: Obrigatório
- Usar secrets manager para configurações sensíveis

## Segurança

- Todas as senhas são hasheadas com BCrypt
- Tokens JWT com assinatura HMAC-SHA256
- Rate limiting por IP
- CORS configurável
- Validação de entrada em todos os endpoints
- Soft delete para auditoria
- HTTPS obrigatório em produção

## Versionamento

Versão atual: **v1.0.0**

A API não possui versionamento explícito na URL. Mudanças breaking serão comunicadas e uma estratégia de versionamento será implementada conforme necessário.

## Suporte

Para dúvidas ou problemas com a API, consulte:
- Documentação completa: `/swagger`
- Arquitetura: `docs/ARCHITECTURE.md`
- Banco de dados: `docs/DATABASE.md`
