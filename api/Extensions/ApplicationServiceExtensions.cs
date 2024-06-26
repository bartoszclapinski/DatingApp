﻿using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ApplicationServiceExtensions 
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        // DATABASE CONNECTION
        services.AddDbContext<AppDbContext>
                        (options => options.UseSqlServer(
                                        config.GetConnectionString("DbConnection"),
                                        x => x.UseDateOnlyTimeOnly()));
        
        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", policy =>
            {
                policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("https://localhost:4200");
            });
        });
        
        // DEPENDENCY INJECTION
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPhotoService, PhotoService>();
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddScoped<DataSeeder>();
        services.AddScoped<LogUserActivity>();
        services.AddSignalR();
        services.AddSingleton<PresenceTracker>();
        
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
        
        
        
        return services;
    }
}