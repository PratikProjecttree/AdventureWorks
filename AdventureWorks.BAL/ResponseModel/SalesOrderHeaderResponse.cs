using AdventureWorks.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AdventureWorks.BAL.ResponseModel
{

    public partial class SalesOrderHeaderResponse
    {
        public int SalesOrderId { get; set; }
        public byte RevisionNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ShipDate { get; set; }
        public byte Status { get; set; }
        public bool? OnlineOrderFlag { get; set; }
        public string SalesOrderNumber { get; set; } = null!;

        public string? PurchaseOrderNumber { get; set; }
        public string? AccountNumber { get; set; }
        public int CustomerId { get; set; }
        public int? ShipToAddressId { get; set; }
        public int? BillToAddressId { get; set; }
        public string ShipMethod { get; set; } = null!;
        public string? CreditCardApprovalCode { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmt { get; set; }
        public decimal Freight { get; set; }
        public decimal TotalDue { get; set; }
        public string? Comment { get; set; }
        public Guid Rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }
        public dynamic SalesOrderDetailss
        {
            get => SalesOrderDetails;
        }
        [JsonIgnore]
        public List<SalesOrderDetailResponse> SalesOrderDetails { get; set; } = new List<SalesOrderDetailResponse>();
    }
    public partial class SalesOrderHeaderResponseCustom
    {
        public int SalesOrderId { get; set; }
        public byte RevisionNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ShipDate { get; set; }
        public byte Status { get; set; }
        public bool? OnlineOrderFlag { get; set; }
        public string SalesOrderNumber { get; set; } = null!;

        public string? PurchaseOrderNumber { get; set; }
        public string? AccountNumber { get; set; }
        public int CustomerId { get; set; }
        public int? ShipToAddressId { get; set; }
        public int? BillToAddressId { get; set; }
        public string ShipMethod { get; set; } = null!;
        public string? CreditCardApprovalCode { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmt { get; set; }
        public decimal Freight { get; set; }
        public decimal TotalDue { get; set; }
        public string? Comment { get; set; }
        public Guid Rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }
        public List<SalesOrderDetailResponse> SalesOrderDetails { get; set; } = new List<SalesOrderDetailResponse>();
    }
}
