using Bookify.Domain.Entities;

namespace Bookify.wep.Models.Guests
{
    public record GuestDto(int Gid, string Phone, string Fullname, int? UserId);

    public record CreateGuestDto(string Phone, string Fullname, int? UserId);

    public record UpdateGuestDto(string Phone, string Fullname, int? UserId);

    public static class GuestMappings
    {
        public static GuestDto ToDto(this Guest entity)
            => new GuestDto(entity.Gid, entity.Phone, entity.Fullname, entity.UserId);

        public static Guest ToEntity(this CreateGuestDto dto)
            => new Guest
            {
                Phone = dto.Phone,
                Fullname = dto.Fullname,
                UserId = dto.UserId
            };

        public static void Apply(this UpdateGuestDto dto, Guest entity)
        {
            entity.Phone = dto.Phone;
            entity.Fullname = dto.Fullname;
            entity.UserId = dto.UserId;
        }
    }
}


