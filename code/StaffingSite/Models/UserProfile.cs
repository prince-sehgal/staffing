using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace StaffingSite.Models
{
    public class UserProfile
    {
        public ObjectId _id{ get; set; }

        [Required(ErrorMessage = "UserName is required")]
        public string username{ get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string password { get; set; }

        public List<Positions> list { get; set; }
        public string name { get; set; }
        public string lob { get; set; }
        public string photo { get; set; }
        public string usertype{ get; set; }
        public string isactive { get; set; }
    }
}