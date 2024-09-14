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
        public string IDUser { get; set; }
        public int Status { get; set; }
        public string DaysLeft
        {
            get
            {
                TimeSpan timeSpan = EndDate - DateTime.Now;

                if (timeSpan.TotalSeconds < 0)
                {
                    return "Đã hết hạn";
                }

                int days = timeSpan.Days;
                int hours = timeSpan.Hours;
                int minutes = timeSpan.Minutes;
                int seconds = timeSpan.Seconds;

                return $"{days} ngày, {hours} giờ, {minutes} phút, {seconds} giây";
            }
        }


    }
}
