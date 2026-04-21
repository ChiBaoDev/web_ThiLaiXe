using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.Certificates;
using HeThongThiBangLai.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongThiBangLai.Api.Controllers;

[ApiController]
[Route("api/v1/public/certificates")]
[AllowAnonymous]
[Produces("application/json")]
public class CertificatesPublicController : ControllerBase
{
    private readonly ICertificateService _service;

    public CertificatesPublicController(ICertificateService service)
    {
        _service = service;
    }

    [HttpGet("verify/{code}")]
    [ProducesResponseType(typeof(ApiResponse<CertificateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VerifyByCode(string code)
    {
        var result = await _service.VerifyByCodeAsync(code);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
