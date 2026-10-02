using DishesAPI.DbContexts;
using DishesAPI.Extensions;
using DishesAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

builder.Services.AddDbContext<DishesDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DishesDBConnectionString")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    // Configure the HTTP request pipeline.
    app.UseExceptionHandler();
}


app.UseHttpsRedirection();
app.UseStatusCodePages();


app.MapGet("/testerror", () =>
{
    throw new NotImplementedException();
});


app.MapGet("/dishes", async Task<Ok<IEnumerable<DishDto>>> (DishesDbContext db) =>
{
    var dishes = await db.Dishes.ToListAsync();
    return TypedResults.Ok(dishes.ToDishDtoList());
});

app.MapGet("/dishes/{dishId:guid}", async Task<Results<Ok<DishDto>, NotFound>> (DishesDbContext db, Guid dishId) =>
{
    var dish = await db.Dishes
        .FirstOrDefaultAsync(d => d.Id == dishId);

    return dish is not null
        ? TypedResults.Ok(dish.ToDishDto())
        : TypedResults.NotFound();
});

app.MapGet("/dishes/{dishName}", async Task<Results<Ok<DishDto>, NotFound>> (DishesDbContext db, string dishName) =>
{
    var dish = await db.Dishes
        .FirstOrDefaultAsync(d => d.Name == dishName);

    return dish is not null
        ? TypedResults.Ok(dish.ToDishDto())
        : TypedResults.NotFound();
});

app.MapGet("/dishes/{dishId:guid}/ingredients", async Task<Results<Ok<IEnumerable<IngredientDto>>, NotFound>> (DishesDbContext db, Guid dishId) =>
{
    var dish = await db.Dishes
        .Include(d => d.Ingredients)
        .FirstOrDefaultAsync(d => d.Id == dishId);

    return dish is not null
        ? TypedResults.Ok(dish.Ingredients.ToIngredientDtoList(dishId))
        : TypedResults.NotFound();
});

app.Run();
