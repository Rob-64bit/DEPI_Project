using Bookify.wep.Models.Guests;
using FluentValidation;

namespace Bookify.wep.Validators.Guests
{
    public class CreateGuestDtoValidator : AbstractValidator<CreateGuestDto>
    {
        public CreateGuestDtoValidator()
        {
            RuleFor(x => x.Fullname).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
        }
    }

    public class UpdateGuestDtoValidator : AbstractValidator<UpdateGuestDto>
    {
        public UpdateGuestDtoValidator()
        {
            RuleFor(x => x.Fullname).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
        }
    }
}


