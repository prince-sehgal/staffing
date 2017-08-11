using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StaffingSite.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.IO;
using System.Configuration;
namespace StaffingSite.Controllers
{
    public class EmployeeController : Controller
    {
        ObjectId id = new ObjectId();

        MongoClient client = null;
        MongoServer server = null;
        MongoDatabase database = null;
        MongoCollection UserDetailscollection = null;
        //static Positions DefaultPositionObject = null;
        string globalJobId = null;
        string connectionString = "mongodb://localhost";

        public ActionResult Index(string JobId)
        {
            Session["UserID"] = "59845c13a3a77448a866ecdf";
            if (Session["UserID"] != null)
            {
                Session["globalJobId"] = JobId;
                Positions position = new Positions();
                client = new MongoClient(connectionString);
                server = client.GetServer();
                database = server.GetDatabase("staffing");
                position.jobid = Convert.ToString(Session["globalJobId"]);
                position.positions = new SelectList(database.GetCollection<Positions>("position").FindAll().ToList(), "_id", "title");
                Session["model"] = position;

                return View(Session["model"]);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult History()
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
                    profile.ReferedCandidatesList = database.GetCollection<ReferredList>("candidate").Find(Query.EQ("referedby", Session["UserID"].ToString())).ToList();
                    foreach (ReferredList candidate in profile.ReferedCandidatesList)
	{
                        var job = database.GetCollection<Positions>("position").FindOne(Query.EQ("_id", ObjectId.Parse(candidate.jobid)));
                        candidate.designation = job.designation;
                        candidate.taleonumber = job.taleonumber;
	}
                    return View(profile);
                }
                catch (Exception ex)
                {
                    ViewBag.Message=ex.Message;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string JobId, Positions position, HttpPostedFileBase cvfile)
        {
            Console.WriteLine(Request.QueryString["JobId"]);
            if (cvfile == null)
            {
                ModelState.AddModelError("CustomError", "Please attach CV");
                return View(Session["model"]);
            }

            if (!(cvfile.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document" ||
                cvfile.ContentType == "application/pdf"))
            {
                ModelState.AddModelError("CustomError", "Only .docx and .pdf file allowed");
                return View(Session["model"]);
            }
            try
            {
                candidate obj = new candidate();
                string fileName = Guid.NewGuid() + Path.GetExtension(cvfile.FileName);
                cvfile.SaveAs(Path.Combine(Server.MapPath("~/Attachments"), fileName));
                client = new MongoClient(connectionString);
                server = client.GetServer();
                database = server.GetDatabase("staffing");
                long ReferenceId = database.GetCollection<candidate>("candidate").Count() + long.Parse(ConfigurationManager.AppSettings["InitialCount"]) + 1;
                MongoCollection<candidate> collection = database.GetCollection<candidate>("candidate");
                BsonDocument candidate = new BsonDocument
                                                 {  
                                                        {"jobid",position.jobid},  
                                                        {"firstname",position.firstname},  
                                                        {"midname",position.midname ?? ""},  
                                                        {"lastname",position.lastname},  
                                                        {"dob",position.dob},  
                                                        {"primaryskill",position.primaryskill},  
                                                        {"secondaryskill",position.secondaryskill},  
                                                        {"totalexp",position.totalexp},  
                                                        {"currentexp",position.currentexp},  
                                                        {"currentlocation",position.currentlocation},
                                                        {"preferredlocation",position.preferredlocation},
                                                        {"referedby",Session["userid"].ToString()},  
                                                        //{"lob",position.lob},
                                                        //{"sublob",position.sublob},
                                                         {"lob",0},
                                                        {"sublob",0},
                                                        //{"taleonumber",position.taleonumber},
                                                        {"mobile",position.mobile},  
                                                        {"emailid", position.emailid},
                                                        {"attachment", fileName},
                                                        {"referedon",DateTime.Now},
                                                        {"referenceid",ReferenceId}
                                                 };
                collection.Insert(candidate);
                ModelState.Clear();
                ViewBag.Message = "Candidate Details has been saved with reference id - " + ReferenceId + ".";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error! Please try again" + ex.Message;
                return View(Session["model"]);
            }
            return View(Session["model"]);
        }
    }
}
