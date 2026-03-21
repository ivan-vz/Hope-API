namespace Hope.Infrastructure.Data.Seed.DTOs
{
    internal class SeedMenuDto
    {
        public required string Name { get; init; }
        public required ICollection<string> AvailableMonths { get; init; }
        public required ICollection<string> Meals { get; init; }
    }
}
