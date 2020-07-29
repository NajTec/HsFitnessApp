using System;
using System.Collections.Generic;

namespace FitnessApp.Models
{
    public partial class RefreshToken
    {
        public int TokenId { get; set; }
        public string Alias { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }

        public virtual UserData AliasNavigation { get; set; }
    }
}
