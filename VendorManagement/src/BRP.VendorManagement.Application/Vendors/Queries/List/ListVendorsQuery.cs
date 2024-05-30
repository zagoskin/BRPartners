using BRP.VendorManagement.Domain.VendorAggregate;
using ErrorOr;
using MediatR;

namespace BRP.VendorManagement.Application.Vendors.Queries.List;
public record ListVendorsQuery() : IRequest<ErrorOr<List<Vendor>>>;
