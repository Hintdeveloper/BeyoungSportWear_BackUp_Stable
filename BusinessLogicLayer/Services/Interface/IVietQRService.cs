using BusinessLogicLayer.Viewmodels.VietQR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.Interface
{
    public interface IVietQRService
    {
        public Task<VietQRResponse> GenerateQR(VietQRRequest request);
    }
}
