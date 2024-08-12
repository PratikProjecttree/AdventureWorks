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
    public class AddressService : IAddressService
    {
        private readonly dbContext context;
        private readonly IMapper _mapper;
        private readonly IDbConnection _dbConnection;
        public AddressService(dbContext context, IMapper mapper, IDbConnection dbConnection)
        {
            this.context = context;
            _mapper = mapper;
            _dbConnection = dbConnection;
        }
        public async Task<dynamic> GetDynamic(string query)
        {
            var retVal = await Get(query);
            dynamic dynamicResponse = ResponseToDynamic.ConvertTo(retVal, ResponseToDynamic.getFields(query));
            return dynamicResponse;
        }

        public async Task<List<CustomerAddressResponse>> Get(string query)
        {
            IQueryable<CustomerAddressResponse> result = context.CustomerAddresses.Include(x => x.Address)
                              .Select(data => new CustomerAddressResponse()
                              {
                                  CustomerId = data.CustomerId,
                                  AddressId = data.AddressId,

                                  AddressLine1 = data.Address.AddressLine1,
                                  AddressLine2 = data.Address.AddressLine2,
                                  City = data.Address.City,
                                  CountryRegion = data.Address.CountryRegion,
                                  ModifiedDate = data.Address.ModifiedDate,
                                  PostalCode = data.Address.PostalCode,
                                  Rowguid = data.Address.Rowguid,
                                  StateProvince = data.Address.StateProvince,
                              });

            var addressResponse = await ResponseToDynamic.contextResponse(result, query);
            var retVal = (JsonSerializer.Deserialize<List<CustomerAddressResponse>>(JsonSerializer.Serialize(addressResponse))) ?? new List<CustomerAddressResponse>();
            return retVal;
        }
    }
}
