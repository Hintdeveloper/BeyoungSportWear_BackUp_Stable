using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.VietQR
{
    public class VietQRRequest
    {
        public string AccountNo { get; set; }
        public string AccountName { get; set; }
        public int AcqId { get; set; }
        public int? Amount { get; set; }
        public string AddInfo { get; set; }
        public string Format { get; set; }
        public string Template { get; set; }
    }
}
