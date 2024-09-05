using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.OrderHistory
{
    public class OrderHistoryUpdateVM
    {
        public string ModifiedBy { get; set; }
        public string IDUser { get; set; }
        public Guid IDOrder { get; set; }
        public string EditingHistory { get; set; }
        public DateTime ChangeDate { get; set; }
        public string ChangeType { get; set; }
        public string ChangeDetails { get; set; }
        public int Status { get; set; }
        public string? BillOfLadingCode { get; set; }

    }
}
