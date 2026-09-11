namespace StayBill.Api.Domain;

public static class OccupancyRules
{
    public static bool CanCreateActiveContract(RoomStatus roomStatus, bool hasActiveContract) =>
        roomStatus == RoomStatus.Vacant && !hasActiveContract;
}
