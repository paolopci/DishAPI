using DishesAPI.DbContexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddValidation(); 

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

// MapGroup
var dishesEndPoints = app.MapGroup("/dishes");
var dishWithGuidIdEndpoints = dishesEndPoints.MapGroup("/{dishId:guid}");
var ingredientsEndpoints = dishWithGuidIdEndpoints.MapGroup("/ingredients");


dishesEndPoints.MapGet("");

dishWithGuidIdEndpoints.MapGet("").WithName("GetDishById");

dishesEndPoints.MapGet("/{dishName}");

ingredientsEndpoints.MapGet("");

dishesEndPoints.MapPost("");

dishWithGuidIdEndpoints.MapPut("");

dishWithGuidIdEndpoints.MapDelete("");



app.Run();
