using FluentValidation;

namespace BRP.VendorManagement.Application.Vendors.Queries.GetById;
internal sealed class GetVendorByIdQueryValidator : AbstractValidator<GetVendorByIdQuery>
{
    public GetVendorByIdQueryValidator()
    {
        RuleFor(x => x.VendorId)
            .NotEmpty()
            .WithMessage("VendorId is required.");
    }
}
