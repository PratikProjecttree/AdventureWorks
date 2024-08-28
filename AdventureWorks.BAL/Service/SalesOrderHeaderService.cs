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
    public class SalesOrderHeaderService : ISalesOrderHeaderService
    {
        private readonly dbContext context;
        private readonly IMapper _mapper;
        private readonly IDbConnection _dbConnection;
        private readonly IProductService _productService;
        public SalesOrderHeaderService(dbContext context, IMapper mapper, IDbConnection dbConnection, IProductService productService)
        {
            this.context = context;
            _mapper = mapper;
            _dbConnection = dbConnection;
            _productService = productService;
        }
        public async Task<dynamic> GetDynamic(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0)
        {
            var retVal = await Get(fields, filters, include, sort, pageNo, pageSize);
            dynamic dynamicResponse = ResponseToDynamic.ConvertTo(retVal, fields);
            return dynamicResponse;
        }

        public async Task<List<SalesOrderHeaderResponse>> Get(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0)
        {
            IQueryable<SalesOrderHeaderResponse> result = context.SalesOrderHeaders
                                                            .Select(data => new SalesOrderHeaderResponse()
                                                            {
                                                                SalesOrderId = data.SalesOrderId,
                                                                RevisionNumber = data.RevisionNumber,
                                                                OrderDate = data.OrderDate,
                                                                DueDate = data.DueDate,
                                                                ShipDate = data.ShipDate,
                                                                Status = data.Status,
                                                                OnlineOrderFlag = data.OnlineOrderFlag,
                                                                SalesOrderNumber = data.SalesOrderNumber,
                                                                PurchaseOrderNumber = data.PurchaseOrderNumber,
                                                                AccountNumber = data.AccountNumber,
                                                                CustomerId = data.CustomerId,
                                                                ShipToAddressId = data.ShipToAddressId,
                                                                BillToAddressId = data.BillToAddressId,
                                                                ShipMethod = data.ShipMethod,
                                                                CreditCardApprovalCode = data.CreditCardApprovalCode,
                                                                SubTotal = data.SubTotal,
                                                                TaxAmt = data.TaxAmt,
                                                                Freight = data.Freight,
                                                                TotalDue = data.TotalDue,
                                                                Comment = data.Comment,
                                                                Rowguid = data.Rowguid,
                                                                ModifiedDate = data.ModifiedDate
                                                            });
            var foundSalesOrderFilter = false;
            var includes = ResponseToDynamic.ParseIncludeParameter(include);
            var salesorderDetailParts = new SubQueryParam();
            List<SalesOrderDetailResponse> salesOrderDetails = new List<SalesOrderDetailResponse>();

            if (includes.Any(x => x.objectName?.ToLower() == "salesorderdetails"))
            {
                salesorderDetailParts = includes.FirstOrDefault(x => x.objectName?.ToLower() == "salesorderdetails") ?? new SubQueryParam();
                if (!string.IsNullOrEmpty(salesorderDetailParts.filters))
                {
                    foundSalesOrderFilter = true;
                    salesOrderDetails = await GetSalesOrderDetail(salesorderDetailParts.fields ?? "", salesorderDetailParts.filters ?? "", salesorderDetailParts.include ?? "");
                }
            }

            if (salesOrderDetails.Any() && foundSalesOrderFilter)
            {
                filters = (string.IsNullOrEmpty(filters) ? "" : "(" + filters + ");") + $"salesorderid=in=({string.Join(",", salesOrderDetails.Select(x => x.SalesOrderId).ToArray())})";
            }

            var SalesOrderHeaderResponse = await ResponseToDynamic.contextResponse(result, fields, filters, sort, pageNo, pageSize);
            List<SalesOrderHeaderResponse> retVal = (JsonSerializer.Deserialize<List<SalesOrderHeaderResponse>>(JsonSerializer.Serialize(SalesOrderHeaderResponse))) ?? new List<SalesOrderHeaderResponse>();

            if (salesOrderDetails.Any() && foundSalesOrderFilter)
            {
                if (!string.IsNullOrEmpty(filters))
                {
                    retVal = retVal.Where(x => salesOrderDetails.Any(y => y.SalesOrderId == x.SalesOrderId)).ToList();
                }
            }

            if (includes.Any(x => x.objectName?.ToLower() == "salesorderdetails") && !foundSalesOrderFilter && retVal.Any())
            {
                salesorderDetailParts.filters = $"salesorderid=in=({string.Join(",", retVal.Select(x => x.SalesOrderId).ToArray())})";
                salesOrderDetails = await GetSalesOrderDetail(salesorderDetailParts.fields ?? "", salesorderDetailParts.filters ?? "", salesorderDetailParts.include ?? "");
            }

            if (salesOrderDetails.Any() && retVal.Any())
            {
                retVal.ForEach(x =>
                {
                    x.SalesOrderDetails = ResponseToDynamic.ConvertTo(salesOrderDetails.Where(y => y.SalesOrderId == x.SalesOrderId).ToList(), salesorderDetailParts.fields ?? "");
                });
            }

            return retVal;
        }

        public async Task<List<SalesOrderDetailResponse>> GetSalesOrderDetail(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0)
        {
            IQueryable<SalesOrderDetailResponse> result = context.SalesOrderDetails
                                                          .Select(data => new SalesOrderDetailResponse()
                                                          {
                                                              SalesOrderId = data.SalesOrderId,
                                                              SalesOrderDetailId = data.SalesOrderDetailId,
                                                              OrderQty = data.OrderQty,
                                                              ProductId = data.ProductId,
                                                              UnitPrice = data.UnitPrice,
                                                              UnitPriceDiscount = data.UnitPriceDiscount,
                                                              LineTotal = data.LineTotal,
                                                              Rowguid = data.Rowguid,
                                                              ModifiedDate = data.ModifiedDate,
                                                          });


            var foundProductDetailFilter = false;
            var includes = ResponseToDynamic.ParseIncludeParameter(include);
            var productDetailParts = new SubQueryParam();
            List<ProductResponse> productDetail = new List<ProductResponse>();

            if (includes.Any(x => x.objectName?.ToLower() == "product"))
            {
                productDetailParts = includes.FirstOrDefault(x => x.objectName?.ToLower() == "product") ?? new SubQueryParam();
                if (!string.IsNullOrEmpty(productDetailParts.filters))
                {
                    foundProductDetailFilter = true;
                    productDetail = await _productService.Get(productDetailParts.fields ?? "", productDetailParts.filters ?? "", productDetailParts.include ?? "");
                }
            }

            if (productDetail.Any() && foundProductDetailFilter)
            {
                filters = (string.IsNullOrEmpty(filters) ? "" : "(" + filters + ");") + $"productid=in=({string.Join(",", productDetail.Select(x => x.ProductId).ToArray())})";
            }

            var salesOrderDetailResponse = await ResponseToDynamic.contextResponse(result, fields, filters, sort, pageNo, pageSize);
            List<SalesOrderDetailResponse> retVal = (JsonSerializer.Deserialize<List<SalesOrderDetailResponse>>(JsonSerializer.Serialize(salesOrderDetailResponse))) ?? new List<SalesOrderDetailResponse>();

            if (productDetail.Any() && foundProductDetailFilter)
            {
                if (!string.IsNullOrEmpty(filters))
                {
                    retVal = retVal.Where(x => productDetail.Any(y => y.ProductId == x.ProductId)).ToList();
                }
            }

            if (includes.Any(x => x.objectName?.ToLower() == "product") && !foundProductDetailFilter && retVal.Any())
            {
                productDetailParts.filters = $"productid=in=({string.Join(",", retVal.Select(x => x.ProductId).ToArray())})";
                productDetail = await _productService.Get(productDetailParts.fields ?? "", productDetailParts.filters ?? "", productDetailParts.include ?? "");
            }

            if (productDetail.Any() && retVal.Any())
            {
                retVal.ForEach(x =>
                {
                    x.Product = ResponseToDynamic.ConvertTo(productDetail.Where(y => y.ProductId == x.ProductId).FirstOrDefault(), productDetailParts.fields ?? "");
                });
            }
            return retVal;
        }
    }
}
