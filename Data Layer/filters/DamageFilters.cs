
namespace Data_Layer.filters;

public class DamageFilters : Pagination
{
    public int Id { get; set; }

    public bool? IsIncludeActions { get; set; }
    
    public bool? IsIncludeInventoryBatch { get; set; }

    public int InventoryBatchId { get; set; }

    public int productId { get; set; }
}
