using Hope.Domain.Models.Auxiliary;

namespace Hope.Domain.Models
{
    public class Ingredient(string name, bool isLiquid)
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = name;
        public bool IsLiquid { get; set; } = isLiquid;

        public ICollection<MealIngredient> Meals { get; set; } = [];
    }
}
