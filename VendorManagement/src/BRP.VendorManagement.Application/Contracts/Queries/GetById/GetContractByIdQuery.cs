using BRP.ContractManagement.Application.Contracts;
using BRP.VendorManagement.Application.Common;
using BRP.VendorManagement.Domain.ContractAggregate;
using BRP.VendorManagement.Domain.ContractAggregate.ValueObjects;
using ErrorOr;
using MediatR;

namespace BRP.VendorManagement.Application.Contracts.Queries.GetById;
public record GetContractByIdQuery(Guid ContractId) : IRequest<ErrorOr<Contract>>;

internal sealed class GetContractByIdQueryHandler : IRequestHandler<GetContractByIdQuery, ErrorOr<Contract>>
{
    private readonly IContractRepository _contractRepository;

    public GetContractByIdQueryHandler(IContractRepository contractRepository)
    {
        _contractRepository = contractRepository;
    }

    public async Task<ErrorOr<Contract>> Handle(GetContractByIdQuery request, CancellationToken cancellationToken)
    {
        var contractId = ContractId.Create(request.ContractId);
        var contract = await _contractRepository.GetContractByIdAsync(contractId, cancellationToken);

        return contract is not null
            ? contract
            : GeneralErrors.NotFound(request.ContractId, typeof(Contract));
    }
}
