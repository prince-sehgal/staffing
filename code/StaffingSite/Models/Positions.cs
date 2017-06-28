using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StaffingSite.Models
{
    public class Positions
    {
        public ObjectId _id { get; set; }

        public string level { get; set; }
        public string title { get; set; }
        public string Description { get; set; }
        public string releasedon { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string name { get; set; }

        [Required(ErrorMessage = "Email id is required")]
        public string emailid { get; set; }

        [Required(ErrorMessage = "Mobile Number is required")]
        public string mobile { get; set; }
        public string attachment { get; set; }
        public DateTime referedon { get; set; }

        [Required(ErrorMessage = "Please select a valid position")]
        public string jobid { get; set; }
        public SelectList positions { get; set; }
    }
    public class candidate {
        public ObjectId _id { get; set; }

        public string name { get; set; }

        public string emailid { get; set; }

        public string mobile { get; set; }

        public string attachment { get; set; }
        public DateTime referedon { get; set; }
        public ObjectId jobid { get; set; }
    }
}