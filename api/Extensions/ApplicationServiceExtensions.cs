using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ApplicationServiceExtensions 
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        // DATABASE CONNECTION
        services.AddDbContext<AppDbContext>
                        (options => options.UseSqlServer(config.GetConnectionString("DbConnection")));
        
        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", policy =>
            {
                policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
            });
        });
        
        // DEPENDENCY INJECTION
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<DataSeeder>();
        

        return services;
    }
}