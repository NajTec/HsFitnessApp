using System;
using System.Collections.Generic;

namespace FitnessApp.Models
{
    public partial class UserFood
    {
        public string UserAlias { get; set; }
        public string FoodName { get; set; }
        public int? FoodGramDay { get; set; }
        public int? FoodGramWeek { get; set; }
        public int? FoodGramMonth { get; set; }
        public int? FoodKcalDay { get; set; }
        public int? FoodKcalWeek { get; set; }
        public int? FoodKcalMonth { get; set; }

        public virtual FoodData FoodNameNavigation { get; set; }
        public virtual UserData UserAliasNavigation { get; set; }
    }
}
