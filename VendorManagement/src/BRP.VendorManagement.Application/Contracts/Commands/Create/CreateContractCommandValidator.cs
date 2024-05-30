using FluentValidation;

namespace BRP.VendorManagement.Application.Contracts.Commands.Create;
internal sealed class CreateContractCommandValidator : AbstractValidator<CreateContractCommand>
{
    public CreateContractCommandValidator()
    {
        RuleFor(c => c.VendorId)
            .NotEmpty()
            .WithMessage("VendorId is required and cannot be an empty identifier");

        RuleFor(c => c.Title)
            .NotEmpty()
            .WithMessage("Title is required");

        RuleFor(c => c.DeadLineUtc)
            .Must(date => date != DateTime.MinValue)
            .WithMessage("DeadLineUtc is required");
    }
}
