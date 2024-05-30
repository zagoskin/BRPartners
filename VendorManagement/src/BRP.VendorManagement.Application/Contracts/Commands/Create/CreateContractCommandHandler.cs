using BRP.ContractManagement.Application.Contracts;
using BRP.VendorManagement.Application.Vendors;
using BRP.VendorManagement.Domain.Common.Abstractions;
using BRP.VendorManagement.Domain.ContractAggregate;
using BRP.VendorManagement.Domain.VendorAggregate.ValueObjects;
using ErrorOr;
using MediatR;

namespace BRP.VendorManagement.Application.Contracts.Commands.Create;
internal sealed class CreateContractCommandHandler : IRequestHandler<CreateContractCommand, ErrorOr<Contract>>
{
    private readonly IVendorRepository _vendorRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IUser _user;

    public CreateContractCommandHandler(IVendorRepository vendorRepository, IContractRepository contractRepository, IUser user)
    {
        _vendorRepository = vendorRepository;
        _contractRepository = contractRepository;
        _user = user;
    }
    public async Task<ErrorOr<Contract>> Handle(CreateContractCommand request, CancellationToken cancellationToken)
    {
        var vendorId = VendorId.Create(request.VendorId);
        if (!await _vendorRepository.ExistsByIdAsync(vendorId, cancellationToken))
        {
            return Error.Validation(description: $"Vendor '{request.VendorId}' does not exist");
        }

        var createContractResult = Contract.Create(
            _user,
            request.Title,
            vendorId,
            request.DeadLineUtc,
            request.EstimatedValue);

        if (createContractResult.IsError)
        {
            return createContractResult.Errors;
        }

        var contract = createContractResult.Value;
        await _contractRepository.AddContractAsync(contract);
        return contract;
    }
}
