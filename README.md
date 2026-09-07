# Funeral Management Backend

> **Status: Work in Progress**

Backend API for a funeral management application designed to support the internal organization and management of funeral cases.

The project is currently under active development. The initial focus is on establishing the backend architecture, database model, and core case management functionality.

## Tech Stack

- ASP.NET Core Web API
- C#
- Entity Framework Core
- PostgreSQL
- Docker / Docker Compose
- pgAdmin
- OpenAPI / Swagger

## Project Goals

The backend is intended to provide a central API for managing funeral cases and related information.

Planned functionality includes:

- User authentication and authorization
- Role-based access control
- Funeral case management
- Contact management
- Location management
- Document management
- Tasks and appointments
- Reminders
- Audit logging
- Document storage
- Search and filtering

Additional functionality may be added in later development phases.

## Current Development Status

The project is currently in its early development stage.

Currently implemented / being developed:

- ASP.NET Core Web API project structure
- PostgreSQL database connection
- Entity Framework Core integration
- Docker-based PostgreSQL development environment
- pgAdmin development environment
- Initial database model
- User entity
- Basic user API
- Swagger / OpenAPI integration

The database schema and API structure are still subject to change.

## Architecture

The backend currently follows a simple layered structure:

```text
HTTP Request
     │
     ▼
Controller
     │
     ▼
Service Layer
     │
     ▼
Entity Framework Core
     │
     ▼
PostgreSQL
