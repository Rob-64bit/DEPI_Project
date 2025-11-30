using System;
using System.Collections.Generic;

namespace Bookify.Domain.Entities;

public partial class Booking
{
    public int BookingId { get; set; }

    public int RoomId { get; set; }

    public int Gid { get; set; }

    public DateOnly CheckIn { get; set; }

    public DateOnly CheckOut { get; set; }

    public string BookingStatus { get; set; } = null!;

    public virtual Guest GidNavigation { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Room Room { get; set; } = null!;
}


