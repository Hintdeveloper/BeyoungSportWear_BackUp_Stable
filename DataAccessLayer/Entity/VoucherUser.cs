using DataAccessLayer.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entity
{
    public partial class VoucherUser : NoIDEntityBase
    {
        public Guid IDVoucher {  get; set; }
        public string IDUser { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }    
        public virtual Voucher Voucher { get; set; }
    }
}
