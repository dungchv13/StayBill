# Domain

Chủ nhà dùng API để theo dõi phòng, khách, hợp đồng và hóa đơn tháng (tiền phòng + điện + nước).

## Entities

### User

Người đăng nhập (chủ nhà). MVP: không phân role.

| Field | Type | Note |
|---|---|---|
| Id | Guid | PK |
| Email | string | unique, required |
| PasswordHash | string | required |
| FullName | string | required |
| CreatedAt | DateTimeOffset | UTC |

### Room

| Field | Type | Note |
|---|---|---|
| Id | Guid | PK |
| Code | string | unique, vd `P101` |
| AreaM2 | decimal? | optional |
| MonthlyRent | decimal(18,2) | tiền phòng mặc định |
| Status | RoomStatus | `Vacant` \| `Occupied` |
| CreatedAt | DateTimeOffset | UTC |

`Status` là denormalize từ hợp đồng Active. Nguồn sự thật: có/không `Contract` status `Active` trên room. Khi create/end contract phải cập nhật cả hai.

### Tenant

| Field | Type | Note |
|---|---|---|
| Id | Guid | PK |
| FullName | string | required |
| Phone | string | required |
| IdNumber | string | CMND/CCCD, required |
| Email | string? | optional |
| CreatedAt | DateTimeOffset | UTC |

Khách có thể có nhiều hợp đồng theo thời gian; tại một thời điểm MVP không chặn 1 khách 2 phòng (giữ đơn giản).

### Contract

| Field | Type | Note |
|---|---|---|
| Id | Guid | PK |
| RoomId | Guid | FK |
| TenantId | Guid | FK |
| StartDate | DateOnly | |
| EndDate | DateOnly? | dự kiến; null = chưa xác định |
| MonthlyRent | decimal(18,2) | snapshot giá lúc ký |
| Deposit | decimal(18,2) | mặc định 0 |
| Status | ContractStatus | `Active` \| `Ended` |
| EndedAt | DateTimeOffset? | lúc kết thúc thực tế |
| CreatedAt | DateTimeOffset | UTC |

### Invoice

| Field | Type | Note |
|---|---|---|
| Id | Guid | PK |
| ContractId | Guid | FK |
| Period | string | `yyyy-MM`, unique theo ContractId |
| RentAmount | decimal(18,2) | copy từ contract.MonthlyRent lúc tạo |
| ElectricityKwh | decimal(18,2) | ≥ 0 |
| ElectricityUnitPrice | decimal(18,2) | ≥ 0 |
| WaterM3 | decimal(18,2) | ≥ 0 |
| WaterUnitPrice | decimal(18,2) | ≥ 0 |
| Total | decimal(18,2) | **server tính** |
| Status | InvoiceStatus | `Unpaid` \| `Paid` |
| PaidAt | DateTimeOffset? | |
| CreatedAt | DateTimeOffset | UTC |

## Công thức

```
Total = RentAmount
      + ElectricityKwh * ElectricityUnitPrice
      + WaterM3 * WaterUnitPrice
```

Làm tròn 2 chữ số decimal, `MidpointRounding.AwayFromZero`.

## Rules

1. **Một room chỉ 1 contract Active.** Create contract khi room `Occupied` hoặc đã có Active → `409 ROOM_OCCUPIED`.
2. **Create contract thành công** → `Room.Status = Occupied`, xóa cache phòng trống.
3. **End contract** → `Status = Ended`, `EndedAt = now`, `Room.Status = Vacant`, xóa cache. Idempotent fail: end lần 2 → `409 CONTRACT_ALREADY_ENDED`.
4. **Không xóa room** đang có contract Active → `409`.
5. **Invoice chỉ trên contract Active.** Period trùng → `409 INVOICE_PERIOD_EXISTS`.
6. **Pay** chỉ khi `Unpaid`. Paid rồi → `409 INVOICE_ALREADY_PAID`. Client không đổi `Total`.
7. Mọi timestamp lưu UTC.

## Trạng thái phòng

```
Vacant --(create Active contract)--> Occupied
Occupied --(end contract)--> Vacant
```

Không có trạng thái “bảo trì” ở MVP.
