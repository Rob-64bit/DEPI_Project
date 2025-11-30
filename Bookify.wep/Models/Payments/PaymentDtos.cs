using System;
using Bookify.Domain.Entities;

namespace Bookify.wep.Models.Payments
{
    public record PaymentDto(int Id, int BookingId, string StripePaymentIntentId, string Status, decimal Amount, DateTime PaymentDate);

    public record CreatePaymentDto(int BookingId, string StripePaymentIntentId, string Status, decimal Amount, DateTime PaymentDate);

    public record UpdatePaymentDto(int BookingId, string StripePaymentIntentId, string Status, decimal Amount, DateTime PaymentDate);

    public static class PaymentMappings
    {
        public static PaymentDto ToDto(this Payment entity)
            => new PaymentDto(entity.Id, entity.BookingId, entity.StripePaymentIntentId, entity.Status, entity.Amount, entity.PaymentDate);

        public static Payment ToEntity(this CreatePaymentDto dto)
            => new Payment
            {
                BookingId = dto.BookingId,
                StripePaymentIntentId = dto.StripePaymentIntentId,
                Status = dto.Status,
                Amount = dto.Amount,
                PaymentDate = dto.PaymentDate
            };

        public static void Apply(this UpdatePaymentDto dto, Payment entity)
        {
            entity.BookingId = dto.BookingId;
            entity.StripePaymentIntentId = dto.StripePaymentIntentId;
            entity.Status = dto.Status;
            entity.Amount = dto.Amount;
            entity.PaymentDate = dto.PaymentDate;
        }
    }
}


