using System;
using System.Collections.Generic;

namespace FitnessApp.Models
{
    public partial class UserFoodTotal
    {
        public string UserAlias { get; set; }
        public int? TotalGramDay { get; set; }
        public int? TotalGramWeek { get; set; }
        public int? TotalGramMonth { get; set; }
        public int? TotalKcalDay { get; set; }
        public int? TotalKcalWeek { get; set; }
        public int? TotalKcalMonth { get; set; }

        public virtual UserData UserAliasNavigation { get; set; }
    }
}
