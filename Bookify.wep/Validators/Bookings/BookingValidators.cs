using System;
using Bookify.wep.Models.Bookings;
using FluentValidation;

namespace Bookify.wep.Validators.Bookings
{
    public class CreateBookingDtoValidator : AbstractValidator<CreateBookingDto>
    {
        public CreateBookingDtoValidator()
        {
            RuleFor(x => x.RoomId).GreaterThan(0);
            RuleFor(x => x.Gid).GreaterThan(0);
            RuleFor(x => x.CheckIn).NotEmpty();
            RuleFor(x => x.CheckOut).GreaterThan(x => x.CheckIn);
            RuleFor(x => x.BookingStatus).NotEmpty().MaximumLength(20);
        }
    }

    public class UpdateBookingDtoValidator : AbstractValidator<UpdateBookingDto>
    {
        public UpdateBookingDtoValidator()
        {
            RuleFor(x => x.RoomId).GreaterThan(0);
            RuleFor(x => x.Gid).GreaterThan(0);
            RuleFor(x => x.CheckIn).NotEmpty();
            RuleFor(x => x.CheckOut).GreaterThan(x => x.CheckIn);
            RuleFor(x => x.BookingStatus).NotEmpty().MaximumLength(20);
        }
    }
}


