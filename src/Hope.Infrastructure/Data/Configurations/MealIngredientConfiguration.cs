using Hope.Domain.Models.Auxiliary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hope.Infrastructure.Data.Configurations
{
    public class MealIngredientConfiguration : IEntityTypeConfiguration<MealIngredient>
    {
        public void Configure(EntityTypeBuilder<MealIngredient> builder)
        {
            builder.HasKey(x => new { x.MealId, x.IngredientId });

            builder.Property(x => x.Quantity).HasPrecision(10, 3).IsRequired();

            builder.HasOne(x => x.Ingredient).WithMany(x => x.Meals).HasForeignKey(x => x.IngredientId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
