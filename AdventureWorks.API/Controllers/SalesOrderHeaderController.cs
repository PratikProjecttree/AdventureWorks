using AdventureWorks.BAL.IService;
using AdventureWorks.BAL.ResponseModel;
using AdventureWorks.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdventureWorks.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SalesOrderHeaderController : ControllerBase
    {
        private readonly ISalesOrderHeaderService _salesOrderHeaderService;
        public SalesOrderHeaderController(ISalesOrderHeaderService salesOrderHeaderService)
        {
            this._salesOrderHeaderService = salesOrderHeaderService;
        }
        [HttpGet]
        public async Task<ActionResult<dynamic>> Get([FromQuery] QueryParam queryParam)
        {
            var response = await _salesOrderHeaderService.GetDynamic(queryParam.fields ?? "", queryParam.filters ?? "", queryParam.include ?? "", queryParam.sort ?? "", queryParam.pageno ?? 0, queryParam.pagesize ?? 0);
            return Ok(response);
        }
    }
}
