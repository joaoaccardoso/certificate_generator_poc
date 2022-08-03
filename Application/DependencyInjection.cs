using System.Reflection;
using MediatR;

namespace CertificateGeneratorTest.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddCertificateManager();
        return services;
    }
}