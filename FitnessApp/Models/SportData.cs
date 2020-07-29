using System;
using System.Collections.Generic;

namespace FitnessApp.Models
{
    public partial class SportData
    {
        public SportData()
        {
            UserSport = new HashSet<UserSport>();
        }

        public string Name { get; set; }
        public int? KcalH { get; set; }

        public virtual ICollection<UserSport> UserSport { get; set; }
    }
}
