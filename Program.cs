using DishesAPI.DbContexts;
using DishesAPI.Extensions;
using DishesAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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
}).WithName("GetDishById");

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

app.MapPost("/dishes", async Task<CreatedAtRoute<DishDto>> (DishesDbContext db,
    [FromBody] DishForCreationDto DishForCreationDto) =>
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
});

app.MapPut("/dishes/{dishId:guid}", async Task<Results<Ok<DishDto>, NotFound>> (
    DishesDbContext db,
    Guid dishId,
    [FromBody] DishForUpdateDto dishToUpdate) =>
{
    var dish = await db.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
    if (dish is null)
    {
        return TypedResults.NotFound();
    }

    dish.UpdateFromDto(dishToUpdate);
    await db.SaveChangesAsync();

    return TypedResults.Ok(dish.ToDishDto());
});

app.MapDelete("/dishes/{dishId:guid}", async Task<Results<NoContent, NotFound>> (
    DishesDbContext db,
    Guid dishId) =>
{
    var dish = await db.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
    if (dish is null)
    {
        return TypedResults.NotFound();
    }

    db.Dishes.Remove(dish);
    await db.SaveChangesAsync();

    return TypedResults.NoContent();
});



app.Run();
