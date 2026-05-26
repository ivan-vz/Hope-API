namespace Hope.Infrastructure.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IMenuRepository MenuRepository { get; }
        IIngredientRepository IngredientRepository { get; }
        IMealRepository MealRepository { get; }
        IOrderRepository OrderRepository { get; }
        ITagRepository TagRepository { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}
