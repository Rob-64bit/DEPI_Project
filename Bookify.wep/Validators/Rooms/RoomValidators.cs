using Bookify.wep.Models.Rooms;
using FluentValidation;

namespace Bookify.wep.Validators.Rooms
{
    public class CreateRoomDtoValidator : AbstractValidator<CreateRoomDto>
    {
        public CreateRoomDtoValidator()
        {
            RuleFor(x => x.RoomType).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Rstatus).NotEmpty().MaximumLength(10);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        }
    }

    public class UpdateRoomDtoValidator : AbstractValidator<UpdateRoomDto>
    {
        public UpdateRoomDtoValidator()
        {
            RuleFor(x => x.RoomType).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Rstatus).NotEmpty().MaximumLength(10);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        }
    }
}


