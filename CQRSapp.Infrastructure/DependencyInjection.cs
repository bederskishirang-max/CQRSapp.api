using CQRSapp.Application.Auth.Services;
using CQRSapp.Domain.Interfaces;
using CQRSapp.Infrastructure.Authentication;
using CQRSapp.Infrastructure.Data;
using CQRSapp.Infrastructure.Respository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace CQRSapp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                // Configure your database provider and connection string here
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"));
            });
           
           

            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            return services;
        }
    }
}
