using StayBill.Api.Domain;

namespace StayBill.Api.Tests;

public class OccupancyRulesTests
{
    [Fact]
    public void Vacant_room_without_active_contract_can_be_leased()
    {
        OccupancyRules.CanCreateActiveContract(RoomStatus.Vacant, hasActiveContract: false).Should().BeTrue();
    }

    [Fact]
    public void Occupied_room_cannot_be_leased()
    {
        OccupancyRules.CanCreateActiveContract(RoomStatus.Occupied, hasActiveContract: false).Should().BeFalse();
    }

    [Fact]
    public void Vacant_room_with_active_contract_cannot_be_leased()
    {
        OccupancyRules.CanCreateActiveContract(RoomStatus.Vacant, hasActiveContract: true).Should().BeFalse();
    }
}
