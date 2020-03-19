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
    public class ParcelTypeController : Controller
    {
        JalaramDBEntities db = new JalaramDBEntities();

        public async Task<ActionResult> Index()
        {
            var ParcelTypeList = await Task.Run(() => db.ParcelTypeMasters.ToList());
            List<ParcelTypeVM> ParcelTypeVMList = new List<ParcelTypeVM>();

            foreach (var item in ParcelTypeList)
            {
                ParcelTypeVM busVM = new ParcelTypeVM();
                busVM.ParcelTypeID = item.ParcelTypeID;
                busVM.ParcelTypeName = item.ParcelTypeName;
                ParcelTypeVMList.Add(busVM);
            }

            return View(ParcelTypeVMList);
        }

        public async Task<ActionResult> Create(int Id = 0)
        {
            ParcelTypeVM parcelTypeVM;
            if (Id == 0)
            {
                parcelTypeVM = new ParcelTypeVM();
            }
            else
            {
                ParcelTypeMaster parcelTypeMaster = new ParcelTypeMaster();
                parcelTypeMaster = await Task.Run(() => db.ParcelTypeMasters.Find(Id));
                parcelTypeVM = new ParcelTypeVM();

                if (parcelTypeMaster != null)
                {
                    parcelTypeVM.ParcelTypeID = parcelTypeMaster.ParcelTypeID;
                    parcelTypeVM.ParcelTypeName = parcelTypeMaster.ParcelTypeName;
                }
            }
            return View(parcelTypeVM);
        }

        [HttpPost]
        public async Task<JsonResult> Create(ParcelTypeVM parcelTypeVM)
        {
            var loginResult = new LoginResult();
            loginResult.IsError = false;
            loginResult.ErrorMessage = "";
            var ParcelList = await Task.Run(() => db.ParcelTypeMasters.ToList());
            var exits = ParcelList.FirstOrDefault(t => t.ParcelTypeName == parcelTypeVM.ParcelTypeName);
            if (exits != null)
            {
                loginResult.IsError = true;
                loginResult.ErrorMessage = "Data already exists";
            }
            else
            {
                ParcelTypeMaster obj = new ParcelTypeMaster();

                if (parcelTypeVM.ParcelTypeID == 0)
                {
                    obj.ParcelTypeName = parcelTypeVM.ParcelTypeName;
                    await Task.Run(() => db.ParcelTypeMasters.Add(obj));
                    await Task.Run(() => db.SaveChanges());
                }
                else
                {
                    obj = await Task.Run(() => db.ParcelTypeMasters.Find(parcelTypeVM.ParcelTypeID));
                    if (obj !=null)
                    {
                        obj.ParcelTypeID = parcelTypeVM.ParcelTypeID;
                        obj.ParcelTypeName = parcelTypeVM.ParcelTypeName;
                        await Task.Run(() => db.Entry(obj).State = System.Data.Entity.EntityState.Modified);
                        await Task.Run(() => db.SaveChanges());

                    }
                }
            }
            return Json(loginResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int Id)
        {
            ParcelTypeMaster Obj = await Task.Run(() => db.ParcelTypeMasters.Find(Id));

            if (Obj != null)
            {
                await Task.Run(() => db.ParcelTypeMasters.Remove(Obj));
                await Task.Run(() => db.SaveChanges());
            }
            return Json(Obj, JsonRequestBehavior.AllowGet);
        }
    }
}