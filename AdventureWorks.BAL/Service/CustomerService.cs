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
            List<CustomerAddressResponse> addressDetails = new List<CustomerAddressResponse>();

            var foundAddressFilter = false;
            var includes = ResponseToDynamic.getInclude(query);
            var customerFilters = ResponseToDynamic.getFilters(query);
            var addressParts = new QueryIncludeModel();

            if (includes.Any(x => x.objectName?.ToLower() == "customeraddresses"))
            {
                addressParts = includes.FirstOrDefault(x => x.objectName?.ToLower() == "customeraddresses") ?? new QueryIncludeModel();
                if (!string.IsNullOrEmpty(addressParts.objectFilters))
                {
                    foundAddressFilter = true;
                    addressDetails = await _addressService.Get(addressParts.objectQuery ?? "");
                }
            }

            if (addressDetails.Any() && foundAddressFilter)
            {
                if (string.IsNullOrEmpty(customerFilters))
                {
                    query = query + $"&filters=customerid=in=({string.Join(",", addressDetails.Select(x => x.CustomerId).ToArray())})";
                }
            }

            var customerResponse = await ResponseToDynamic.contextResponse(result, query);
            List<CustomerResponse> retVal = (JsonSerializer.Deserialize<List<CustomerResponse>>(JsonSerializer.Serialize(customerResponse))) ?? new List<CustomerResponse>();


            if (addressDetails.Any() && foundAddressFilter)
            {
                if (!string.IsNullOrEmpty(customerFilters))
                {
                    retVal = retVal.Where(x => addressDetails.Any(y => y.CustomerId == x.CustomerId)).ToList();
                }
            }

            if (includes.Any(x => x.objectName?.ToLower() == "customeraddresses") && !foundAddressFilter && retVal.Any())
            {
                addressParts = includes.FirstOrDefault(x => x.objectName?.ToLower() == "customeraddresses") ?? new QueryIncludeModel();
                var addressQuery = (!string.IsNullOrEmpty(addressParts.objectQuery) ? addressParts.objectQuery + "&" : "") + $"filters=customerid=in=({string.Join(",", retVal.Select(x => x.CustomerId).ToArray())})";
                addressDetails = await _addressService.Get(addressQuery);
            }

            if (addressDetails.Any() && retVal.Any())
            {
                retVal.ForEach(x =>
                {
                    x.CustomerAddresses =
                    ResponseToDynamic.ConvertTo(addressDetails.Where(y => y.CustomerId == x.CustomerId).ToList(), addressParts.objectFields ?? "");
                });
            }


            dynamic dynamicResponse = ResponseToDynamic.ConvertTo(retVal, ResponseToDynamic.getFields(query));

            return dynamicResponse;
        }
    }
}
