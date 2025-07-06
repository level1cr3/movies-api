using FluentValidation;
using Movies.Application.DTOs.Auth;

namespace Movies.Application.Features.Auth.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(r => r.FirstName).NotEmpty().MaximumLength(200);

        RuleFor(r => r.LastName).MaximumLength(200);

        RuleFor(r => r.Email).NotEmpty().EmailAddress().MaximumLength(200);

        RuleFor(r => r.Password).NotEmpty()
            .Length(12, 128).WithMessage("Password must be between 12 and 128 characters")
            .Matches(@"[!@#$%^&*]").WithMessage("Password must contain at least one special character (!@#$%^&*)")
            .Must(p => p.Any(char.IsUpper)).WithMessage("Password must contain at least one uppercase letter")
            .Must(p => p.Any(char.IsLower)).WithMessage("Password must contain at least one lowercase letter")
            .Must(p => p.Any(char.IsDigit)).WithMessage("Password must contain at least one number")
            .Must(p => !p.Any(char.IsWhiteSpace)).WithMessage("Password cannot contain spaces");
    }
}