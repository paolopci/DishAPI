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
            dishesEndPoints.MapPut("", DishesHandlers.UpdateDishAsync);
            dishesEndPoints.MapDelete("", DishesHandlers.DeleteDishAsync);
        }
    }
}
