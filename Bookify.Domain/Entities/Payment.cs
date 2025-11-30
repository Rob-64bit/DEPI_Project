using System;
using System.Collections.Generic;

namespace Bookify.Domain.Entities;

public partial class Payment
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public string StripePaymentIntentId { get; set; } = null!;

    public string Status { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}


