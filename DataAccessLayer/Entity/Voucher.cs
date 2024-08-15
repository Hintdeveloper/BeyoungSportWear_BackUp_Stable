using DataAccessLayer.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.Entity.Base.EnumBase;

namespace DataAccessLayer.Entity
{
    public partial class Voucher : EntityBase
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Quantity { get; set; }
        public Types Type { get; set; }
        public decimal MinimumAmount { get; set; }
        public decimal MaximumAmount { get; set; }
        public decimal ReducedValue { get; set; }
        public StatusVoucher? IsActive { get; set; }
        public enum Types
        {
            Percent,
            Cash,
        }
        public virtual ICollection<VoucherUser> VoucherUser { get; set; } = null!;
    }
}
