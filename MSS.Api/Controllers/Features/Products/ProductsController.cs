using Microsoft.AspNetCore.Mvc;
using MSS.Domain.Entities;
using MSS.Domain.Features.Products;

namespace MSS.Api.Controllers.Features.Products;

/// <summary>
/// Products API controller - Feature-based organization
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;

    public ProductsController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    // GET: api/Products
    [HttpGet]
    //[HasPermission("product:list")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        var products = await _productRepository.GetAllAsync();
        return Ok(products);
    }

    // GET: api/Products/5
    [HttpGet("{id}")]
    //[HasPermission("product:list")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    // POST: api/Products
    [HttpPost]
   // [HasPermission("product:create")]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        var id = await _productRepository.CreateAsync(product);
        product.Id = id;
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    // PUT: api/Products/5
    [HttpPut("{id}")]
    //[HasPermission("product:edit")]
    public async Task<IActionResult> UpdateProduct(int id, Product product)
    {
        if (id != product.Id)
        {
            return BadRequest();
        }

        var result = await _productRepository.UpdateAsync(product);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE: api/Products/5
    [HttpDelete("{id}")]
   // [HasPermission("product:delete")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var result = await _productRepository.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}

