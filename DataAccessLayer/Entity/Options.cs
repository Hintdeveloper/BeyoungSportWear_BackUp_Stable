using DataAccessLayer.Entity.Base;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entity
{
    public partial class Options : EntityBase
    {
        public Guid IDProductDetails { get; set; }
        public Guid IDColor { get; set; }
        public Guid IDSize { get; set; }
        public int StockQuantity { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal? Discount { get; set; }
        public string? ImageURL { get; set; }
        public bool IsActive { get; set; }
        public virtual Colors Colors { get; set; } = null!;
        public virtual Sizes Sizes { get; set; } = null!;
        public virtual ProductDetails ProductDetails { get; set; } = null!;
        public virtual ICollection<OrderDetails> OrderDetails { get; set; }
        public virtual ICollection<CartOptions> CartOptions { get; set; }

    }
}
