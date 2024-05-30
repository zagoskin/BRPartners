using BRP.VendorManagement.API.Mappings;
using BRP.VendorManagement.Application.Vendors.Queries.GetById;
using BRP.VendorManagement.Application.Vendors.Queries.List;
using BRPartners.Common.Responses.VendorManagement;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BRP.VendorManagement.API.Controllers;

public class VendorsController : ApiController
{
    private readonly ISender _sender;

    public VendorsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(VendorListResponse), 200)]
    public async Task<IActionResult> ListVendorsAsync()
    {
        var vendorsResult = await _sender.Send(new ListVendorsQuery());
        return vendorsResult.Match(
            vendors => Ok(vendors.ToResponse()),
            Problem);
    }

    [HttpGet("{vendorId:guid}")]
    [ProducesResponseType(typeof(VendorResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> GetVendorAsync(Guid vendorId)
    {
        var vendorResult = await _sender.Send(new GetVendorByIdQuery(vendorId));
        return vendorResult.Match(
            vendor => Ok(vendor.ToResponse()),
            Problem);
    }
}
