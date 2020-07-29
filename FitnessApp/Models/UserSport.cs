using System;
using System.Collections.Generic;

namespace FitnessApp.Models
{
    public partial class UserSport
    {
        public string UserAlias { get; set; }
        public string SportName { get; set; }
        public int? SportTimeDay { get; set; }
        public int? SportTimeMonth { get; set; }
        public int? SportTimeWeek { get; set; }
        public int? SportKcalDay { get; set; }
        public int? SportKcalWeek { get; set; }
        public int? SportKcalMonth { get; set; }

        public virtual SportData SportNameNavigation { get; set; }
        public virtual UserData UserAliasNavigation { get; set; }
    }
}
