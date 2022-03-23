using AutoMapper;
using Challange.Core.Common;
using Challange.Core.Repositories;
using Challenge.Application.Abstract;
using Challenge.Application.Dto.Product.Request;
using Challenge.Application.Dto.Product.Response;
using Challenge.Application.Mapping;
using Challenge.Domain;
using Challenge.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Challenge.Application.Concrete;

public class ProductManager : IProductManager
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly User _currentUser;
    public ProductManager(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ProductProfile>()).CreateMapper();
        _currentUser = httpContextAccessor.HttpContext.Items["User"] as User;
        //_currentUser = new User { Role = "Admin" };
    }

    public async Task<Response<List<ProductResponse>>> Get()
    {

        var product = await _unitOfWork.Repository<IProductRepository>()
            .GetListAsync(a => a.IsActive && !a.IsDeleted && (_currentUser.Role == "Admin" || a.CreatedById == _currentUser.Id), null, 0, 0, a => a.History);
        var mapped = _mapper.Map<List<ProductResponse>>(product);
        return Response<List<ProductResponse>>.CreateResponse(mapped, StatusCodes.Status200OK);

    }

    public async Task AddProduct(ProductAddRequest request)
    {
        var product = _mapper.Map<Product>(request);
        _unitOfWork.Repository<IProductRepository>().Add(product);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateProduct(ProductUpdateRequest request)
    {
        var repo = _unitOfWork.Repository<IProductRepository>();

        var product = await repo.GetAsync(a => a.Id == request.Id);
        product.LogHistory();

        _mapper.Map(request, product);

        repo.Update(product);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteProduct(Guid id)
    {
        var repo = _unitOfWork.Repository<IProductRepository>();
        var product = await repo.GetAsync(a => a.Id == id);
        repo.Delete(product);
        await _unitOfWork.SaveChangesAsync();
    }
}