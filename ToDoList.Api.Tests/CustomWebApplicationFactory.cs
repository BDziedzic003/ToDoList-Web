using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToDoList.Data;

namespace ToDoList.Api.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string databaseName = Guid.NewGuid().ToString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<TodoContext>>();
                services.RemoveAll<TodoContext>();
                services.RemoveAll<IDbContextOptionsConfiguration<TodoContext>>();

                services.AddDbContext<TodoContext>(options =>
                {
                    options.UseInMemoryDatabase(databaseName);
                });
            });
        }
    }
}