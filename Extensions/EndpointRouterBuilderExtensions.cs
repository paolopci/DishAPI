using DishesAPI.EndpointHandlers;

namespace DishesAPI.Extensions
{
    public static class EndpointRouterBuilderExtensions
    {
        public static void RegisterDishesEndPoints(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            // MapGroup
            var dishesEndPoints = endpointRouteBuilder.MapGroup("/dishes")
                .RequireAuthorization()
                .WithTags("Dishes");
            // l'autorizzazione è in cascata da dishesEndPoints
            var dishWithGuidIdEndpoints = dishesEndPoints.MapGroup("/{dishId:guid}");

            dishesEndPoints.MapGet("", DishesHandlers.GetDishesAsync)
                .WithSummary("Get all dishes")
                .WithDescription("Returns all dishes, optionally filtered by name");

            dishWithGuidIdEndpoints.MapGet("", DishesHandlers.GetDishByIdAsync)
                .WithName("GetDishById")
                .WithSummary("Get a dish")
                .WithDescription("Get a  dish. Requires the Id for the dish")
                // Indica che questo endpoint può restituire un problema di validazione con codice 400
                .ProducesValidationProblem(400);

            // questo endpoint non richiede l'autorizzazione perché ho messo AllowAnonymous()
            dishesEndPoints.MapGet("/{dishName}", DishesHandlers.DishByNameAsync)
                .AllowAnonymous()
                .WithSummary("Get a dish by name")
                .WithDescription("Returns a single dish identified by its name.  " +
                                 "This endpoint allows anonymous access.");

            dishesEndPoints.MapPost("", DishesHandlers.CreateDishAsync)
                .RequireAuthorization("RequiredAdminFromBelgium")
                .WithSummary("Create a dish")
                .WithDescription("Creates a new dish. Requires the admin role and country Belgium")
                // Indica che questo endpoint può restituire un problema di validazione con codice 400
                .ProducesValidationProblem(400); 

            dishWithGuidIdEndpoints.MapPut("", DishesHandlers.UpdateDishAsync)
                .RequireAuthorization("RequiredAdminFromBelgium")
                .WithSummary("Update a dish")
                .WithDescription("Update a dish. Requires the admin role and country Belgium")
                // Indica che questo endpoint può restituire un problema di validazione con codice 400
                .ProducesValidationProblem(400);

            dishWithGuidIdEndpoints.MapDelete("", DishesHandlers.DeleteDishAsync)
                .RequireAuthorization("RequiredAdminFromBelgium")
                .WithSummary("Delete a dish")
                .WithDescription("Deletes a dish. Requires the admin role and country Belgium")
                // Indica che questo endpoint può restituire un problema di validazione con codice 400
                .ProducesValidationProblem(400);
        }

        public static void RegisterIngredientsEndPoints(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            var ingredientsEndPoint = endpointRouteBuilder.MapGroup("dishes/{dishId:guid}/ingredients")
                .RequireAuthorization() // l'autorizzazione 
                .WithTags("Ingredients");

            ingredientsEndPoint.MapGet("", IngredientsHandlers.GetIngredientsAsync)
                .WithSummary("Get all ingredients")
                .WithDescription("Returns all ingredients, optionally filtered by name"); ;
        }
    }
}
