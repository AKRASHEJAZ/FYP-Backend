using Data_Layer.Entities;

namespace Business_Layer.DTOS;

public class ProductDto
{
    public ProductDto() 
    {
    
    }

    public ProductDto(Product p)
    {
        Id = p.Id;
        Category = new CategoryDto
        {
            Id = p.Category.Id,
            Name = p.Category.Name,
            CreatedAt = p.Category.CreatedAt
        };
        CategoryId = p.CategoryId;
        CreatedAt = p.CreatedAt;
        DoesExpire = p.DoesExpire;
        InternalCode = p.InternalCode;
        IsActive = p.IsActive;
        IsPurchasable = p.IsPurchasable;
        IsSellable = p.IsSellable;
        Name = p.Name;
        Unit = new UnitDto
        {
            Id = p.Unit.Id,
            Name = p.Unit.Name,
            Symbol = p.Unit.Symbol,
            CreatedAt = p.Unit.CreatedAt
        };
        UnitId = p.UnitId;
    }

    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int CategoryId { get; set; }

    public int UnitId { get; set; }

    public string? InternalCode { get; set; }

    public bool IsActive { get; set; }

    public bool IsSellable { get; set; }

    public bool IsPurchasable { get; set; }

    public bool DoesExpire { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigational Properties
    public CategoryDto? Category { get; set; }
    public UnitDto? Unit { get; set; }
}


public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}


public class UnitDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Symbol { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

}
