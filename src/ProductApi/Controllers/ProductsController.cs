using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductApi.Data;
using ProductApi.Models;

namespace ProductApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
	private readonly AppDbContext _context;
	private readonly ILogger<ProductsController> _logger;

	public ProductsController(AppDbContext context, ILogger<ProductsController> logger)
	{
		_context = context;
		_logger = logger;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<Product>>> GetAll()
		=> Ok(await _context.Products.ToListAsync());

	[HttpGet("{id:int}")]
	public async Task<ActionResult<Product>> GetById(int id)
	{
		var p = await _context.Products.FindAsync(id);
		return p is null ? NotFound() : Ok(p);
	}

	[HttpPost]
	public async Task<ActionResult<Product>> Create(Product product)
	{
		if (product.Name == "bug")
			throw new InvalidOperationException("Demo runtime bug: product named 'bug' is not allowed!");

		_logger.LogInformation("Creating product {ProductName} with price {Price}", product.Name, product.Price);
		_context.Products.Add(product);
		await _context.SaveChangesAsync();
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
		return NoContent();
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> Delete(int id)
	{
		var p = await _context.Products.FindAsync(id);
		if (p is null) return NotFound();
		_context.Products.Remove(p);
		await _context.SaveChangesAsync();
		return NoContent();
	}
}
