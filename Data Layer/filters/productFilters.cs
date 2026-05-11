
namespace Data_Layer.filters;

public class ProductFilters
{
    public IList<int>? Id { get; set; }

    public IList<string>? Name { get; set; }

    public IList<int>? CategoryId { get; set; }

    public IList<int>? UnitId { get; set; }

    public IList<string>? InternalCode { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsSellable { get; set; }

    public bool? IsPurchasable { get; set; }

    public bool? DoesExpire { get; set; }
}
