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
    public class PickUpBoyController : Controller
    {
        JalaramDBEntities db = new JalaramDBEntities();
        public async Task<ActionResult> Index()
        {
            var pickUpList = await Task.Run(() => db.PickUpBoys.ToList()) ;
            List<PickUpBoyVM> pickUpVMList = new List<PickUpBoyVM>();

            foreach (var item in pickUpList)
            {
                PickUpBoyVM pickUpVM = new PickUpBoyVM();
                pickUpVM.PickUpBoyID = item.PickUpBoyID;
                pickUpVM.PickUpBoyName = item.PickUpBoyName;
                pickUpVM.PickUpBoyNumber = (decimal)item.PickUpBoyNumber;
                pickUpVM.Flag = item.Flag;
                pickUpVMList.Add(pickUpVM);
            }

            return View(pickUpVMList);
        }

        public async Task<ActionResult> Create(int Id = 0)
        {
            PickUpBoyVM pickUpVM;
            if (Id == 0)
            {
                pickUpVM = new PickUpBoyVM();
            }
            else
            {
                PickUpBoy pickUp = new PickUpBoy();
                pickUp = await Task.Run(() => db.PickUpBoys.Find(Id)) ;
                pickUpVM = new PickUpBoyVM();

                if (pickUp != null)
                {
                    pickUpVM.PickUpBoyID = pickUp.PickUpBoyID;
                    pickUpVM.PickUpBoyName = pickUp.PickUpBoyName;
                    pickUpVM.PickUpBoyNumber = (decimal)pickUp.PickUpBoyNumber;
                }
            }
            return View(pickUpVM);
        }

        [HttpPost]
        public async Task<JsonResult> Create(PickUpBoyVM pickUpVM)
        {
            var loginResult = new LoginResult();
            loginResult.IsError = false;
            loginResult.ErrorMessage = "";
            var PickUpBoyList = await Task.Run(() => db.PickUpBoys.ToList());
            var exits = PickUpBoyList.FirstOrDefault(t => t.PickUpBoyName == pickUpVM.PickUpBoyName && t.PickUpBoyNumber==pickUpVM.PickUpBoyNumber);
            if (exits != null)
            {
                loginResult.IsError = true;
                loginResult.ErrorMessage = "Data already exists";
            }
            else
            {
                PickUpBoy pickUpObj = new PickUpBoy();

                if (pickUpVM.PickUpBoyID == 0)
                {
                    pickUpObj.PickUpBoyName = pickUpVM.PickUpBoyName;
                    pickUpObj.PickUpBoyNumber = (decimal)pickUpVM.PickUpBoyNumber;
                    pickUpObj.CreateDate = GetCurrentSession.CurrentDateTime();
                    pickUpObj.CreateUser = (int)GetCurrentSession.CurrentUser();
                    pickUpObj.Flag = "A";
                    await Task.Run(() => db.PickUpBoys.Add(pickUpObj));
                    await Task.Run(() => db.SaveChanges());

                }
                else
                {
                    pickUpObj = await Task.Run(() => db.PickUpBoys.Find(pickUpVM.PickUpBoyID));
                    if (pickUpObj !=null)
                    {
                        {
                            pickUpObj.PickUpBoyName = pickUpVM.PickUpBoyName;
                            pickUpObj.PickUpBoyNumber = (decimal)pickUpVM.PickUpBoyNumber;
                            pickUpObj.UpdateDate = GetCurrentSession.CurrentDateTime();
                            pickUpObj.UpdateUser = (int)GetCurrentSession.CurrentUser();
                            pickUpObj.Flag = "A";
                            await Task.Run(() => db.Entry(pickUpObj).State = System.Data.Entity.EntityState.Modified);
                            await Task.Run(() => db.SaveChanges());
                        }
                    }
                }
            }

            return Json(loginResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int Id)
        {
            PickUpBoy Obj = db.PickUpBoys.Find(Id);
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
            }
           
            return Json(Obj, JsonRequestBehavior.AllowGet);
        }

    }
}