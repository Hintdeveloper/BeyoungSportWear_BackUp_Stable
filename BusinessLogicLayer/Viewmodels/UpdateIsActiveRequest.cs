using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels
{
    public class UpdateIsActiveRequest
    {
        public Guid IDEntity { get; set; }
        public bool IsActive { get; set; }
    }
}
