# Propuesta Técnica – API de Incidentes (ASP.NET Core 8)

Este proyecto implementa una API RESTful para gestionar **incidentes técnicos**, **categorías** y **comentarios**, aplicando buenas prácticas de arquitectura, patrones de diseño y herramientas modernas del ecosistema .NET.

---

## 🧱 Arquitectura del Proyecto

La solución está organizada en una arquitectura en capas para asegurar **claridad**, **mantenibilidad** y **facilidad de pruebas**:

<img width="1493" height="875" alt="dg" src="https://github.com/user-attachments/assets/84b0acc6-3044-4822-8078-80e6912ded30" />

```text
API Layer
Configurations Layer
Business Layer
Common Layer
DataAccess Layer
Database (SQLite)
```

### 🔷 API Layer (PropuestaTecnica.WebAPI)
- Controladores RESTful (`IncidentsController`, `CategoriesController`, `CommentsController`).
- Validaciones de modelo con Data Annotations.
- Documentación y prueba interactiva con **Swagger**.
- **Output Caching** en endpoints de lectura para mejorar rendimiento.
- Middleware global de excepciones que estandariza las respuestas de error en JSON.
- Configuración de **Serilog** para logging estructurado.

### 🔷 Configurations Layer (PropuestaTecnica.Configurations)
- Extensiones de `IServiceCollection` para registrar todos los servicios y dependencias.
- Configuración del **DbContext** (`AppDbContext`) usando el patrón **Options** (`DbOptions`).
- Registro de:
  - `AppDbContext`
  - `IUnitOfWork`
  - Repositorios
  - Servicios de negocio
  - Mapster (mapeos DTO ↔ entidades)
- Este proyecto centraliza la **inyección de dependencias (DI)** y evita duplicación de configuración.

### 🔷 Business Layer (PropuestaTecnica.Bussiness)
- Servicios como `IncidentService`, `CategoryService`, `CommentService`.
- Encapsulan reglas de negocio (por ejemplo, estado inicial de un incidente, validaciones de categoría, etc.).
- Usan **Unit of Work** para garantizar consistencia en operaciones de escritura.
- Trabajan únicamente con **DTOs**, no exponen entidades de EF Core a la capa API.

### 🔷 Common Layer (PropuestaTecnica.Common)
- Entidades de dominio: `Incident`, `Category`, `Comment`, `ApplicationUser`.
- DTOs para requests y responses.
- Enums (por ejemplo, `IncidentStatus`).
- Configuración de mapeos con **Mapster** (`MapsterConfig`).

### 🔷 DataAccess Layer (PropuestaTecnica.DataAccess)
- `AppDbContext` (hereda de `IdentityDbContext<ApplicationUser>`).
- Repositorios específicos (`IncidentRepository`, `CategoryRepository`, `CommentRepository`).
- Implementación de **Unit of Work** (`IUnitOfWork`, `UnitOfWork`).
- Seeder (`DbInitializer`) para poblar categorías iniciales y datos base.
- Uso de **Entity Framework Core** con **SQLite** como motor de base de datos local.

### 🔷 Database (SQLite)
- Base de datos liviana ideal para entorno local y pruebas.
- EF Core crea la base de datos y aplica migraciones automáticamente al iniciar la aplicación.
- Se crea un archivo `PropuestaTecnica.db` en la carpeta de salida del proyecto WebAPI.

---

## 🗄️ Base de Datos – Creación Automática (SQLite)

No se requiere instalar SQL Server ni otro motor adicional.

Al iniciar la API:

1. Se aplica `db.Database.Migrate()`.
2. Si la base no existe, EF Core la crea automáticamente.
3. `DbInitializer` ejecuta un seeder que:
   - Inserta categorías por defecto (Base de Datos, Redes, Software, Infraestructura, Hardware).
   - Garantiza que el sistema quede listo para registrar incidentes.

Ruta típica del archivo:

```text
PropuestaTecnica.WebAPI/PropuestaTecnica.db
```

---

# 🔐 Autenticación con Identity, JWT y Refresh Tokens

