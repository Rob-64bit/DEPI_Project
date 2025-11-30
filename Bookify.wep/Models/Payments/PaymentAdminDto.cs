namespace Bookify.wep.Models.Payments
{
    public record PaymentAdminDto(
        int Id,
        int BookingId,
        string? GuestName,
        string StripePaymentIntentId,
        string Status,
        decimal Amount,
        DateTime PaymentDate
    );
}
