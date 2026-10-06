using DishesAPI.DbContexts;
using DishesAPI.Extensions;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddValidation();

builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();

builder.Services.AddAuthorizationBuilder().AddPolicy("RequiredAdminFromBelgium", policy =>
    policy.RequireAuthenticatedUser()
        .RequireRole("admin")
        .RequireClaim("country", "Belgium"));

builder.Services.AddDbContext<DishesDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DishesDBConnectionString")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // opeapi/v1.json
    app.MapOpenApi();

    // Configure the HTTP request pipeline. lo usi in Development
    app.MapScalarApiReference();
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
