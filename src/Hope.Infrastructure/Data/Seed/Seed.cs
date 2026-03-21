using Hope.Domain.Models;
using Hope.Infrastructure.Data.Seed.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Hope.Infrastructure.Data.Seed
{
    public class Seed
    {
        public static async Task SeedAll(ApplicationDbContext context)
        {
            var tags = await SeedTags(context);
            var meals = await SeedMeals(context, tags);
            var users = await SeedUsers(context);
            await SeedMenus(context, meals);
            await SeedOrders(context, users, meals);
        }

        private static async Task<List<Tag>> SeedTags(ApplicationDbContext context)
        {
            var tags = new List<Tag>();

            if (await context.Tags.AnyAsync()) return tags;

            var data = await File.ReadAllTextAsync("Data/Seed/Data/TagSeedData.json");
            var tagsData = JsonSerializer.Deserialize<List<SeedTagDto>>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (tagsData is null)
            {
                Console.WriteLine("No tags in seed data");
                return tags;
            }

            foreach (var td in tagsData)
            {
                var tag = new Tag(td.Name);

                context.Tags.Add(tag);
                tags.Add(tag);
            }

            await context.SaveChangesAsync();
            return tags;
        }

        private static async Task<List<Meal>> SeedMeals(ApplicationDbContext context, List<Tag> tags)
        {
            var meals = new List<Meal>();
            if (await context.Meals.AnyAsync()) return meals;

            var data = await File.ReadAllTextAsync("Data/Seed/Data/MealSeedData.json");
            var mealsData = JsonSerializer.Deserialize<List<SeedMealDto>>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (mealsData is null)
            {
                Console.WriteLine("No meals in seed data");
                return meals;
            }

            foreach(var md in mealsData)
            {
                var meal = new Meal(md.Name, md.Description, md.Price);

                foreach (var tagName in md.Tags)
                {
                    var tag = tags.SingleOrDefault(x => x.Name == tagName);
                    if (tag is null)
                    {
                        Console.WriteLine($"Tag '{tagName}' not found for meal '{md.Name}'");
                        continue;
                    }

                    meal.Tags.Add(tag);
                }

                context.Meals.Add(meal);
                meals.Add(meal);
            }

            await context.SaveChangesAsync();
            return meals;
        }

        private static async Task<List<User>> SeedUsers(ApplicationDbContext context)
        {
            var users = new List<User>();
            
            if (await context.Users.AnyAsync()) return users;

            var data = await File.ReadAllTextAsync("Data/Seed/Data/UserSeedData.json");
            var usersData = JsonSerializer.Deserialize<List<User>>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (usersData is null)
            {
                Console.WriteLine("No users in seed data");
                return users;
            }


            foreach (var user in usersData) 
            {
                var u = new User(user.Name, user.Surname, user.Email, user.PhoneNumber, user.Address);
                context.Users.Add(u);
                users.Add(u);
            }

            await context.SaveChangesAsync();

            return users;
        }

        private static async Task SeedMenus(ApplicationDbContext context, List<Meal> meals)
        {

            if (await context.Menus.AnyAsync()) return;

            var data = await File.ReadAllTextAsync("Data/Seed/Data/MenuSeedData.json");
            var menusData = JsonSerializer.Deserialize<List<SeedMenuDto>>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (menusData is null)
            {
                Console.WriteLine("No menus in seed data");
                return;
            }

            foreach (var md in menusData)
            {
                var menu = new Menu(md.Name)
                {
                    AvailableMonths = [.. md.AvailableMonths.Select(x => DateOnly.Parse(x))]
                };

                foreach (var mealName in md.Meals)
                {
                    var meal = meals.SingleOrDefault(x => x.Name == mealName);
                    if (meal is null)
                    {
                        Console.WriteLine($"Meal '{mealName}' not found for menu '{md.Name}'");
                        continue;
                    }

                    menu.Meals.Add(meal);
                }

                context.Menus.Add(menu);
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedOrders(ApplicationDbContext context, List<User> users, List<Meal> meals) 
        {
            if (await context.Orders.AnyAsync()) return;

            var data = await File.ReadAllTextAsync("Data/Seed/Data/OrderSeedData.json");
            var ordersData = JsonSerializer.Deserialize<List<SeedOrderDto>>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (ordersData is null) 
            {
                Console.WriteLine("No orders in seed data");
                return;
            }

            foreach (var od in ordersData)
            {
                var user = users.SingleOrDefault(x => x.Email == od.UserEmail);
                if (user is null)
                {
                    Console.WriteLine($"Email '{od.UserEmail}' not found for order'");
                    continue;
                }
                
                var order = new Order(od.Total, user.Id, od.DeliverTo, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(od.DaysUntilDelivery)));

                foreach (var mealName in od.Meals)
                {
                    var meal = meals.SingleOrDefault(x => x.Name == mealName);
                    if (meal is null)
                    {
                        Console.WriteLine($"Meal '{mealName}' not found for order");
                        continue;
                    }

                    order.Meals.Add(meal);
                }

                context.Orders.Add(order);
            }

            await context.SaveChangesAsync();
        }
    }
}
