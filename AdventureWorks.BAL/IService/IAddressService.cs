using AdventureWorks.BAL.ResponseModel;
using AdventureWorks.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorks.BAL.IService
{
    public interface IAddressService
    {
        Task<dynamic> GetDynamic(string query);
        Task<List<CustomerAddressResponse>>  Get(string query);
    }
}
