# EducationInstitution 📚

Sistema de inscripción académica en línea para estudiantes bajo un programa de créditos. Permite autenticación, inscripción a materias, visualización de compañeros y validación de reglas académicas.

---

## Características

- Registro y autenticación de estudiantes
- Inscripción a materias con reglas de negocio:
  - Máximo 3 materias por estudiante
  - No repetir profesor en más de una materia
- Visualización de compañeros por materia
- API RESTful con Swagger
- Validación en cliente y servidor
- Seguridad contra XSS, CSRF, y SQL Injection
- Optimización de imágenes con carga diferida (lazy loading)
- Manejo centralizado de errores con Serilog y middleware
- Pruebas unitarias y de integración con xUnit

---

## Arquitectura

Proyecto basado en .NET 8 con patrón de capas:

RegistrationSubjects.Api/ 
    ├── Controllers/ 
    ├── Services/ 
RegistrationSubjects.Core/ 
    ├── Entities/ 
    ├── DTOs/ 
    ├── Interfaces/ 
RegistrationSubjects.Infrastructure/ 
    ├── Contexts/ 
RegistrationSubjects.Tests/ 
    ├── Services/ 
RegistrationSubjects.IntegrationTests/ 
    ├── CustomWebApplicationFactory.cs

- **Api**: Controladores y servicios de negocio
- **Core**: Entidades, DTOs y contratos
- **Infrastructure**: Acceso a datos con EF Core
- **Tests**: Pruebas unitarias y de integración

---

## Tecnologías

- .NET 8 + ASP.NET Core
- SQL Server Express
- Visual Studio Code
- HTML5 + Bootstrap 5
- Serilog
- Swagger
-

---

## Seguridad

- **Autenticación JWT** con protección de endpoints vía `[Authorize]`
- **Hashing de contraseñas** con BCrypt
- **Validación de entrada** con `ModelState` y anotaciones
- **Protección contra CSRF**: uso de JWT en headers, sin cookies
- **Consultas parametrizadas** con EF Core 

---

## Pruebas

- **Unitarias**: `EnrollmentServiceTests.cs`
- **Integración**: `EnrollmentIntegrationTests.cs` con `CustomWebApplicationFactory`
- Simulación de autenticación con `TestAuthHandler`
- Validación de respuestas, códigos y contenido

---

## Frontend

- HTML5 + Bootstrap 5
- Formularios de login, registro, inscripción y visualización
- Carga diferida de imágenes (`loading="lazy"`)
- Validación con `required`, `minlength`, y feedback visual
- Comunicación con API vía `fetch` + JWT

---

## Instalación y ejecución

### Requisitos

- .NET 8 SDK
- Python 3 (para servidor frontend)
- SQL Server Express

### Comandos

```bash
# Backend
cd src/RegistrationSubjects.Api
dotnet run

# Frontend
cd frontend
python -m http.server 5500

# Pruebas
dotnet test
