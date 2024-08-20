using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorks.BAL.ResponseModel
{
    public class ProductCategoryResponse
    {
        public int ProductCategoryId { get; set; }
        public int? ParentProductCategoryId { get; set; }
        public string? Name { get; set; }
        public Guid Rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
