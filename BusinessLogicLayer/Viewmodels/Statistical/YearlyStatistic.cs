using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.Statistical
{
    public class YearlyStatistic
    {
        public int Year { get; set; }
        public List<decimal> MonthlyRevenues { get; set; }
    }
}
