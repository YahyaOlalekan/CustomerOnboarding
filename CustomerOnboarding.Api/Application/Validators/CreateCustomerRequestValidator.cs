using CustomerOnboarding.Api.DTOs;
using FluentValidation;


namespace CustomerOnboarding.Api.Application.Validators
{
    public sealed class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
    {
        public CreateCustomerRequestValidator()
        {
            RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^0[789][01]\d{8}$").WithMessage("Invalid Nigerian phone number format");
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
            RuleFor(x => x.StateId).GreaterThan(0);
            RuleFor(x => x.LgaId).GreaterThan(0);
        }
    }
}
