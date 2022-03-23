using Challange.Core.Repositories;
using Challenge.Domain.Interfaces;
using Challenge.Infrastructure.Context;
using Challenge.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Challenge.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContext<ChallengeContext>(x =>
        {
            x.UseNpgsql(configuration.GetConnectionString("Default"));
            x.EnableSensitiveDataLogging();
        });
        services.TryAddScoped<DbContext, ChallengeContext>();
        services.AddHttpContextAccessor();


        // add hardcoded test user to db on startup
        using (var serviceScope = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            Console.WriteLine("Service scope created");
            var context = serviceScope.ServiceProvider.GetRequiredService<ChallengeContext>();
            Console.WriteLine("Context received");
            context.Database.Migrate();
            Console.WriteLine("migration succeed");
        }


        services.TryAddScoped(typeof(IUnitOfWork), typeof(UnitOfWork<DbContext>));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}