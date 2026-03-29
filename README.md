# ProToolRent
Платформа для аренды инструментов между пользователями.

## О проекте

`ProToolRent` — backend-проект на ASP.NET Core, построенный по принципам Clean Architecture.  
Сейчас реализована серверная часть, фронтенд — в разработке.

## Стек

- ASP.NET Core 8
- Entity Framework Core
- PostgreSQL
- MediatR
- FluentValidation
- JWT Authentication

## Основные возможности

- регистрация и авторизация пользователей
- refresh token через cookie
- роли пользователей: `Tenant`, `Landlord`, `Admin`
- профиль пользователя
- создание и управление инструментами
- заказы на аренду

## Архитектура

```text
src/
├─ ProToolRent.Domain
├─ ProToolRent.Application
├─ ProToolRent.Infrastructure
└─ ProToolRent.Api
```

- `Domain` — сущности и бизнес-правила
- `Application` — команды, запросы, интерфейсы
- `Infrastructure` — EF Core, JWT, хэширование, репозитории
- `Api` — контроллеры, middleware, Swagger

## Запуск проекта

### Требования

- .NET SDK 8
- PostgreSQL

### Настройка

 `appsettings.Development.json` :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ProToolRent;Username=postgres;Password=YOUR_PASSWORD"
  },
  "JwtOptions": {
    "SecretKey": "YOUR_SECRET_KEY",
    "ExpiryMinutes": 60,
    "Issuer": "ProToolRent",
    "Audience": "ProToolRentUsers"
  }
}
```

### Миграции

```bash
dotnet ef database update
```

### Запуск API

```bash
dotnet run --project src/ProToolRent.Api
```

`Swagger`:

```text
https://localhost:<port>/swagger
```

## Авторизация

Для защищённых endpoint’ов используется JWT access token.  
Refresh token хранится в `HttpOnly` cookie.

## API

### Auth
- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/logout`
- `POST /api/auth/refresh`

### Users
- `GET /api/users/{id}`
- `PUT /api/users/{id}`

### Tools
- `GET /api/tools`
- `POST /api/tools`
- `PUT /api/tools/{id}`
- `DELETE /api/tools/{id}`

### Orders
- `GET /api/orders`
- `POST /api/orders`
- `PUT /api/orders/{id}`
- `DELETE /api/orders/{id}`

## Статус

- Backend: в разработке
- Frontend: в разработке

## License

MIT
