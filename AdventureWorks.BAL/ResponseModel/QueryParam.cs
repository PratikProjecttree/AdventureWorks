using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureWorks.BAL.ResponseModel
{
    public class QueryParam
    {
        public string? fields { get; set; }
        public string? filters { get; set; }
        public string? include { get; set; }
        public string? sort { get; set; }
        public int? pageno { get; set; }
        public int? pagesize { get; set; }
    }

    public class SubQueryParam
    {
        public string? objectName { get; set; }
        public string? fields { get; set; }
        public string? filters { get; set; }
        public string? include { get; set; }
    }
}