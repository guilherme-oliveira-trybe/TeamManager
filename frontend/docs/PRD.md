# Product Requirements Document (PRD) - GFA Team Manager Frontend

**Version:** 1.0
**Date:** 27/01/2026
**Status:** In Development

## 1. Introduction
The **GFA Team Manager Frontend** is a web-based administrative portal for managing an American Football team. It serves as the primary interface for Administrators, Coaches, and Staff to manage players (Athletes), monitor activities, and handle operational tasks.

### 1.1 Purpose
To provide a secure, responsive, and intuitive interface for:
- Managing user accounts (Athletes, Staff, Admins).
- Controlling access via role-based permissions.
- Tracking team activities and player engagement.
- Streamlining the onboarding process via pre-registration codes.

### 1.2 Scope
This document covers the **Frontend** application built with Next.js. It includes Authentication, Dashboard, User Management, and Pre-Registration modules.

---

## 2. User Personas & Roles

| Role | Access Level | Responsibilities |
| :--- | :--- | :--- |
| **Admin** | Full Access | Manage users, settings, system configuration, all data. |
| **Coach** | High Access | Manage training, view athlete data, attendance. |
| **Staff** | Medium Access | Operational tasks, specific department view. |
| **Athlete** | Restricted | View own profile, schedule, and team announcements. |

> **Note:** Initial implementation focuses heavily on **Admin** management capabilities and **Athlete** profiles.

---

## 3. Functional Requirements

### 3.1 Authentication & Security
- **Login:** Email/Password authentication using JWT.
- **Forgot Password:** Self-service password reset flow via email.
- **Role-Based Access Control (RBAC):** UI elements must adapt based on the user's `ProfileType` (Admin, Coach, Staff, Athlete).
- **Session Management:** Auto-logout on token expiration; Secure storage of tokens (HttpOnly cookies preferred or secure local handling).

### 3.2 Dashboard (Home)
- **Overview Cards:** Display key metrics (Total Users, Pending Actions, Active Players).
- **Recent Activity:** Feed of latest system events (new registrations, updates).
- **Quick Actions:** Shortcuts to common tasks (e.g., "New Pre-Registration").
- **Admin Specifics:** Only Admins see full system stats; Athletes see personal stats.

### 3.3 User Management (`/dashboard/users`)
- **User List:**
  - Paginated table displaying users.
  - Columns: Name, Email, Role, Unit, Status, Actions.
  - **Searching:** Text search by name or email.
  - **Filtering:** Filter by User Status (Active, Inactive, Pending) and Role.
- **User Details:**
  - View comprehensive profile (Personal Info, Contact, Emergency Contact, Physical Stats).
  - **Emergency Contacts:** Name and Phone number.
  - **Physical Stats:** Weight and Height.
- **Actions:**
  - **Approve/Activate:** Activate pending users.
  - **Deactivate:** Ban or disable access for a user.
  - **Delete:** Permanently remove a user (Admin only).
  - **Edit:** Update profile information.

### 3.4 Pre-Registration (`/dashboard/pre-registrations`)
- **Concept:** Admins generate a code that allows a person to sign up and link themselves to a specific role/profile immediately.
- **Create Pre-Registration:**
  - Admin selects Role (e.g., Athlete), Unit (e.g., Offense), and Position (e.g., WR).
  - System generates a unique alpha-numeric code.
- **Manage Codes:**
  - List valid and used codes.
  - **Regenerate:** Create a new code if the previous one expired or was lost.
  - **Revoke:** Cancel a pre-registration code.

### 3.5 Units & Positions (American Football)
The system strictly follows American Football structure:
- **Units:**
  - **Offense** (Ataque)
  - **Defense** (Defesa)
- **Positions:**
  - **Offense:** QB, RB, WR, OL
  - **Defense:** DL, LB, DB

---

## 4. Technical Specifications

### 4.1 Tech Stack
- **Framework:** Next.js 15+ (App Router)
- **Language:** TypeScript
- **Styling:** Tailwind CSS (v4) + Shadcn/UI (Radix Primitives)
- **State Management:** React Query (TanStack Query) for server state; React Context for global UI state.
- **Form Handling:** React Hook Form + Zod (Validation).
- **HTTP Client:** Axios (with interceptors for Auth).

### 4.2 API Integration
- **Base URL:** `/api` (Proxied to Backend)
- **Standard Headers:** 
  - `Authorization: Bearer <token>`
  - `Content-Type: application/json`
- **Pagination:** Uses `X-Pagination` header for metadata (TotalCount, PageSize, etc.).

### 4.3 UI/UX Guidelines
- **Responsive Design:** Mobile-first approach. All dashboards must be usable on phones.
- **Feedback:** Use Toasts (`react-toastify`) for success/error messages.
- **Loading States:** Skeletons for table loading; Spinners for button actions.
- **Theme:** Dark/Light mode support (System default).

---

## 5. Future Roadmap
- **Team Management:** Roster views, Depth Charts.
- **Practice Schedule:** Calendar integration for trainings.
- **Financial:** Player dues and payment tracking.
- **Notifications:** Push notifications for announcements.
