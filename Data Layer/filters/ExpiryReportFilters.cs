
namespace Data_Layer.filters;

public class ExpiryReportFilters : Pagination
{
    public DateTime Date { get; set; }
    public IList<int> ProductIds { get ; set; } = new List<int>();
}