La API implementa un sistema completo de autenticación basado en **ASP.NET Core Identity** y **JWT**, permitiendo que cada incidente y comentario quede automáticamente asociado al usuario autenticado. Este módulo garantiza seguridad, trazabilidad y control de acceso en todos los endpoints protegidos con `[Authorize]`.

---

## 🧰 Componentes Principales

- **Identity (AspNetUsers, Roles, Claims)**  
  Maneja usuarios, contraseñas, roles y validaciones internas del framework.

- **JWT Access Tokens**  
  Tokens de corta duración utilizados para autenticar solicitudes a la API.

- **Refresh Tokens**  
  Permiten renovar el JWT sin necesidad de volver a iniciar sesión.

- **TokenService**  
  Servicio encargado de generar Access Tokens y Refresh Tokens.

- **Middleware de Autenticación**  
  Valida tokens, autentica usuarios y carga sus claims en cada request.

---

## 🔑 Flujo de Autenticación

### 🟦 Registro 
```http
POST /api/v1/auth/register
```  
Crea un nuevo usuario en la tabla `AspNetUsers`.

### 🟦 Login  
```http
POST /api/v1/auth/login
```    
Retorna:
- **Access Token (JWT)**
- **Refresh Token**, almacenado en la tabla `RefreshTokens`

### 🟦 Consumo de la API  
Toda solicitud protegida debe incluir:

```
Authorization: Bearer {access_token}
```

### 🟦 Renovación del Token  
```http
POST /api/v1/auth/refresh
```    
Retorna un nuevo Access Token y un nuevo Refresh Token.

### 🟦 Logout
```http
POST /api/v1/auth/logout
```    
Revoca el Refresh Token previamente emitido.

---

## 🧩 JWT y Claims

Cada JWT incluye claims esenciales:

```json
{
  "sub": "{userId}",
  "email": "{userEmail}"
}
```

En los controladores, el usuario autenticado se obtiene mediante:

```csharp
var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
```

Este `userId` se asigna automáticamente a:

- **Incidentes (`Incident.UserId`)**
- **Comentarios (`Comment.UserId`)**

➡️ **El cliente ya no envía manualmente el `UserId`**, lo que evita suplantación de identidad.

---

## 🗄️ Refresh Tokens

Los Refresh Tokens se almacenan en la base de datos:

```csharp
public class RefreshToken
{
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public string UserId { get; set; }
}
```

Con esto se logra:

- Revocar tokens de forma individual  
- Prevenir ataques de reutilización  
- Mantener sesiones seguras y persistentes  

---

## 🔒 Seguridad Aplicada

- Access Tokens de corta duración  
- Refresh Tokens con expiración extendida  
- Clave JWT protegida vía **Options Pattern (`JwtSettings`)**  
- Controladores protegidos con `[Authorize]`  
- Prohibido enviar `UserId` desde el cliente  
- Arquitectura **stateless**, lista para escalar horizontalmente  

---

## 🧪 Soporte en Pruebas Unitarias

Las pruebas unitarias simulan usuarios autenticados mediante Claims:

```csharp
var user = new ClaimsPrincipal(new ClaimsIdentity(
    new[] { new Claim(ClaimTypes.NameIdentifier, "test-user-123") },
    "mockAuth"
));
```

Esto permite probar controladores autenticados sin necesidad del middleware real.

---

Con este módulo de autenticación basado en Identity + JWT + Refresh Tokens, la API queda preparada para entornos empresariales donde la seguridad, trazabilidad y escalabilidad son fundamentales.

## ▶️ Cómo Ejecutar la API

### 1. Requisitos

- **.NET 8 SDK**
- **Visual Studio 2022** o **VS Code**
- (Opcional) **Docker Desktop** para usar Seq.

### 2. Clonar el repositorio

```bash
git clone https://github.com/tu-usuario/tu-repo.git
cd tu-repo
```

### 3. Restaurar paquetes

```bash
dotnet restore
```

### 4. Ejecutar la API

Desde la carpeta raíz del proyecto:

```bash
dotnet run --project PropuestaTecnica.WebAPI
```

o desde Visual Studio, seleccionando **PropuestaTecnica.WebAPI** como proyecto de inicio y presionando **F5**.

### 5. Probar la API en Swagger

Una vez levantada la API:

