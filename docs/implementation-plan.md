# Implementation plan

Làm tuần tự. Đánh `[x]` khi phase xong.

## Phase 0 — Skeleton

- [ ] `dotnet new sln -n StayBill`
- [ ] `dotnet new webapi -n StayBill.Api -o src/StayBill.Api --use-controllers`
- [ ] `dotnet new xunit -n StayBill.Api.Tests -o tests/StayBill.Api.Tests`
- [ ] Add project to sln; test project reference API
- [ ] `docker-compose.yml`: Postgres 16 + Redis 7, volume, healthcheck
- [ ] `.env.example`: `ConnectionStrings__Default`, `ConnectionStrings__Redis`, `Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience`
- [ ] `appsettings.json` + bind env
- [ ] Swagger bật ở Development
- [ ] `GET /health` → 200
- [ ] Cập nhật README lệnh chạy thật

Commit gợi ý: `chore: scaffold API, tests, and docker compose`

## Phase 1 — Domain + EF Core

- [ ] Entity: `User`, `Room`, `Tenant`, `Contract`, `Invoice` đúng `docs/domain.md`
- [ ] Enum: `RoomStatus`, `ContractStatus`, `InvoiceStatus`
- [ ] `StayBillDbContext` + Fluent API (index unique `Room.Code`, FK, precision decimal)
- [ ] `dotnet ef migrations add Init`
- [ ] `dotnet ef database update` chạy được với Compose

Commit gợi ý: `feat: add domain entities and EF Core initial migration`

## Phase 2 — JWT auth

- [ ] `POST /api/auth/register` — hash password (ASP.NET Identity hasher hoặc BCrypt)
- [ ] `POST /api/auth/login` — trả `{ accessToken, expiresAt }`
- [ ] `[Authorize]` default cho controller nghiệp vụ
- [ ] Allow anonymous: register, login, health, swagger
- [ ] 401 khi thiếu/sai token

Commit gợi ý: `feat: add JWT register and login`

## Phase 3 — Rooms + Redis cache

- [ ] CRUD `/api/rooms`
- [ ] `GET /api/rooms/vacant` — chỉ `Vacant`; cache Redis key `rooms:vacant`, TTL 60s
- [ ] Invalidate cache khi create/update/delete room hoặc đổi status (hợp đồng phase 4 sẽ gọi cùng helper)
- [ ] `IRoomCache` wrapper, không gọi Redis rải trong controller

Commit gợi ý: `feat: room CRUD with Redis vacant-room cache`

## Phase 4 — Tenants

- [ ] CRUD `/api/tenants`
- [ ] Validate phone / CMND không rỗng khi create
- [ ] 404 khi id không tồn tại

Commit gợi ý: `feat: tenant CRUD`

## Phase 5 — Contracts + occupancy

- [ ] `POST /api/contracts` — room phải `Vacant` và không có contract `Active` khác → nếu không: `409`
- [ ] Sau create thành công: room → `Occupied`, invalidate vacant cache
- [ ] `GET /api/contracts`, `GET /api/contracts/{id}`
- [ ] `POST /api/contracts/{id}/end` — status `Ended`, `EndedAt = UtcNow`, room → `Vacant`, invalidate cache
- [ ] Không cho end contract đã `Ended` (`409`)

Commit gợi ý: `feat: contracts with room occupancy rules`

## Phase 6 — Invoices

- [ ] `POST /api/invoices` — gắn `ContractId` đang `Active`; period `yyyy-MM` unique theo contract
- [ ] Tính `Total` server-side (client không gửi total)
- [ ] `GET /api/invoices`, filter optional `?status=Unpaid`
- [ ] `POST /api/invoices/{id}/pay` → `Paid`, `PaidAt = UtcNow`
- [ ] Không pay hai lần (`409`)

Commit gợi ý: `feat: monthly invoices with pay flow`

## Phase 7 — Tests + polish

- [ ] Unit: occupancy rule (thuê phòng Occupied → fail)
- [ ] Unit: công thức `Total`
- [ ] Integration (optional): login + create room + vacant cache
- [ ] Seed Development: 1 user `admin@staybill.local` / `Admin123!`, 2 phòng, 1 khách (không seed production)
- [ ] Rà `docs/api.md` vs controller thật
- [ ] README “Chạy local” cuối cùng

Commit gợi ý: `test: occupancy and invoice total; add dev seed`

## Thứ tự không đổi

```
0 skeleton → 1 EF → 2 JWT → 3 rooms/redis → 4 tenants → 5 contracts → 6 invoices → 7 tests
```

Phase 5 phụ thuộc 3. Phase 6 phụ thuộc 5. Đừng làm invoice trước contract.
