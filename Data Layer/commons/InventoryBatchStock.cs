namespace Data_Layer.commons;

public class InventoryBatchStock
{
    public int BatchId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Sold { get; set; } = 0;
    public decimal Damaged { get; set; } = 0;
    public decimal Returned { get; set; } = 0;
    public decimal AvailableStock => Quantity - Sold - Damaged + Returned;
}
