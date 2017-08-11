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

        [Required(ErrorMessage = "Designation is required")]
        public string designation { get; set; }
        [Required(ErrorMessage = "Line of Business is required")]
        public string lob { get; set; }

        public string sublob { get; set; }

        public int headcount { get; set; }

        [Required(ErrorMessage = "Work Location is required")]
        public string worklocation { get; set; }

        [Required(ErrorMessage = "Shift Time is required")]
        public string shift { get; set; }
        [Required(ErrorMessage = "Role is required")]
        public string role { get; set; }
        [Required(ErrorMessage = "Skills are required")]
        public string Skills { get; set; }

        [Required(ErrorMessage = "Taleo Number is required")]
        public string taleonumber { get; set; }

        public string releasedon { get; set; }

        [Required(ErrorMessage = "Position Validity is required")]
        public string validity { get; set; }

        [Required(ErrorMessage = "Start Date is required")]
        public string sdate { get; set; }
        [Required(ErrorMessage = "End Date is required")]
        public string edate { get; set; }

        public DateTime publishedon { get; set; }

        public string title {
            get {
                return this.designation + "-" + taleonumber;
            }
        }
        public string FullName {
            get { 
                return firstname+' '+midname +' '+ lastname;
            }
        }
        [Required(ErrorMessage = "First Name is required")]
        public string firstname { get; set; }

        //[Required(ErrorMessage = "Middle Name is required")]
        public string midname { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string lastname { get; set; }
        
        [Required(ErrorMessage = "Email id is required")]
        [EmailAddress(ErrorMessage = "Invalid Email id")]
        public string emailid { get; set; }

        [Required(ErrorMessage = "Mobile Number is required")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
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

        public string referedby { get; set; }

        public string publishedby { get; set; }
        public string attachment { get; set; }

        public DateTime referedon { get; set; }

        [Required(ErrorMessage = "Please select any position")]
        public string jobid { get; set; }

        public SelectList positions { get; set; }

        public SelectList Shifts { get; set; }
    }
    public class Shift {
        public int id { get; set; }
        public string Name { get; set; }
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


    public class ReferredList
    {
        
        public int headcount { get; set; }
        public ObjectId _id { get; set; }
        public Int64 lob { get; set; }
        public string designation { get; set; }
        public Int64 sublob { get; set; }
        public Int64 referenceid { get; set; }
        public string worklocation { get; set; }
        public string shift { get; set; }
        public string role { get; set; }
        public string Skills { get; set; }
        public string taleonumber { get; set; }
        public string releasedon { get; set; }

        public string title
        {
            get
            {
                return this.designation + "-" + taleonumber;
            }
        }
        public string FullName
        {
            get
            {
                return firstname + ' ' + midname + ' ' + lastname;
            }
        }
        public string firstname { get; set; }

        public string midname { get; set; }

        public string lastname { get; set; }

        public string emailid { get; set; }

        public string mobile { get; set; }

        public string dob { get; set; }

        public string primaryskill { get; set; }

        public string secondaryskill { get; set; }

        public string totalexp { get; set; }

        public string currentexp { get; set; }

        public string currentlocation { get; set; }

        public string preferredlocation { get; set; }

        public string referedby { get; set; }

        public string attachment { get; set; }

        public DateTime referedon { get; set; }

        public string jobid { get; set; }

        public SelectList positions { get; set; }
    }
}