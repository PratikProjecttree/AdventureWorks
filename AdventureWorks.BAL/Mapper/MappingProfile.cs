using AdventureWorks.BAL.ResponseModel;
using AdventureWorks.DAL.Models;
using AutoMapper;

namespace AdventureWorks.BAL.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Customer, CustomerResponse>();
            CreateMap<CustomerAddress, CustomerAddressResponse>();
            CreateMap<SalesOrderHeader, SalesOrderHeaderResponse>();
            CreateMap<SalesOrderDetail, SalesOrderDetailResponse>();
            CreateMap<Product, ProductResponse>();
            CreateMap<ProductCategory, ProductCategoryResponse>();
        }
    }
}
