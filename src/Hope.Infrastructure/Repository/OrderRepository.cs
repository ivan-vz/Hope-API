using Hope.Domain.Models;
using Hope.Infrastructure.Data;
using Hope.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hope.Infrastructure.Repository
{
    internal class OrderRepository(ApplicationDbContext context) : IOrderRepository
    {
        public void Add(Order order) => context.Orders.Add(order);

        public async Task<IReadOnlyList<Order>> GetAllByDateAsync(DateOnly date, CancellationToken ct) => 
            await context.Orders.Include(x => x.User)
            .Include(x => x.Meals).ThenInclude(om => om.Meal).ThenInclude(m => m.Tags)
            .Where(x => x.To == date).ToListAsync(ct);

        public async Task<IReadOnlyList<Order>> GetAllByUserAsync(Guid userId, CancellationToken ct) => 
            await context.Orders.Include(x => x.User)
            .Include(x => x.Meals).ThenInclude(om => om.Meal).ThenInclude(m => m.Tags)
            .Where(x => x.UserId == userId).ToListAsync(ct);

        public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct) => await context.Orders
            .Include(x => x.User)
            .Include(x => x.Meals).ThenInclude(om => om.Meal).ThenInclude(m => m.Tags)
            .SingleOrDefaultAsync(x => x.Id == id, ct);
    }
}
