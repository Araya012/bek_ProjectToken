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
        // Configuración de DbContext
        services.AddDbContext<YourDbContext>(options =>
        //  options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));
        options.UseMySql(Configuration.GetConnectionString("DefaultConnection"),
              new MySqlServerVersion(new Version(7, 0, 0)))); // Especifica la versión del servidor MySQL que estás utilizando

        // Otros servicios
        services.AddControllers();
        // Otros servicios que necesites agregar
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Configuración de la aplicación
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            // Configuración de errores en producción
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        // Middleware y enrutamiento
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
