using DishesAPI.DbContexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// connection string 
builder.Services.AddDbContext<DishesDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DishesDBConnectionString"))
);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();


app.MapGet("/dishes", async (DishesDbContext db) =>
{
    return await db.Dishes.ToListAsync();
});

app.MapGet("/dishes/{dishId:guid}", async (DishesDbContext db, Guid dishId) =>
{
    var dish = await db.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
    return dish is not null ? Results.Ok(dish) : Results.NotFound();

});

app.MapGet("/dishes/{dishName}", async (DishesDbContext db, string dishName) =>
{
    var dish = await db.Dishes.FirstOrDefaultAsync(d => d.Name == dishName);
    return dish is not null ? Results.Ok(dish) : Results.NotFound();

});

app.MapGet("/dishes/{dishId}/ingredients", async (DishesDbContext db, Guid dishId) =>
{
    return (await db.Dishes
        .Include(d => d.Ingredients)
        .FirstOrDefaultAsync(d => d.Id == dishId))?.Ingredients;

});








app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
