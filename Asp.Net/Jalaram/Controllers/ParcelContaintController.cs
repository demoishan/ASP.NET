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
    public class ParcelContaintController : Controller
    {
        JalaramDBEntities db = new JalaramDBEntities();

        public async Task<ActionResult> Index()
        {
            var ParcelContainerList = await Task.Run(() => db.ParcelContainerMasters.ToList());
            List<ParcelContainerVM> pList = new List<ParcelContainerVM>();

            foreach (var item in ParcelContainerList)
            {
                ParcelContainerVM parcelContainerVM = new ParcelContainerVM();
                parcelContainerVM.ParcelContainerID = item.ParcelContainerID;
                parcelContainerVM.ParcelContainerName = item.ParcelContainerName;
                pList.Add(parcelContainerVM);
            }

            return View(pList);
        }

        public async Task<ActionResult> Create(int Id = 0)
        {
            ParcelContainerVM parcelContainerVM;
            if (Id == 0)
            {
                parcelContainerVM = new ParcelContainerVM();
            }
            else
            {
                ParcelContainerMaster parcelContainer = new ParcelContainerMaster();
                parcelContainer = await Task.Run(() => db.ParcelContainerMasters.Find(Id));
                parcelContainerVM = new ParcelContainerVM();

                if (parcelContainer != null)
                {
                    parcelContainerVM.ParcelContainerID = parcelContainer.ParcelContainerID;
                    parcelContainerVM.ParcelContainerName = parcelContainer.ParcelContainerName;
                }
            }
            return View(parcelContainerVM);
        }

        [HttpPost]
        public async Task<JsonResult> Create(ParcelContainerVM parcelContainerVM)
        {
            var loginResult = new LoginResult();
            loginResult.IsError = false;
            loginResult.ErrorMessage = "";
            var ParcelList = await Task.Run(() => db.ParcelContainerMasters.ToList());
            var exits = ParcelList.FirstOrDefault(t => t.ParcelContainerName == parcelContainerVM.ParcelContainerName);
            if (exits != null)
            {
                loginResult.IsError = true;
                loginResult.ErrorMessage = "Data already exists";
            }
            else
            {
                ParcelContainerMaster ParcelObj = new ParcelContainerMaster();
                if (parcelContainerVM.ParcelContainerID == 0)
                {
                    ParcelObj.ParcelContainerName = parcelContainerVM.ParcelContainerName;
                    await Task.Run(() => db.ParcelContainerMasters.Add(ParcelObj));
                    await Task.Run(() => db.SaveChanges());
                }
                else
                {
                    ParcelObj = await Task.Run(() => db.ParcelContainerMasters.Find(parcelContainerVM.ParcelContainerID));
                    if (ParcelObj != null)
                    {
                        ParcelObj.ParcelContainerID = parcelContainerVM.ParcelContainerID;
                        ParcelObj.ParcelContainerName = parcelContainerVM.ParcelContainerName;
                        await Task.Run(() => db.Entry(ParcelObj).State = System.Data.Entity.EntityState.Modified);
                        await Task.Run(() => db.SaveChanges());

                    }
                }
            }

            return Json(loginResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int Id)
        {
            ParcelContainerMaster Obj = await Task.Run(() => db.ParcelContainerMasters.Find(Id));

            if (Obj != null)
            {
                await Task.Run(() => db.ParcelContainerMasters.Remove(Obj));
                await Task.Run(() => db.SaveChanges());
            }
            return Json(Obj, JsonRequestBehavior.AllowGet);
        }
    }
}