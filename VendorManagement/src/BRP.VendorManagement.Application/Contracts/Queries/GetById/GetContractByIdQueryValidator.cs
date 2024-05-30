using FluentValidation;

namespace BRP.VendorManagement.Application.Contracts.Queries.GetById;
internal sealed class GetContractByIdQueryValidator : AbstractValidator<GetContractByIdQuery>
{
    public GetContractByIdQueryValidator()
    {
        RuleFor(x => x.ContractId)
            .NotEmpty()
            .WithMessage("ContractId is required.");
    }
}
