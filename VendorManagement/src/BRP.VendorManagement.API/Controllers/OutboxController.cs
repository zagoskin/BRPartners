using BRP.VendorManagement.Infrastructure.JustATest;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BRP.VendorManagement.API.Controllers;

// controller just for the sake of the example
public class OutboxController : ApiController
{
    private readonly ISender _sender;

    public OutboxController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("domain-events")]
    public async Task<IActionResult> ListDomainEventsAsync(CancellationToken token = default)
    {
        var query = new ListOutboxDomainEventsQuery();

        var result = await _sender.Send(query, token);

        return Ok(result);
    }

    [HttpGet("integration-events")]
    public async Task<IActionResult> ListIntegrationEventsAsync(CancellationToken token = default)
    {
        var query = new ListOutboxIntegrationEventsQuery();

        var result = await _sender.Send(query, token);

        return Ok(result);
    }
}
