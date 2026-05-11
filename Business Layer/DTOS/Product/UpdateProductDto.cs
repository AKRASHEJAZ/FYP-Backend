namespace Business_Layer.DTOS;

public class UpdateProductDto
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? CategoryId { get; set; }

    public int? UnitId { get; set; }

    public string? InternalCode { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsSellable { get; set; }

    public bool? IsPurchasable { get; set; }

    public bool? DoesExpire { get; set; }
}


public class UpdateCategoryDto
{
    public string? Name { get; set; }
}


public class UpdateUnitDto
{
    public string? Name { get; set; }
    public string? Symbol { get; set; }

}
