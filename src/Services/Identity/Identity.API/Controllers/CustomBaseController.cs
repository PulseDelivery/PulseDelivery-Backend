using Asp.Versioning;
using Identity.API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers;

[ApiVersion("1.0")] 
[Route("api/v{version:apiVersion}/[controller]")] 
[ApiController]
public class CustomBaseController : ControllerBase
{
    public IActionResult CreateActionResult<T>(ResponseDto<T> response)
    {
        if (response.StatusCode == 204)
            return new ObjectResult(null) { StatusCode = response.StatusCode };

        return new ObjectResult(response) { StatusCode = response.StatusCode };
    }
}