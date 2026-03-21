namespace Hope.Domain.Models
{
    public class Order
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public decimal Total { get; init; }
        public DateTimeOffset Created { get; init; } = DateTimeOffset.UtcNow;
        public string? DeliverTo { get; init; }
        public DateTimeOffset? Delivered { get; private set; } = null;
        public DateOnly To { get; init; }
        public DateTimeOffset? LastUpdate { get; private set; }

        // Navigation Properties
        public Guid UserId { get; init; }
        public User User { get; init; } = null!;
        public ICollection<Meal> Meals { get; init; } = [];

        private Order() {}

        public Order(decimal total, Guid userId, string? address, DateOnly to)
        {
            Total = total;
            UserId = userId;
            DeliverTo = address;
            To = to;
        }
    }
}
