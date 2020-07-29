using System;
using System.Collections.Generic;

namespace FitnessApp.Models
{
    public partial class UserSleep
    {
        public string UserAlias { get; set; }
        public float? SleepTimeDay { get; set; }
        public float? SleepTimeWeek { get; set; }
        public float? SleepTimeMonth { get; set; }

        public virtual UserData UserAliasNavigation { get; set; }
    }
}
