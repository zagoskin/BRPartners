using BRP.VendorManagement.Domain.VendorAggregate;
using BRP.VendorManagement.Domain.VendorAggregate.ValueObjects;
using ErrorOr;
using MediatR;

namespace BRP.VendorManagement.Application.Vendors.Queries.GetById;

#warning Para todos os retornos usei a entidade direto para simplificar, já que de todas formas o controller retorna um response que não é a entidade mas dependendo a complexidade do projeto poderia existir um "Dto" do application para não expor a entidade de domínio
public record GetVendorByIdQuery(Guid VendorId)
    : IRequest<ErrorOr<Vendor>>;
