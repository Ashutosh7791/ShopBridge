using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using ShopBridgeAPI.Model;
using ShopBridgeAPI.Service;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ShopBridgeAPI.Repository
{
    public class ProductService : IProductService
    {
        public IDbConnection DbConnection { get; set; }
        public IConfiguration Configuration { get; }

        public ProductService(IConfiguration configuration)
        {
            Configuration = configuration;
            DbConnection = new SqlConnection(Configuration.GetConnectionString("ShopBridgeDBConnection"));
        }

        public void Dispose()
        {
            DbConnection?.Dispose();
        }

        /// <summary>
        /// Get All ProductList from Database
        /// </summary>
        /// <param name="objProductPaging"></param>
        /// <returns></returns>
        public async Task<List<Product>> GetProductList(ProductPaging objProductPaging)
        {
            try
            {
                var objDynamicParameters = new DynamicParameters();
                objDynamicParameters.Add("@PageIndex", objProductPaging.pageIndex);
                objDynamicParameters.Add("@PageSize", objProductPaging.pageSize);
                objDynamicParameters.Add("@SortCol", objProductPaging.sortCol);
                objDynamicParameters.Add("@SortDir", objProductPaging.sortDir);
                List<Product> lstProduct = new List<Product>();
                dynamic lstProducts = await DbConnection.QueryAsync<Product>("GetProductList", objDynamicParameters, commandType: CommandType.StoredProcedure);
                return lstProduct = lstProducts;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Get Product By Id from database
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<Product> GetProduct(int Id)
        {
            try
            {
                var objDynamicParameters = new DynamicParameters();
                objDynamicParameters.Add("@Id", Id);
                dynamic Product = await DbConnection.QueryFirstOrDefaultAsync<Product>("GetProduct", objDynamicParameters, commandType: CommandType.StoredProcedure);
                Product objProduct = Product;
                return objProduct;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Product operation from Database Insert,Update,Delete
        /// </summary>
        /// <param name="objProduct"></param>
        /// <returns></returns>
        public async Task<int> ProductOperation(Product objProduct)
        {
            try
            {
                var objDynamicParameters = new DynamicParameters();
                objDynamicParameters.Add("@Id", objProduct.Id);
                objDynamicParameters.Add("@ProductName", objProduct.ProductName);
                objDynamicParameters.Add("@Description", objProduct.Description);
                objDynamicParameters.Add("@Price", objProduct.Price);
                objDynamicParameters.Add("@FileName", objProduct.FileName);
                objDynamicParameters.Add("@OperationType", objProduct.OperationType);
                List<Product> lstProduct = new List<Product>();
                return await DbConnection.ExecuteScalarAsync<int>("ProductOperation", objDynamicParameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
