using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace CQRSapp.Domain
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDomainDI(this IServiceCollection services)
        {
            // Add your domain services here, e.g., domain event handlers, domain services, etc.
            // Example: services.AddScoped<IDomainService, DomainServiceImplementation>();
            
            return services;
        }
    }
}
