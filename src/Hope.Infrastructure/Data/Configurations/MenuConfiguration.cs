using Hope.Domain.Models;
using Hope.Domain.Models.Auxiliary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hope.Infrastructure.Data.Configurations
{
    public class MenuConfiguration : IEntityTypeConfiguration<Menu>
    {
        public void Configure(EntityTypeBuilder<Menu> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).HasMaxLength(60).IsRequired();
            builder.Property(x => x.AvailableMonths).IsRequired();
            builder.Property(x => x.Created).IsRequired();
            builder.Property(x => x.IsDeleted).IsRequired();

            builder.HasMany(x => x.Meals).WithMany(x => x.Menu).UsingEntity<MealMenu>(
                y => y.HasOne(x => x.Meal).WithMany().HasForeignKey("MealId").OnDelete(DeleteBehavior.Restrict),
                y => y.HasOne(x => x.Menu).WithMany().HasForeignKey("MenuId").OnDelete(DeleteBehavior.Restrict),
                y =>
                {
                    y.HasKey("MenuId", "MealId");
                    y.HasIndex("MealId");
                }
            );

            builder.HasIndex(x => x.Name).IsUnique().HasFilter("\"IsDeleted\" = false");
            builder.HasIndex(x => x.IsDeleted);
        }
    }
}
