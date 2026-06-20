namespace Business_Layer.DTOS;

public class StockInfoDto
{
    public int id { get; set; }
    public string? name { get; set; }
    public string? Category { get; set; }
    public string? Unit { get; set; }
    public decimal PurchasedAmount { get; set; }
    public decimal SoldAmount { get; set; }
    public decimal DamagedAmount { get; set; }
    public decimal ReturnedAmount { get; set; }
    public decimal AvailableStock { get; set; } = 0;
}
