namespace Hope.Domain.Models
{
    public class Menu(string name)
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Name { get; set; } = name;
        public List<DateOnly> AvailableMonths { get; set; } = [];
        public DateTimeOffset Created { get; init; } = DateTimeOffset.UtcNow;
        public bool IsDeleted { get; set; } = false;

        // Navigation Properties
        public ICollection<Meal> Meals { get; set; } = [];
    }
}
