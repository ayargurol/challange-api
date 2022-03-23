using AutoMapper;
using Challenge.Application.Dto.Product.Request;
using Challenge.Application.Dto.Product.Response;
using Challenge.Domain;

namespace Challenge.Application.Mapping;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<ProductAddRequest, Product>();
        CreateMap<ProductUpdateRequest, Product>();

        CreateMap<Product, ProductResponse>();


    }
}