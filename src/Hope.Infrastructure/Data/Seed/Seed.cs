using Hope.Domain.Models;
using Hope.Domain.Models.Auxiliary;
using Hope.Infrastructure.Data.Seed.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Hope.Infrastructure.Data.Seed
{
    public class Seed
    {
        public static async Task SeedAll(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            await SeedRoles(roleManager);
            var tags = await SeedTags(context);
            var ingredients = await SeedIngredients(context);
            var meals = await SeedMeals(context, tags, ingredients);
            var users = await SeedUsers(userManager);
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

        private static async Task<List<Ingredient>> SeedIngredients(ApplicationDbContext context)
        {
            var ingredients = new List<Ingredient>();

            if (await context.Ingredients.AnyAsync()) return ingredients;

            var data = await File.ReadAllTextAsync("Data/Seed/Data/IngredientSeedData.json");
            var ingData = JsonSerializer.Deserialize<List<SeedIngredientDto>>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (ingData is null)
            {
                Console.WriteLine("No ingredients in seed data");
                return ingredients;
            }

            foreach (var id in ingData)
            {
                var ing = new Ingredient(id.Name, id.IsLiquid);
                
                context.Ingredients.Add(ing);
                ingredients.Add(ing);
            }

            await context.SaveChangesAsync();
            return ingredients;
        }

        private static async Task<List<Meal>> SeedMeals(ApplicationDbContext context, List<Tag> tags, List<Ingredient> ingredients)
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

                foreach (var(name, quantity) in md.Ingredients)
                {
                    var ing = ingredients.SingleOrDefault(x => x.Name == name);
                    if (ing is null)
                    {
                        Console.WriteLine($"Ingredient '{name}' not found for meal '{md.Name}'");
                        continue;
                    }

                    var mealIngredient = new MealIngredient
                    {
                        Quantity = quantity,
                        MealId = meal.Id,
                        Meal = meal,
                        IngredientId = ing.Id,
                        Ingredient = ing
                    };

                    meal.Ingredients.Add(mealIngredient);
                }

                context.Meals.Add(meal);
                meals.Add(meal);
            }

            await context.SaveChangesAsync();
            return meals;
        }

        private static async Task<List<User>> SeedUsers(UserManager<User> userManager)
        {
            var users = new List<User>();
            
            if (await userManager.Users.AnyAsync()) return users;

            var data = await File.ReadAllTextAsync("Data/Seed/Data/UserSeedData.json");
            var usersData = JsonSerializer.Deserialize<List<User>>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (usersData is null)
            {
                Console.WriteLine("No users in seed data");
                return users;
            }


            foreach (var user in usersData) 
            {
                var u = new User(user.Name, user.Surname!, user.Email!, user.PhoneNumber!, user.Address);
                var result = await userManager.CreateAsync(u, "Pa$$w0rd");
                if (!result.Succeeded)
                {
                    Console.WriteLine(result.Errors.First().Description);
                    continue;
                }

                await userManager.AddToRoleAsync(u, "User");

                users.Add(u);
            }

            var chef = new User("chef", "Ratatouille", "chef@test.com", "+598 97 982 756", null);
            var resultChef = await userManager.CreateAsync(chef, "Pa$$w0rd");
            if (resultChef.Succeeded) await userManager.AddToRoleAsync(chef, "Chef");

            var admin = new User("admin", null, "admin@test.com", "+598 98 123 456", null);
            var resultAdmin = await userManager.CreateAsync(admin, "Pa$$w0rd");
            if (resultAdmin.Succeeded) await userManager.AddToRoleAsync(admin, "Admin");

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
                
                var order = new Order(od.Total, user.Id, od.Address, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(od.DaysUntilDelivery)), od.Message);

                foreach (var mealItem in od.Meals)
                {
                    var meal = meals.SingleOrDefault(x => x.Name == mealItem.Name);
                    if (meal is null)
                    {
                        Console.WriteLine($"Meal '{mealItem.Name}' not found for order");
                        continue;
                    }

                    var orderMeal = new OrderMeal 
                    {
                        Quantity = mealItem.Quantity,
                        MealId = meal.Id,
                        Meal = meal,
                        OrderId = order.Id,
                        Order = order
                    };

                    order.Meals.Add(orderMeal);
                }

                context.Orders.Add(order);
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedRoles(RoleManager<IdentityRole<Guid>> roleManager)
        {
            var roles = new[] { "Admin", "User", "Chef" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }
        }
    }
}
