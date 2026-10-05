using System.Security.Claims;
using DishesAPI.DbContexts;
using DishesAPI.Extensions;
using DishesAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DishesAPI.EndpointHandlers
{
    public static class DishesHandlers
    {
        public static async Task<Ok<IEnumerable<DishDto>>> GetDishesAsync(DishesDbContext db,
            ClaimsPrincipal claimsPrincipal, string? name)
        {
            Console.WriteLine($"User authenticated? {claimsPrincipal.Identity?.IsAuthenticated}");
            var dishes = await db.Dishes.ToListAsync();
            return TypedResults.Ok(dishes.ToDishDtoList());
        }

        public static async Task<Results<Ok<DishDto>, NotFound>> GetDishByIdAsync(DishesDbContext db, Guid dishId)
        {
            var dish = await db.Dishes
                .FirstOrDefaultAsync(d => d.Id == dishId);

            return dish is not null
                ? TypedResults.Ok(dish.ToDishDto())
                : TypedResults.NotFound();
        }

        public static async Task<Results<Ok<DishDto>, NotFound>> DishByNameAsync(DishesDbContext db, string dishName)
        {
            var dish = await db.Dishes
                .FirstOrDefaultAsync(d => d.Name == dishName);

            return dish is not null
                ? TypedResults.Ok(dish.ToDishDto())
                : TypedResults.NotFound();
        }

        public static async Task<CreatedAtRoute<DishDto>> CreateDishAsync(DishesDbContext db,
        [FromBody] DishForCreationDto DishForCreationDto)
        {
            var newDish = DishForCreationDto.ToDish();
            db.Add(newDish);
            await db.SaveChangesAsync();

            var dishToReturn = newDish.ToDishDto();

            return TypedResults.CreatedAtRoute(
                dishToReturn,
                "GetDishById",
                new { dishId = dishToReturn.Id }
            );
        }

        public static async Task<Results<Ok<DishDto>, NotFound>> UpdateDishAsync(
        DishesDbContext db, Guid dishId, [FromBody] DishForUpdateDto dishToUpdate)
        {
            var dish = await db.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
            if (dish is null)
            {
                return TypedResults.NotFound();
            }

            dish.UpdateFromDto(dishToUpdate);
            await db.SaveChangesAsync();

            return TypedResults.Ok(dish.ToDishDto());
        }

        public static async Task<Results<NoContent, NotFound>> DeleteDishAsync (
            DishesDbContext db, Guid dishId) 
        {
            var dish = await db.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
            if (dish is null)
            {
                return TypedResults.NotFound();
            }

            db.Dishes.Remove(dish);
            await db.SaveChangesAsync();

            return TypedResults.NoContent();
        }

    }

}
