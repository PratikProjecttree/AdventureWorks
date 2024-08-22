using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureWorks.DAL.Data
{
    public class QueryTrackerService
    {
        public int QueryCount { get; private set; }

        public void Increment()
        {
            QueryCount++;
        }

        public void Reset()
        {
            QueryCount = 0;
        }
    }
}