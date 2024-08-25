using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.Entity.Base.EnumBase;
using static DataAccessLayer.Entity.Voucher;

namespace BusinessLogicLayer.Viewmodels.VoucherM
{
    public class GetAllVoucherVM
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
    }
    public class VoucherViewModel
    {
        public Guid ID { get; set; }
        public Guid IDUser { get; set; }
        public decimal MinimumAmount { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Quantity { get; set; }
        public decimal ReducedValue { get; set; }
        public StatusVoucher? IsActive { get; set; }
        public int status { get; set; }
        public int DaysLeft
        {
            get
            {
                return (EndDate - DateTime.Today).Days;
            }
        }

    }
}
