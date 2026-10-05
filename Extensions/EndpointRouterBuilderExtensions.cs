using DishesAPI.EndpointHandlers;

namespace DishesAPI.Extensions
{
    public static class EndpointRouterBuilderExtensions
    {
        public static void RegisterDishesEndPoints(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            // MapGroup
            var dishesEndPoints = endpointRouteBuilder.MapGroup("/dishes");
            var dishWithGuidIdEndpoints = dishesEndPoints.MapGroup("/{dishId:guid}");

            dishesEndPoints.MapGet("", DishesHandlers.GetDishesAsync);
            dishWithGuidIdEndpoints.MapGet("", DishesHandlers.GetDishByIdAsync).WithName("GetDishById");
            dishesEndPoints.MapGet("/{dishName}", DishesHandlers.DishByNameAsync);
            dishesEndPoints.MapPost("", DishesHandlers.CreateDishAsync);
            dishWithGuidIdEndpoints.MapPut("", DishesHandlers.UpdateDishAsync);
            dishWithGuidIdEndpoints.MapDelete("", DishesHandlers.DeleteDishAsync);
        }

        public static void RegisterIngredientsEndPoints(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            var ingredientsEndPoint = endpointRouteBuilder.MapGroup("dishes/{dishId:guid}/ingredients");

            ingredientsEndPoint.MapGet("", IngredientsHandlers.GetIngredientsAsync);
        }
    }
}
