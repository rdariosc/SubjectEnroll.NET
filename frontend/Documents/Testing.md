# Estrategia de Pruebas — EducationInstitution

Este documento describe las pruebas implementadas en el sistema de inscripción académica, incluyendo pruebas unitarias, de integración, simulación de autenticación y validación de reglas de negocio.

---

## Herramientas utilizadas

- **xUnit**: framework de pruebas unitarias
- **Microsoft.AspNetCore.Mvc.Testing**: pruebas de integración con servidor embebido
- **EF Core InMemory**: base de datos en memoria para pruebas
- **TestAuthHandler**: autenticación simulada para endpoints protegidos
- **CustomWebApplicationFactory**: configuración personalizada del entorno de pruebas

---

## Tipos de pruebas

### 1. Pruebas unitarias

Ubicadas en `RegistrationSubjects.Tests/Services/EnrollmentServiceTests.cs`

- Validan la lógica de negocio sin depender de la base de datos ni del servidor HTTP
- Casos cubiertos:
  - Inscripción válida (3 materias, sin repetir profesor)
  - Rechazo por exceso de materias
  - Rechazo por profesor duplicado
  

### 2. Pruebas de integración

Ubicadas en `RegistrationSubjects.IntegrationTests/EnrollmentIntegrationTests.cs`

- Simulan solicitudes HTTP reales contra la API
- Usan `CustomWebApplicationFactory` para levantar el entorno
- Casos cubiertos:
  - Registro y login de estudiante
  - Inscripción vía `POST /api/enrollments`
  - Visualización de materias inscritas
  - Visualización de compañeros compartidos

---

## Simulación de autenticación

- Se utiliza `TestAuthHandler.cs` para simular un usuario autenticado en pruebas
- Permite probar endpoints protegidos con `[Authorize]` sin necesidad de tokens reales
- Se configura en `CustomWebApplicationFactory`:

```csharp
builder.ConfigureTestServices(services =>
{
    services.AddAuthentication("Test")
        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
});
