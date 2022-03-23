using Challenge.Application.Abstract;
using Challenge.Application.Concrete;
using Challenge.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Challenge.Application;

public static class ServiceRegistration
{

    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddScoped<IUserManager, UserManager>();
        services.AddScoped<IProductManager, ProductManager>();
        services.AddScoped<ITokenManager, TokenManager>();

        services.AddInfrastructure(configuration);



        return services;
    }
}