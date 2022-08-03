using System.Security.Cryptography.X509Certificates;
using CertificateGeneratorTest.Application.Common.Configuration;
using CertificateManager;
using CertificateManager.Models;
using MediatR;
using Microsoft.Extensions.Options;

namespace CertificateGeneratorTest.Application.Certificates.Queries.GetCertificate;


public class GetCertificateQuery : IRequest<Certificate>
{
    public string ClientName { get; set; }
}

public class GetCertificateQueryHandler : IRequestHandler<GetCertificateQuery, Certificate>
{
    private readonly ILogger<GetCertificateQueryHandler> _logger;
    private readonly CreateCertificatesClientServerAuth _createClientServerAuthCerts;
    private readonly ImportExportCertificate _importExportCertificate;
    private readonly RootCertificateConfig _certificateConfiguration;

    public GetCertificateQueryHandler(ILogger<GetCertificateQueryHandler> logger,
        IConfiguration configuration,
        CreateCertificatesClientServerAuth createClientServerAuthCerts,
        ImportExportCertificate importExportCertificate)
    {
        _logger = logger;
        _certificateConfiguration = configuration.GetSection("RootCertificateConfig").Get<RootCertificateConfig>();
        _createClientServerAuthCerts = createClientServerAuthCerts;
        _importExportCertificate = importExportCertificate;
    }
    public async Task<Certificate> Handle(GetCertificateQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var intermediate = new X509Certificate2(_certificateConfiguration.CertFile, _certificateConfiguration.Password);
        
            // use lowercase for dps
            var testDevice01 = _createClientServerAuthCerts.NewDeviceChainedCertificate(
                new DistinguishedName { CommonName = request.ClientName },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                request.ClientName, intermediate);
            testDevice01.FriendlyName = request.ClientName;
        
            string password = _certificateConfiguration.Password;


            var deviceInPfxBytes = await Task.Run(() =>
                _importExportCertificate.ExportChainedCertificatePfx(password, testDevice01, intermediate));
            //File.WriteAllBytes("testDevice01.pfx", deviceInPfxBytes);

            return new Certificate
            {
                PfxFile = deviceInPfxBytes
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}