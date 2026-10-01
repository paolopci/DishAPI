using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DishesAPI.Entities;

public class Dish
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public required string Name { get; set; }

    public ICollection<Ingredient> Ingredients { get; set; } = [];


    // The parameterless constructor is required by EF Core for materialization of the entity.
    public Dish()
    {
    }

    // The constructor with parameters is used to create instances of the entity with required properties.
    [SetsRequiredMembers]
    public Dish(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}
