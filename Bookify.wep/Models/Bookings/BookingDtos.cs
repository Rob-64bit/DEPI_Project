using System;
using Bookify.Domain.Entities;

namespace Bookify.wep.Models.Bookings
{
    public record BookingDto(int BookingId, int RoomId, int Gid, DateOnly CheckIn, DateOnly CheckOut, string BookingStatus);

    public record CreateBookingDto(int RoomId, int Gid, DateOnly CheckIn, DateOnly CheckOut, string BookingStatus);

    public record UpdateBookingDto(int RoomId, int Gid, DateOnly CheckIn, DateOnly CheckOut, string BookingStatus);

    public static class BookingMappings
    {
        public static BookingDto ToDto(this Booking entity)
            => new BookingDto(entity.BookingId, entity.RoomId, entity.Gid, entity.CheckIn, entity.CheckOut, entity.BookingStatus);

        public static Booking ToEntity(this CreateBookingDto dto)
            => new Booking
            {
                RoomId = dto.RoomId,
                Gid = dto.Gid,
                CheckIn = dto.CheckIn,
                CheckOut = dto.CheckOut,
                BookingStatus = dto.BookingStatus
            };

        public static void Apply(this UpdateBookingDto dto, Booking entity)
        {
            entity.RoomId = dto.RoomId;
            entity.Gid = dto.Gid;
            entity.CheckIn = dto.CheckIn;
            entity.CheckOut = dto.CheckOut;
            entity.BookingStatus = dto.BookingStatus;
        }
    }
}


