using DishesAPI.DbContexts;
using DishesAPI.Extensions;
using DishesAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace DishesAPI.EndpointHandlers
{
    public static class IngredientsHandlers
    {
        public static async Task<Results<Ok<IEnumerable<IngredientDto>>, NotFound>> GetIngredientsAsync (DishesDbContext db, Guid dishId) 
        {
            var dish = await db.Dishes
                .Include(d => d.Ingredients)
                .FirstOrDefaultAsync(d => d.Id == dishId);

            return dish is not null
                ? TypedResults.Ok(dish.Ingredients.ToIngredientDtoList(dishId))
                : TypedResults.NotFound();
        }
}
}
