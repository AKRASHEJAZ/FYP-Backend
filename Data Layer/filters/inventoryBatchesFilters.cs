namespace Data_Layer.filters;

public class InventoryBatchFilters
{
    public IList<int>? Id { get; set; }

    public IList<int>? ProductId { get; set; }

    public IList<string>? BatchCode { get; set; }
    public DateOnly? ExpiryDate { get; set; }

}