﻿using AdventureWorks.BAL.IService;
using AdventureWorks.BAL.ResponseModel;
using AdventureWorks.DAL.Data;
using AdventureWorks.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdventureWorks.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;
        public AddressController(IAddressService addressService, QueryTrackerService queryTracker)
        {
            this._addressService = addressService;
        }
        [HttpGet]
        public async Task<ActionResult<dynamic>> Get([FromQuery] QueryParam queryParam)
        {
            var response = await _addressService.GetDynamic(queryParam.fields ?? "", queryParam.filters ?? "", queryParam.include ?? "", queryParam.sort ?? "", queryParam.pageno ?? 0, queryParam.pagesize ?? 0);
            return Ok(response);
        }
    }
}
