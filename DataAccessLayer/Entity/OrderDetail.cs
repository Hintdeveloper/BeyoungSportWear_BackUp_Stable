using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entity;

public partial class OrderDetail
{
    public Guid Id { get; set; }

    public Guid Idorder { get; set; }

    public Guid Idoptions { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal? Discount { get; set; }

    public string CreateBy { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? DeleteBy { get; set; }

    public DateTime? DeleteDate { get; set; }

    public int Status { get; set; }

    public virtual Options IdoptionsNavigation { get; set; } = null!;

    public virtual Order IdorderNavigation { get; set; } = null!;
}
