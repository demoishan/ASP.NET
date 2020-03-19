using DemoIshan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoIshan.Controllers
{
    public class HomeController : Controller
    {
        DemoDBEntities db = new DemoDBEntities();
        public ActionResult Index()
        {
            Student stu = new Student();
            return View(stu);
        }

        public ActionResult Index2()
        {
            StudentVM stu = new StudentVM();
            return View(stu);
        }

        public ActionResult GetDetails()
        {
            List<Student> stu = new List<Student>();
            stu = db.Students.ToList();
            return PartialView("~/views/Home/_GetDetails.cshtml", stu);
        }

        [HttpPost]
        public ActionResult Index(Student stu1)
        {
            if (ModelState.IsValid)
            {
                db.Students.Add(stu1);
                db.SaveChanges();
                List<Student> stu = new List<Student>();
                stu = db.Students.ToList();
                //return PartialView("~/views/Home/Index.cshtml", stu);
                //return View(stu);
                //return RedirectToAction("index", "home");
                return Json(stu, JsonRequestBehavior.AllowGet);
            }
            return View(stu1);
        }


    }
}