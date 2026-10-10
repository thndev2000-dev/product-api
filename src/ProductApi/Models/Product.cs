using System.ComponentModel.DataAnnotations;

namespace ProductApi.Models;

public class Product
{
	public int Id { get; set; }

	[Required, StringLength(200)]
	public required string Name { get; set; }

	[Range(0, double.MaxValue, ErrorMessage = "Price must be >= 0")]
	public decimal Price { get; set; }

	[StringLength(1000)]
	public string? Description { get; set; }

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
