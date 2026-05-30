
using Data_Layer.commons;
using Data_Layer.Entities;

namespace Business_Layer.DTOS;

public class InventoryBatchDto
{
    public InventoryBatchDto()
    {
    }

    public InventoryBatchDto(InventoryBatch b)
    {
        Id = b.Id;
        ProductId = b.ProductId;
        BatchCode = b.BatchCode;

        PurchasePrice = b.PurchasePrice;
        SellingPrice = b.SellingPrice;

        MFGDate = b.Mfgdate;
        ExpiryDate = b.ExpiryDate;

        CreatedAt = b.CreatedAt;

        Product = b.Product != null
            ? new ProductDto(b.Product)
            : null;
        PurchasedQuantity = b.Quantity;
    }

    public int Id { get; set; }

    public int ProductId { get; set; }

    public string? BatchCode { get; set; }

    public decimal PurchasePrice { get; set; }

    public decimal SellingPrice { get; set; }

    public decimal PurchasedQuantity { get; set; }

    public DateOnly? MFGDate { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public DateTime CreatedAt { get; set; }
    public InventoryBatchStock? Stocks { get; set; }

    // Navigation
    public ProductDto? Product { get; set; }
}

public class AddInventoryBatchDto
{
    public int ProductId { get; set; }

    public decimal PurchasePrice { get; set; }

    public decimal SellingPrice { get; set; }

    public decimal PurchasedQuantity { get; set; }

    public DateOnly? MFGDate { get; set; }

    public DateOnly? ExpiryDate { get; set; }
}