using AdventureWorks.BAL.IService;
using AdventureWorks.BAL.ResponseModel;
using AdventureWorks.DAL.Data;
using AdventureWorks.DAL.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        private readonly ISalesOrderHeaderService _salesOrderHeaderService;
        public CustomerService(dbContext context, IMapper mapper, IDbConnection dbConnection, IAddressService addressService, ISalesOrderHeaderService salesOrderHeaderService)
        {
            this.context = context;
            _mapper = mapper;
            _dbConnection = dbConnection;
            _addressService = addressService;
            _salesOrderHeaderService = salesOrderHeaderService;
        }
        public async Task<dynamic> GetDynamic(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0)
        {
            var retVal = await Get(fields, filters, include, sort, pageNo, pageSize);
            dynamic dynamicResponse = ResponseToDynamic.ConvertTo(retVal, fields);
            return dynamicResponse;
        }
        public async Task<List<CustomerResponse>> Get(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0)
        {

            IQueryable<CustomerResponse> result = context.Customers.Select(data => new CustomerResponse()
            {
                CustomerId = data.CustomerId,
                CompanyName = data.CompanyName,
                EmailAddress = data.EmailAddress,
                FirstName = data.FirstName,
                LastName = data.LastName,
                MiddleName = data.MiddleName,
                ModifiedDate = DateTime.Now,
                NameStyle = data.NameStyle,
                PasswordHash = data.PasswordHash,
                PasswordSalt = data.PasswordSalt,
                Phone = data.Phone,
                Rowguid = data.Rowguid,
                SalesPerson = data.SalesPerson,
                Suffix = data.Suffix,
                Title = data.Title,
                SalesOrderCount = data.SalesOrderHeaders.Count(),
            });

            List<CustomerAddressResponse> addressDetails = new List<CustomerAddressResponse>();
            List<SalesOrderHeaderResponse> salesOrders = new List<SalesOrderHeaderResponse>();

            var foundAddressFilter = false;
            var foundSalesOrderFilter = false;
            var includes = ResponseToDynamic.ParseIncludeParameter(include);
            var addressParts = new SubQueryParam();
            var salesorderParts = new SubQueryParam();

            /*Address Detail add*/
            if (includes.Any(x => x.objectName?.ToLower() == "customeraddresses"))
            {
                addressParts = includes.FirstOrDefault(x => x.objectName?.ToLower() == "customeraddresses") ?? new SubQueryParam();
                if (!string.IsNullOrEmpty(addressParts.filters))
                {
                    foundAddressFilter = true;
                    addressDetails = await _addressService.Get(addressParts.fields ?? "", addressParts.filters ?? "");
                }
            }

            if (addressDetails.Any() && foundAddressFilter)
            {
                filters = (string.IsNullOrEmpty(filters) ? "" : "(" + filters + ");") + $"customerid=in=({string.Join(",", addressDetails.Select(x => x.CustomerId).ToArray())})";
            }

            /*Sales Order Detail add*/
            if (includes.Any(x => x.objectName?.ToLower() == "salesorderheaders"))
            {
                salesorderParts = includes.FirstOrDefault(x => x.objectName?.ToLower() == "salesorderheaders") ?? new SubQueryParam();
                if (!string.IsNullOrEmpty(salesorderParts.filters))
                {
                    foundSalesOrderFilter = true;
                    salesOrders = await _salesOrderHeaderService.Get(salesorderParts.fields ?? "", salesorderParts.filters ?? "", salesorderParts.include ?? "");
                }
            }

            if (salesOrders.Any() && foundSalesOrderFilter)
            {
                filters = (string.IsNullOrEmpty(filters) ? "" : "(" + filters + ");") + $"customerid=in=({string.Join(",", salesOrders.Select(x => x.CustomerId).ToArray())})";
            }


            var customerResponse = await ResponseToDynamic.contextResponse(result, fields, filters, sort, pageNo, pageSize);
            List<CustomerResponse> retVal = (JsonSerializer.Deserialize<List<CustomerResponse>>(JsonSerializer.Serialize(customerResponse))) ?? new List<CustomerResponse>();


            if (addressDetails.Any() && foundAddressFilter)
            {
                if (!string.IsNullOrEmpty(filters))
                {
                    retVal = retVal.Where(x => addressDetails.Any(y => y.CustomerId == x.CustomerId)).ToList();
                }
            }
            if (salesOrders.Any() && foundSalesOrderFilter)
            {
                if (!string.IsNullOrEmpty(filters))
                {
                    retVal = retVal.Where(x => salesOrders.Any(y => y.CustomerId == x.CustomerId)).ToList();
                }
            }

            if (includes.Any(x => x.objectName?.ToLower() == "customeraddresses") && !foundAddressFilter && retVal.Any())
            {
                addressParts.filters = $"customerid=in=({string.Join(",", retVal.Select(x => x.CustomerId).ToArray())})";
                addressDetails = await _addressService.Get(addressParts.fields ?? "", addressParts.filters ?? "");
            }

            if (includes.Any(x => x.objectName?.ToLower() == "salesorderheaders") && !foundSalesOrderFilter && retVal.Any())
            {
                salesorderParts.filters = $"customerid=in=({string.Join(",", retVal.Select(x => x.CustomerId).ToArray())})";
                salesOrders = await _salesOrderHeaderService.Get(salesorderParts.fields ?? "", salesorderParts.filters ?? "", salesorderParts.include ?? "");
            }

            if ((addressDetails.Any() || salesOrders.Any()) && retVal.Any())
            {
                retVal.ForEach(x =>
                {
                    x.CustomerAddresses = addressDetails.Any() ?
                    ResponseToDynamic.ConvertTo(addressDetails.Where(y => y.CustomerId == x.CustomerId).ToList(), addressParts.fields ?? "") : null;
                    x.SalesOrderHeaders = salesOrders.Any() ?
                    ResponseToDynamic.ConvertTo(salesOrders.Where(y => y.CustomerId == x.CustomerId).ToList(), salesorderParts.fields ?? "") : null;
                });
            }

            return retVal;
        }
    }
}
