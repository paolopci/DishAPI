using DishesAPI.DbContexts;
using DishesAPI.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddValidation();

builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();

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
    // Configure the HTTP request pipeline. lo usi in Production
    app.UseExceptionHandler();
}


app.UseHttpsRedirection();
app.UseStatusCodePages();
// autenticazione
// app.UseAuthentication();
//app.UseAuthorization();

app.MapGet("/testerror", () =>
{
    throw new NotImplementedException();
});


// chiamo i EndpointRouterBuilderExtensions
app.RegisterDishesEndPoints();
app.RegisterIngredientsEndPoints();

app.Run();
