namespace Business_Layer.DTOS;

public class AddInventoryActionDto
{
    public int InventoryBatchId { get; set; }
    public decimal Quantity { get; set; }
    public string? Notes { get; set; }
}


public class AddSaleDto
{
    public int CustomerId { get; set; }

    public IList<AddInventoryActionDto> InventoryActions { get; set; } = new List<AddInventoryActionDto>();
}

public class AddDamageDto
{
    public IList<AddInventoryActionDto> InventoryActions { get; set; } = new List<AddInventoryActionDto>();
}