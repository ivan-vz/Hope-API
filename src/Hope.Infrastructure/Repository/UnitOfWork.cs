using Hope.Infrastructure.Data;
using Hope.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hope.Infrastructure.Repository
{
    public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
    {
        private IUserRepository? _userRepository;
        private IMenuRepository? _menuRepository;
        private IIngredientRepository? _ingredientRepository;
        private IMealRepository? _mealRepository;
        private IOrderRepository? _orderRepository;
        private ITagRepository? _tagRepository;

        public IUserRepository UserRepository => _userRepository ??= new UserRepository(context);

        public IMenuRepository MenuRepository => _menuRepository ??= new MenuRepository(context);

        public IIngredientRepository IngredientRepository => _ingredientRepository ??= new IngredientRepository(context);

        public IMealRepository MealRepository => _mealRepository ??= new MealRepository(context);

        public IOrderRepository OrderRepository => _orderRepository ??= new OrderRepository(context);

        public ITagRepository TagRepository => _tagRepository ??= new TagRepository(context);

        public async Task<bool> Complete()
        {
            try
            {
                return await context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateException ex) 
            {
                throw new Exception("An error occured while saving changes", ex);
            }
        }

        public bool HasChanges() => context.ChangeTracker.HasChanges();
    }
}
