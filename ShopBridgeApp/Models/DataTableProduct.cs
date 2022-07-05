using System.Collections.Generic;

namespace ShopBridgeApp.Models
{
    public class DataTableProduct
    {
        /// <summary>
        /// Gets or sets the draw.
        /// </summary>
        /// <value>The draw.</value>
        public int draw { get; set; }

        /// <summary>
        /// Gets or sets the records total.
        /// </summary>
        /// <value>The records total.</value>
        public int recordsTotal { get; set; }

        /// <summary>
        /// Gets or sets the records filtered.
        /// </summary>
        /// <value>The records filtered.</value>
        public int recordsFiltered { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public List<Product> data { get; set; }
    }
}
