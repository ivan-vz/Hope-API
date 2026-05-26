namespace Hope.Domain.Models.Auxiliary
{
    public class MealIngredient
    {
        public decimal Quantity { get; set; }

        public Guid MealId { get; set; }
        public Meal Meal { get; set; } = null!;

        public Guid IngredientId { get; set; }
        public Ingredient Ingredient { get; set; } = null!;
    }
}
