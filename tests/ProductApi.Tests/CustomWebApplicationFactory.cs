using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Data;

namespace ProductApi.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.ConfigureServices(services =>
		{
			services.Remove(services.Single(
				d => d.ServiceType == typeof(DbContextOptions<AppDbContext>)));

			services.AddDbContext<AppDbContext>(options =>
				options.UseSqlServer(
					"Server=(localdb)\\MSSQLLocalDB;Database=ProductApi_TestDb;Trusted_Connection=True;TrustServerCertificate=True;"));
		});
	}
}
