using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Revisao.Models;
using Revisao.Repositories;

namespace Revisao.Controllers;

[ApiController]
[Route("api/product")]
public class ProductController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    public ProductController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    [HttpGet("list")]
    public IActionResult GetProducts()
    {
        var products = _productRepository.GetProducts();
        return Ok(products);
    }

    [HttpPost("add")]
    [Authorize(Roles = "ADMINISTRATOR")]
    public IActionResult Add([FromBody] Product product)
    {
        _productRepository.Add(product);
        return Created("", product);
    }
}