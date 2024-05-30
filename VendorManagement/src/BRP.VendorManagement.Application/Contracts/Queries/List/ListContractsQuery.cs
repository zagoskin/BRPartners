using BRP.VendorManagement.Domain.ContractAggregate;
using ErrorOr;
using MediatR;

namespace BRP.VendorManagement.Application.Contracts.Queries.List;
public record ListContractsQuery() : IRequest<ErrorOr<List<Contract>>>;
