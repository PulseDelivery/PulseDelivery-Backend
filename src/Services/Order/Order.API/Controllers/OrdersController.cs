using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

using Order.Application.Commands.CreateOrder;

using PulseDelivery.Shared.ControllerBases;
using PulseDelivery.Shared.Authorization;

namespace Order.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class OrdersController : CustomBaseController
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = Permissions.OrderCreate)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;

        command.CustomerId = userId ?? "guest";
        command.Email = userEmail ?? "no-email";

        var response = await _mediator.Send(command);
        
        return CreateActionResult(response);
    }
}