# AGENT.md

Hướng dẫn cho agent (và người) implement StayBill. Đọc file này **trước khi sửa code**.

## Mục tiêu

Xây một ASP.NET Core 8 Web API **cơ bản**, đủ để demo và nói trong phỏng vấn — khớp 4 ý trên CV:

1. REST: phòng (trống / đã thuê), khách thuê, hợp đồng
2. Hóa đơn tháng: tiền phòng + điện/nước; đã thu / chưa thu
3. JWT; Redis cache danh sách phòng trống
4. EF Core Code First + Docker Compose local

## Đọc trước (bắt buộc)

1. `docs/implementation-plan.md` — làm **đúng thứ tự phase**, đừng nhảy cóc
2. `docs/domain.md` — rule nghiệp vụ (phòng Occupied, công thức hóa đơn)
3. `docs/api.md` — contract HTTP
4. `docs/architecture.md` — thư mục, layer

## Ràng buộc

- Một Web API project: `src/StayBill.Api`. Không tách Clean Architecture 4 project.
- Layer trong folder: `Controllers` → `Services` → `Data` (DbContext). Domain entity trong `Domain/`.
- C# nullable enable, `async`/`await`, `CancellationToken` xuyên suốt.
- Không GraphQL, Kafka, gRPC, Hangfire, MediatR, AutoMapper (map tay DTO).
- Không frontend, không Blazor.
- Không Kubernetes, không cloud deploy.
- Tiếng Anh cho code (class, route, comment ngắn). Docs có thể tiếng Việt.
- Secret chỉ qua env / `appsettings.Development.json` (gitignored nếu có secret). Commit `.env.example`.

## Stack cố định

- `net8.0`
- `Npgsql.EntityFrameworkCore.PostgreSQL`
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `StackExchange.Redis`
- `Swashbuckle.AspNetCore`
- Test: `xunit` + `FluentAssertions` + `Microsoft.AspNetCore.Mvc.Testing` (WebApplicationFactory, PostgreSQL testcontainer **không bắt buộc** phase 1 — in-memory hoặc Testcontainers Postgres ở phase test)

## Cách làm từng phase

- Chỉ implement phase hiện tại trong `docs/implementation-plan.md`.
- Mỗi phase xong: solution build được, endpoint/phase đó gọi được (hoặc test xanh).
- Đặt tên commit gợi ý nằm ở cuối mỗi phase trong plan. **Không tự commit** trừ khi user yêu cầu.
- Khi xong một entity CRUD: validation, 404, 409 cho rule nghiệp vụ — đừng chỉ happy path.

## Definition of done (toàn repo)

- `docker compose up -d` chạy Postgres + Redis
- `dotnet ef database update` tạo schema
- Register / login trả JWT
- CRUD rooms / tenants / contracts / invoices đúng `docs/api.md`
- Thuê phòng đang có hợp đồng Active → `409`
- Kết thúc hợp đồng → phòng `Vacant`, Redis cache phòng trống bị xóa/cập nhật
- `GET /api/rooms/vacant` cache Redis (TTL ~60s)
- Hóa đơn: `Total = Rent + kWh * đơn giá điện + m3 * đơn giá nước`; `POST .../pay` → `Paid`
- Swagger chạy local
- README mục “Chạy local” đúng lệnh thật (sửa README khi scaffold xong)


