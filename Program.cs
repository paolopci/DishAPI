using DishesAPI.DbContexts;
using DishesAPI.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DishesDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DishesDBConnectionString")));

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/dishes", async (DishesDbContext db) =>
{
    var dishes = await db.Dishes.ToListAsync();
    return Results.Ok(dishes.ToDishDtoList());
});

app.MapGet("/dishes/{dishId:guid}", async (DishesDbContext db, Guid dishId) =>
{
    var dish = await db.Dishes
        .FirstOrDefaultAsync(d => d.Id == dishId);

    return dish is not null
        ? Results.Ok(dish.ToDishDto())
        : Results.NotFound();
});

app.MapGet("/dishes/{dishName}", async (DishesDbContext db, string dishName) =>
{
    var dish = await db.Dishes
        .FirstOrDefaultAsync(d => d.Name == dishName);

    return dish is not null
        ? Results.Ok(dish.ToDishDto())
        : Results.NotFound();
});

app.MapGet("/dishes/{dishId:guid}/ingredients", async (DishesDbContext db, Guid dishId) =>
{
    var dish = await db.Dishes
        .Include(d => d.Ingredients)
        .FirstOrDefaultAsync(d => d.Id == dishId);

    return dish is not null
        ? Results.Ok(dish.Ingredients.ToIngredientDtoList(dishId))
        : Results.NotFound();
});

app.Run();
