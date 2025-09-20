# Frontend — EducationInstitution

Este documento describe la estructura, comportamiento y medidas de seguridad aplicadas en el cliente HTML del sistema de inscripción académica. El frontend está construido con HTML5, Bootstrap 5 y JavaScript puro, sin frameworks adicionales.

---

## Estructura de archivos

frontend/ 
├── index.html → Dashboard principal 
├── login.html → Formulario de inicio de sesión 
├── register.html → Formulario de registro 
├── images/ → Recursos visuales (logos, ilustraciones)


---

## Componentes principales

### `login.html`

- Formulario con campos de correo y contraseña
- Validación visual con Bootstrap (`required`, `minlength`)
- Envío de datos vía `fetch` a `POST /api/auth/login`
- Almacena `token` y `studentId` en `localStorage`
- Redirige a `index.html` tras login exitoso

### `register.html`

- Formulario con nombre, apellido, correo y contraseña
- Validación de entrada en cliente
- Envío de datos a `POST /api/auth/register`
- Almacena token y redirige al dashboard

### `index.html`

- Navbar con botón de logout
- Botones para inscribir materias, ver inscritas y ver compañeros
- Contenedor dinámico (`#contentArea`) que se actualiza según la acción
- Llamadas a la API protegidas con JWT en el header
- Validación de reglas académicas y visualización de respuestas

---

## Diseño responsivo

- Uso de Bootstrap 5 para adaptabilidad en móviles, tablets y escritorio
- Componentes como `card`, `form-control`, `btn`, `list-group` para consistencia visual
- Layout centrado verticalmente (`place-items: center`) en login y registro

---

## Seguridad en el cliente

- Validación de formularios con `required`, `type="email"`, `minlength`
- Prevención de envío si el formulario no es válido (`checkValidity`)
- Escapado de contenido dinámico para evitar XSS (`textContent`, no `innerHTML`)
- Uso de `localStorage` para guardar token (no cookies → evita CSRF)
- Redirección automática si no hay token (`location.href = "/login.html"`)

---

## Optimización visual

- Carga diferida de imágenes con `loading="lazy"` en `<img>`
- Uso de imágenes comprimidas y adaptadas a pantalla

---

## Comunicación con la API

- Todas las llamadas se hacen con `fetch` y `Authorization: Bearer <token>`
- Manejo de errores con `try/catch` y mensajes visuales (`alert`)
- Ejemplo de inscripción:

```javascript
const res = await fetch(`${API_BASE}/api/enrollments`, {
  method: "POST",
  headers: {
    "Content-Type": "application/json",
    Authorization: "Bearer " + token
  },
  body: JSON.stringify({ studentId, subjectIds: ids })
});
