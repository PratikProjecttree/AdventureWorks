using AdventureWorks.BAL.ResponseModel;
using AdventureWorks.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorks.BAL.IService
{
    public interface IProductService
    {
        Task<dynamic> GetDynamic(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0);
        Task<List<ProductResponse>> Get(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0);
    }
}