```text
https://localhost:7163/swagger
```

Desde allí se pueden probar todos los endpoints (incidentes, categorías, comentarios) sin necesidad de herramientas externas.

---

## 🔌 Endpoints Principales

### Incidentes

```http
GET    /api/v1/incidents
GET    /api/v1/incidents/{id}
POST   /api/v1/incidents
PUT    /api/v1/incidents/{id}
DELETE /api/v1/incidents/{id}
```

Soporta paginación por query string en `GET /api/v1/incidents` (por ejemplo: `?page=1&pageSize=10`).

### Categorías

```http
GET /api/v1/categories
```

Usado para poblar listas de selección en frontends.

### Comentarios

```http
POST /api/v1/comments/{incidentId}
GET  /api/v1/comments/{incidentId}
```

Permite registrar y consultar comentarios asociados a un incidente.

---

## 🧩 Patrones y Buenas Prácticas

- **Repository Pattern**  
  Aísla el acceso a datos y facilita cambios futuros (por ejemplo, cambiar SQLite por SQL Server).

- **Unit of Work**  
  Agrupa varias operaciones de repositorio bajo una misma transacción para garantizar consistencia.

- **DTO Pattern**  
  Evita exponer entidades de dominio directamente hacia la API; se usan DTOs para requests/responses.

- **Middleware global de excepciones**  
  Cualquier error no controlado se transforma en una respuesta JSON estándar, amigable para el consumidor.

- **Logging estructurado con Serilog**  
  Permite registrar información técnica detallada (consultas, errores, tiempos) en consola, archivo y opcionalmente Seq.

- **Output Caching**  
  Aplicado a endpoints de lectura (por ejemplo, listados de incidentes) para disminuir carga sobre la base de datos.

---

## 📊 Logging con Serilog y Seq (Opcional)

El proyecto está configurado con **Serilog**. Por defecto escribe en:

- Consola
- Archivo: `logs/log-.txt` (rotación diaria)

Además, incluye configuración opcional para enviar logs a **Seq**.

### 1. Activar Seq en el código

En `Program.cs`, dejar habilitada la línea:

```csharp
.WriteTo.Seq("http://localhost:5341")
```

Si no se desea usar Seq, se puede comentar esa línea sin afectar el resto de la app.

### 2. Levantar Seq con Docker Desktop

Con Docker Desktop instalado, ejecutar en PowerShell o terminal:

```bash
docker pull datalust/seq

docker run -d --name seq ^
  -e ACCEPT_EULA=Y ^
  -e SEQ_FIRSTRUN_ADMINPASSWORD=Admin123$ ^
  -p 5341:80 datalust/seq
```

Luego abrir:

```text
http://localhost:5341
```

Ahí se visualizarán los logs emitidos por la API (requests, errores, etc.) en tiempo real.

---
<img width="1919" height="921" alt="Seq" src="https://github.com/user-attachments/assets/b3f897cf-d0c8-4ef8-8e1b-9a98c273e412" />

<img width="1919" height="918" alt="Seq2" src="https://github.com/user-attachments/assets/cd33d007-e2db-4444-a45d-ee5a91c7f9ba" />

## 🚀 Futuras Mejoras / Roadmap

Algunas extensiones naturales de esta API en un contexto empresarial serían:

- Uso de **cache distribuido** (Redis) en lugar de MemoryCache/OutputCache local para escenarios con múltiples instancias.
- Aplicar **Polly** para resiliencia (reintentos, circuit breakers, timeouts en operaciones críticas).
- Health checks y métricas de observabilidad (Prometheus / OpenTelemetry).
- Pruebas de integración y de carga (k6, JMeter).
- Migrar SQLite a SQL Server o PostgreSQL en entornos productivos.

---

## ✅ Resumen

Esta solución:

- Implementa una API RESTful completa para incidentes, categorías y comentarios.
- Aplica arquitectura en capas, patrones de diseño y buenas prácticas de .NET.
- Crea y configura automáticamente una base de datos SQLite.
- Integra logging estructurado con Serilog y soporte opcional para Seq mediante Docker.
- Está preparada para escalar y extenderse sin reescribir la arquitectura base.
