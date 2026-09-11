# API

Base URL local: `http://localhost:5080`

Header sau login: `Authorization: Bearer <accessToken>`

Content-Type: `application/json`

## Auth

### POST `/api/auth/register` (anonymous)

```json
{ "email": "owner@example.com", "password": "Admin123!", "fullName": "Nguyen Van A" }
```

`201` → `{ "id": "...", "email": "...", "fullName": "..." }`  
`409 EMAIL_TAKEN` nếu email đã có.

Password tối thiểu 8 ký tự.

### POST `/api/auth/login` (anonymous)

```json
{ "email": "owner@example.com", "password": "Admin123!" }
```

`200` → `{ "accessToken": "...", "expiresAt": "2026-09-11T12:00:00Z" }`  
`401` sai email/password.

## Health

### GET `/health` (anonymous)

`200` → `{ "status": "ok" }`

## Rooms

### POST `/api/rooms`

```json
{ "code": "P101", "areaM2": 18, "monthlyRent": 3500000 }
```

Tạo xong `status = Vacant`. `201` + body room.  
`409 ROOM_CODE_TAKEN`.

### GET `/api/rooms`

Danh sách tất cả phòng.

### GET `/api/rooms/vacant`

Chỉ phòng `Vacant`. **Đọc Redis trước**; miss thì query DB rồi set cache TTL 60s.

### GET `/api/rooms/{id}`

`200` / `404`

### PUT `/api/rooms/{id}`

Sửa `code`, `areaM2`, `monthlyRent`. Không cho client set `status` (status chỉ đổi qua contract).

### DELETE `/api/rooms/{id}`

`204`. `409 ROOM_HAS_ACTIVE_CONTRACT` nếu đang Occupied / có contract Active.

## Tenants

### POST `/api/tenants`

```json
{ "fullName": "Tran Thi B", "phone": "0901234567", "idNumber": "001234567890", "email": "b@example.com" }
```

### GET `/api/tenants`

### GET `/api/tenants/{id}`

### PUT `/api/tenants/{id}`

### DELETE `/api/tenants/{id}`

`409 TENANT_HAS_ACTIVE_CONTRACT` nếu đang gắn contract Active.

## Contracts

### POST `/api/contracts`

```json
{
  "roomId": "...",
  "tenantId": "...",
  "startDate": "2026-09-01",
  "endDate": null,
  "monthlyRent": 3500000,
  "deposit": 3500000
}
```

Nếu `monthlyRent` omit: lấy `Room.MonthlyRent`.  
`201`. Side effect: room Occupied.  
`409 ROOM_OCCUPIED` | `404` room/tenant.

### GET `/api/contracts`

Optional: `?roomId=` `?status=Active`

### GET `/api/contracts/{id}`

### POST `/api/contracts/{id}/end`

Body rỗng. `200` contract đã end. `409 CONTRACT_ALREADY_ENDED`.

Không có PUT sửa hợp đồng ở MVP (tránh phức tạp occupancy).

## Invoices

### POST `/api/invoices`

```json
{
  "contractId": "...",
  "period": "2026-09",
  "electricityKwh": 120,
  "electricityUnitPrice": 3500,
  "waterM3": 8,
  "waterUnitPrice": 20000
}
```

`RentAmount` lấy từ contract, không nhận từ client.  
`Total` server tính. `201`.  
`409 INVOICE_PERIOD_EXISTS` | `409 CONTRACT_NOT_ACTIVE`.

### GET `/api/invoices`

Optional: `?contractId=` `?status=Unpaid`

### GET `/api/invoices/{id}`

### POST `/api/invoices/{id}/pay`

Body rỗng. `200` `{ ..., "status": "Paid", "paidAt": "..." }`.  
`409 INVOICE_ALREADY_PAID`.

## DTO room (response)

```json
{
  "id": "...",
  "code": "P101",
  "areaM2": 18,
  "monthlyRent": 3500000,
  "status": "Vacant"
}
```

Enum JSON: string (`"Vacant"`, `"Occupied"`, `"Active"`, `"Ended"`, `"Unpaid"`, `"Paid"`).
