using Microsoft.Extensions.DependencyInjection;
using ProductApi.Data;
using System.Net;
using System.Net.Http.Json;

namespace ProductApi.Tests;

public class ProductsApiTests : IClassFixture<CustomWebApplicationFactory>
{
	private readonly CustomWebApplicationFactory _factory;
	private readonly HttpClient _client;

	public ProductsApiTests(CustomWebApplicationFactory factory)
	{
		_factory = factory;
		_client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		scope.ServiceProvider.GetRequiredService<AppDbContext>()
			.Database.EnsureCreated();
	}

	[Fact]
	public async Task Post_ValidProduct_Returns201()
	{
		var response = await _client.PostAsJsonAsync("/api/products",
			new { name = "Bút bi", price = 5000m, description = "Test" });

		Assert.Equal(HttpStatusCode.Created, response.StatusCode);
	}

	[Fact]
	public async Task Post_EmptyName_Returns400()
	{
		var response = await _client.PostAsJsonAsync("/api/products",
			new { name = "", price = 5000m });

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Post_NegativePrice_Returns400()
	{
		var response = await _client.PostAsJsonAsync("/api/products",
			new { name = "Bút bi", price = -100m });

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Get_NonExistentId_Returns404()
	{
		var response = await _client.GetAsync("/api/products/99999");

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

}
