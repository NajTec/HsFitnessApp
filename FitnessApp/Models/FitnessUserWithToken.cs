using FitnessApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


    public class FitnessUserWithToken : UserData
    {

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public FitnessUserWithToken(UserData user)
        {
            this.Alias = user.Alias;
            this.Password = user.Password;
            this.Name = user.Name;
            this.Size = user.Size;
            this.Region = user.Region;
            this.SleepTime = user.SleepTime;
            

          //  this.Role = user.Role;
        }
    }
