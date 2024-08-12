using AdventureWorks.BAL.IService;
using AdventureWorks.BAL.ResponseModel;
using AdventureWorks.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdventureWorks.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService AddressService;
        public AddressController(IAddressService AddressService)
        {
            this.AddressService = AddressService;
        }
        [HttpGet("{query?}")]
        public async Task<ActionResult<dynamic>> Get(string? query)
        {
            var response = await AddressService.GetDynamic(query ?? "");
            return Ok(response);
        }
    }     
}
