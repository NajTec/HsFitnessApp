using System;
using System.Collections.Generic;

namespace FitnessApp.Models
{
    public partial class UserData
    {
        public UserData()
        {
            RefreshToken = new HashSet<RefreshToken>();
            UserFood = new HashSet<UserFood>();
            UserSport = new HashSet<UserSport>();
        }

        public string Alias { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public DateTime? Birthday { get; set; }
        public int? Size { get; set; }
        public int? Weight { get; set; }
        public double? SleepTime { get; set; }
        public string Gender { get; set; }
        public string Region { get; set; }

        public virtual UserSportTotal UserSportTotal { get; set; }
        public virtual ICollection<RefreshToken> RefreshToken { get; set; }
        public virtual ICollection<UserFood> UserFood { get; set; }
        public virtual ICollection<UserSport> UserSport { get; set; }
    }
}
