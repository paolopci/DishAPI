using DishesAPI.EndpointHandlers;

namespace DishesAPI.Extensions
{
    public static class EndpointRouterBuilderExtensions
    {
        public static void RegisterDishesEndPoints(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            // MapGroup
            var dishesEndPoints = endpointRouteBuilder.MapGroup("/dishes").RequireAuthorization();
            // l'autorizzazione è in cascata da dishesEndPoints
            var dishWithGuidIdEndpoints = dishesEndPoints.MapGroup("/{dishId:guid}");

            dishesEndPoints.MapGet("", DishesHandlers.GetDishesAsync);
            dishWithGuidIdEndpoints.MapGet("", DishesHandlers.GetDishByIdAsync).WithName("GetDishById");
            // questo endpoint non richiede l'autorizzazione perché ho messo AllowAnonymous()
            dishesEndPoints.MapGet("/{dishName}", DishesHandlers.DishByNameAsync).AllowAnonymous();
            dishesEndPoints.MapPost("", DishesHandlers.CreateDishAsync).RequireAuthorization("RequiredAdminFromBelgium");
            dishWithGuidIdEndpoints.MapPut("", DishesHandlers.UpdateDishAsync).RequireAuthorization("RequiredAdminFromBelgium");
            dishWithGuidIdEndpoints.MapDelete("", DishesHandlers.DeleteDishAsync).RequireAuthorization("RequiredAdminFromBelgium");
        }

        public static void RegisterIngredientsEndPoints(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            var ingredientsEndPoint = endpointRouteBuilder.MapGroup("dishes/{dishId:guid}/ingredients")
                .RequireAuthorization(); // l'autorizzazione 

            ingredientsEndPoint.MapGet("", IngredientsHandlers.GetIngredientsAsync);
        }
    }
}
