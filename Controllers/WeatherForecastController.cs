using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class VersionController : ControllerBase
{


    private readonly ILogger<VersionController> _logger;

    public VersionController(ILogger<VersionController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public SoftwareVersion Get()
    {
        return new SoftwareVersion
        {
          Ver = "v1.0"
        };
    }
}
