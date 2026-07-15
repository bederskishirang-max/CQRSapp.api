using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQRSapp.Infrastructure;
using CQRSapp.Application;


namespace CQRSapp.api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiDI(this IServiceCollection services, IConfiguration configuration)
        {
            // Add your API services here, e.g., controllers, middleware, etc.
            // Example: services.AddControllers();
           
            services.AddApplicationDI().AddInfrastructureDI(configuration); // Call the extension methods from Infrastructure and Application layers

            return services;
        }
    }
}
