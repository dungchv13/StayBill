namespace StayBill.Api.Services;

public static class InvoiceCalculator
{
    public static decimal Total(
        decimal rentAmount,
        decimal electricityKwh,
        decimal electricityUnitPrice,
        decimal waterM3,
        decimal waterUnitPrice)
    {
        var raw = rentAmount
                  + electricityKwh * electricityUnitPrice
                  + waterM3 * waterUnitPrice;
        return decimal.Round(raw, 2, MidpointRounding.AwayFromZero);
    }
}
