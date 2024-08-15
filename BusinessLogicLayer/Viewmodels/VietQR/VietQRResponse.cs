using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.VietQR
{
    public class VietQRResponse
    {
        public string Code { get; set; }
        public string Desc { get; set; }
        public VietQRData Data { get; set; }
    }
}
