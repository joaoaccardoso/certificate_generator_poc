using CertificateGeneratorTest.Application.Certificates.Queries;
using CertificateGeneratorTest.Application.Certificates.Queries.GetCertificate;
using CertificateGeneratorTest.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CertificateGeneratorTest.Controllers;

[ApiController]
[Route("[controller]")]
public class CertificatesController  : ApiControllerBase
{
    private readonly ILogger<CertificatesController> _logger;

    public CertificatesController(ILogger<CertificatesController> logger)
    {
        _logger = logger;
    }
    
    [HttpGet(Name = "GetCertificate")]
    public async Task<Certificate> Get()
    {
        return await Mediator.Send(new GetCertificateQuery());
    }
}