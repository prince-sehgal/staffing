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
        //db.position.insert({"designation":"SSE","lob":"GTSS","worklocation":"Gurgaon","shift":"11:30 AM to 8:30 PM","role":"Understand requirements, application area as it relates to assigned projects/tasks","Skills":"Must to have hands-on development & design experience in C#, ASP.NET, MVC 5.0 (Razor View Engine), JavaScript, AJAX, LINQ","taleonumber":"GUR007R8"})
        public string designation { get; set; }
        public string lob { get; set; }

        public string sublob { get; set; }

        public string worklocation { get; set; }
        public string shift { get; set; }
        public string role { get; set; }
        public string Skills { get; set; }
        public string taleonumber { get; set; }
        public string releasedon { get; set; }

        public string title {
            get {
                return this.designation + "-" + taleonumber;
            }
        }
        [Required(ErrorMessage = "First Name is required")]
        public string firstname { get; set; }

        //[Required(ErrorMessage = "Middle Name is required")]
        public string midname { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string lastname { get; set; }
        
        [Required(ErrorMessage = "Email id is required")]
        public string emailid { get; set; }

        [Required(ErrorMessage = "Mobile Number is required")]
        public string mobile { get; set; }

        [Required(ErrorMessage = "please select candidate date of birth")]
        public string dob { get; set; }   

        [Required(ErrorMessage = "Primary Skill is required")]
        public string primaryskill { get; set; }

        [Required(ErrorMessage = "Secondary Skill is required")]
        public string secondaryskill { get; set; }

        [Required(ErrorMessage = "Total experience is required")]
        public string totalexp { get; set; }

        [Required(ErrorMessage = "Current company experience is required")]
        public string currentexp { get; set; }

        [Required(ErrorMessage = "Current location is required")]
        public string currentlocation { get; set; }

        [Required(ErrorMessage = "Preffered location is required")]
        public string preferredlocation { get; set; }

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