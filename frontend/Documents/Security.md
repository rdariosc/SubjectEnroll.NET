# Seguridad — EducationInstitution

Este documento describe las medidas de seguridad implementadas en el sistema de inscripción académica en línea, desarrollado con .NET 8, SQL Server Express y frontend HTML5 + Bootstrap.

---

## utenticación

- **JWT (JSON Web Tokens)**: los usuarios reciben un token al iniciar sesión, que se incluye en el header `Authorization: Bearer <token>` en cada solicitud.
- **Protección de endpoints**: uso de `[Authorize]` en controladores para restringir acceso a usuarios autenticados.
- **Simulación de autenticación en pruebas**: `TestAuthHandler` permite validar endpoints protegidos en pruebas de integración.

---

## Hashing de contraseñas

- Contraseñas almacenadas con **BCrypt**, que aplica salting automático y múltiples rondas de hashing.
- Verificación segura con `BCrypt.Verify` en el login.

---

## Validación de entrada

### En el servidor (.NET)

- Uso de `ModelState.IsValid` en controladores
- Atributos como `[Required]`, `[EmailAddress]`, `[MinLength]` en DTOs
- Rechazo de solicitudes malformadas con `400 Bad Request`

---

## Protección contra inyecciones SQL

- Uso exclusivo de **Entity Framework Core**, que parametriza todas las consultas
- Consultas manuales protegidas con `FromSqlInterpolated`

---

## Protección contra XSS (Cross-Site Scripting)

- Escapado de contenido dinámico en frontend (`textContent`, `innerText`)
- Evita `innerHTML` sin sanitización
- No se renderiza HTML desde datos del backend

---

## Protección contra CSRF (Cross-Site Request Forgery)

- Uso de JWT en headers, no en cookies → no vulnerable a CSRF
- Todas las solicitudes sensibles requieren autenticación

---

## Seguridad en tránsito

- Uso de `UseHttpsRedirection()` en backend
- Recomendación de HTTPS en producción

---

## Buenas prácticas adicionales

- Middleware de errores (`ExceptionMiddleware.cs`) para evitar fugas de stack traces
- Logging seguro con Serilog (sin exponer datos sensibles)
- Índices únicos en base de datos para evitar duplicados (`Email`)
- Validación de reglas de negocio en servicios (créditos, profesores, materias)

---

## Validación en pruebas

- Pruebas de integración simulan autenticación
- Validación de respuestas con contenido, no solo status
- Pruebas de inscripción con reglas de seguridad activas

