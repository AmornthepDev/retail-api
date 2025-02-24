using Microsoft.AspNetCore.Mvc;
using Retail.Application.Services;
using Retail.Contracts.Product;

namespace Retail.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllProductsRequest request, CancellationToken token = default)
        {
            var products = await _productService.GetAllAsync(request, token);
            return Ok(products);
        }
    }
}
