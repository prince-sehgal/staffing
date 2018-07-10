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
using System.ComponentModel;

namespace StaffingSite.Models
{
    public class CandidateData
    {
        public List<CandidateFeedback> Feedbacks;
        public List<InspectCandidate> Candidature;
        public CandidateLead Reference;
    }
    public class CandidateLead
    {
        public string name { get; set; }
        public string employeeid { get; set; }
        public string lob { get; set; }
    }
    public class InspectCandidate
    {
        public ObjectId _id { get; set; }
        public Int64 StatusId { get; set; }
        public string PositionTitle { get; set; }
        public string WorkdayId { get; set; }

    }
    public class CandidateFeedback
    {

        public ObjectId _id { get; set; }
        public string HrName { get; set; }
        public string Feedback { get; set; }
        public string Status { get; set; }
        public Int64 CandidateId { get; set; }
        public string DateString { get; set; }
        public Int64 StatusId { get; set; }
        public DateTime SavedOn { get; set; }
        public string SavedBy { get; set; }
    }
    public class Positions
    {
        public ObjectId _id { get; set; }
        //db.position.insert({"designation":"SSE","lob":"GTSS","worklocation":"Gurgaon","shift":"11:30 AM to 8:30 PM","role":"Understand requirements, application area as it relates to assigned projects/tasks","Skills":"Must to have hands-on development & design experience in C#, ASP.NET, MVC 5.0 (Razor View Engine), JavaScript, AJAX, LINQ","taleonumber":"GUR007R8"})

        [Required(ErrorMessage = "Designation is required")]
        public string designation { get; set; }
        [Required(ErrorMessage = "Line of Business is required")]
        public string lob { get; set; }

        [Required(ErrorMessage = "Mode of Interview is required")]
        public string modeofinterview { get; set; }
        [Required(ErrorMessage = "Recruiter Name is required")]
        public string RecruiterName { get; set; }

        public string sublob { get; set; }  

        public int candidatecount{ get; set; }

        [Required(ErrorMessage = "Number of Position is required")]
        [RegularExpression("\\d+", ErrorMessage = "Only numeric values are allowed")]
        public Int64? headcount { get; set; }

        //[Required(ErrorMessage = "Work Location is required")]
        public string worklocation { get; set; }

        public bool Gurgaon { get; set; }
        public bool ASF { get; set; }
        public bool Noida { get; set; }


        [Required(ErrorMessage = "Shift Time is required")]
        public string shift { get; set; }

        [Required(ErrorMessage = "Grade is required")]
        public string grade { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [AllowHtml]
        public string role { get; set; }

        [Required(ErrorMessage = "Skills are required")]
        [AllowHtml]
        public string Skills { get; set; }

        [Required(ErrorMessage = "Interview Details are required")]
        [AllowHtml]
        public string InterviewDetails { get; set; }


        [DisplayName("Diversity Position")]
        public bool Diversity_Position { get; set; }

        [Required(ErrorMessage = "Workday ID is required")]
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
        
        [Required(ErrorMessage = "Email Id is required")]
        [EmailAddress(ErrorMessage = "Invalid Email id")]
        public string emailid { get; set; }

        [Required(ErrorMessage = "Mobile Number is required")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string mobile { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        public string dob { get; set; }   

        [Required(ErrorMessage = "Primary Skill is required")]
        public string primaryskill { get; set; }

        [Required(ErrorMessage = "Secondary Skill is required")]
        public string secondaryskill { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public bool? IsFemale { get; set; }

        [Required(ErrorMessage = "Total Experience is required")]
        public string totalexp { get; set; }

        [Required(ErrorMessage = "Current Company is required")]
        public string currentexp { get; set; }

        [Required(ErrorMessage = "Current Location is required")]
        public string currentlocation { get; set; }

        [Required(ErrorMessage = "Preferred Location is required")]
        public string preferredlocation { get; set; }

        public string referedby { get; set; }

        public string publishedby { get; set; }
        public string attachment { get; set; }

        public DateTime referedon { get; set; }

        [Required(ErrorMessage = "Position is required")]
        public string jobid { get; set; }

        [Required(ErrorMessage = "Resume is required")]
        [RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.docx|.doc|.pdf)$", ErrorMessage = "Only docx | doc | pdf type allowed. Please select a valid file type.")]
        public HttpPostedFileBase PostedFile { get; set; }

        [RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.docx|.doc|.pdf)$", ErrorMessage = "Only docx | doc | pdf type allowed. Please select a valid file type.")]
        public HttpPostedFileBase AttachedFile { get; set; }
        public SelectList positions { get; set; }

        public SelectList Shifts { get; set; }

        public SelectList Grades { get; set; }

        public SelectList Recruiters { get; set; }

        public SelectList lobs { get; set; }
    }
    public class Lob
    {
        public int id { get; set; }
        public string Name { get; set; }
    }

    public class Dashboard
    {
        public string Name;
        public List<Positions> Positions;
        public List<ReferredList> candidates;
    }
    public class SPOC
    {
        public Int64 HeadCount;
        public string Name;
        public List<LOBPieData> LOBPieData;
        public List<LOBPieData> GenderPieData;
        public List<LineData> GradeLineData;
        public List<LineData> StatusAreaData;
    }
    public class LineData
    {
        public string Label;
        public Int64 data;
    }
    public class LOBPieData
    {
        public Int64 value;
        public string color;
        public string highlight;
        public string label;
    }
    public class Content
    {
        public string Name;
        public Int64 data;
    }
    public class PositionData
    {
        public Int64 HeadCount;
        public List<SPOC> Spoc;
        public List<LOBPieData> LOBPieData;
        public List<LOBPieData> GenderPieData;
        public List<LineData> GradeLineData;
        public List<LineData> StatusAreaData;

    }
    public class VisualInsights
    {
        public PositionData TotalPositions;
        public PositionData ActivePositions;
        public PositionData ClosedPositions;
        public Int64 ResumeUploaded;
    }
    public class Recruiter {
        public int id { get; set; }
        public string Name { get; set; }
    }
    public class Grade {
        public int id { get; set; }
        public string Name { get; set; }
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

        public bool? IsFemale { get; set; }
        public ObjectId _id { get; set; }
        public Int64 lob { get; set; }
        public string designation { get; set; }
        public Int64 sublob { get; set; }
        public Int64 referenceid { get; set; }
        public Int64 statusid { get; set; }
        public string worklocation { get; set; }
        public string shift { get; set; }
        public string grade { get; set; }
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

        //public string emailid { get; set; }

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