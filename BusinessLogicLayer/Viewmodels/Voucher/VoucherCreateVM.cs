using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.Entity.Base.EnumBase;
using static DataAccessLayer.Entity.Voucher;

namespace BusinessLogicLayer.Viewmodels.Voucher
{
    public class VoucherCreateVM
    {
        public string CreateBy { get; set; }
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
        public List<string> SelectedUser { get; set; } = new List<string>();
        public int Status { get; set; }
    }
}
