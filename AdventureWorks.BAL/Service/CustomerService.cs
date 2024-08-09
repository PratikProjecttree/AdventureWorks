using AdventureWorks.BAL.IService;
using AdventureWorks.BAL.ResponseModel;
using AdventureWorks.DAL.Data;
using AdventureWorks.DAL.Models;
using AutoMapper;
using System.Data;
using System.Linq.Dynamic.Core;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace AdventureWorks.BAL.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly dbContext context;
        private readonly IMapper _mapper;
        private readonly IDbConnection _dbConnection;
        public CustomerService(dbContext context, IMapper mapper, IDbConnection dbConnection)
        {
            this.context = context;
            _mapper = mapper;
            _dbConnection = dbConnection;
        }
        public dynamic Get(string query)
        {
            var queryParts = query.Split('&');
            var filter = (queryParts.Where(x => x.StartsWith("filter=")).FirstOrDefault() ?? "").Replace("filter=", "");
            var select = (queryParts.Where(x => x.StartsWith("select=")).FirstOrDefault() ?? "").Replace("select=", "");
            var predicate = ConvertFiqlToLinq.FiqlToLinq(filter);
            IQueryable<Customer> result = context.Customers;
            var customerResponse = result.Select($"new ({select})").Where(predicate).ToDynamicList();

            var retVal = (JsonSerializer.Deserialize<List<CustomerResponse>>(JsonSerializer.Serialize(customerResponse))) ?? new List<CustomerResponse>();

            dynamic dynamic = ResponseToDynamic.ConvertTo(retVal, select);

            return dynamic;
        }
    }
}
