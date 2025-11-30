using System;
using System.Collections.Generic;

namespace Bookify.Domain.Entities;

public partial class Room
{
    public int RoomId { get; set; }

    public string RoomType { get; set; } = null!;

    public string Rstatus { get; set; } = null!;

    public int Price { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}


