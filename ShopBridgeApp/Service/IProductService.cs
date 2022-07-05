using ShopBridgeApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopBridgeApp.Service
{
    public interface IProductService
    {
        Task<List<Product>> GetProductList(int pageIndex, int pageSize, string shortColumnName, string sortDirection);
        Task<Product> GetProduct(int Id);
        Task<int> ProductOperation(Product ObjProduct);
    }
}
