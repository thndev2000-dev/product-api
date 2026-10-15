using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using ProductApi.Data;
using ProductApi.Models;

namespace ProductApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProductsController> _logger;
    private readonly IDistributedCache _cache;
    private const string ProductsCacheKey = "products:all";

    public ProductsController(AppDbContext context, ILogger<ProductsController> logger, IDistributedCache cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        // 1. Hỏi Redis trước
        var cachedJson = await _cache.GetStringAsync(ProductsCacheKey);
        if (cachedJson != null)
            return Ok(JsonSerializer.Deserialize<List<Product>>(cachedJson));

        // 2. Cache miss → query SQL
        var products = await _context.Products.ToListAsync();

        // 3. Lưu vào Redis, hết hạn sau 5 phút
        await _cache.SetStringAsync(ProductsCacheKey,
            JsonSerializer.Serialize(products),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var p = await _context.Products.FindAsync(id);
        return p is null ? NotFound() : Ok(p);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create(Product product)
    {
        _logger.LogInformation("Creating product {ProductName} with price {Price}", product.Name, product.Price);
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        await _cache.RemoveAsync(ProductsCacheKey); // Xé cache vì data đã đổi
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Product product)
    {
        if (id != product.Id) return BadRequest();
        _context.Entry(product).State = EntityState.Modified;
        try { await _context.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Products.AnyAsync(p => p.Id == id)) return NotFound();
            throw;
        }
        await _cache.RemoveAsync(ProductsCacheKey); // Xé cache vì data đã đổi
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await _context.Products.FindAsync(id);
        if (p is null) return NotFound();
        _context.Products.Remove(p);
        await _context.SaveChangesAsync();
        await _cache.RemoveAsync(ProductsCacheKey); // Xé cache vì data đã đổi
        return NoContent();
    }
}
