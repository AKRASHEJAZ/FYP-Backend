using Data_Layer.commons;
using Data_Layer.Entities;
using Data_Layer.filters;

namespace Data_Layer.Interfaces;

public interface IInventoryActionRepository
{
    // Sales Method
    Task CreateSaleAsync(Sale newSale, IList<InventoryAction> inventoryActions);
    Task<PaginatedResult<Sale>> GetAllSaleAsync(SaleFilters filters);
    Task<IList<InventoryAction>> GetInventoryActionsAsync(int referenceId, bool includeBatch);
}
