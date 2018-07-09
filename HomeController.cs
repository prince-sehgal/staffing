using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StaffingSite.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using MongoDB.Driver.Builders;
using System.Configuration;   
using StaffingSite.MongoDB;
using System.Threading.Tasks;

namespace StaffingSite.Controllers
{
    public class HomeController : Controller
    {
        public MongoDBConnector Connector;
        public HomeController()
        {
            this.Connector = new MongoDBConnector();
        }
        private List<UserProfile> _UserList = new List<UserProfile>();

        public ActionResult Login()
        {
            try
            {
                string NetworkId = System.Web.HttpContext.Current.User.Identity.Name;
                var result = Connector.GetDatabase().GetCollection<UserProfile>("UserProfile").FindOne(Query.EQ("username",NetworkId.Substring(NetworkId.IndexOf("\\") + 1).ToLower()));
                if (result != null && Boolean.Parse(result.isactive))
                {
                    Session["UserID"] = result._id;
                    Session["UserType"] = result.usertype;
                    Session["lob"] = result.lob;
                    Session["name"] = result.name;
                    Session["employeeid"] = result.employeeid;
                    Session["photo"] = result.photo;
                    if (result.usertype == "0")
                        return RedirectToAction("HrDashboard");
                    else if (result.usertype == "1")
                        return RedirectToAction("Dashboard");
                    return View();
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }    
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
        [HttpPost]
        public ActionResult Login(UserProfile objUser)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = Connector.GetDatabase().GetCollection<UserProfile>("UserProfile").FindOne(Query.EQ("username", objUser.username.ToLower()));
                    if (result != null && Boolean.Parse(result.isactive))
                    {
                        if (result.password == objUser.password)
                        {
                            objUser.isactive = result.isactive;
                            objUser.name = result.name;
                            objUser._id = result._id;
                            objUser.photo = result.photo;
                            objUser.lob = result.lob;
                            Session["UserID"] = objUser._id;
                            Session["UserType"] = result.usertype;
                            Session["lob"] = result.lob;
                            Session["name"] = result.name;
                            Session["employeeid"] = result.employeeid; 
                            Session["photo"] = result.photo;
                            if (result.usertype == "0")
                                return RedirectToAction("Hr");
                            else if (result.usertype == "1")
                                return RedirectToAction("Dashboard");
                            return View();
                        }
                        else
                        {
                            ViewBag.Message = "Please verify your credentials.";
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Please verify your credentials.";
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View();
                }     
            }
            return View();
        }
        public ActionResult SelectStatus()
        {
            return View();
        }

        public ActionResult HrDashboard()
        {
            if (Session["UserID"] != null && Convert.ToString(Session["UserType"]) == "0")
            {
                try
                {
                    return View();
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult Hr()
        {
            if (Session["UserID"] != null && Convert.ToString(Session["UserType"]) == "0")
            {
                try
                {
                    UserProfile profile = new UserProfile();
                    var result = Connector.GetDatabase().GetCollection<UserProfile>("UserProfile").FindOne(Query.EQ("_id", ObjectId.Parse(Session["UserID"].ToString())));
                    profile.isactive = result.isactive;
                    profile.name = result.name;
                    profile._id = result._id;
                    profile.photo = result.photo;
                    profile.lob = result.lob;
                    profile.employeeid = result.employeeid;
                    profile.list = Connector.GetDatabase().GetCollection<Positions>("position").FindAll().ToList();
                    //profile.list = Connector.GetDatabase().GetCollection<Positions>("position").Find(Query.EQ("publishedby", Session["UserID"].ToString())).ToList();
                    foreach (Positions position in profile.list)
                    {
                        position.candidatecount = Connector.GetDatabase().GetCollection<ReferredList>("candidate").Find(Query.EQ("jobid", Convert.ToString(position._id))).ToList().Count;
                    }
                    List<SelectListItem> items = new List<SelectListItem>();
                    items.Add(new SelectListItem { Text = "To be evaluated", Value = "0" });
                    items.Add(new SelectListItem { Text = "CV shortlisted", Value = "1" });
                    items.Add(new SelectListItem { Text = "HR Shortlisted", Value = "2" });
                    items.Add(new SelectListItem { Text = "HR Rejected", Value = "3" });
                    items.Add(new SelectListItem { Text = "Operations Interview shortlisted", Value = "4" });
                    items.Add(new SelectListItem { Text = "Operations Interview rejected", Value = "5" });
                    items.Add(new SelectListItem { Text = "Profile on Hold", Value = "6" });
                    items.Add(new SelectListItem { Text = "Position on Hold", Value = "7" });
                    items.Add(new SelectListItem { Text = "To be offered", Value = "8" });
                    items.Add(new SelectListItem { Text = "Offered", Value = "9" });
                    items.Add(new SelectListItem { Text = "Joined", Value = "10" });
                    items.Add(new SelectListItem { Text = "Not Joined", Value = "11" });
                    //items.Add(new SelectListItem { Text = "In Process", Value = "0" });
                    //items.Add(new SelectListItem { Text = "Hr Discussion", Value = "1" });
                    //items.Add(new SelectListItem { Text = "Short Listed", Value = "2" });
                    //items.Add(new SelectListItem { Text = "Operation Round", Value = "3" });
                    //items.Add(new SelectListItem { Text = "Rejected", Value = "3" });
                    //items.Add(new SelectListItem { Text = "To be Offered", Value = "3" });
                    //items.Add(new SelectListItem { Text = "Offered", Value = "3" });
                    //items.Add(new SelectListItem { Text = "Join", Value = "3" });
                    ViewBag.StatusType = items;
                    return View(profile);
                }
                catch (Exception ex)
                {
                      ViewBag.Message = ex.Message;
                      return View();
                }     
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult Position(string workdayId)
        {
            if (Session["UserID"] != null && Convert.ToString(Session["UserType"]) == "0")
            {
                Session["WorkDayId"] = workdayId;
                Positions position = new Positions();
                try
                {
                    if (Session["WorkDayId"] != null)
                    {
                        MongoDatabase objDatabse = Connector.GetDatabase();
                        IMongoQuery query = Query.EQ("taleonumber", new BsonString(Session["WorkDayId"].ToString()));

                        Positions data = objDatabse.GetCollection<Positions>("position").Find(query).SingleOrDefault();
                        position.taleonumber = data.taleonumber;
                        position.sdate = data.sdate;
                        position.edate = data.edate;
                        position.grade = data.grade;
                        position.Diversity_Position = data.Diversity_Position;
                        position.designation = data.designation;
                        position.currentexp = data.currentexp;
                        position.currentlocation = data.currentlocation;
                        position.lob = data.lob;
                        position.headcount = data.headcount;
                        position.worklocation = data.worklocation;
                        position.modeofinterview = data.modeofinterview;
                        position.RecruiterName = data.RecruiterName;
                        position.IsFemale = data.IsFemale;
                        position.Skills = data.Skills;
                        position.InterviewDetails = data.InterviewDetails;
                        position.shift = data.shift;
                        position.role = data.role;
                        string[] worklocations = position.worklocation.Split(',');
                        foreach (string location in worklocations)
                        {
                            if (location.Trim() == "ASF")
                                position.ASF = true;
                            if (location.Trim() == "Gurgaon")
                                position.Gurgaon = true;
                            if (location.Trim() == "Noida")
                                position.Noida = true;
                        }
                    }
                    position.Shifts = new SelectList(getShifts().Select(s => new SelectListItem() { Value = s.Name }), "Value", "Value");
                    position.Grades = new SelectList(getGrades().Select(s => new SelectListItem() { Value = s.Name }), "Value", "Value");
                    position.Recruiters = new SelectList(getRecruiters().Select(s => new SelectListItem() { Value = s.Name }), "Value", "Value");
                    position.lobs = new SelectList(getLobs().Select(s => new SelectListItem() { Value = s.Name }), "Value", "Value");


                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Error! Please try again" + ex.Message;
                    return View(position);
                }
                return View(position);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult ApplyPosition(string workdayId)
        {
            if (Session["UserID"] != null && Convert.ToString(Session["UserType"]) == "1")
            {
                Session["WorkDayId"] = workdayId;
                Positions position = new Positions();
                try
                {
                    if (Session["WorkDayId"] != null)
                    {
                        MongoDatabase objDatabse = Connector.GetDatabase();
                        IMongoQuery query = Query.EQ("taleonumber", new BsonString(Session["WorkDayId"].ToString()));

                        Positions data = objDatabse.GetCollection<Positions>("position").Find(query).SingleOrDefault();
                        position.taleonumber = data.taleonumber;
                        position.sdate = data.sdate;
                        position.edate = data.edate;
                        position.grade = data.grade;
                        position.designation = data.designation;
                        position.currentexp = data.currentexp;
                        position.currentlocation = data.currentlocation;
                        position.lob = data.lob;
                        position.worklocation = data.worklocation;
                        position.RecruiterName = data.RecruiterName;
                        position.modeofinterview = data.modeofinterview;
                        position.Skills = data.Skills;
                        position.InterviewDetails = data.InterviewDetails;
                        position.shift = data.shift;
                        position.role = data.role;
                        Session["PositionId"] = data._id;
                    }
                    position.Shifts = new SelectList(getShifts().Select(s => new SelectListItem() { Value = s.Name }), "Value", "Value");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Error! Please try again" + ex.Message;
                    return View(position);
                }
                return View(position);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Position(Positions position)
        {
            if (Session["UserID"] != null && Convert.ToString(Session["UserType"]) == "0")
            {
            position.Shifts = new SelectList(getShifts().Select(s => new SelectListItem() { Value = s.Name }), "Value", "Value");
            position.Grades = new SelectList(getGrades().Select(s => new SelectListItem() { Value = s.Name }), "Value", "Value");
            position.Recruiters = new SelectList(getRecruiters().Select(s => new SelectListItem() { Value = s.Name }), "Value", "Value");
            position.lobs = new SelectList(getLobs().Select(s => new SelectListItem() { Value = s.Name }), "Value", "Value");
            position.worklocation = concatWorkLocation(position);
                try
                {
                if (Session["WorkDayId"] == null)
                {
                    if (Connector.GetDatabase().GetCollection<Positions>("position").Find(Query.EQ("taleonumber", position.taleonumber)).ToList().Count > 0)
                    {
                        ViewBag.Message = "Error: Workday ID already exists, please enter a unique Workday ID.";
                        return View(position);
                    }
                    else
                    {
                        MongoCollection<Positions> collection = Connector.GetDatabase().GetCollection<Positions>("position");
                        BsonDocument newPosition = new BsonDocument
                                                     {
                                                            {"taleonumber",position.taleonumber},
                                                            {"grade",position.grade},
                                                            {"designation",position.designation},
                                                            {"Diversity_Position",position.Diversity_Position},
                                                            {"lob",position.lob},
                                                            {"headcount",position.headcount},
                                                            {"worklocation",position.worklocation},
                                                            {"shift",position.shift},
                                                            {"modeofinterview",position.modeofinterview},
                                                            {"RecruiterName",position.RecruiterName},
                                                            {"role",position.role == null ? string.Empty : position.role},
                                                            {"Skills",position.Skills == null ? string.Empty : position.Skills},
                                                            {"InterviewDetails",position.InterviewDetails == null ? string.Empty : position.InterviewDetails},
                                                            {"sdate",position.sdate},
                                                            {"edate",position.edate},
                                                            {"publishedby",Session["userid"].ToString()},
                                                            {"publishedon",DateTime.Now}
                                                     };
                        collection.Insert(newPosition);
                        ModelState.Clear();
                        ViewBag.Message = "Position has been published in Employee's Dashboard";
                        position.taleonumber = position.designation = position.lob = position.worklocation = position.role = position.Skills = position.sdate = position.edate = position.InterviewDetails = position.RecruiterName =  position.modeofinterview  = position.grade = "";
                        position.shift = "Select";
                      position.Diversity_Position = position.ASF = position.Gurgaon = position.Noida = false;
                        return View(position);
                    }
                }
                else
                {
                    position.taleonumber = Session["WorkDayId"].ToString();
                    MongoDatabase objDatabse = Connector.GetDatabase();
                    IMongoQuery query = Query.EQ("taleonumber", new BsonString(Session["WorkDayId"].ToString()));

                    IMongoUpdate updateQuery = Update.Set("designation", position.designation)
                                                    .Set("grade", position.grade)
                                                    .Set("Diversity_Position", position.Diversity_Position)
                                                    .Set("lob", position.lob)
                                                    .Set("headcount", position.headcount)
                                                    .Set("worklocation", position.worklocation)
                                                    .Set("shift", position.shift)
                                                    .Set("modeofinterview", position.modeofinterview)
                                                    .Set("RecruiterName", position.RecruiterName)
                                                    .Set("role", position.role == null ? string.Empty : position.role)
                                                    .Set("InterviewDetails", position.InterviewDetails == null ? string.Empty : position.InterviewDetails)
                                                    .Set("Skills", position.Skills == null ? string.Empty : position.Skills)
                                                    .Set("sdate", position.sdate)
                                                    .Set("edate", position.edate)
                                                    .Set("publishedon", DateTime.Now);

                    objDatabse.GetCollection<ReferredList>("position").FindAndModify(query, SortBy.Null, updateQuery);

                    position.taleonumber = Session["WorkDayId"].ToString();
                    ViewBag.Message = "Position has been re-published in Employee's Dashboard";
                    return View(position);
                }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Error! Please try again" + ex.Message;
                    return View(position);
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        private string concatWorkLocation(Positions position) 
        {
            string WorkLocation = string.Empty;
            if (position.ASF)
                WorkLocation += "ASF, ";
            if (position.Gurgaon)
                WorkLocation += "Gurgaon, ";
            if (position.Noida)
                WorkLocation += "Noida, ";
            if (WorkLocation.Length > 2)
                return WorkLocation.Substring(0, WorkLocation.Length - 2);
            else
                return string.Empty;
        }
        //static async Task upateStatus()
        //{
        //    //  var result = Connector.GetDatabase().GetCollection<ReferredList>("candidate").FindOne(Query.EQ("referenceid", new BsonInt64(CandidateId)));

        //    //if (result != null)
        //    //{
        //    //    result.statusid = StatusCode;
        //    //    Connector.GetDatabase().GetCollection<ReferredList>("candidate").Update(Query.EQ("referenceid", Convert.ToString(CandidateId)), (IMongoUpdate)result);


        //    var conString = "mongodb://localhost:27017";
        //    var Client = new MongoClient(conString);
        //    var DB = new MongoDBConnector().GetDatabase();
        //    var collection = DB.GetCollection<BsonDocument>("candidate");

        //    //find the MasterID with 1130 and replace it with 1120
        //    var result = await collection.FindOneAndUpdateAsync(
        //                        Builders<BsonDocument>.Filter.Eq("MasterID", 1110),
        //                        Builders<BsonDocument>.Update.Set("MasterID", 1120)
        //                        );
        //    //retrive the data from collection
        //    await collection.Find(new BsonDocument())
        //     .ForEachAsync(x => Console.WriteLine(x));

        //}
        [HttpPost]
        public JsonResult GetDashboardData()
        {
            if (Session["UserID"] != null && Convert.ToString(Session["UserType"]) == "0")
            {
                try
                {
                    VisualInsights visualInsights = new VisualInsights();
                    List<Positions> TotalPositions = Connector.GetDatabase().GetCollection<Positions>("position").FindAll().ToList();
                    Dashboard dashboard = new Dashboard();
                    dashboard.Positions = TotalPositions;
                    dashboard.candidates = new List<ReferredList>();
                    foreach (ObjectId jobid in TotalPositions.Select(p => p._id).Distinct().ToList())
                    {
                        dashboard.candidates.AddRange(Connector.GetDatabase().GetCollection<ReferredList>("candidate").Find(Query.EQ("jobid", Convert.ToString(jobid))).ToList());
                    }
                    visualInsights.TotalPositions = new PositionData()
                    {
                        GradeLineData = new List<LineData>(),
                        GenderPieData = new List<LOBPieData>(),
                        LOBPieData = new List<LOBPieData>(),
                        StatusAreaData = new List<LineData>(),
                        HeadCount = TotalPositions.Sum(s => s.headcount) ?? 0,
                        Spoc = new List<SPOC>()
                    };

                    visualInsights.ClosedPositions = new PositionData()
                    {
                        GradeLineData = new List<LineData>(),
                        GenderPieData = new List<LOBPieData>(),
                        LOBPieData = new List<LOBPieData>(),
                        StatusAreaData = new List<LineData>(),
                        HeadCount = dashboard.candidates.Where(c => c.statusid == 10).Count(),
                        Spoc = new List<SPOC>()
                    };

                    visualInsights.ActivePositions = new PositionData()
                    {
                        GradeLineData = new List<LineData>(),
                        GenderPieData = new List<LOBPieData>(),
                        LOBPieData = new List<LOBPieData>(),
                        StatusAreaData = new List<LineData>(),
                        HeadCount = visualInsights.TotalPositions.HeadCount - visualInsights.ClosedPositions.HeadCount,
                        Spoc = new List<SPOC>()
                    };

                    visualInsights.ResumeUploaded = Connector.GetDatabase().GetCollection<ReferredList>("candidate").FindAll().ToList().Count;

                    List<string> Grades = TotalPositions.Select(s => s.grade).Distinct().ToList();
                    List<string> LOB = TotalPositions.Select(s => s.lob).Distinct().ToList();

                    List<Content> totalGrades = new List<Content>();
                    List<Content> closedGrades = new List<Content>();
                    List<Content> activeGrades = new List<Content>();

                    List<Content> totallob = new List<Content>();
                    List<Content> closedlob = new List<Content>();
                    List<Content> activelob = new List<Content>();

                    List<Content> totalgender = new List<Content>();
                    List<Content> closedgender = new List<Content>();
                    List<Content> activegender = new List<Content>();

                    List<Content> totalstatus = new List<Content>();
                    List<Content> closedstatus = new List<Content>();
                    List<Content> activestatus = new List<Content>();
                    foreach (Positions item in TotalPositions)
                    {
                        List<ReferredList> List = Connector.GetDatabase().GetCollection<ReferredList>("candidate").Find(Query.EQ("jobid", Convert.ToString(item._id))).ToList();
                        foreach (ReferredList candidate in List)
                        {
                            totalgender.Add(new Content()
                            {
                                Name = candidate.IsFemale ?? false ? "Female" : "Male",
                                data = 1
                            });
                            activegender.Add(new Content()
                            {
                                Name = candidate.IsFemale ?? false ? "Female" : "Male",
                                data = 1
                            });
                            closedgender.Add(new Content()
                            {
                                Name = candidate.IsFemale ?? false ? "Female" : "Male",
                                data = 1
                            });
                            string label = string.Empty;
                            if (candidate.statusid < 2)
                            {
                                label = "Resume Uploaded";
                                totalstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                activestatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                closedstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                            }
                            else if (candidate.statusid < 4 && candidate.statusid >= 2)
                            {
                                label = "Resume Uploaded";
                                totalstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                activestatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                closedstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                label = "HR Shortlisted";
                                totalstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                activestatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                closedstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                            }
                            else if (candidate.statusid < 9 && candidate.statusid >= 4)
                            {
                                label = "Resume Uploaded";
                                totalstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                activestatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                closedstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                label = "HR Shortlisted";
                                totalstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                activestatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                closedstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                label = "Interview Shortlisted";
                                totalstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                activestatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                closedstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                            }
                            else if (candidate.statusid == 9)
                            {
                                label = "Resume Uploaded";
                                totalstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                activestatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                closedstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                label = "HR Shortlisted";
                                totalstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                activestatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                closedstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                label = "Interview Shortlisted";
                                totalstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                activestatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                closedstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                label = "Offered";
                                totalstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                activestatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                closedstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                            }
                            else if (candidate.statusid == 10)
                            {
                                label = "Resume Uploaded";
                                totalstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                activestatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                closedstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                label = "HR Shortlisted";
                                totalstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                activestatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                closedstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                label = "Interview Shortlisted";
                                totalstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                activestatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                closedstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                label = "Offered";
                                totalstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                activestatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                closedstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                label = "Joined";
                                totalstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                activestatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                closedstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                            }
                            else if (candidate.statusid == 11)
                            {
                                label = "Resume Uploaded";
                                totalstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                activestatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                closedstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                label = "HR Shortlisted";
                                totalstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                activestatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                closedstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                label = "Interview Shortlisted";
                                totalstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                activestatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                closedstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                label = "Offered";
                                totalstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                activestatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                closedstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                label = "Not Joined";
                                totalstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                closedstatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                                activestatus.Add(new Content()
                                {
                                    Name = label,
                                    data = 1
                                });
                            }
                        }
                        Int64 total = item.headcount ?? 0;
                        Int64 closed = (from l in List where l.statusid == 10 select l).ToList().Count;
                        Int64 active = total - closed;
                        //totalgender.Add(new Content()
                        //{
                        //    Name = item.Diversity_Position ? "Female": "Male",
                        //    data = total
                        //});
                        totallob.Add(new Content()
                        {
                            Name = item.lob,
                            data = total
                        });
                        totalGrades.Add(new Content()
                        {
                            Name = item.grade,
                            data = total
                        });
                        if (closed > 0)
                        {
                            //closedgender.Add(new Content()
                            //{
                            //    Name = item.Diversity_Position ? "Female" : "Male",
                            //    data = closed
                            //});
                            closedlob.Add(new Content()
                            {
                                Name = item.lob,
                                data = closed
                            });
                            closedGrades.Add(new Content()
                            {
                                Name = item.grade,
                                data = closed
                            });
                        }
                        if (active > 0)
                        {
                            //activegender.Add(new Content()
                            //{
                            //    Name = item.Diversity_Position ? "Female" : "Male",
                            //    data = active
                            //});
                            activelob.Add(new Content()
                            {
                                Name = item.lob,
                                data = active
                            });
                            activeGrades.Add(new Content()
                            {
                                Name = item.grade,
                                data = active
                            });
                        }
                    }
                    //visualInsights.TotalPositions.StatusAreaData
                    foreach (string status in totalstatus.Select(s => s.Name).Distinct().ToList())
                    {
                        Int64 count = totalstatus.Where(s => s.Name == status).Sum(s => s.data);
                        visualInsights.TotalPositions.StatusAreaData.Add(new LineData()
                        {
                            Label = status,
                            data = count
                        });
                    }
                    foreach (string status in activestatus.Select(s => s.Name).Distinct().ToList())
                    {
                        Int64 count = activestatus.Where(s => s.Name == status).Sum(s => s.data);
                        visualInsights.ActivePositions.StatusAreaData.Add(new LineData()
                        {
                            Label = status,
                            data = count
                        });
                    }
                    foreach (string status in closedstatus.Select(s => s.Name).Distinct().ToList())
                    {
                        Int64 count = closedstatus.Where(s => s.Name == status).Sum(s => s.data);
                        visualInsights.ClosedPositions.StatusAreaData.Add(new LineData()
                        {
                            Label = status,
                            data = count
                        });
                    }
                    string[] pieColorArray = { "#f56954", "#00a65a", "#f39c12", "#00c0ef", "#3c8dbc", "#d2d6de" };
                    int colorIndex = 0;
                    foreach (string gender in totalgender.Select(G => G.Name).Distinct().ToList())
                    {
                        Int64 count = totalgender.Where(g => g.Name == gender).Sum(g => g.data);
                        visualInsights.TotalPositions.GenderPieData.Add(new LOBPieData()
                        {
                            label = gender,
                            value = count,
                            color = pieColorArray[colorIndex],
                            highlight = pieColorArray[colorIndex++],
                        });
                    }
                    colorIndex = 0;
                    foreach (string gender in closedgender.Select(G => G.Name).Distinct().ToList())
                    {
                        Int64 count = closedgender.Where(g => g.Name == gender).Sum(g => g.data);
                        visualInsights.ClosedPositions.GenderPieData.Add(new LOBPieData()
                        {
                            label = gender,
                            value = count,
                            color = pieColorArray[colorIndex],
                            highlight = pieColorArray[colorIndex++],
                        });
                    }
                    colorIndex = 0;
                    foreach (string gender in activegender.Select(G => G.Name).Distinct().ToList())
                    {
                        Int64 count = activegender.Where(g => g.Name == gender).Sum(g => g.data);
                        visualInsights.ActivePositions.GenderPieData.Add(new LOBPieData()
                        {
                            label = gender,
                            value = count,
                            color = pieColorArray[colorIndex],
                            highlight = pieColorArray[colorIndex++],
                        });
                    }
                    colorIndex = 0;
                    foreach (string lob in totallob.Select(G => G.Name).Distinct().ToList())
                    {
                        Int64 count = totallob.Where(g => g.Name == lob).Sum(g => g.data);
                        visualInsights.TotalPositions.LOBPieData.Add(new LOBPieData()
                        {
                            label = lob,
                            value = count,
                            color = pieColorArray[colorIndex],
                            highlight = pieColorArray[colorIndex++],
                        });
                    }
                    colorIndex = 0;
                    foreach (string lob in closedlob.Select(G => G.Name).Distinct().ToList())
                    {
                        Int64 count = closedlob.Where(g => g.Name == lob).Sum(g => g.data);
                        visualInsights.ClosedPositions.LOBPieData.Add(new LOBPieData()
                        {
                            label = lob,
                            value = count,
                            color = pieColorArray[colorIndex],
                            highlight = pieColorArray[colorIndex++],
                        });
                    }
                    colorIndex = 0;
                    foreach (string lob in activelob.Select(G => G.Name).Distinct().ToList())
                    {
                        Int64 count = activelob.Where(g => g.Name == lob).Sum(g => g.data);
                        visualInsights.ActivePositions.LOBPieData.Add(new LOBPieData()
                        {
                            label = lob,
                            value = count,
                            color = pieColorArray[colorIndex],
                            highlight = pieColorArray[colorIndex++],
                        });
                    }
                    foreach (string grade in totalGrades.Select(G => G.Name).Distinct().ToList())
                    {
                        Int64 count = totalGrades.Where(g => g.Name == grade).Sum(g => g.data);
                        visualInsights.TotalPositions.GradeLineData.Add(new LineData()
                        {
                            Label = grade,
                            data = count
                        });
                    }
                    foreach (string grade in closedGrades.Select(G => G.Name).Distinct().ToList())
                    {
                        Int64 count = closedGrades.Where(g => g.Name == grade).Sum(g => g.data);
                        visualInsights.ClosedPositions.GradeLineData.Add(new LineData()
                        {
                            Label = grade,
                            data = count
                        });
                    }
                    foreach (string grade in activeGrades.Select(G => G.Name).Distinct().ToList())
                    {
                        Int64 count = activeGrades.Where(g => g.Name == grade).Sum(g => g.data);
                        visualInsights.ActivePositions.GradeLineData.Add(new LineData()
                        {
                            Label = grade,
                            data = count
                        });
                    }
                    List<string> Recuriters = TotalPositions.Select(s => s.RecruiterName).Distinct().ToList();

                    foreach (string name in Recuriters)
                    {
                        totalGrades = new List<Content>();
                        closedGrades = new List<Content>();
                        activeGrades = new List<Content>();

                        totallob = new List<Content>();
                        closedlob = new List<Content>();
                        activelob = new List<Content>();

                        totalgender = new List<Content>();
                        closedgender = new List<Content>();
                        activegender = new List<Content>();

                        totalstatus = new List<Content>();
                        closedstatus = new List<Content>();
                        activestatus = new List<Content>();
                        List<ReferredList> AllCandidates = new List<ReferredList>();
                        List<ReferredList> ActiveCandidates = new List<ReferredList>();
                        List<ReferredList> ClosedCandidates = new List<ReferredList>();
                        foreach (ObjectId jobid in TotalPositions.Where(p => p.RecruiterName == name).Select(p => p._id).Distinct().ToList())
                        {
                            AllCandidates.AddRange(Connector.GetDatabase().GetCollection<ReferredList>("candidate").Find(Query.EQ("jobid", Convert.ToString(jobid))).ToList());
                        }
                        ActiveCandidates.AddRange(from r in AllCandidates where r.statusid != 10 select r);
                        ClosedCandidates.AddRange(from r in AllCandidates where r.statusid == 10 select r);

                        Int64 TotalCount = 0, ActiveCount = 0, ClosedCount = 0;
                        foreach (Positions item in TotalPositions.Where(p => p.RecruiterName == name).ToList())
                        {
                            List<ReferredList> List = Connector.GetDatabase().GetCollection<ReferredList>("candidate").Find(Query.EQ("jobid", Convert.ToString(item._id))).ToList();

                            foreach (ReferredList candidate in List)
                            {
                                totalgender.Add(new Content()
                                {
                                    Name = candidate.IsFemale ?? false ? "Female" : "Male",
                                    data = 1
                                });
                                activegender.Add(new Content()
                                {
                                    Name = candidate.IsFemale ?? false ? "Female" : "Male",
                                    data = 1
                                });
                                closedgender.Add(new Content()
                                {
                                    Name = candidate.IsFemale ?? false ? "Female" : "Male",
                                    data = 1
                                });
                                string label = string.Empty;
                                if (candidate.statusid < 2)
                                {
                                    label = "Resume Uploaded";
                                    totalstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    activestatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    closedstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                }
                                else if (candidate.statusid < 4 && candidate.statusid >= 2)
                                {
                                    label = "Resume Uploaded";
                                    totalstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    activestatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    closedstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    label = "HR Shortlisted";
                                    totalstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    activestatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    closedstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                }
                                else if (candidate.statusid < 9 && candidate.statusid >= 4)
                                {
                                    label = "Resume Uploaded";
                                    totalstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    activestatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    closedstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    label = "HR Shortlisted";
                                    totalstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    activestatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    closedstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    label = "Interview Shortlisted";
                                    totalstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    activestatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    closedstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                }
                                else if (candidate.statusid == 9)
                                {
                                    label = "Resume Uploaded";
                                    totalstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    activestatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    closedstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    label = "HR Shortlisted";
                                    totalstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    activestatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    closedstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    label = "Interview Shortlisted";
                                    totalstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    activestatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    closedstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    label = "Offered";
                                    totalstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    activestatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    closedstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                }
                                else if (candidate.statusid == 10)
                                {
                                    label = "Resume Uploaded";
                                    totalstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    activestatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    closedstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    label = "HR Shortlisted";
                                    totalstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    activestatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    closedstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    label = "Interview Shortlisted";
                                    totalstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    activestatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    closedstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    label = "Offered";
                                    totalstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    activestatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    closedstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    label = "Joined";
                                    totalstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    activestatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    closedstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                }
                                else if (candidate.statusid == 11)
                                {
                                    label = "Resume Uploaded";
                                    totalstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    activestatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    closedstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    label = "HR Shortlisted";
                                    totalstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    activestatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    closedstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    label = "Interview Shortlisted";
                                    totalstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    activestatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    closedstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    label = "Offered";
                                    totalstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    activestatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    closedstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    label = "Not Joined";
                                    totalstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    closedstatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                    activestatus.Add(new Content()
                                    {
                                        Name = label,
                                        data = 1
                                    });
                                }
                            }
                            Int64 total = item.headcount ?? 0;
                            Int64 closed = (from l in List where l.statusid == 10 select l).ToList().Count;
                            Int64 active = total - closed;
                            TotalCount += total;
                            ClosedCount += closed;
                            ActiveCount += active;
                            //totalgender.Add(new Content()
                            //{
                            //    Name = item.Diversity_Position ? "Female" : "Male",
                            //    data = total
                            //});
                            totallob.Add(new Content()
                            {
                                Name = item.lob,
                                data = total
                            });
                            totalGrades.Add(new Content()
                            {
                                Name = item.grade,
                                data = total
                            });
                            if (closed > 0)
                            {
                                //closedgender.Add(new Content()
                                //{
                                //    Name = item.Diversity_Position ? "Female" : "Male",
                                //    data = closed
                                //});
                                closedlob.Add(new Content()
                                {
                                    Name = item.lob,
                                    data = closed
                                });
                                closedGrades.Add(new Content()
                                {
                                    Name = item.grade,
                                    data = closed
                                });
                            }
                            if (active > 0)
                            {
                                //activegender.Add(new Content()
                                //{
                                //    Name = item.Diversity_Position ? "Female" : "Male",
                                //    data = active
                                //});
                                activelob.Add(new Content()
                                {
                                    Name = item.lob,
                                    data = active
                                });
                                activeGrades.Add(new Content()
                                {
                                    Name = item.grade,
                                    data = active
                                });
                            }
                        }
                        List<LineData> TotalPositionsStatus = new List<LineData>();
                        foreach (string status in totalstatus.Select(s => s.Name).Distinct().ToList())
                        {
                            Int64 count = totalstatus.Where(s => s.Name == status).Sum(s => s.data);
                            TotalPositionsStatus.Add(new LineData()
                            {
                                Label = status,
                                data = count
                            });
                        }
                        List<LineData> ActivePositionsStatus = new List<LineData>();
                        foreach (string status in activestatus.Select(s => s.Name).Distinct().ToList())
                        {
                            Int64 count = activestatus.Where(s => s.Name == status).Sum(s => s.data);
                            ActivePositionsStatus.Add(new LineData()
                            {
                                Label = status,
                                data = count
                            });
                        }
                        List<LineData> ClosedPositionsStatus = new List<LineData>();
                        foreach (string status in closedstatus.Select(s => s.Name).Distinct().ToList())
                        {
                            Int64 count = closedstatus.Where(s => s.Name == status).Sum(s => s.data);
                            ClosedPositionsStatus.Add(new LineData()
                            {
                                Label = status,
                                data = count
                            });
                        }
                        colorIndex = 0;
                        List<LOBPieData> TotalPositionsGender = new List<LOBPieData>();
                        foreach (string gender in totalgender.Select(G => G.Name).Distinct().ToList())
                        {
                            Int64 count = totalgender.Where(g => g.Name == gender).Sum(g => g.data);
                            TotalPositionsGender.Add(new LOBPieData()
                            {
                                label = gender,
                                value = count,
                                color = pieColorArray[colorIndex],
                                highlight = pieColorArray[colorIndex++],
                            });
                        }
                        colorIndex = 0;
                        List<LOBPieData> ClosedPositionsGender = new List<LOBPieData>();
                        foreach (string gender in closedgender.Select(G => G.Name).Distinct().ToList())
                        {
                            Int64 count = closedgender.Where(g => g.Name == gender).Sum(g => g.data);
                            ClosedPositionsGender.Add(new LOBPieData()
                            {
                                label = gender,
                                value = count,
                                color = pieColorArray[colorIndex],
                                highlight = pieColorArray[colorIndex++],
                            });
                        }
                        colorIndex = 0;
                        List<LOBPieData> ActivePositionsGender = new List<LOBPieData>();
                        foreach (string gender in activegender.Select(G => G.Name).Distinct().ToList())
                        {
                            Int64 count = activegender.Where(g => g.Name == gender).Sum(g => g.data);
                            ActivePositionsGender.Add(new LOBPieData()
                            {
                                label = gender,
                                value = count,
                                color = pieColorArray[colorIndex],
                                highlight = pieColorArray[colorIndex++],
                            });
                        }
                        colorIndex = 0;
                        List<LOBPieData> TotalPositionsLOB = new List<LOBPieData>();
                        foreach (string lob in totallob.Select(G => G.Name).Distinct().ToList())
                        {
                            Int64 count = totallob.Where(g => g.Name == lob).Sum(g => g.data);
                            TotalPositionsLOB.Add(new LOBPieData()
                            {
                                label = lob,
                                value = count,
                                color = pieColorArray[colorIndex],
                                highlight = pieColorArray[colorIndex++],
                            });
                        }
                        colorIndex = 0;
                        List<LOBPieData> ClosedPositionsLOB = new List<LOBPieData>();
                        foreach (string lob in closedlob.Select(G => G.Name).Distinct().ToList())
                        {
                            Int64 count = closedlob.Where(g => g.Name == lob).Sum(g => g.data);
                            ClosedPositionsLOB.Add(new LOBPieData()
                            {
                                label = lob,
                                value = count,
                                color = pieColorArray[colorIndex],
                                highlight = pieColorArray[colorIndex++],
                            });
                        }
                        colorIndex = 0;
                        List<LOBPieData> ActivePositionsLOB = new List<LOBPieData>();
                        foreach (string lob in activelob.Select(G => G.Name).Distinct().ToList())
                        {
                            Int64 count = activelob.Where(g => g.Name == lob).Sum(g => g.data);
                            ActivePositionsLOB.Add(new LOBPieData()
                            {
                                label = lob,
                                value = count,
                                color = pieColorArray[colorIndex],
                                highlight = pieColorArray[colorIndex++],
                            });
                        }
                        List<LineData> TotalPositionsGrades = new List<LineData>();
                        foreach (string grade in totalGrades.Select(G => G.Name).Distinct().ToList())
                        {
                            Int64 count = totalGrades.Where(g => g.Name == grade).Sum(g => g.data);
                            TotalPositionsGrades.Add(new LineData()
                            {
                                Label = grade,
                                data = count
                            });
                        }
                        List<LineData> ClosedPositionsGrades = new List<LineData>();
                        foreach (string grade in closedGrades.Select(G => G.Name).Distinct().ToList())
                        {
                            Int64 count = closedGrades.Where(g => g.Name == grade).Sum(g => g.data);
                            ClosedPositionsGrades.Add(new LineData()
                            {
                                Label = grade,
                                data = count
                            });
                        }
                        List<LineData> ActivePositionsGrades = new List<LineData>();
                        foreach (string grade in activeGrades.Select(G => G.Name).Distinct().ToList())
                        {
                            Int64 count = activeGrades.Where(g => g.Name == grade).Sum(g => g.data);
                            ActivePositionsGrades.Add(new LineData()
                            {
                                Label = grade,
                                data = count
                            });
                        }

                        visualInsights.ActivePositions.Spoc.Add(new SPOC()
                        {
                            Name = name,
                            HeadCount = ActiveCount,// ActiveCandidates.Count,
                            GenderPieData = ActivePositionsGender,
                            GradeLineData = ActivePositionsGrades,
                            LOBPieData = ActivePositionsLOB,
                            StatusAreaData = ActivePositionsStatus
                        });
                        visualInsights.ClosedPositions.Spoc.Add(new SPOC()
                        {
                            Name = name,
                            HeadCount = ClosedCount, // ClosedCandidates.Count,
                            GenderPieData = ClosedPositionsGender,
                            GradeLineData = ClosedPositionsGrades,
                            LOBPieData = ClosedPositionsLOB,
                            StatusAreaData = ClosedPositionsStatus
                        });

                        visualInsights.TotalPositions.Spoc.Add(new SPOC()
                        {
                            Name = name,
                            HeadCount = TotalCount, //AllCandidates.Count,
                            GenderPieData = TotalPositionsGender,
                            GradeLineData = TotalPositionsGrades,
                            LOBPieData = TotalPositionsLOB,
                            StatusAreaData = TotalPositionsStatus
                        });
                    }
                    return Json(visualInsights, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json("error", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("Unauthorized User", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult AddCandidateStatus(Int64 CandidateId, Int64 StatusId, string Feedback)
        {
            if (Session["UserID"] != null && Convert.ToString(Session["UserType"]) == "0")
            {
            try
            {
                MongoDatabase objDatabse = Connector.GetDatabase();
                IMongoQuery query = Query.And(
                         Query.EQ("CandidateId", new BsonInt64(CandidateId))
                      );
                List<CandidateFeedback> collection = objDatabse.GetCollection<CandidateFeedback>("feedback").Find(query).ToList();

                ReferredList result = Connector.GetDatabase().GetCollection<ReferredList>("candidate").FindOne(Query.EQ("referenceid", CandidateId));
                //result.jobid 
                List<ReferredList> RefferedList = Connector.GetDatabase().GetCollection<ReferredList>("candidate").Find(Query.EQ("jobid", Convert.ToString(result.jobid))).ToList();

                Int64? Offered = 0;
                foreach (ReferredList candidate in RefferedList)
                {
                    if (candidate.statusid == 9 || candidate.statusid == 10)
                    {
                        Offered += 1;
                    }
                }
                var job = Connector.GetDatabase().GetCollection<Positions>("position").FindOne(Query.EQ("_id", ObjectId.Parse(result.jobid)));
                if(result.statusid == 9 && StatusId == 10)
                {
                     objDatabse = Connector.GetDatabase();
                    query = Query.EQ("referenceid", new BsonInt64(CandidateId));
                    IMongoUpdate updateQuery = Update.Set("statusid", StatusId);
                    objDatabse.GetCollection<ReferredList>("candidate").FindAndModify(query, SortBy.Null, updateQuery);

                    MongoCollection<CandidateFeedback> Candidatecollection = Connector.GetDatabase().GetCollection<CandidateFeedback>("feedback");
                    BsonDocument newFeedback = new BsonDocument
                                                     {
                                                            {"CandidateId",CandidateId},
                                                            {"StatusId",StatusId},
                                                            {"Feedback",Feedback},
                                                            {"SavedBy",Session["userid"].ToString()},
                                                            {"HrName",Session["name"].ToString()},
                                                            {"SavedOn",DateTime.Now},
                                                            {"DateString",DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt")}
                                                     };
                    Candidatecollection.Insert(newFeedback);
                    return Json("success", JsonRequestBehavior.AllowGet); 
                }
                else if (StatusId == 9 || StatusId == 10)
                {
                    if (Offered < job.headcount)
                    {
                        objDatabse = Connector.GetDatabase();
                        query = Query.EQ("referenceid", new BsonInt64(CandidateId));
                        IMongoUpdate updateQuery = Update.Set("statusid", StatusId);
                        objDatabse.GetCollection<ReferredList>("candidate").FindAndModify(query, SortBy.Null, updateQuery);

                        MongoCollection<CandidateFeedback> Candidatecollection = Connector.GetDatabase().GetCollection<CandidateFeedback>("feedback");
                        BsonDocument newFeedback = new BsonDocument
                                                     {
                                                            {"CandidateId",CandidateId},
                                                            {"StatusId",StatusId},
                                                            {"Feedback",Feedback},
                                                            {"SavedBy",Session["userid"].ToString()},
                                                            {"HrName",Session["name"].ToString()},
                                                            {"SavedOn",DateTime.Now},
                                                            {"DateString",DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt")}
                                                     };
                        Candidatecollection.Insert(newFeedback);
                        return Json("success", JsonRequestBehavior.AllowGet); 
                    }
                    else
                    {
                        return Json("limit", JsonRequestBehavior.AllowGet);
                    }
                }
                else 
                {
                    objDatabse = Connector.GetDatabase();
                    query = Query.EQ("referenceid", new BsonInt64(CandidateId));
                    IMongoUpdate updateQuery = Update.Set("statusid", StatusId);
                    objDatabse.GetCollection<ReferredList>("candidate").FindAndModify(query, SortBy.Null, updateQuery);

                    MongoCollection<CandidateFeedback> Candidatecollection = Connector.GetDatabase().GetCollection<CandidateFeedback>("feedback");
                    BsonDocument newFeedback = new BsonDocument
                                                     {
                                                            {"CandidateId",CandidateId},
                                                            {"StatusId",StatusId},
                                                            {"Feedback",Feedback},
                                                            {"SavedBy",Session["userid"].ToString()},
                                                            {"HrName",Session["name"].ToString()},
                                                            {"SavedOn",DateTime.Now},
                                                            {"DateString",DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt")}
                                                     };
                    Candidatecollection.Insert(newFeedback);
                    return Json("success", JsonRequestBehavior.AllowGet); 
                }
            }
            catch (Exception ex)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            }
            else
            {
                 return Json("Unauthorized User", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult GetCandidateStatus(Int64 CandidateId)
        {
            if (Session["UserID"] != null && Convert.ToString(Session["UserType"]) == "0")
            {
                try
                {
                    MongoDatabase objDatabse = Connector.GetDatabase();
                    IMongoQuery query = Query.And(
                             Query.EQ("CandidateId", new BsonInt64(CandidateId))
                              //Query.EQ("SavedBy", new BsonString(Session["userid"].ToString()))
                          );
                    List<CandidateFeedback> collection = objDatabse.GetCollection<CandidateFeedback>("feedback").Find(query).ToList();

                    var result = Connector.GetDatabase().GetCollection<ReferredList>("candidate").FindOne(Query.EQ("referenceid", CandidateId));

                    string referenceId = result.referedby;
                    var referenceObj = Connector.GetDatabase().GetCollection<UserProfile>("UserProfile").FindOne(Query.EQ("_id", ObjectId.Parse(referenceId)));

                    CandidateLead lead = new CandidateLead()
                    {
                        employeeid = referenceObj.employeeid,
                        name = referenceObj.name,
                        lob = referenceObj.lob
                    };
                    query = Query.And(
                              Query.EQ("mobile", new BsonString(result.mobile)),
                              Query.EQ("emailid", new BsonString(result.emailid.ToLower()))
                          );
                    List<ReferredList> refferedCollection = objDatabse.GetCollection<ReferredList>("candidate").Find(query).ToList();

                    List<InspectCandidate> CandidatureList = new List<InspectCandidate>();
                    foreach (ReferredList item in refferedCollection)
                    {
                        var job = Connector.GetDatabase().GetCollection<Positions>("position").FindOne(Query.EQ("_id", ObjectId.Parse(item.jobid)));
                        CandidatureList.Add(new InspectCandidate()
                        {
                            PositionTitle = job.designation,
                            StatusId = item.statusid,
                            WorkdayId = job.taleonumber
                        });
                    }

                    CandidateData data = new CandidateData()
                    {
                        Feedbacks = collection.OrderByDescending(m => m.SavedOn).ToList(),
                        Candidature = CandidatureList,
                        Reference = lead

                    };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json("error", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("Unauthorized User", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetCandidateInfo(Int64 CandidateId)
        {
            if (Session["UserID"] != null && Convert.ToString(Session["UserType"]) == "0")
            {
                var result = Connector.GetDatabase().GetCollection<ReferredList>("candidate").FindOne(Query.EQ("referenceid", CandidateId));
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Unauthorized User", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult UpdateCandidateStatus(Int64 CandidateId, int StatusCode)
        {
            if (Session["UserID"] != null && Convert.ToString(Session["UserType"]) == "0")
            {
                MongoDatabase objDatabse = Connector.GetDatabase();
                IMongoQuery query = Query.EQ("referenceid", new BsonInt64(CandidateId));

                IMongoUpdate updateQuery = Update.Set("statusid", StatusCode);
                //ReferredList user = objDatabse.GetCollection<ReferredList>("candidate").Find(query).SingleOrDefault();
                //objDatabse.GetCollection("candidate").Update(query, updateQuery);


                objDatabse.GetCollection<ReferredList>("candidate").FindAndModify(query, SortBy.Null, updateQuery);


                string statusText = "Status updated for Ref.No.: " + CandidateId;
                return Json(statusText, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Unauthorized User", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult CandidateAction(string ReferenceId)
        {
            if (Session["UserID"] != null && Convert.ToString(Session["UserType"]) == "0")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult Applications(string PositionId)
        {
            if (Session["UserID"] != null && Convert.ToString(Session["UserType"]) == "0")
            {
                try
                {
                    UserProfile profile = new UserProfile();
                    var result = Connector.GetDatabase().GetCollection<UserProfile>("UserProfile").FindOne(Query.EQ("_id", ObjectId.Parse(Session["UserID"].ToString())));
                    profile.isactive = result.isactive;
                    profile.name = result.name;
                    profile._id = result._id;
                    profile.photo = result.photo;
                    profile.lob = result.lob;
                    profile.ReferedCandidatesList = Connector.GetDatabase().GetCollection<ReferredList>("candidate").Find(Query.EQ("jobid", Convert.ToString(PositionId))).ToList();

                    var job = Connector.GetDatabase().GetCollection<Positions>("position").FindOne(Query.EQ("_id", ObjectId.Parse(PositionId)));
                    profile.taleonumber = job.taleonumber;
                    return View(profile);
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public FileResult DownloadCV(string CurrentFileName, string CandidateName)
        {
            string contentType = string.Empty;

            if (CurrentFileName.Contains(".pdf"))
            {
                contentType = "application/pdf";
                CandidateName += ".pdf";
            }
            else if (CurrentFileName.Contains(".docx"))
            {
                contentType = "application/docx";
                CandidateName += ".docx";
            }
            else if (CurrentFileName.Contains(".doc"))
            {
                contentType = "application/doc";
                CandidateName += ".doc";
            }
            return File(string.Format(@"~\Attachments\{0}", CurrentFileName), contentType, CandidateName);   
        }
        public ActionResult Dashboard()
        {
            if (Session["UserID"] != null && Convert.ToString(Session["UserType"]) == "1")
            {
                try
                {
                    UserProfile profile = new UserProfile();
                    var result = Connector.GetDatabase().GetCollection<UserProfile>("UserProfile").FindOne(Query.EQ("_id", ObjectId.Parse(Session["UserID"].ToString())));
                    profile.isactive = result.isactive;
                    profile.name = result.name;
                    profile._id = result._id;
                    profile.photo = result.photo;
                    profile.lob = result.lob;
                    profile.employeeid = result.employeeid;
                    profile.list = Connector.GetDatabase().GetCollection<Positions>("position").FindAll().ToList();
                    profile.list = (from data in profile.list where DateTime.ParseExact(data.edate, "dd/MM/yyyy", null) >= DateTime.Today select data).ToList();
                    
                    return View(profile);
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        private List<Recruiter> getRecruiters()
        {
            string[] recruiters = { "Rajiv Rao","Sneha Arora","Divya Arora","Maheep Sawhney","Monika Miglani","Paridhi Dhoundiyal","Nidhi Kataria"};
            Array.Sort(recruiters);
            List<Recruiter> RecruiterList = new List<Recruiter>();
            for (int i = 0; i < recruiters.Length; i++)
            {
                RecruiterList.Add(
                 new Recruiter()
                 {
                     id = i,
                     Name = recruiters[i]
                 });
            }
            return RecruiterList;

        }
        private List<Grade> getGrades()
        {
            string[] grades = { "B1", "B2", "C1", "C2", "D1", "D2", "E", "F", "G", "H" };
            Array.Sort(grades);
            List<Grade> GradeList = new List<Grade>();
            for (int i = 0; i < grades.Length; i++)
            {
                GradeList.Add(
                 new Grade()
                 {
                     id = i,
                     Name = grades[i]
                 });
            }
            return GradeList;
        }
        private List<Lob> getLobs()
        {
            string[] lobs = { "Health", "RPA", "Talent", "Finance", "GTI", "MMC Functions", "	F&A", "US H&B", "WAS" };
            Array.Sort(lobs);
            List<Lob> LobList = new List<Lob>();
            for (int i = 0; i < lobs.Length; i++)
            {
                LobList.Add(
                 new Lob()
                 {
                     id = i,
                     Name = lobs[i]
                 });
            }
            return LobList;
        }
        private List<Shift> getShifts()
        {
            return new List<Shift>(){
                        new Shift(){
                             Name ="08:00 AM to 05:00 PM"
                        },
                        new Shift(){
                             Name ="11:30 AM to 08:30 PM"
                        },
                        new Shift(){
                             Name ="01:00 PM to 10:00 PM"
                        }
                    };
        }

    }
}
