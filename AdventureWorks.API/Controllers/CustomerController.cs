using AdventureWorks.BAL.IService;
using AdventureWorks.BAL.ResponseModel;
using AdventureWorks.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using AdventureWorks.BAL.ODataToSqlConverter;

namespace AdventureWorks.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CustomerController : ODataController
    {
        private readonly ICustomerService customerService;
        public CustomerController(ICustomerService customerService)
        {
            this.customerService = customerService;
        }
        [HttpGet]
        [EnableQuery(MaxExpansionDepth = 4, MaxAnyAllExpressionDepth = 4)]
        public ActionResult<dynamic> Get(bool includeAddresses, bool includeSalesOrderHeaders)
        {
            var response = customerService.Get(includeAddresses, includeSalesOrderHeaders);
            return Ok(response);
        }

        [HttpGet("GetCustom")]
        [EnableQuery(MaxExpansionDepth = 4, MaxAnyAllExpressionDepth = 4)]
        public async Task<ActionResult<List<CustomerResponseCustom>>> GetCustomeQuery(ODataQueryOptions<CustomerResponseCustom> queryOptions, bool includeAddresses = false, bool includeSalesOrderHeaders = false)
        {
            var response = await customerService.GetCustomeQuery(queryOptions.Filter?.RawValue, queryOptions.OrderBy?.RawValue, includeAddresses, includeSalesOrderHeaders);
            return Ok(response);
        }
        [HttpGet("GetFromSqlRaw")]
        [EnableQuery(MaxExpansionDepth = 4, MaxAnyAllExpressionDepth = 4)]
        public async Task<ActionResult<List<CustomerResponseCustom>>> GetFromSqlRaw(ODataQueryOptions<CustomerResponse> queryOptions, bool includeAddresses = false, bool includeSalesOrderHeaders = false)
        {
            var response = await customerService.GetFromSqlRaw(queryOptions, includeAddresses, includeSalesOrderHeaders);
            return Ok(response);
        }
    }
}
