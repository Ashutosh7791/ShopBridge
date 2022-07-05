using ShopBridgeApp.Models;
using ShopBridgeApp.Service;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;

namespace ShopBridgeApp.Repository
{
    public class ProductRepository : IProductService
    {
        /// <summary>
        /// Create HTTPClient objetc to call API
        /// </summary>
        /// <returns></returns>
        public HttpClient GetHttpClient()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:62704/api/products/");
            //httpClient.BaseAddress = new Uri("http://localhost:1256/api/products/");
            httpClient.Timeout = TimeSpan.FromMinutes(10);
            return httpClient;
        }

        /// <summary>
        /// Call API to Get All Products
        /// </summary>
        /// <returns></returns>
        public async Task<List<Product>> GetProductList(int pageIndex, int pageSize, string shortColumnName, string sortDirection)
        {
            try
            {
                ProductPaging objProductPaging = new ProductPaging();
                objProductPaging.pageIndex = pageIndex;
                objProductPaging.pageSize = pageSize;
                objProductPaging.sortCol = shortColumnName;
                objProductPaging.sortDir = sortDirection;
                List<Product> lstProduct = new List<Product>();
                using (var httpClient = GetHttpClient())
                {
                    var response = httpClient.PostAsJsonAsync<ProductPaging>("GetProductList", objProductPaging);
                    var result = await response.Result.Content.ReadAsStringAsync();
                    lstProduct = JsonConvert.DeserializeObject<List<Product>>(result);
                }
                return lstProduct;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        
        /// <summary>
        /// Call API to get Product by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<Product> GetProduct(int Id)
        {
            try
            {
                Product getProduct = new Product();
                getProduct.Id = Id;
                using (var httpClient = GetHttpClient())
                {
                    var response = httpClient.PostAsJsonAsync<Product>("GetProduct", getProduct);
                    var result = await response.Result.Content.ReadAsStringAsync();
                    getProduct = JsonConvert.DeserializeObject<Product>(result);
                }
                return getProduct;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Call API to Add and Update products
        /// </summary>
        /// <param name="ObjProduct"></param>
        /// <returns></returns>
        public async Task<int> ProductOperation(Product ObjProduct)
        {
            try
            {
                int Id = 0;
                using (var httpClient = GetHttpClient())
                {
                    var response =  httpClient.PostAsJsonAsync<Product>("ProductOperation", ObjProduct);
                    var result = await response.Result.Content.ReadAsStringAsync();
                    Id = JsonConvert.DeserializeObject<int>(result);
                }
                return  Id;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}
