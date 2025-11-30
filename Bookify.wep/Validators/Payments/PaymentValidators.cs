using System;
using Bookify.wep.Models.Payments;
using FluentValidation;

namespace Bookify.wep.Validators.Payments
{
    public class CreatePaymentDtoValidator : AbstractValidator<CreatePaymentDto>
    {
        public CreatePaymentDtoValidator()
        {
            RuleFor(x => x.BookingId).GreaterThan(0);
            RuleFor(x => x.StripePaymentIntentId).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Status).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.PaymentDate).LessThanOrEqualTo(DateTime.UtcNow.AddDays(1));
        }
    }

    public class UpdatePaymentDtoValidator : AbstractValidator<UpdatePaymentDto>
    {
        public UpdatePaymentDtoValidator()
        {
            RuleFor(x => x.BookingId).GreaterThan(0);
            RuleFor(x => x.StripePaymentIntentId).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Status).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.PaymentDate).LessThanOrEqualTo(DateTime.UtcNow.AddDays(1));
        }
    }
}


