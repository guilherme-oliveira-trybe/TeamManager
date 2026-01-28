# Product Requirements Document (PRD) - GFA Team Manager Backend

**Version:** 1.0
**Date:** 27/01/2026
**Status:** In Development

## 1. Introduction
The **GFA Team Manager Backend** is the core API service powering the GFA Team Manager platform. It provides the business logic, data persistence, and security layer for the administrative portal and future applications.

### 1.1 Purpose
To provide a robust, scalable, and secure RESTful API that handles:
- Authentication and Authorization.
- User data management and persistence.
- Complex business rules for team management (Units, Positions).
- Integration with external services (future).

### 1.2 Scope
This document covers the **Backend** API built with .NET 8. It follows Clean Architecture principles and serves as the single source of truth for the system.

---

## 2. Architecture & Tech Stack

### 2.1 Technical Stack
- **Framework:** .NET 8 (ASP.NET Core Web API)
- **Language:** C#
- **Database:** PostgreSQL
- **ORM:** Entity Framework Core (Code First)
- **Authentication:** JWT (JSON Web Tokens)
- **Documentation:** Swagger/OpenAPI
- **Testing:** xUnit, Moq, Testcontainers

### 2.2 Architectural Pattern
The project follows **Clean Architecture** (Onion Architecture) to ensure separation of concerns:
1.  **Domain:** Core entities, Enums, Interfaces, and Enterprise Logic. No external dependencies.
2.  **Application:** Use Cases, DTOs, Services Interfaces, Validators (FluentValidation).
3.  **Infrastructure:** Implementation of data access (EF Core), external services (Email, etc.), and Auth logic.
4.  **Api:** Controllers/Endpoints, Middleware, Configuration, Dependency Injection root.

---

## 3. Key Entities & Domain Models

### 3.1 User (`User`)
Represents any system user.
- **Properties:** Id, Name, Email, PasswordHash, Phone, CPF (Tax ID), ProfileType, Unit, Position, IsActive.
- **Enums:**
  - `ProfileType`: Admin, Coach, Staff, Athlete.
  - `PlayerUnit`: Offense, Defense.
  - `PlayerPosition`: QB, RB, WR, OL (Offense) | DL, LB, DB (Defense).

### 3.2 Pre-Registration (`PreRegistrationCode`)
Allows deferred account creation tied to a specific role.
- **Properties:** Id, Code (Unique), ProfileType, Unit, Position, IsActive, CreatedAt, ExpiresAt.
- **Logic:** Codes are single-use (conceptually) or valid until revoked.

### 3.3 Activity (`Activity`) - *Planned*
Represents training sessions, games, or meetings.
- **Properties:** Id, Title, Date, Description, Unit (Target Audience), Location.

---

## 4. Functional Requirements (API)

### 4.1 Authentication Module (`/api/auth`)
- **POST /login:** Validates credentials and returns JWT Access Token.
- **POST /forgot-password:** Initiates reset flow (sends email/code).
- **POST /reset-password:** Completes reset flow with token and new password.
- **POST /change-password:** Authenticated route for user to change their own password.

### 4.2 User Management Module (`/api/users`)
- **GET /:** List users with pagination, sorting, and filtering (by Status, Role, Name).
  - *Headers:* Returns `X-Pagination` metadata.
- **GET /{id}:** Get details of a specific user.
- **POST /:** Create a new user (usually via Pre-Registration completion, but Admin can create direct).
- **PUT /{id}:** Update user details.
- **PATCH /{id}/status:** Activate/Deactivate a user.
- **DELETE /{id}:** Soft or hard delete a user.

### 4.3 Pre-Registration Module (`/api/pre-registration`)
- **POST /:** Generate a new invite code.
  - *Input:* Role, Unit, Position.
- **GET /:** List active codes.
- **POST /validate:** Check if a code is valid (Public endpoint for signup form).
- **DELETE /{id}:** Revoke a code.

---

## 5. Security Requirements

### 5.1 Authorization Policies
- **AdminOnly:** Restricted to users with `ProfileType.Admin`.
- **CoachPlus:** Accessible by Admin and Coach.
- **Authenticated:** Accessible by any logged-in user.

### 5.2 Data Protection
- Passwords must be hashed using strong algorithms (currently BCrypt or similar standard via framework).
- Sensitive data (CPF, Phone) should be protected.
- API must enforce HTTPS in production.

---

## 6. API Contracts & Standards
- **Response Format:** JSON.
- **Error Handling:** Standardized error response (`ProblemDetails` or custom wrapper) containing `Status`, `Message`, and `Errors` (validation details).
- **Date/Time:** All date-times should be stored and transmitted in UTC.
