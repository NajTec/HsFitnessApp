using System;
using System.Collections.Generic;

namespace FitnessApp.Models
{
    public partial class FoodData
    {
        public FoodData()
        {
            UserFood = new HashSet<UserFood>();
        }

        public string Name { get; set; }
        public int? Kcal100g { get; set; }

        public virtual ICollection<UserFood> UserFood { get; set; }
    }
}
