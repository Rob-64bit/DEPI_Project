using Bookify.Domain.Entities;

namespace Bookify.wep.Models.Rooms
{
    public record RoomDto(int RoomId, string RoomType, string Rstatus, int Price);

    public record CreateRoomDto(string RoomType, string Rstatus, int Price);

    public record UpdateRoomDto(string RoomType, string Rstatus, int Price);

    public static class RoomMappings
    {
        public static RoomDto ToDto(this Room entity)
            => new RoomDto(entity.RoomId, entity.RoomType, entity.Rstatus, entity.Price);

        public static Room ToEntity(this CreateRoomDto dto)
            => new Room
            {
                RoomType = dto.RoomType,
                Rstatus = dto.Rstatus,
                Price = dto.Price
            };

        public static void Apply(this UpdateRoomDto dto, Room entity)
        {
            entity.RoomType = dto.RoomType;
            entity.Rstatus = dto.Rstatus;
            entity.Price = dto.Price;
        }
    }
}


