using BRP.ContractManagement.Application.Contracts;
using BRP.VendorManagement.Domain.ContractAggregate;
using ErrorOr;
using MediatR;

namespace BRP.VendorManagement.Application.Contracts.Queries.List;

internal sealed class ListContractsQueryHandler : IRequestHandler<ListContractsQuery, ErrorOr<List<Contract>>>
{
    private readonly IContractRepository _contractRepository;

    public ListContractsQueryHandler(IContractRepository contractRepository)
    {
        _contractRepository = contractRepository;
    }

    public async Task<ErrorOr<List<Contract>>> Handle(ListContractsQuery request, CancellationToken cancellationToken)
    {
        var contracts = await _contractRepository.ListContractsAsync(cancellationToken);
        return contracts;
    }
}
