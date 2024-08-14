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
    public class ProductService : IProductService
    {
        private readonly dbContext context;
        private readonly IMapper _mapper;
        private readonly IDbConnection _dbConnection;
        public ProductService(dbContext context, IMapper mapper, IDbConnection dbConnection)
        {
            this.context = context;
            _mapper = mapper;
            _dbConnection = dbConnection;
        }
        public async Task<dynamic> GetDynamic(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0)
        {
            var retVal = await Get(fields, filters, include, sort, pageNo, pageSize);
            dynamic dynamicResponse = ResponseToDynamic.ConvertTo(retVal, fields);
            return dynamicResponse;
        }

        public async Task<List<ProductResponse>> Get(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0)
        {
            IQueryable<ProductResponse> result = context.Products
                                                .Select(data => new ProductResponse()
                                                {
                                                    ProductId = data.ProductId,
                                                    Name = data.Name,
                                                    ProductNumber = data.ProductNumber,
                                                    Color = data.Color,
                                                    StandardCost = data.StandardCost,
                                                    ListPrice = data.ListPrice,
                                                    Size = data.Size,
                                                    Weight = data.Weight,
                                                    ProductCategory = data.ProductCategory.Name,
                                                    ProductModel = data.ProductModel.Name,
                                                    SellStartDate = data.SellStartDate,
                                                    SellEndDate = data.SellEndDate,
                                                    DiscontinuedDate = data.DiscontinuedDate,
                                                    ThumbnailPhotoFileName = data.ThumbnailPhotoFileName,
                                                    Rowguid = data.Rowguid,
                                                    ModifiedDate = data.ModifiedDate,
                                                });


            var productResponse = await ResponseToDynamic.contextResponse(result, fields, filters, sort, pageNo, pageSize);
            var retVal = (JsonSerializer.Deserialize<List<ProductResponse>>(JsonSerializer.Serialize(productResponse))) ?? new List<ProductResponse>();
            return retVal;
        }
    }
}
