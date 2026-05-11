using Data_Layer.Entities;

using Data_Layer.filters;

namespace Data_Layer.Interfaces;
public interface IProductRepository
{

    // Categories
    Task<Category?> GetCategoryByIdAsync(int id);
    Task<IList<Category>> GetAllCategoriesAsync(CategoryFilters filters);
    Task AddCategoryAsync(Category category);
    Task UpdateCategoryAsync(int id, Category category);
    Task DeleteCategoryAsync(int id);

    // Units
    Task<Unit?> GetUnitByIdAsync(int id);
    Task<IList<Unit>> GetAllUnitsAsync(UnitFilters filters);
    Task AddUnitAsync(Unit unit);
    Task UpdateUnitAsync(int id, Unit unit);
    Task DeleteUnitAsync(int id);

    // Products
    Task<IList<Product>> GetAllProductsAsync(ProductFilters filters);
    Task<Product?> GetProductByIdAsync(int id);
    Task AddProductAsync(Product product);
    Task UpdateProductAsync(int id, Product product);
    Task DeleteProductAsync(int id);

}
