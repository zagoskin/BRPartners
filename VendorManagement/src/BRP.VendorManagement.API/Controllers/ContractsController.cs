using BRP.VendorManagement.API.Mappings;
using BRP.VendorManagement.Application.Contracts.Commands.Create;
using BRP.VendorManagement.Application.Contracts.Queries.GetById;
using BRP.VendorManagement.Application.Contracts.Queries.List;
using BRPartners.Shared.Requests.VendorManagement;
using BRPartners.Shared.Responses.VendorManagement;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BRP.VendorManagement.API.Controllers;
#pragma warning disable IDE1006 // Naming Styles
public class ContractsController : ApiController
{
    private readonly ISender _sender;

    public ContractsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{contractId}")]
    [ProducesResponseType(typeof(ContractResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetContract(Guid contractId)
    {
        var query = new GetContractByIdQuery(contractId);

        var result = await _sender.Send(query);

        return result.Match<IActionResult>(
            contract => Ok(contract.ToResponse()),
            NotFound);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ContractListResponse), StatusCodes.Status200OK)]

    public async Task<IActionResult> ListContracts()

    {
        var query = new ListContractsQuery();

        var result = await _sender.Send(query);

        return result.Match(
            contracts => Ok(contracts.ToResponse()),
            Problem);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ContractResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateContract(CreateContractRequest request)
    {
        var command = new CreateContractCommand(
            request.VendorId,
            request.Title,
            request.DeadLineUtc.ToDateTime(TimeOnly.MinValue),
            request.EstimatedValue);

        var result = await _sender.Send(command);

        return result.Match(
            contract => CreatedAtAction(
                nameof(GetContract),
                new { contractId = contract.Id.Value.ToString() },
                contract.ToResponse()),
            Problem);
    }
}
#pragma warning restore IDE1006 // Naming Styles
