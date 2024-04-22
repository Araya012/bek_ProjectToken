using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using bek_ProjectToken.Models;
using Microsoft.Extensions.Options;


public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // DbContext configuration
        services.AddDbContext<YourDbContext>(options =>
        //  options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));
        options.UseMySql(Configuration.GetConnectionString("DefaultConnection"),
              new MySqlServerVersion(new Version(7, 0, 0)))); // Here you specify the version of the MySQL server you are using

        // Other services
        services.AddControllers();
        // Other services you need to add
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // application configuration
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            // Error configuration in production
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        // Middleware and routing
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
