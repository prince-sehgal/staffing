using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StaffingSite.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;   

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
                    var result = database.GetCollection<UserProfile>("UserProfile").FindOne(Query.EQ("username", objUser.username));
                    if (result != null && result.password == objUser.password)
                    {
                        objUser.isactive = result.isactive;
                        objUser.name = result.name;
                        objUser._id = result._id;
                        objUser.photo = result.photo;
                        objUser.lob = result.lob;
                        Session["UserID"] = objUser._id;
                    }
                    else
                    {
                        ViewBag.Message = "Invalid credentials, please verify.";
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }     
            }
            return RedirectToAction("Dashboard");
        }
        public ActionResult Dashboard()
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
                    profile.list = database.GetCollection<Positions>("position").FindAll().ToList();
                    return View(profile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return View();
                }     
            }
            else
            {
                return RedirectToAction("Login");
            }
        }  

    }
}
