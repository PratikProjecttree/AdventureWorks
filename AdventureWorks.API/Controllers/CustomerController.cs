using System.Text.Json;
using System.Text.Json.Serialization;
using AdventureWorks.BAL.IService;
using AdventureWorks.BAL.ResponseModel;
using AdventureWorks.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdventureWorks.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            this._customerService = customerService;
        }
        [HttpGet]
        public async Task<ActionResult<dynamic>> Get([FromQuery] QueryParam queryParam)
        {
            var response = await _customerService.GetDynamic(queryParam.fields ?? "", queryParam.filters ?? "", queryParam.include ?? "", queryParam.sort ?? "", queryParam.pageno ?? 0, queryParam.pagesize ?? 0);
            return Ok(response);
        }
    }


}
