
namespace Data_Layer.filters;

public class SaleFilters : Pagination
{
    public int Id { get; set; }

    public int CustomerId { get; set; }
    
    public bool? IsIncludeCustomer { get; set; }

    public bool? IsIncludeUser { get; set; }

    public bool? IsInculdeActions { get; set; }
    
    public bool? IsIncludeInventoryBatch { get; set; }
}
