using Hope.Application.DTOs.Records;

namespace Hope.Application.DTOs.Insert
{
    public class MealInsertDto
    {
        public required string Name { get; init; }
        public required string Description { get; init; }
        public required decimal Price { get; init; }
        public required ICollection<string> Tags { get; init; }
        public required ICollection<IngredientItem> Ingredients { get; set; }
    }
}
