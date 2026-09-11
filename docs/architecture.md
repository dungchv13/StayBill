# Architecture

## Nguyên tắc

Một process: ASP.NET Core 8 Web API. Layer bằng folder, không split nhiều class library.

```
HTTP → Controller → Service → DbContext / Redis
                 ↘ Domain entities
```

Controller không viết SQL, không đụng `IDatabase` Redis trực tiếp. Service chứa rule. Domain chỉ data + enum.

## Thư mục đích

```
src/StayBill.Api/
  Controllers/
    AuthController.cs
    RoomsController.cs
    TenantsController.cs
    ContractsController.cs
    InvoicesController.cs
    HealthController.cs
  Domain/
    User.cs
    Room.cs
    Tenant.cs
    Contract.cs
    Invoice.cs
    Enums.cs
  Data/
    StayBillDbContext.cs
    Configurations/          # IEntityTypeConfiguration
  Services/
    AuthService.cs
    RoomService.cs
    TenantService.cs
    ContractService.cs
    InvoiceService.cs
  Cache/
    IVacantRoomCache.cs
    RedisVacantRoomCache.cs
  Contracts/                 # request/response DTO
  Auth/
    JwtOptions.cs
    JwtTokenFactory.cs
  Program.cs
```

## Docker Compose

Hai service, không build API trong Compose ở MVP (chạy API bằng `dotnet run`):

| Service | Image | Port host |
|---|---|---|
| `postgres` | `postgres:16-alpine` | 5432 |
| `redis` | `redis:7-alpine` | 6379 |

DB: `staybill` / user `staybill` / password từ env.

API đọc:

- `ConnectionStrings:Default` — Npgsql
- `ConnectionStrings:Redis` — `localhost:6379`
- `Jwt:Key` (≥ 32 chars), `Jwt:Issuer`, `Jwt:Audience`, `Jwt:ExpiresMinutes` (mặc định 480)

## EF Core

- Provider: Npgsql
- Migrations trong `src/StayBill.Api/Data/Migrations`
- Lệnh (từ root repo):

```bash
dotnet ef migrations add Init --project src/StayBill.Api --output-dir Data/Migrations
dotnet ef database update --project src/StayBill.Api
```

## Redis

- Key: `rooms:vacant`
- Value: JSON danh sách DTO phòng trống (id, code, rentPrice, area)
- TTL: 60 seconds
- Invalidate: mọi chỗ đổi `Room` hoặc occupancy (create/update/delete room, create/end contract)

## Auth

- Bearer JWT, HMAC-SHA256
- Claim `sub` = user id
- Register lưu `PasswordHash` (không bao giờ trả hash ra API)

## Lỗi HTTP

| Code | Khi nào |
|---|---|
| 400 | Validation (FluentValidation **không bắt buộc**; dùng DataAnnotations + check tay) |
| 401 | Thiếu/sai JWT |
| 404 | Entity không tồn tại |
| 409 | Rule nghiệp vụ (phòng đang thuê, pay 2 lần, period trùng) |

Response lỗi thống nhất:

```json
{ "error": "ROOM_OCCUPIED", "message": "Room already has an active contract." }
```

## Logging

`ILogger<T>` mặc định. Không log password, không log full JWT.
