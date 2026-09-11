# StayBill

API quản lý nhà trọ cho chủ nhà: phòng, khách thuê, hợp đồng và hóa đơn tháng.

Dự án cá nhân học **ASP.NET Core** — backend Web API, không có frontend.

## Stack

| Thành phần | Công nghệ |
|---|---|
| API | ASP.NET Core 8 Web API |
| ORM | EF Core 8 (Code First) |
| Database | PostgreSQL 16 |
| Cache | Redis |
| Auth | JWT |
| Chạy local | Docker Compose |

## Phạm vi (đúng CV)

- REST API: phòng (trống / đã thuê), khách thuê, hợp đồng thuê
- Hóa đơn tháng: tiền phòng + điện/nước; trạng thái đã thu / chưa thu
- Auth JWT; cache danh sách phòng trống trên Redis
- EF Core Code First; Docker Compose chạy local

Ngoài phạm vi: GraphQL, Kafka, Kubernetes, UI, thanh toán online, multi-tenant SaaS.

## Cấu trúc

```
StayBill/
  src/StayBill.Api/          # Web API
  tests/StayBill.Api.Tests/  # Tests
  docs/                      # Kiến trúc, domain, API, plan
  docker-compose.yml         # PostgreSQL + Redis
  AGENT.md
```

Chi tiết: [docs/architecture.md](docs/architecture.md).

## Chạy local

Yêu cầu: .NET 8 SDK, Docker.

```bash
copy .env.example .env
docker compose up -d
dotnet run --project src/StayBill.Api --launch-profile http
```

Sau khi có migration (phase 1+), thêm:

```bash
dotnet ef database update --project src/StayBill.Api
```

- Health: `http://localhost:5080/health`
- Swagger: `http://localhost:5080/swagger`
- PostgreSQL: `127.0.0.1:15432` (tránh lệch cổng 5432/5433 trên máy local)
- Redis: `localhost:16379`

Không commit `.env`.

## Tài liệu

| File | Nội dung |
|---|---|
| [AGENT.md](AGENT.md) | Quy tắc cho AI / người implement |
| [docs/implementation-plan.md](docs/implementation-plan.md) | Plan theo phase, thứ tự làm |
| [docs/architecture.md](docs/architecture.md) | Layer, thư mục, Docker |
| [docs/domain.md](docs/domain.md) | Entity, rule nghiệp vụ |
| [docs/api.md](docs/api.md) | Endpoint, DTO, lỗi |

## License

Private / personal project.
