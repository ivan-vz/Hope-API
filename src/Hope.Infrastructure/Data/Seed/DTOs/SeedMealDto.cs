namespace Hope.Infrastructure.Data.Seed.DTOs
{
    internal class SeedMealDto
    {
        public required string Name { get; init; }
        public required string Description { get; init; }
        public required decimal Price { get; init; }
        public string? ImageUrl { get; init; }
        public required ICollection<string> Tags { get; init; }
    }
}
