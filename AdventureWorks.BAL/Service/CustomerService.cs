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
        private readonly IAddressService _addressService;
        public CustomerService(dbContext context, IMapper mapper, IDbConnection dbConnection, IAddressService addressService)
        {
            this.context = context;
            _mapper = mapper;
            _dbConnection = dbConnection;
            _addressService = addressService;
        }
        public async Task<dynamic> Get(string query)
        {
            IQueryable<Customer> result = context.Customers;
            var customerResponse = await ResponseToDynamic.contextResponse(result, query);

            List<CustomerResponse> retVal = (JsonSerializer.Deserialize<List<CustomerResponse>>(JsonSerializer.Serialize(customerResponse))) ?? new List<CustomerResponse>();

            var includes = ResponseToDynamic.getInclude(query);

            var addressQuery = includes.FirstOrDefault().Value + $"&filters=customerid=in=({string.Join(",", retVal.Select(x => x.CustomerId).ToArray())})";
            var addressDetails = await _addressService.Get(addressQuery);
            retVal.ForEach(x =>
            {
                x.CustomerAddresses = 
                ResponseToDynamic.ConvertTo(addressDetails.Where(y => y.CustomerId == x.CustomerId).ToList(), ResponseToDynamic.getFields(includes.FirstOrDefault().Value));
            });
            dynamic dynamicResponse = ResponseToDynamic.ConvertTo(retVal, ResponseToDynamic.getFields(query));

            return dynamicResponse;
        }
    }
}
