using System;
using System.Collections.Generic;

namespace FitnessApp.Models
{
    public partial class UserSportTotal
    {
        public string UserAlias { get; set; }
        public int? TotalTimeDay { get; set; }
        public int? TotalTimeWeek { get; set; }
        public int? TotalTimeMonth { get; set; }
        public int? TotalKcalDay { get; set; }
        public int? TotalKcalWeek { get; set; }
        public int? TotalKcalMonth { get; set; }

        public virtual UserData UserAliasNavigation { get; set; }
    }
}
