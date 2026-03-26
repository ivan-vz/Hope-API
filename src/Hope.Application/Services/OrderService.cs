using FluentValidation;
using FluentValidation.Results;
using Hope.Application.DTOs.Detail;
using Hope.Application.DTOs.Insert;
using Hope.Application.DTOs.Update;
using Hope.Application.Extensions;
using Hope.Application.Interfaces;
using Hope.Domain.Models;
using Hope.Domain.Models.Auxiliary;
using Hope.Infrastructure.Interfaces;

namespace Hope.Application.Services
{
    public class OrderService(IUnitOfWork uow, IValidator<OrderInsertDto> insertValidator, IValidator<OrderUpdateDto> updateValidator) : IOrderService
    {
        private readonly IUnitOfWork _uow = uow;
        private readonly IValidator<OrderInsertDto> _insertValidator = insertValidator;
        private readonly IValidator<OrderUpdateDto> _updateValidator = updateValidator;

        public async Task<(OrderDto?, ValidationResult)> CreateAsync(OrderInsertDto dtInsert, CancellationToken ct)
        {
            var validation = await _insertValidator.ValidateAsync(dtInsert, ct);
            if (!validation.IsValid) return (null, validation);

            var user = await _uow.UserRepository.GetByIdAsync(dtInsert.UserId, ct);
            if (user is null)
            {
                validation.Errors.Add(new ValidationFailure("User", $"User with id {dtInsert.UserId} not found"));
                return (null, validation);
            }

            var oMeals = new List<(Meal, int)>();
            decimal total = 0;
            foreach (var mealItem in dtInsert.Meals.Distinct()) 
            {
                var meal = await _uow.MealRepository.GetByIdAsync(mealItem.Id, ct);
                if (meal is null)
                {
                    validation.Errors.Add(new ValidationFailure("Meal", $"Meal {mealItem.Id} not found"));
                    continue;
                }

                total += (meal.Price * mealItem.Quantity);
                oMeals.Add((meal, mealItem.Quantity));
            }

            if (!validation.IsValid) return (null, validation);

            var order = new Order(
                total, 
                user.Id, 
                (dtInsert.Delivery) ? user.Address : null, 
                dtInsert.To
                );

            order.User = user;

            foreach (var (meal, quantity) in oMeals)
            {
                var orderMeal = new OrderMeal
                {
                    OrderId = order.Id,
                    Order = order,
                    MealId = meal.Id,
                    Meal = meal,
                    Quantity = quantity
                };

                order.Meals.Add(orderMeal);
            }

            _uow.OrderRepository.Add(order);
            await _uow.Complete();

            return (order.ToDto(), validation);
        }

        public async Task<ValidationResult> DeleteAsync(Guid id, CancellationToken ct)
        {
            var validation = new ValidationResult();

            var order = await _uow.OrderRepository.GetByIdAsync(id, ct);
            if (order is null)
            {
                validation.Errors.Add(new ValidationFailure("Id", "Order not found"));
                return validation;
            }

            order.IsCancelled = true;
            await _uow.Complete();

            return validation;
        }

        public async Task<IReadOnlyList<OrderDto>> GetAllByDateAsync(DateOnly date, CancellationToken ct) => 
            [..(await _uow.OrderRepository.GetAllByDateAsync(date, ct)).Select(x => x.ToDto())];

        public async Task<IReadOnlyList<OrderDto>> GetAllByUserAsync(Guid userId, CancellationToken ct) =>
            [.. (await _uow.OrderRepository.GetAllByUserAsync(userId, ct)).Select(x => x.ToDto())];

        public async Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken ct) => (await _uow.OrderRepository.GetByIdAsync(id, ct))?.ToDto();

        public async Task<(OrderDto?, ValidationResult)> UpdateAsync(OrderUpdateDto dtUpdate, CancellationToken ct)
        {
            var validation = await _updateValidator.ValidateAsync(dtUpdate, ct);
            if (!validation.IsValid) return (null, validation);

            var order = await _uow.OrderRepository.GetByIdAsync(dtUpdate.Id, ct);
            if(order is null) return (null, validation);

            var oMeals = new List<(Meal, int)>();
            decimal total = 0;
            foreach (var mealItem in dtUpdate.Meals.Distinct())
            {
                var meal = await _uow.MealRepository.GetByIdAsync(mealItem.Id, ct);
                if (meal is null)
                {
                    validation.Errors.Add(new ValidationFailure("Meal", $"Meal {mealItem.Id} not found"));
                    continue;
                }

                total += (meal.Price * mealItem.Quantity);
                oMeals.Add((meal, mealItem.Quantity));
            }

            if (!validation.IsValid) return (null, validation);

            order.Meals.Clear();
            foreach (var (meal, quantity) in oMeals)
            {
                var orderMeal = new OrderMeal
                {
                    OrderId = order.Id,
                    Order = order,
                    MealId = meal.Id,
                    Meal = meal,
                    Quantity = quantity
                };

                order.Meals.Add(orderMeal);
            }
            
            order.LastUpdate = DateTimeOffset.UtcNow;
            order.DeliverTo = (dtUpdate.Delivery) ? order.User.Address : null;

            await _uow.Complete();

            return (order.ToDto(), validation);
        }
    }
}
