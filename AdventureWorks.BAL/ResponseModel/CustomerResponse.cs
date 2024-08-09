using AdventureWorks.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AdventureWorks.BAL.ResponseModel
{
    public class CustomerResponse
    {
        public int CustomerId { get; set; }
        public bool NameStyle { get; set; }
        public string? Title { get; set; }
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = null!;
        public string? Suffix { get; set; }
        public string? CompanyName { get; set; }
        public string? SalesPerson { get; set; }
        public string? EmailAddress { get; set; }
        public string? Phone { get; set; }
        public string PasswordHash { get; set; } = null!;
        public string PasswordSalt { get; set; } = null!;
        public Guid Rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }
        public List<CustomerAddressResponse> CustomerAddresses { get; set; } = new List<CustomerAddressResponse>();
        public List<SalesOrderHeaderResponse> SalesOrderHeaders { get; set; } = new List<SalesOrderHeaderResponse>();
    }
    public class CustomerResponseCustom
    {
        public int CustomerId { get; set; }
        public bool NameStyle { get; set; }
        public string? Title { get; set; }
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = null!;
        public string? Suffix { get; set; }
        public string? CompanyName { get; set; }
        public string? SalesPerson { get; set; }
        public string? EmailAddress { get; set; }
        public string? Phone { get; set; }
        public string PasswordHash { get; set; } = null!;
        public string PasswordSalt { get; set; } = null!;
        public Guid Rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }
        public List<CustomerAddressResponse> CustomerAddresses { get; set; } = new List<CustomerAddressResponse>();
        public List<SalesOrderHeaderResponseCustom> SalesOrderHeaders { get; set; } = new List<SalesOrderHeaderResponseCustom>();
    }
}
