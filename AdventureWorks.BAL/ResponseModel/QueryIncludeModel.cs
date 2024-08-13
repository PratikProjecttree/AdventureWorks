using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureWorks.BAL.ResponseModel
{
    public class QueryIncludeModel
    {
        public string? objectName { get; set; }
        public string? objectQuery { get; set; }
        public string? objectFields { get; set; }
        public string? objectFilters { get; set; }
    }
}