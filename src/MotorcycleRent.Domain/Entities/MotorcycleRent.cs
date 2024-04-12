namespace MotorcycleRent.Domain.Entities;

public sealed record MotorcycleRent : BaseEntity
{
    public DeliveryPartner? DeliveryPartner { get; init; }
    public Motorcycle? Motorcycle { get; init; }
    public DateTimeRange RentPeriod { get; init; }
    public ERentPlan RentPlan { get; init; }
    public decimal RentCost { get; init; }
    public decimal FeeCost { get; init; }    
    public ERentStatus RentStatus { get; init; }

    /// <summary>
    /// Gets the total cost of the motorcycle rent, which includes both the rent cost and any associated fees.
    /// </summary>
    /// <value>
    /// The actual cost is calculated by summing the <see cref="MotorcycleRent.RentCost"/> and <see cref="MotorcycleRent.FeeCost"/>.
    /// The resulting sum is then rounded to the nearest whole number using standard rounding rules.
    /// </value>
    /// <remarks>
    /// This property is useful for displaying the total payable amount by the customer.
    /// The rounding ensures that the displayed cost is always in a format suitable for financial transactions.
    /// </remarks>
    public decimal ActualCost => Math.Round(RentCost + FeeCost, 2);
}