namespace Data_Layer.filters;

public class InventoryActionFilters : Pagination
{
    public int referenceId { get; set; }
   
    public bool includeBatch { get; set; }

    public int batchId { get; set; }
}
