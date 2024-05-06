using Microsoft.AspNetCore.Mvc;
using tutorial6.Services;

namespace tutorial6.Controllers;

[ApiController]
public class WarehouseController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ProductService _productService;

    public WarehouseController(IConfiguration configuration)
    {
        _productService = new ProductService();
        _configuration = configuration;
    }
    
    [HttpPost]
    public IActionResult AddProduct(AddProductRequest request)
    {
        int productWarehouseId = _productService.AddProduct(_configuration, request.IdProduct, request.IdWarehouse, request.Amount, request.CreatedAt);
    
        return StatusCode(StatusCodes.Status201Created, new { ProductWarehouseId = productWarehouseId });
    }
    
}