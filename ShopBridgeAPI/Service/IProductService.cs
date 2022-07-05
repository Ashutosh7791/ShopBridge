using ShopBridgeAPI.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopBridgeAPI.Service
{
    public interface IProductService
    {
        Task<List<Product>> GetProductList(ProductPaging objProductPaging);
        Task<Product> GetProduct(int Id);
        Task<int> ProductOperation(Product ObjProduct);
    }
}
