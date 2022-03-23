using Challenge.Application.Abstract;
using Challenge.Application.Dto.Product.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Challenge.Api.Controllers;


[Route("api/[controller]"), ApiController, Authorize]
public class ProductController : ControllerBase
{
    private readonly IProductManager _productManager;

    public ProductController(IProductManager productManager)
    {
        _productManager = productManager;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _productManager.Get();

        return Ok(result);
    }

    [HttpGet("GetById")]
    public async Task<IActionResult> GetById()
    {
        var result = await _productManager.Get();

        return Ok(result);
    }


    [HttpPut]
    public async Task<IActionResult> Put(ProductUpdateRequest request)
    {
        await _productManager.UpdateProduct(request);

        return Ok();
    }


    [HttpPost]
    public async Task<IActionResult> Post(ProductAddRequest request)
    {

        await _productManager.AddProduct(request);

        return Ok();
    }



    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _productManager.DeleteProduct(id);
        return Ok();
    }

}