using DataAccessLayer.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entity
{
    public partial class OrderHistory : EntityBase
    {
        public string IDUser { get; set; }
        public Guid IDOrder { get; set; }
        public string EditingHistory { get; set; }
        public DateTime ChangeDate { get; set; }
        public string ChangeType { get; set; }
        public string ChangeDetails { get; set; }
        public string? BillOfLadingCode { get; set; }
        public virtual Order Order { get; set; }    
    }
}
