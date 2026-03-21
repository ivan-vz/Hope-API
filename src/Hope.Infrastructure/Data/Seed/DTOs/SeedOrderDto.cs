namespace Hope.Infrastructure.Data.Seed.DTOs
{
    internal class SeedOrderDto
    {
        public decimal Total { get; init; }
        public required string DeliverTo { get; init; }
        public int DaysUntilDelivery { get; init; }
        public required string UserEmail { get; init; }
        public required ICollection<string> Meals { get; init; }
    }
}
