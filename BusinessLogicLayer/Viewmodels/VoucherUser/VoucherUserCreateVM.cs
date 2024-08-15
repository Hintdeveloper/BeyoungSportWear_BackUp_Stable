using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.VoucherUser
{
    public class VoucherUserCreateVM
    {
        public string CreateBy { get; set; }
        public Guid IDVoucher { get; set; }
        public string IDUser { get; set; } = null!;
        public int Status { get; set; }
    }
}
