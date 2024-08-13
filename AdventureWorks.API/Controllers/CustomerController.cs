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
        private readonly ICustomerService customerService;
        public CustomerController(ICustomerService customerService)
        {
            this.customerService = customerService;
        }
        [HttpGet]
        public async Task<ActionResult<dynamic>> Get(string? query)
        {
            var response = await customerService.Get(query ?? "");
            return Ok(response);
        }
    }     
}
