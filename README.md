# GFA Team Manager

<div align="center">

### Plataforma de Gerenciamento de Time de Futebol Americano

[![.NET](https://img.shields.io/badge/.NET-8.0+-512BD4?style=flat&logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791?style=flat&logo=postgresql)](https://www.postgresql.org/)
[![Next.js](https://img.shields.io/badge/Next.js-PWA-000000?style=flat&logo=next.js)](https://nextjs.org/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

</div>

## 📋 Sobre o Projeto

O GFA Team Manager é uma plataforma completa para gerenciamento de times de futebol americano, desenvolvida para facilitar a administração de atletas, treinos, jogos e atividades do time. A plataforma é otimizada para uso mobile através de PWA (Progressive Web App), permitindo que atletas e staff acessem facilmente via dispositivos móveis.

### 🎯 Objetivos

- Gerenciar cadastro e controle de acesso de atletas, coaches, staff e administradores
- Controlar atividades do time (treinos, jogos, reuniões)
- Facilitar comunicação e organização da agenda do time
- Oferecer experiência mobile-first para até 120 usuários simultâneos

## 🏗️ Arquitetura

O projeto segue os princípios da **Clean Architecture**, organizado em camadas bem definidas:

```
┌─────────────────────────────────────────┐
│           GFATeamManager.Api            │  ← API Layer (Controllers/Endpoints)
├─────────────────────────────────────────┤
│       GFATeamManager.Application        │  ← Application Layer (Services/DTOs)
├─────────────────────────────────────────┤
│         GFATeamManager.Domain           │  ← Domain Layer (Entities/Interfaces)
├─────────────────────────────────────────┤
│      GFATeamManager.Infrastructure      │  ← Infrastructure Layer (Data/Repositories)
└─────────────────────────────────────────┘
```

Para mais detalhes, consulte [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)

## 🚀 Tecnologias

### Backend
- **.NET 8+** - Framework principal
- **PostgreSQL 16** - Banco de dados
- **Entity Framework Core** - ORM
- **BCrypt.NET** - Hashing de senhas
- **JWT** - Autenticação
- **FluentValidation** - Validação de dados
- **Swagger** - Documentação da API

### Frontend (Planejado)
- **Next.js** - Framework React
- **TypeScript** - Linguagem
- **Tailwind CSS** - Estilização
- **PWA** - Progressive Web App

## 📦 Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download) ou superior
- [Docker](https://www.docker.com/) e Docker Compose
- [Node.js 18+](https://nodejs.org/) (para o frontend, quando implementado)

## 🔧 Configuração e Instalação

### 1. Clone o repositório

```bash
git clone <repository-url>
cd GFATeamManager
```

### 2. Configure o banco de dados

Inicie o PostgreSQL usando Docker:

```bash
docker-compose up -d
```

Isso criará um container PostgreSQL rodando em `localhost:5432` com:
- **Usuário**: postgres
- **Senha**: Dev@123456
- **Database**: gfateammanager

### 3. Configure as variáveis de ambiente

No arquivo `src/GFATeamManager.Api/appsettings.Development.json`, ajuste as configurações se necessário.

### 4. Execute as migrations

As migrations são executadas automaticamente ao iniciar a aplicação. Um usuário admin padrão será criado:

- **CPF**: 12345678901
- **Senha**: Admin@123

### 5. Execute a aplicação

```bash
cd src/GFATeamManager.Api
dotnet run
```

A API estará disponível em:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger: `https://localhost:5001/swagger`

## 📚 Documentação

- **[Arquitetura](docs/ARCHITECTURE.md)** - Detalhes da arquitetura e estrutura do projeto
- **[Banco de Dados](docs/DATABASE.md)** - Schema e relacionamentos do banco de dados
- **[API](docs/API.md)** - Documentação dos endpoints da API

## 🧪 Testes

O projeto possui cobertura completa de testes unitários e de integração:

```bash
# Executar todos os testes
dotnet test

# Executar testes com cobertura
dotnet test /p:CollectCoverage=true
```

Os testes estão organizados em:
- `GFATeamManager.Domain.Tests` - Testes unitários das entidades
- `GFATeamManager.Application.Tests` - Testes unitários dos serviços
- `GFATeamManager.Api.Tests` - Testes de integração dos endpoints
- `GFATeamManager.Infrastructure.Tests` - Testes de integração dos repositórios

## 🔐 Segurança

- Senhas são hasheadas usando **BCrypt**
- Autenticação via **JWT tokens**
- Rate limiting configurado
- CORS configurado para origens permitidas
- Soft delete para preservação de dados
- Validação de dados com FluentValidation

## 📖 Funcionalidades Implementadas

### Controle de Acesso

#### Pré-cadastro
- Admins podem criar pré-cadastros com CPF e perfil (Admin, Coach, Athlete, Staff)
- Sistema gera código de ativação de 8 caracteres
- Códigos expiram em 7 dias
- Códigos podem ser regenerados se não utilizados

#### Registro de Usuários
- Usuários completam cadastro usando CPF e código de ativação
- Informações coletadas: nome completo, data de nascimento, peso, altura, telefone, email
- Contato de emergência obrigatório
- Senhas seguem regras de segurança (mínimo 8 caracteres)
- Status inicial: "Aguardando Ativação"

#### Ativação de Usuários
- Admins podem ativar ou rejeitar usuários pendentes
- Apenas usuários ativos podem fazer login
- Sistema registra quem ativou e quando

#### Autenticação
- Login com CPF ou email
- JWT token com validade de 8 horas
- Suporte a senha temporária para reset

#### Reset de Senha
- Usuários podem solicitar reset de senha
- Admins aprovam solicitações e sistema gera senha temporária
- Usuário pode trocar senha usando a temporária

#### Gerenciamento de Usuários
- Admins podem listar usuários por status
- Atualização de informações do usuário
- Desativação de usuários
- Exclusão lógica (soft delete)

## 🗺️ Roadmap

- [x] Sistema de controle de acesso
- [x] Autenticação JWT
- [x] Gerenciamento de usuários
- [ ] Gerenciamento de atividades (treinos, jogos, reuniões)
- [ ] Frontend Next.js com PWA
- [ ] Notificações
- [ ] Relatórios e estatísticas

## 🤝 Contribuindo

1. Faça fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📝 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 📞 Contato

Para dúvidas ou sugestões, entre em contato com a equipe de desenvolvimento.

---

<div align="center">
Desenvolvido com ❤️ para a comunidade de Futebol Americano
</div>
