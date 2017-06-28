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
namespace StaffingSite.Controllers
{
    public class EmployeeController : Controller
    {
        ObjectId id = new ObjectId();

        MongoClient client = null;
        MongoServer server = null;
        MongoDatabase database = null;
        MongoCollection UserDetailscollection = null;

        string connectionString = "mongodb://localhost";

        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                client = new MongoClient(connectionString);
                server = client.GetServer();
                database = server.GetDatabase("staffing");
                Positions position = new Positions();
                position.positions = new SelectList(database.GetCollection<Positions>("position").FindAll().ToList(), "_id", "title");
                return View(position);
                //foreach (Positions item in result)
                //{
                
                //}
                //return View(objPositions);
            }
            else
            {
                return RedirectToAction("Login","Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Positions position, HttpPostedFileBase file)
        {
            if (file == null)
            {
                ModelState.AddModelError("CustomError", "Please attach CV");
                client = new MongoClient(connectionString);
                server = client.GetServer();
                database = server.GetDatabase("staffing");
                position.positions = new SelectList(database.GetCollection<Positions>("position").FindAll().ToList(), "_id", "title");
                return View(position);
            }

            if (!(file.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document" ||
                file.ContentType == "application/pdf"))
            {
                ModelState.AddModelError("CustomError", "Only .docx and .pdf file allowed");
                client = new MongoClient(connectionString);
                server = client.GetServer();
                database = server.GetDatabase("staffing");
                position.positions = new SelectList(database.GetCollection<Positions>("position").FindAll().ToList(), "_id", "title");
                return View(position);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    candidate obj = new candidate();
                    string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    file.SaveAs(Path.Combine(Server.MapPath("~/Attachments"), fileName));
                    client = new MongoClient(connectionString);
                    server = client.GetServer();
                    database = server.GetDatabase("staffing");
                    MongoCollection<candidate> collection = database.GetCollection<candidate>("candidate");

                    BsonDocument candidate = new BsonDocument
         {  
               {"name",position.name},  
               {"jobid",position.jobid},  
               {"referedby",Session["userid"].ToString()},  
               {"mobile",position.mobile},  
               {"emailid", position.emailid},
               {"attachment", fileName},
               {"referedon",DateTime.Now}
         };

                    collection.Insert(candidate);  
                    ViewBag.Message = "Successfully Done";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Error! Please try again";
                    position.positions = new SelectList(database.GetCollection<Positions>("position").FindAll().ToList(), "_id", "title");
                    return View(position);
                }
            }
            position.positions = new SelectList(database.GetCollection<Positions>("position").FindAll().ToList(), "_id", "title");
            return View(position);
            //if (ModelState.IsValid)
            //{
            //    if (attachment != null)
            //    {
            //        //position.ImageMimeType = attachment.ContentType;
            //        //position.ImageData = new byte[attachment.ContentLength];
            //        //attachment.InputStream.Read(position.ImageData, 0, attachment.ContentLength);
            //    }
            //    //productsRepository.SaveProduct(product);
            //    //TempData["message"] = product.Name + " has been saved.";
            //    return RedirectToAction("Index");
            //}
            //else // Validation error, so redisplay same view
            //    return RedirectToAction("Index");
        }

    }
}
