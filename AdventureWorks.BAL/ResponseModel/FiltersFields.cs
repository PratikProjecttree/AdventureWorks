using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureWorks.BAL.ResponseModel
{
    public class FiltersAndProperties
    {
        public string filters { get; set; } = string.Empty;
        public List<string> properties { get; set; } = new List<string>();
    }
}