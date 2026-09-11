using StayBill.Api.Services;

namespace StayBill.Api.Tests;

public class InvoiceCalculatorTests
{
    [Fact]
    public void Total_is_rent_plus_electricity_plus_water()
    {
        InvoiceCalculator.Total(
                rentAmount: 3_500_000,
                electricityKwh: 120,
                electricityUnitPrice: 3500,
                waterM3: 8,
                waterUnitPrice: 20_000)
            .Should().Be(4_080_000);
    }

    [Fact]
    public void Total_rounds_away_from_zero()
    {
        InvoiceCalculator.Total(100, 1, 0.005m, 0, 0).Should().Be(100.01m);
    }
}
