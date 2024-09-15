using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.Entity.Base.EnumBase;
using static DataAccessLayer.Entity.Voucher;

namespace BusinessLogicLayer.Viewmodels.Voucher
{
    public class VoucherVM
    {
        public Guid ID { get; set; }
        public DateTime CreateDate { get; set; }
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
        public List<string> IDUser { get; set; }
        public int Status { get; set; }
        public int IsUsed { get; set; }
    }
}
