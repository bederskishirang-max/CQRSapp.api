using CQRSapp.Domain.Interfaces;
using CQRSapp.Infrastructure.Data;
using CQRSapp.Infrastructure.Respository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRSapp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                // Configure your database provider and connection string here
                options.UseSqlServer("DefaultConnection");
               
            });
           
           

            services.AddScoped<IEmployeeRepository, EmployeeRepository>(); 
            return services;
        }
    }
}
