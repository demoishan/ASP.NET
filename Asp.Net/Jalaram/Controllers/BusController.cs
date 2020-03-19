using JalaramTravels.Filters;
using JalaramTravels.Models;
using JalaramTravels.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace JalaramTravels.Controllers
{
    [CheckSessionTimeOut]
    public class BusController : Controller
    {
        JalaramDBEntities db = new JalaramDBEntities();
        
        public async Task<ActionResult> Index()
        {
             var busList = await Task.Run(() => db.Buses.ToList()) ;
            List<BusVM> busVMList = new List<BusVM>();

            foreach (var item in busList)
            {
                BusVM busVM = new BusVM();
                busVM.BusID = item.BusID;
                busVM.BusName = item.BusName;
                busVM.BusNumber = item.BusNumber;
                busVM.Flag = item.Flag;
                busVMList.Add(busVM);
            }

            return View(busVMList);
        }

        public async Task<ActionResult> Create(int Id = 0)
        {
            BusVM busVM;
            if (Id == 0)
            {
                busVM = new BusVM();
            }
            else
            {
                Bus bus = new Bus();
                bus = await Task.Run(() => db.Buses.Find(Id));
                busVM = new BusVM();

                if (bus != null)
                {
                    busVM.BusID = bus.BusID;
                    busVM.BusName = bus.BusName;
                    busVM.BusNumber = bus.BusNumber;
                }
            }
            return View(busVM);
        }

        [HttpPost]
        public async Task<JsonResult> Create(BusVM busVM)
        {
            var loginResult = new LoginResult();
            loginResult.IsError = false;
            loginResult.ErrorMessage = "";
            var busList = await Task.Run(() => db.Buses.ToList());
            var exits = busList.FirstOrDefault(t => t.BusName == busVM.BusName && t.BusNumber == busVM.BusNumber);
            if (exits != null)
            {
                loginResult.IsError = true;
                loginResult.ErrorMessage = "Data already exists";
            }
            else
            {
                Bus busObj = new Bus();

                if (busVM.BusID == 0)
                {
                    busObj.BusName = busVM.BusName;
                    busObj.BusNumber = busVM.BusNumber;
                    busObj.CreateDate = GetCurrentSession.CurrentDateTime();
                    busObj.CreateUser = (int)GetCurrentSession.CurrentUser();
                    busObj.Flag = "A";
                    await Task.Run(() => db.Buses.Add(busObj));
                    await Task.Run(() => db.SaveChanges());

                }
                else
                {
                    busObj = await Task.Run(() => db.Buses.Find(busVM.BusID));
                    if (busObj != null)
                    {
                        busObj.BusID = busVM.BusID;
                        busObj.BusName = busVM.BusName;
                        busObj.BusNumber = busVM.BusNumber;
                        busObj.Flag = "A";
                        busObj.UpdateDate = GetCurrentSession.CurrentDateTime();
                        busObj.UpdateUser = (int)GetCurrentSession.CurrentUser();
                        await Task.Run(() => db.Entry(busObj).State = System.Data.Entity.EntityState.Modified);
                        await Task.Run(() => db.SaveChanges());
                    }
                }
            }
            return Json(loginResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int Id)
        {
            Bus Obj = await Task.Run(() => db.Buses.Find(Id)) ;

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
                //await Task.Run(() => db.Buses.Remove(Obj));
                //await Task.Run(() => db.SaveChanges());
            }
            return Json(Obj, JsonRequestBehavior.AllowGet);
        }
    }
}