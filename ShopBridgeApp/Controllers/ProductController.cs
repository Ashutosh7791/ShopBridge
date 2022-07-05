using Microsoft.AspNetCore.Mvc;
using ShopBridgeApp.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using ShopBridgeApp.Service;

namespace ShopBridgeApp.Controllers
{
    public class ProductController : Controller
    {
        private IWebHostEnvironment Environment;

        private readonly IProductService productService;
        public ProductController(IProductService _productService, IWebHostEnvironment _environment)
        {
            Environment = _environment;
            productService = _productService;
        }

        public ActionResult ProductList()
        {           
            return View();
        }

        public async Task<ActionResult> DataTableProductList(int draw, int start, int length, string sortCol, string sortDir)
        {
            try
            {
                var page = (start / length) + 1;
                DataTableProduct dataTableData = new DataTableProduct();
                dataTableData.draw = draw;
                int recordsFiltered = 0;
                dataTableData.data = await productService.GetProductList(page, length, sortCol, sortDir);

                var TOTAL_ROWS = 0;
                if (dataTableData.data?.Count > 0)
                {
                    dataTableData.data = dataTableData.data;
                    TOTAL_ROWS = dataTableData.data[0].TotalRows;
                    dataTableData.recordsFiltered = dataTableData.data[0].TotalRows;
                }

                dataTableData.recordsTotal = TOTAL_ROWS;
                recordsFiltered = TOTAL_ROWS;

                return Json(dataTableData);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }      

        [HttpPost]
        public async Task<IActionResult> ProductOperation(IFormFile Imagefiles, string data)
        {
            int Id = -1; string OldFileName = string.Empty; Product CurrentProductDetail = null;
            try
            {
                string path = Path.Combine(this.Environment.WebRootPath, "Product Images");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                Product objProduct = JsonConvert.DeserializeObject<Product>(data);
                if (objProduct.Id == 0)
                {
                    objProduct.OperationType = 1;
                    if (Imagefiles != null)
                    {
                        objProduct.FileName = objProduct.ProductName + Path.GetExtension(Imagefiles?.FileName);
                    }
                }
                else
                {
                    objProduct.OperationType = 2;
                    CurrentProductDetail = await productService.GetProduct(objProduct.Id);
                    if (Imagefiles != null)
                    {
                        objProduct.FileName = objProduct.ProductName + Path.GetExtension(Imagefiles?.FileName);
                    }
                    else
                    {
                        if (CurrentProductDetail.FileName?.Split(".")[0].ToLower() != objProduct.ProductName.ToLower())
                        {
                            objProduct.FileName = objProduct.ProductName + "." + CurrentProductDetail.FileName?.Split(".")[1];
                        }
                    }
                }

                //Insert or Update Product details
                Id = await productService.ProductOperation(objProduct);

                //Insert Product Image
                if (Imagefiles != null && Id > 0 && objProduct.OperationType == 1)
                {
                    if (!string.IsNullOrEmpty(objProduct.FileName))
                    {
                        using (FileStream stream = new FileStream(Path.Combine(path, objProduct.FileName), FileMode.Create))
                        {
                            Imagefiles.CopyTo(stream);
                        }
                    }
                }

                //Update Product Image
                if (objProduct.Id > 0)
                {
                    OldFileName = CurrentProductDetail?.FileName;

                    //Change file old name with new name
                    if (Imagefiles == null && OldFileName?.Split(".")[0].ToLower() != objProduct.ProductName.ToLower())
                    {
                        string filePath = Path.Combine(path, OldFileName);
                        string OldFileExt = Path.GetExtension(OldFileName);
                        System.IO.FileInfo fi = new System.IO.FileInfo(filePath);
                        if (fi.Exists)
                        {
                            // Move file with a new name. Hence renamed.  
                            fi.MoveTo(Path.Combine(path, objProduct.ProductName + OldFileExt));
                        }
                    }

                    //Change old image with new image
                    if (Imagefiles != null && Id > 0 && objProduct.OperationType == 2)
                    {
                        if (!string.IsNullOrEmpty(OldFileName))
                        {
                            string filePath = Path.Combine(path, OldFileName);
                            System.IO.File.Delete(filePath);
                        }

                        if (!string.IsNullOrEmpty(objProduct.FileName))
                        {
                            using (FileStream newstream = new FileStream(Path.Combine(path, objProduct.FileName), FileMode.Create))
                            {
                                Imagefiles.CopyTo(newstream);
                            }
                        }
                    }
                }
                return Json(Id);
            }
            catch (Exception)
            {
                return Json(Id);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetProduct(int Id)
        {
            try
            {
                Product objProduct = await productService.GetProduct(Id);
                return Json(objProduct);
            }
            catch (Exception)
            {
                return Json(null);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int Id)
        {
            try
            {
                Product objProduct = new Product();
                objProduct.Id = Id;
                objProduct.OperationType = 3;
                Id = await productService.ProductOperation(objProduct);

                //Remove Product Image
                string path = Path.Combine(this.Environment.WebRootPath, "Product Images");
                var GetProductDetail = await productService.GetProduct(objProduct.Id);
                if (!string.IsNullOrEmpty(GetProductDetail.FileName))
                {
                    string filePath = Path.Combine(path, GetProductDetail.FileName);
                    System.IO.File.Delete(filePath);
                }

                return Json(Id);
            }
            catch (Exception)
            {
                return Json(-1);
            }
        }
    }
}
