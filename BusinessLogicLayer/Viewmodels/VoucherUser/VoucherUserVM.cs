using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.VoucherUser
{
    public class VoucherUserVM
    {
        public Guid IDVoucher { get; set; }
        public string IDUser { get; set; } = null!;
        public int Status { get; set; }
    }
}
