using AdventureWorks.BAL.IService;
using AdventureWorks.BAL.ResponseModel;
using AdventureWorks.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdventureWorks.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            this._productService = productService;
        }
        [HttpGet]
        public async Task<ActionResult<dynamic>> Get([FromQuery] QueryParam queryParam)
        {
            var response = await _productService.GetDynamic(queryParam.fields ?? "", queryParam.filters ?? "", queryParam.include ?? "", queryParam.sort ?? "", queryParam.pageno ?? 0, queryParam.pagesize ?? 0);
            return Ok(response);
        }
    }
}
