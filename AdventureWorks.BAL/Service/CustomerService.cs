using AdventureWorks.BAL.IService;
using AdventureWorks.BAL.ResponseModel;
using AdventureWorks.DAL.Data;
using AdventureWorks.DAL.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dapper;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace AdventureWorks.BAL.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly dbContext context;
        private readonly IMapper _mapper;
        private readonly IDbConnection _dbConnection;
        public CustomerService(dbContext context, IMapper mapper, IDbConnection dbConnection)
        {
            this.context = context;
            _mapper = mapper;
            _dbConnection = dbConnection;
        }
        public IQueryable<CustomerResponse> Get(bool includeAddresses, bool includeSalesOrderHeaders)
        {
            var result = context.Customers
                 .Select(data =>
                 new CustomerResponse
                 {
                     CustomerId = data.CustomerId,
                     NameStyle = data.NameStyle,
                     Title = data.Title,
                     FirstName = data.FirstName,
                     MiddleName = data.MiddleName,
                     LastName = data.LastName,
                     Suffix = data.Suffix,
                     CompanyName = data.CompanyName,
                     SalesPerson = data.SalesPerson,
                     EmailAddress = data.EmailAddress,
                     Phone = data.Phone,
                     PasswordHash = data.PasswordHash,
                     PasswordSalt = data.PasswordSalt,
                     Rowguid = data.Rowguid,
                     ModifiedDate = data.ModifiedDate,
                     CustomerAddresses = includeAddresses ? data.CustomerAddresses.Select(ca => new CustomerAddressResponse
                     {
                         CustomerId = ca.CustomerId,
                         AddressId = ca.AddressId,

                         AddressLine1 = ca.Address.AddressLine1,
                         AddressLine2 = ca.Address.AddressLine2,
                         City = ca.Address.City,
                         StateProvince = ca.Address.StateProvince,
                         CountryRegion = ca.Address.CountryRegion,
                         PostalCode = ca.Address.PostalCode,
                         Rowguid = ca.Address.Rowguid,
                         ModifiedDate = ca.Address.ModifiedDate
                     }).ToList() : new List<CustomerAddressResponse>(),
                     SalesOrderHeaders = includeSalesOrderHeaders ? data.SalesOrderHeaders.Select(soh => new SalesOrderHeaderResponse
                     {
                         SalesOrderId = soh.SalesOrderId,
                         RevisionNumber = soh.RevisionNumber,
                         OrderDate = soh.OrderDate,
                         DueDate = soh.DueDate,
                         ShipDate = soh.ShipDate,
                         Status = soh.Status,
                         OnlineOrderFlag = soh.OnlineOrderFlag,
                         SalesOrderNumber = soh.SalesOrderNumber,
                         PurchaseOrderNumber = soh.PurchaseOrderNumber,
                         AccountNumber = soh.AccountNumber,
                         CustomerId = soh.CustomerId,
                         ShipToAddressId = soh.ShipToAddressId,
                         BillToAddressId = soh.BillToAddressId,
                         ShipMethod = soh.ShipMethod,
                         CreditCardApprovalCode = soh.CreditCardApprovalCode,
                         SubTotal = soh.SubTotal,
                         TaxAmt = soh.TaxAmt,
                         Freight = soh.Freight,
                         TotalDue = soh.TotalDue,
                         Comment = soh.Comment,
                         Rowguid = soh.Rowguid,
                         ModifiedDate = soh.ModifiedDate,
                         SalesOrderDetails = (soh.SalesOrderDetails.Select(sod => new SalesOrderDetailResponse
                         {
                             SalesOrderId = sod.SalesOrderId,
                             SalesOrderDetailId = sod.SalesOrderDetailId,
                             OrderQty = sod.OrderQty,
                             ProductId = sod.ProductId,
                             UnitPrice = sod.UnitPrice,
                             UnitPriceDiscount = sod.UnitPriceDiscount,
                             LineTotal = sod.LineTotal,
                             Rowguid = sod.Rowguid,
                             ModifiedDate = sod.ModifiedDate,
                             Product = new ProductResponse
                             {
                                 ProductId = sod.Product.ProductId,
                                 Name = sod.Product.Name,
                                 ProductNumber = sod.Product.ProductNumber,
                                 Color = sod.Product.Color,
                                 StandardCost = sod.Product.StandardCost,
                                 ListPrice = sod.Product.ListPrice,
                                 Size = sod.Product.Size,
                                 Weight = sod.Product.Weight,
                                 ProductCategory = sod.Product.ProductCategory.Name,
                                 ProductModel = sod.Product.ProductModel.Name,
                                 SellStartDate = sod.Product.SellStartDate,
                                 SellEndDate = sod.Product.SellEndDate,
                                 DiscontinuedDate = sod.Product.DiscontinuedDate,
                                 Rowguid = sod.Product.Rowguid,
                                 ModifiedDate = sod.Product.ModifiedDate
                             }
                         }).ToList())

                     }).ToList() : new List<SalesOrderHeaderResponse>()
                 }
                 ).AsQueryable();
            return result;
        }
        public async Task<List<CustomerResponseCustom>> GetFromSqlRaw(ODataQueryOptions<CustomerResponse> queryOptions, bool includeAddresses, bool includeSalesOrderHeaders)
        {
            try
            {
                var result = context.Customers
      .Select(data =>
      new CustomerResponse
      {
          CustomerId = data.CustomerId,
          NameStyle = data.NameStyle,
          Title = data.Title,
          FirstName = data.FirstName,
          MiddleName = data.MiddleName,
          LastName = data.LastName,
          Suffix = data.Suffix,
          CompanyName = data.CompanyName,
          SalesPerson = data.SalesPerson,
          EmailAddress = data.EmailAddress,
          Phone = data.Phone,
          PasswordHash = data.PasswordHash,
          PasswordSalt = data.PasswordSalt,
          Rowguid = data.Rowguid,
          ModifiedDate = data.ModifiedDate,
          CustomerAddresses = data.CustomerAddresses.Select(ca => new CustomerAddressResponse
          {
              CustomerId = ca.CustomerId,
              AddressId = ca.AddressId,

              AddressLine1 = ca.Address.AddressLine1,
              AddressLine2 = ca.Address.AddressLine2,
              City = ca.Address.City,
              StateProvince = ca.Address.StateProvince,
              CountryRegion = ca.Address.CountryRegion,
              PostalCode = ca.Address.PostalCode,
              Rowguid = ca.Address.Rowguid,
              ModifiedDate = ca.Address.ModifiedDate
          }).ToList(),
          SalesOrderHeaders = data.SalesOrderHeaders.Select(soh => new SalesOrderHeaderResponse
          {
              SalesOrderId = soh.SalesOrderId,
              RevisionNumber = soh.RevisionNumber,
              OrderDate = soh.OrderDate,
              DueDate = soh.DueDate,
              ShipDate = soh.ShipDate,
              Status = soh.Status,
              OnlineOrderFlag = soh.OnlineOrderFlag,
              SalesOrderNumber = soh.SalesOrderNumber,
              PurchaseOrderNumber = soh.PurchaseOrderNumber,
              AccountNumber = soh.AccountNumber,
              CustomerId = soh.CustomerId,
              ShipToAddressId = soh.ShipToAddressId,
              BillToAddressId = soh.BillToAddressId,
              ShipMethod = soh.ShipMethod,
              CreditCardApprovalCode = soh.CreditCardApprovalCode,
              SubTotal = soh.SubTotal,
              TaxAmt = soh.TaxAmt,
              Freight = soh.Freight,
              TotalDue = soh.TotalDue,
              Comment = soh.Comment,
              Rowguid = soh.Rowguid,
              ModifiedDate = soh.ModifiedDate,
              SalesOrderDetails = (soh.SalesOrderDetails.Select(sod => new SalesOrderDetailResponse
              {
                  SalesOrderId = sod.SalesOrderId,
                  SalesOrderDetailId = sod.SalesOrderDetailId,
                  OrderQty = sod.OrderQty,
                  ProductId = sod.ProductId,
                  UnitPrice = sod.UnitPrice,
                  UnitPriceDiscount = sod.UnitPriceDiscount,
                  LineTotal = sod.LineTotal,
                  Rowguid = sod.Rowguid,
                  ModifiedDate = sod.ModifiedDate,
                  Product = new ProductResponse
                  {
                      ProductId = sod.Product.ProductId,
                      Name = sod.Product.Name,
                      ProductNumber = sod.Product.ProductNumber,
                      Color = sod.Product.Color,
                      StandardCost = sod.Product.StandardCost,
                      ListPrice = sod.Product.ListPrice,
                      Size = sod.Product.Size,
                      Weight = sod.Product.Weight,
                      ProductCategory = sod.Product.ProductCategory.Name,
                      ProductModel = sod.Product.ProductModel.Name,
                      SellStartDate = sod.Product.SellStartDate,
                      SellEndDate = sod.Product.SellEndDate,
                      DiscontinuedDate = sod.Product.DiscontinuedDate,
                      Rowguid = sod.Product.Rowguid,
                      ModifiedDate = sod.Product.ModifiedDate
                  }
              }).AsEnumerable().ToList())

          }).ToList()
      }
      );

                var dbCommand = queryOptions.ApplyTo(result).CreateDbCommand();
                var customerQuery = $@"
SELECT [c].[CustomerID], [c].[NameStyle], [c].[Title], [c].[FirstName], [c].[MiddleName], [c].[LastName], [c].[Suffix], [c].[CompanyName], [c].[SalesPerson], [c].[EmailAddress], [c].[Phone], [c].[PasswordHash], [c].[PasswordSalt], [c].[rowguid], [c].[ModifiedDate]
FROM [SalesLT].[Customer] AS [c] WITH(NOLOCK)
";
                customerQuery = customerQuery + " WHERE " + ExtractWhereClause(dbCommand.CommandText);
                var dynamicParameters = new DynamicParameters();
                foreach (DbParameter parameter in dbCommand.Parameters)
                {
                    dynamicParameters.Add(parameter.ParameterName, parameter.Value, parameter.DbType, parameter.Direction, parameter.Size, parameter.Precision, parameter.Scale);
                }




                using SqlConnection conn = new SqlConnection(_dbConnection.ConnectionString);
                var customer = (await conn.QueryAsync<CustomerResponseCustom>(customerQuery, dynamicParameters)).ToList();


                var address = new List<CustomerAddressResponse>();
                if (includeAddresses)
                {
                    var customerAddressQuery = @"
SELECT [c0].[CustomerID] AS [CustomerId], [c0].[AddressID] AS [AddressId], [a].[AddressLine1], [a].[AddressLine2], [a].[City], [a].[StateProvince], [a].[CountryRegion], [a].[PostalCode], [a].[rowguid] AS [Rowguid], [a].[ModifiedDate], [a].[AddressID] AS [AddressID0]
FROM [SalesLT].[CustomerAddress] AS [c0]
INNER JOIN [SalesLT].[Address] AS [a] ON [c0].[AddressID] = [a].[AddressID]
WHERE [c0].[CustomerID] IN @CustomerIDs ";
                    List<int> CustomerIDs = customer.Select(x => x.CustomerId).ToList();
                    address = (await conn.QueryAsync<CustomerAddressResponse>(customerAddressQuery, new { CustomerIDs })).ToList();
                }
                var salesOrderHeader = new List<SalesOrderHeaderResponseCustom>();
                if (includeSalesOrderHeaders)
                {
                    var salesOrderHeaderQuery = @"SELECT * FROM [SalesLT].[SalesOrderHeader] AS [s] WHERE [s].[CustomerID] IN @CustomerID";
                    List<int> CustomerId = customer.Select(x => x.CustomerId).ToList();
                    salesOrderHeader = (await conn.QueryAsync<SalesOrderHeaderResponseCustom>(salesOrderHeaderQuery, new { CustomerId })).ToList();

                    var salesOrderDetailQuery = @"SELECT * FROM [SalesLT].[SalesOrderDetail] AS [a] WHERE [a].[SalesOrderID] IN @SalesOrderID";
                    List<int> SalesOrderID = salesOrderHeader.Select(x => x.SalesOrderId).ToList();
                    var SalesOrderDetail = (await conn.QueryAsync<SalesOrderDetailResponse>(salesOrderDetailQuery, new { SalesOrderID })).ToList();

                    var ProductQuery = @"SELECT [p].*,[p0].Name [ProductCategory],[p1].Name [ProductModel]
        FROM [SalesLT].[Product] AS [p]
        LEFT JOIN [SalesLT].[ProductCategory] AS [p0] ON [p].[ProductCategoryID] = [p0].[ProductCategoryID]
        LEFT JOIN [SalesLT].[ProductModel] AS [p1] ON [p].[ProductModelID] = [p1].[ProductModelID] WHERE [p].[ProductID] IN @ProductID";
                    List<int> ProductID = SalesOrderDetail.Select(x => x.ProductId).ToList();
                    var Product = await conn.QueryAsync<ProductResponse>(ProductQuery, new { ProductID });

                    SalesOrderDetail.ForEach(x => x.Product = Product.Where(y => y.ProductId == x.ProductId).FirstOrDefault() ?? new ProductResponse());
                    salesOrderHeader.ForEach(x => x.SalesOrderDetails = SalesOrderDetail.Where(y => y.SalesOrderId == x.SalesOrderId).ToList());
                }
                customer.ForEach(x =>
                {
                    x.CustomerAddresses = address.Where(y => y.CustomerId == x.CustomerId).ToList();
                    x.SalesOrderHeaders = salesOrderHeader.Where(y => y.CustomerId == x.CustomerId).ToList();
                });


                return customer;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        string ExtractWhereClause(string sqlQuery)
        {
            // Regular expression to match the WHERE clause
            string pattern = @"\bWHERE\b(.*?)(\bORDER\b|\bGROUP\b|\bHAVING\b|$)";
            var match = Regex.Match(sqlQuery, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }

            return string.Empty;
        }
        public async Task<List<CustomerResponseCustom>> GetCustomeQuery(string filter, string orderBy, bool includeAddresses, bool includeSalesOrderHeaders)
        {
            try
            {


                using SqlConnection conn = new SqlConnection(_dbConnection.ConnectionString);

                var customerQuery = $@"
SELECT [c].[CustomerID], [c].[NameStyle], [c].[Title], [c].[FirstName], [c].[MiddleName], [c].[LastName], [c].[Suffix], [c].[CompanyName], [c].[SalesPerson], [c].[EmailAddress], [c].[Phone], [c].[PasswordHash], [c].[PasswordSalt], [c].[rowguid], [c].[ModifiedDate]
FROM [SalesLT].[Customer] AS [c] WITH(NOLOCK)
";

                var whereClause = ODataFilterToSql(filter);
                whereClause = string.IsNullOrEmpty(whereClause) == true ? string.Empty : $" WHERE {whereClause}";
                var orderByClause = string.IsNullOrEmpty(orderBy) == true ? string.Empty : $" ORDER BY {orderBy}";

                customerQuery = $"{customerQuery} {whereClause} {orderByClause}";

                var customer = (await conn.QueryAsync<CustomerResponseCustom>(customerQuery)).ToList();

                var address = new List<CustomerAddressResponse>();
                if (includeAddresses)
                {
                    var customerAddressQuery = @"
SELECT [c0].[CustomerID] AS [CustomerId], [c0].[AddressID] AS [AddressId], [a].[AddressLine1], [a].[AddressLine2], [a].[City], [a].[StateProvince], [a].[CountryRegion], [a].[PostalCode], [a].[rowguid] AS [Rowguid], [a].[ModifiedDate], [a].[AddressID] AS [AddressID0]
FROM [SalesLT].[CustomerAddress] AS [c0]
INNER JOIN [SalesLT].[Address] AS [a] ON [c0].[AddressID] = [a].[AddressID]
WHERE [c0].[CustomerID] IN @CustomerIDs ";
                    List<int> CustomerIDs = customer.Select(x => x.CustomerId).ToList();
                    address = (await conn.QueryAsync<CustomerAddressResponse>(customerAddressQuery, new { CustomerIDs })).ToList();
                }
                var salesOrderHeader = new List<SalesOrderHeaderResponseCustom>();
                if (includeSalesOrderHeaders)
                {
                    var salesOrderHeaderQuery = @"SELECT * FROM [SalesLT].[SalesOrderHeader] AS [s] WHERE [s].[CustomerID] IN @CustomerID";
                    List<int> CustomerId = customer.Select(x => x.CustomerId).ToList();
                    salesOrderHeader = (await conn.QueryAsync<SalesOrderHeaderResponseCustom>(salesOrderHeaderQuery, new { CustomerId })).ToList();

                    var salesOrderDetailQuery = @"SELECT * FROM [SalesLT].[SalesOrderDetail] AS [a] WHERE [a].[SalesOrderID] IN @SalesOrderID";
                    List<int> SalesOrderID = salesOrderHeader.Select(x => x.SalesOrderId).ToList();
                    var SalesOrderDetail = (await conn.QueryAsync<SalesOrderDetailResponse>(salesOrderDetailQuery, new { SalesOrderID })).ToList();

                    var ProductQuery = @"SELECT [p].*,[p0].Name [ProductCategory],[p1].Name [ProductModel]
        FROM [SalesLT].[Product] AS [p]
        LEFT JOIN [SalesLT].[ProductCategory] AS [p0] ON [p].[ProductCategoryID] = [p0].[ProductCategoryID]
        LEFT JOIN [SalesLT].[ProductModel] AS [p1] ON [p].[ProductModelID] = [p1].[ProductModelID] WHERE [p].[ProductID] IN @ProductID";
                    List<int> ProductID = SalesOrderDetail.Select(x => x.ProductId).ToList();
                    var Product = await conn.QueryAsync<ProductResponse>(ProductQuery, new { ProductID });

                    SalesOrderDetail.ForEach(x => x.Product = Product.Where(y => y.ProductId == x.ProductId).FirstOrDefault() ?? new ProductResponse());
                    salesOrderHeader.ForEach(x => x.SalesOrderDetails = SalesOrderDetail.Where(y => y.SalesOrderId == x.SalesOrderId).ToList());
                }
                customer.ForEach(x =>
                {
                    x.CustomerAddresses = address.Where(y => y.CustomerId == x.CustomerId).ToList();
                    x.SalesOrderHeaders = salesOrderHeader.Where(y => y.CustomerId == x.CustomerId).ToList();
                });


                return customer;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private Dictionary<string, string> getCustomerFilterMap()
        {
            var filterMap = new Dictionary<string, string>
                        {
                            { "CustomerID", "[c].[CustomerID]" },
                            { "NameStyle", "[c].[NameStyle]" },
                            { "Title", "[c].[Title]" },
                            { "FirstName", "[c].[FirstName]" },
                            { "MiddleName", "[c].[MiddleName]" },
                            { "LastName", "[c].[LastName]" },
                            { "Suffix", "[c].[Suffix]" },
                            { "CompanyName", "[c].[CompanyName]" },
                            { "SalesPerson", "[c].[SalesPerson]" },
                            { "EmailAddress", "[c].[EmailAddress]" },
                            { "Phone", "[c].[Phone]" },
                            { "PasswordHash", "[c].[PasswordHash]" },
                            { "PasswordSalt", "[c].[PasswordSalt]" },
                            { "rowguid", "[c].[rowguid]" },
                            { "ModifiedDate", "[c].[ModifiedDate]" }
                        };

            return filterMap;
        }
        private string ODataFilterToSql(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return string.Empty;
            }

            filter = filterClean(filter);

            var sqlFilter = filter
                .Replace(" eq ", " = ")
                .Replace(" ne ", " != ")
                .Replace(" gt ", " > ")
                .Replace(" ge ", " >= ")
                .Replace(" lt ", " < ")
                .Replace(" le ", " <= ");

            foreach (var item in getCustomerFilterMap())
            {
                sqlFilter = sqlFilter.ToLower().Replace(item.Key.ToLower(), item.Value);
            }
            return sqlFilter;
        }
        string filterClean(string filter)
        {

            var _filter = filter.Replace(" AND ", ";");
            _filter = _filter.Replace(" OR ", ",");
            _filter = _filter.Replace(" and ", ";");
            _filter = _filter.Replace(" or ", ",");

            var _a = SplitConditions(_filter, new char[] { ';', ',' });
            List<string> removeList = new();
            foreach (var item in _a)
            {
                if (item.IndexOf("/", StringComparison.InvariantCulture) < item.IndexOf(" ", StringComparison.InvariantCulture))
                {
                    removeList.Add(item);
                }
            }
            foreach (var item in removeList)
            {
                filter = filter.Replace(item, string.Empty);
            }
            filter = filter.Trim();
            filter = filter.TrimEnd(new char[] { 'a', 'n', 'd', 'o', 'r' });
            return filter;
        }
        IEnumerable<string> SplitConditions(string query, char[] separators)
        {
            int depth = 0;
            List<int> splitIndexes = new List<int>();

            for (int i = 0; i < query.Length; i++)
            {
                if (query[i] == '(') depth++;
                if (query[i] == ')') depth--;
                if (depth == 0 && Array.Exists(separators, separator => separator == query[i]))
                {
                    splitIndexes.Add(i);
                }
            }

            splitIndexes.Add(query.Length);

            int start = 0;
            foreach (int index in splitIndexes)
            {
                yield return query.Substring(start, index - start).Trim();
                start = index + 1;
            }
        }

    }
}
