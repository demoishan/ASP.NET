using JalaramTravels.Filters;
using JalaramTravels.Models;
using JalaramTravels.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JalaramTravels.Controllers
{
    [CheckSessionTimeOut]
    public class UserController : Controller
    {
        JalaramDBEntities db = new JalaramDBEntities();

        public async Task<ActionResult> Index()
        {
            var loginList = await Task.Run(() => db.Logins.ToList());
            var roleList = await Task.Run(() => db.Roles.ToList());
            List<LoginVM> loginVMList = new List<LoginVM>();

            foreach (var item in loginList)
            {
                LoginVM LoginVM = new LoginVM();
                LoginVM.LoginID = item.LoginID;
                LoginVM.FirstName = item.FirstName;
                LoginVM.LastName = item.LastName;
                LoginVM.RoleID = (int)item.RoleID;
                if (LoginVM.RoleID != 0)
                {
                    var data = roleList.Where(t => t.RoleID == LoginVM.RoleID).FirstOrDefault();
                    if (data != null)
                    {
                        LoginVM.RoleName = data.RoleName;
                    }
                }
                LoginVM.Email = item.Email;
                LoginVM.Password = item.Password;
                LoginVM.Flag = item.Flag;
                loginVMList.Add(LoginVM);
            }

            return View(loginVMList);
        }

        public async Task<ActionResult> Create(int Id = 0)
        {
            LoginVM loginVM = new LoginVM();
            var roleList = await Task.Run(() => db.Roles.ToList());


            loginVM.RoleList = roleList.Select(s => new SelectListItem() { Text = s.RoleName, Value = s.RoleID.ToString() });

            if (Id == 0)
            {
            }
            else
            {
                Login login = new Login();
                login = await Task.Run(() => db.Logins.Find(Id));

                if (login != null)
                {
                    loginVM.LoginID = login.LoginID;
                    loginVM.FirstName = login.FirstName;
                    loginVM.LoginID = login.LoginID;
                    loginVM.FirstName = login.FirstName;
                    loginVM.LastName = login.LastName;
                    loginVM.RoleID = (int)login.RoleID;
                    loginVM.Email = login.Email;
                    loginVM.Password = login.Password;
                    loginVM.Flag = login.Flag;
                }
            }
            return View(loginVM);
        }

        [HttpPost]
        public async Task<JsonResult> Create(LoginVM loginVM)
        {
            var loginResult = new LoginResult();
            loginResult.IsError = false;
            loginResult.ErrorMessage = "";

            Login LoginObj = new Login();
            var loginList = await Task.Run(() => db.Logins.ToList());
            bool Emailexists = false;

            if (loginVM.LoginID == 0)
            {
                var dagtaExists = loginList.Where(t => t.Email == loginVM.Email).FirstOrDefault();
                if (dagtaExists != null)
                {
                    Emailexists = true;
                    loginResult.IsError = true;
                    loginResult.ErrorMessage = "User Name already exits";

                }
                if (!Emailexists)
                { 
                    LoginObj.LoginID = loginVM.LoginID;
                    LoginObj.FirstName = loginVM.FirstName;
                    LoginObj.LoginID = loginVM.LoginID;
                    LoginObj.FirstName = loginVM.FirstName;
                    LoginObj.LastName = loginVM.LastName;
                    LoginObj.RoleID = (int)loginVM.RoleID;
                    LoginObj.Email = loginVM.Email;
                    LoginObj.Password = loginVM.Password;
                    LoginObj.CreateDate = GetCurrentSession.CurrentDateTime();
                    LoginObj.CreateUser = (int)GetCurrentSession.CurrentUser();
                    LoginObj.Flag = "A";
                    await Task.Run(() => db.Logins.Add(LoginObj));
                    await Task.Run(() => db.SaveChanges());
                }

            }
            else
            {
                LoginObj = await Task.Run(() => db.Logins.Where(t => t.LoginID == loginVM.LoginID).FirstOrDefault());
              
                if (LoginObj != null)
                {
                    LoginObj.FirstName = loginVM.FirstName;
                    LoginObj.LastName = loginVM.LastName;
                    LoginObj.RoleID = (int)loginVM.RoleID;
                    LoginObj.Flag = "A";
                    LoginObj.UpdateDate = GetCurrentSession.CurrentDateTime();
                    LoginObj.UpdateUser = (int)GetCurrentSession.CurrentUser();
                    await Task.Run(() => db.Entry(LoginObj).State = System.Data.Entity.EntityState.Modified);
                    await Task.Run(() => db.SaveChanges());
                }


            }

            return Json(loginResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int Id)
        {
            Login Obj = await Task.Run(() => db.Logins.Find(Id));

            if (Obj != null)
            {
                if (Obj.Flag == "A")
                {
                    Obj.Flag = "D";
                }
                else
                {
                    Obj.Flag = "A";
                }
                Obj.UpdateDate = GetCurrentSession.CurrentDateTime();
                Obj.UpdateUser = (int)GetCurrentSession.CurrentUser();
                await Task.Run(() => db.Entry(Obj).State = System.Data.Entity.EntityState.Modified);
                await Task.Run(() => db.SaveChanges());
                //await Task.Run(() => db.Logines.Remove(Obj));
                //await Task.Run(() => db.SaveChanges());
            }
            return Json(Obj, JsonRequestBehavior.AllowGet);
        }
    }
}