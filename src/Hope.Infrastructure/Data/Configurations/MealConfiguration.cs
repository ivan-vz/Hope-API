using Hope.Domain.Models;
using Hope.Domain.Models.Auxiliary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hope.Infrastructure.Data.Configurations
{
    public class MealConfiguration : IEntityTypeConfiguration<Meal>
    {
        public void Configure(EntityTypeBuilder<Meal> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).HasMaxLength(60).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(256).IsRequired();
            builder.Property(x => x.Price).HasPrecision(10, 2).IsRequired();
            builder.Property(x => x.ImageUrl).HasMaxLength(256);
            builder.Property(x => x.Created).IsRequired();
            builder.Property(x => x.IsDeleted).IsRequired();

            builder.HasMany(x => x.Tags).WithMany(x => x.Meals).UsingEntity<MealTag>(
                y => y.HasOne(x => x.Tag).WithMany().HasForeignKey(x => x.TagId),
                y => y.HasOne(x => x.Meal).WithMany().HasForeignKey(x => x.MealId),
                y => y.HasKey(x => new { x.MealId, x.TagId })
            );

            builder.HasIndex(x => x.Name).IsUnique().HasFilter("\"IsDeleted\" = false");
            builder.HasIndex(x => x.IsDeleted);
        }
    }
}