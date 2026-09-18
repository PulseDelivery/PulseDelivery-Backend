using Microsoft.AspNetCore.Mvc;
using PulseDelivery.Shared.DTOs;

namespace PulseDelivery.Shared.ControllerBases;

public class CustomBaseController : ControllerBase
{
    public IActionResult CreateActionResult<T>(ResponseDto<T> response)
    {
        if (response.StatusCode == 204)
            return new ObjectResult(null) { StatusCode = response.StatusCode };

        return new ObjectResult(response) { StatusCode = response.StatusCode };
    }
}