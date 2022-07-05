
namespace ShopBridgeAPI.Model
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string FileName { get; set; }
        public int OperationType { get; set; }
        public int TotalRows { get; set; }
    }

    public class ProductPaging
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public string sortCol { get; set; }
        public string sortDir { get; set; }
    }
}
