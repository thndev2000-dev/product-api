using System.ComponentModel.DataAnnotations;

namespace ProductClient.Models;

public class Product
{
	public int Id { get; set; }

	[Required(ErrorMessage = "Name is required")]
	[MaxLength(200)]
	public string Name { get; set; } = string.Empty;

	[Range(0, double.MaxValue, ErrorMessage = "Price must be >= 0")]
	public decimal Price { get; set; }

	[MaxLength(1000)]
	public string? Description { get; set; }
}
