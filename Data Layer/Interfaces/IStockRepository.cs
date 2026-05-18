
using Data_Layer.commons;
using Data_Layer.Entities;
using Data_Layer.filters;

namespace Data_Layer.Interfaces;

public interface IInventoryBatchRepository
{
    Task<PaginatedResult<InventoryBatch>> GetAllInventoryBatchesAsync(InventoryBatchFilters filters);
    Task<InventoryBatch?> GetInventoryBatchByIdAsync(int id);
    Task AddInventoryBatchAsync(InventoryBatch inventoryBatch);
}
