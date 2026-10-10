using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductApi.Data;
using ProductApi.Models;

namespace ProductApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
	private readonly AppDbContext _db;
	public ProductsController(AppDbContext db) => _db = db;

	[HttpGet]
	public async Task<ActionResult<IEnumerable<Product>>> GetAll()
		=> Ok(await _db.Products.ToListAsync());

	[HttpGet("{id:int}")]
	public async Task<ActionResult<Product>> GetById(int id)
	{
		var p = await _db.Products.FindAsync(id);
		return p is null ? NotFound() : Ok(p);
	}

	[HttpPost]
	public async Task<ActionResult<Product>> Create(Product product)
	{
		_db.Products.Add(product);
		await _db.SaveChangesAsync();
		return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
	}

	[HttpPut("{id:int}")]
	public async Task<IActionResult> Update(int id, Product product)
	{
		if (id != product.Id) return BadRequest();
		_db.Entry(product).State = EntityState.Modified;
		try { await _db.SaveChangesAsync(); }
		catch (DbUpdateConcurrencyException)
		{
			if (!await _db.Products.AnyAsync(p => p.Id == id)) return NotFound();
			throw;
		}
		return NoContent();
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> Delete(int id)
	{
		var p = await _db.Products.FindAsync(id);
		if (p is null) return NotFound();
		_db.Products.Remove(p);
		await _db.SaveChangesAsync();
		return NoContent();
	}
}
