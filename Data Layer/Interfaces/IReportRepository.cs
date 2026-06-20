using Business_Layer.DTOS;
using Data_Layer.commons;
using Data_Layer.Entities;
using Data_Layer.filters;

namespace Data_Layer.Interfaces;

public interface IReportRepository
{
    Task<CountData> GetCountData();
    Task<PaginatedResult<InventoryBatch>> GetExpiredBatches(ExpiryReportFilters filters);
}
