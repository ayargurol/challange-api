using Challange.Core.Common;
using Challenge.Application.Dto.Product.Request;
using Challenge.Application.Dto.Product.Response;

namespace Challenge.Application.Abstract;

public interface IProductManager
{
    Task<Response<List<ProductResponse>>> Get();
    Task AddProduct(ProductAddRequest request);
    Task UpdateProduct(ProductUpdateRequest request);
    Task DeleteProduct(Guid id);

}