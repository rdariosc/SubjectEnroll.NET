# Arquitectura del Proyecto — EducationInstitution

Este documento describe la arquitectura técnica del sistema de inscripción académica en línea desarrollado con .NET 8, SQL Server Express y frontend HTML5 + Bootstrap.

---

## Estructura por capas

El proyecto sigue una arquitectura en capas, separando responsabilidades de forma clara:

EducationInstitution.sln 
├── RegistrationSubjects.Api/ → Capa de presentación (API REST) 
│ ├── Controllers/ → Endpoints HTTP 
│ ├── Services/ → Lógica de negocio 
│ ├── Middlewares/ → Manejo centralizado de errores 
│ └── Program.cs, appsettings.json → Configuración de la aplicación 
├── RegistrationSubjects.Core/ → Capa de dominio 
│ ├── Entities/ → Modelos de datos 
│ ├── DTOs/ → Objetos de transferencia 
│ └── Interfaces/ → Contratos de servicios y repositorios 
├── RegistrationSubjects.Infrastructure/ → Capa de acceso a datos 
│ └── Contexts/ → EF Core DbContext 
├── RegistrationSubjects.Tests/ → Pruebas unitarias 
│ └── Services/ → Tests de lógica de negocio 
├── RegistrationSubjects.IntegrationTests/ → Pruebas de integración 
│ └── CustomWebApplicationFactory.cs 
├── Frontend/ → HTML + Bootstrap + JS 
│ ├── login.html, register.html, index.html 
│ └── images/, scripts/


---

## Patrones de diseño aplicados

- **Repository Pattern (implícito)**: acceso a datos desacoplado mediante interfaces
- **Dependency Injection**: servicios registrados y consumidos vía constructor
- **DTOs (Data Transfer Objects)**: separación entre entidades y datos expuestos
- **Middleware personalizado**: manejo centralizado de excepciones con Serilog
- **Factory para pruebas**: `CustomWebApplicationFactory` para testeo aislado
- **JWT Authentication**: autenticación basada en tokens para proteger endpoints

---

## Flujo de datos

1. **Frontend**: HTML + JS envía datos vía `fetch` al backend
2. **API Controllers**: reciben solicitudes, validan modelos (`ModelState`)
3. **Services**: aplican reglas de negocio (créditos, profesores, materias)
4. **DbContext (EF Core)**: accede a SQL Server con consultas parametrizadas
5. **Middleware**: captura errores y los registra con Serilog
6. **Respuesta**: se envía al frontend con status y contenido JSON

---

## Seguridad integrada

- Autenticación JWT con validación de token en cada endpoint
- Hashing de contraseñas con BCrypt
- Validación de entrada en cliente y servidor
- Protección contra XSS y CSRF (sin cookies, uso de headers)
- Consultas seguras con EF Core (`FromSqlInterpolated`)

---

## Estrategia de pruebas

- **Unitarias**: validan lógica de negocio en servicios
- **Integración**: simulan flujo completo con base de datos en memoria
- **Autenticación simulada**: `TestAuthHandler` para pruebas protegidas
- **Validación de contenido**: no solo status, también estructura de respuesta

---

## Optimización y rendimiento

- Carga diferida de imágenes (`loading="lazy"`)
- Minimización de llamadas innecesarias en frontend
- Uso de `async/await` en toda la API
- Separación de responsabilidades para facilitar mantenimiento

---

## Extensibilidad

- Fácil de extender con nuevos endpoints, reglas o entidades
- Documentado con Swagger para exploración de API
- Frontend modular y responsivo, adaptable



