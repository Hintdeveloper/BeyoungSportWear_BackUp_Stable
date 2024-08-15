using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.Entity.Base.EnumBase;

namespace BusinessLogicLayer.Viewmodels
{
    public class SearchCondition
    {
        public SearchCriteria Criteria { get; set; }
        public string Value { get; set; }
    }

}
