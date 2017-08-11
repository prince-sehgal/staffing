using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StaffingSite.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Configuration;   

namespace StaffingSite.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        ObjectId id = new ObjectId();

        MongoClient client = null;
        MongoServer server = null;
        MongoDatabase database = null;
        MongoCollection UserDetailscollection = null;

        string connectionString = "mongodb://localhost";
        private List<UserProfile> _UserList = new List<UserProfile>();

        public ActionResult Login()
        {
            return View();
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
                    client = new MongoClient(connectionString);
                    server = client.GetServer();
                    database = server.GetDatabase("staffing");
                    var result = database.GetCollection<UserProfile>("UserProfile").FindOne(Query.EQ("username", objUser.username.ToLower()));
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
        public ActionResult Hr()
        {
            if (Session["UserID"] != null && Convert.ToString(Session["UserType"]) == "0")
            {
                try
                {
                    UserProfile profile = new UserProfile();
                    client = new MongoClient(connectionString);
                    server = client.GetServer();
                    database = server.GetDatabase("staffing");
                    var result = database.GetCollection<UserProfile>("UserProfile").FindOne(Query.EQ("_id", ObjectId.Parse(Session["UserID"].ToString())));
                    profile.isactive = result.isactive;
                    profile.name = result.name;
                    profile._id = result._id;
                    profile.photo = result.photo;
                    profile.lob = result.lob;
                    profile.employeeid = result.employeeid;
                    profile.list = database.GetCollection<Positions>("position").FindAll().ToList();
                    foreach (Positions position in profile.list)
                    {
                        position.headcount = database.GetCollection<ReferredList>("candidate").Find(Query.EQ("jobid", Convert.ToString(position._id))).ToList().Count;
                    }
                    List<SelectListItem> items = new List<SelectListItem>();
                    items.Add(new SelectListItem { Text = "In Process", Value = "0" });
                    items.Add(new SelectListItem { Text = "Hr Discussion", Value = "1" });
                    items.Add(new SelectListItem { Text = "Short Listed", Value = "2" });
                    items.Add(new SelectListItem { Text = "Operation Round", Value = "3" });
                    items.Add(new SelectListItem { Text = "Rejected", Value = "3" });
                    items.Add(new SelectListItem { Text = "To be Offered", Value = "3" });
                    items.Add(new SelectListItem { Text = "Offered", Value = "3" });
                    items.Add(new SelectListItem { Text = "Join", Value = "3" });

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
        public ActionResult Position()
        {
            Positions position = new Positions();
            try
            {
                position.Shifts = new SelectList(getShifts().Select(s => new SelectListItem() { Value = s.Name }),"Value","Value");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error! Please try again" + ex.Message;
                return View(position);
            }
            return View(position);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Position(Positions position)
        {
            position.Shifts = new SelectList(getShifts().Select(s => new SelectListItem() { Value = s.Name }), "Value", "Value");
                try
                {
                    client = new MongoClient(connectionString);
                    server = client.GetServer();
                    database = server.GetDatabase("staffing");
                    if (database.GetCollection<Positions>("position").Find(Query.EQ("taleonumber", position.taleonumber)).ToList().Count > 0)
                    {
                        ViewBag.Message = "Error: Taleo Number already exists, please enter a unique Taleo Number.";
                        return View(position);
                    }
                    else
                    {
                        MongoCollection<Positions> collection = database.GetCollection<Positions>("position");
                        BsonDocument newPosition = new BsonDocument
                                                 {  
                                                        {"taleonumber",position.taleonumber},  
                                                        {"designation",position.designation},  
                                                        {"lob",position.lob},  
                                                        {"worklocation",position.worklocation},  
                                                        {"shift",position.shift},  
                                                        {"role",position.role},  
                                                        {"Skills",position.Skills},  
                                                        {"sdate",position.sdate},  
                                                        {"edate",position.edate},
                                                        {"publishedby",Session["userid"].ToString()},  
                                                        {"publishedon",DateTime.Now}
                                                 };
                        collection.Insert(newPosition);
                        ModelState.Clear();
                        ViewBag.Message = "Position has been published in Employee's Dashboard";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Error! Please try again" + ex.Message;
                    return View(position);
                }
            return View(position);
        }
        public ActionResult Applications(string PositionId)
        {
            if (Session["UserID"] != null)
            {
                try
                {
                    UserProfile profile = new UserProfile();
                    client = new MongoClient(connectionString);
                    server = client.GetServer();
                    database = server.GetDatabase("staffing");
                    var result = database.GetCollection<UserProfile>("UserProfile").FindOne(Query.EQ("_id", ObjectId.Parse(Session["UserID"].ToString())));
                    profile.isactive = result.isactive;
                    profile.name = result.name;
                    profile._id = result._id;
                    profile.photo = result.photo;
                    profile.lob = result.lob;
                    profile.ReferedCandidatesList = database.GetCollection<ReferredList>("candidate").Find(Query.EQ("jobid", Convert.ToString(PositionId))).ToList();
                    var job = database.GetCollection<Positions>("position").FindOne(Query.EQ("_id", ObjectId.Parse(PositionId)));
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
        public FileResult DownloadCV(string CurrentFileName)
        {
            string contentType = string.Empty;

            if (CurrentFileName.Contains(".pdf"))
            {
                contentType = "application/pdf";
            }

            else if (CurrentFileName.Contains(".docx"))
            {
                contentType = "application/docx";
            }
            return File(string.Format(@"~\Attachments\{0}", CurrentFileName), contentType, CurrentFileName);   
        }
        public ActionResult Dashboard()
        {
            if (Session["UserID"] != null && Convert.ToString(Session["UserType"]) == "1")
            {
                try
                {
                    UserProfile profile = new UserProfile();
                    client = new MongoClient(connectionString);
                    server = client.GetServer();
                    database = server.GetDatabase("staffing");
                    var result = database.GetCollection<UserProfile>("UserProfile").FindOne(Query.EQ("_id", ObjectId.Parse(Session["UserID"].ToString())));
                    profile.isactive = result.isactive;
                    profile.name = result.name;
                    profile._id = result._id;
                    profile.photo = result.photo;
                    profile.lob = result.lob;
                    profile.employeeid = result.employeeid;
                    profile.list = database.GetCollection<Positions>("position").FindAll().ToList();
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
