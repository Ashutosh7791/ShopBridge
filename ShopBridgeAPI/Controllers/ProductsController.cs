using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShopBridgeAPI.Model;
using ShopBridgeAPI.Service;

namespace ShopBridgeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService productService;
        public ProductsController(IProductService _productService)
        {
            productService = _productService;
        }

        [HttpPost]
        [Route("GetProductList")]
        public async Task<ActionResult> GetProductList(ProductPaging objProductPaging)
        {
            try
            {
                List<Product> lstProduct = await productService.GetProductList(objProductPaging);
                return Ok(lstProduct);
            }
            catch (Exception)
            {
                return Ok(null);
            }
        }

        [HttpPost]
        [Route("GetProduct")]
        public async Task<ActionResult> GetProduct(Product objProduct)
        {
            try
            {
                Product Product = await productService.GetProduct(objProduct.Id);
                return Ok(Product);
            }
            catch (Exception)
            {
                return Ok(null);
            }
        }

        [HttpPost]
        [Route("ProductOperation")]
        public async Task<IActionResult> ProductOperation(Product objProduct)
        {
            try
            {
                int Id = await productService.ProductOperation(objProduct);
                return Ok(Id);
            }
            catch (Exception)
            {
                return Ok(-1);
            }
        }
    }
}