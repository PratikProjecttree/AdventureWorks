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
    public class SalesOrderHeaderController : ControllerBase
    {
        private readonly QueryTrackerService _queryTracker;
        private readonly ISalesOrderHeaderService _salesOrderHeaderService;
        public SalesOrderHeaderController(ISalesOrderHeaderService salesOrderHeaderService, QueryTrackerService queryTracker)
        {
            this._salesOrderHeaderService = salesOrderHeaderService;
            _queryTracker = queryTracker;
        }
        [HttpGet]
        public async Task<ActionResult<dynamic>> Get([FromQuery] QueryParam queryParam)
        {
            var response = await _salesOrderHeaderService.GetDynamic(queryParam.fields ?? "", queryParam.filters ?? "", queryParam.include ?? "", queryParam.sort ?? "", queryParam.pageno ?? 0, queryParam.pagesize ?? 0);
            return Ok(new
            {
                data = response,
                sqlQueryCount = _queryTracker.QueryCount
            });
        }
    }
}
