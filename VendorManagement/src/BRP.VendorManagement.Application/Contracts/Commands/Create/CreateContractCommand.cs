using BRP.VendorManagement.Domain.ContractAggregate;
using ErrorOr;
using MediatR;

namespace BRP.VendorManagement.Application.Contracts.Commands.Create;
public record CreateContractCommand(
    Guid VendorId,
    string Title,    
    DateTime DeadLineUtc,    
    decimal EstimatedValue) : IRequest<ErrorOr<Contract>>;
