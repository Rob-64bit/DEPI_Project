using System;
using System.Collections.Generic;

namespace Bookify.Domain.Entities;

public partial class Guest
{
    public int Gid { get; set; }

    public string Phone { get; set; } = null!;

    public string Fullname { get; set; } = null!;

    public int? UserId { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Usere? User { get; set; }
}


