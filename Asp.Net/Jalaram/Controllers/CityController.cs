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
    public class CityController : Controller
    {
        JalaramDBEntities db = new JalaramDBEntities();

        public async Task<ActionResult> Index()
        {
            var cityList = await Task.Run(() => db.Cities.ToList());
            List<CityVM> cityVMList = new List<CityVM>();

            foreach (var item in cityList)
            {
                CityVM cityVM = new CityVM();
                cityVM.CityID = item.CityID;
                cityVM.CityName = item.CityName;
                cityVM.Flag = item.Flag;
                cityVMList.Add(cityVM);
            }

            return View(cityVMList);
        }

        public async Task<ActionResult> Create(int Id = 0)
        {
            CityVM cityVM;
            if (Id == 0)
            {
                cityVM = new CityVM();
            }
            else
            {
                City city = new City();
                city = await Task.Run(() => db.Cities.Find(Id));
                cityVM = new CityVM();

                if (city != null)
                {
                    cityVM.CityID = city.CityID;
                    cityVM.CityName = city.CityName;
                }
            }
            return View(cityVM);
        }

        [HttpPost]
        public async Task<JsonResult> Create(CityVM cityVM)
        {
            var loginResult = new LoginResult();
            loginResult.IsError = false;
            loginResult.ErrorMessage = "";
            var cityList = await Task.Run(() => db.Cities.ToList());
            var exits = cityList.FirstOrDefault(t => t.CityName == cityVM.CityName);
            if (exits !=null)
            {
                loginResult.IsError = true;
                loginResult.ErrorMessage = "Data already exists";
            }
            else
            {
                City cityObj = new City();

                if (cityVM.CityID == 0)
                {
                    cityObj.CityName = cityVM.CityName;
                    cityObj.CreateDate = GetCurrentSession.CurrentDateTime();
                    cityObj.CreateUser = (int)GetCurrentSession.CurrentUser();
                    cityObj.Flag = "A";
                    await Task.Run(() => db.Cities.Add(cityObj));
                    await Task.Run(() => db.SaveChanges());
                }
                else
                {
                    cityObj = await Task.Run(() => db.Cities.Find(cityVM.CityID));
                    if (cityObj != null)
                    {
                        cityObj.CityID = cityVM.CityID;
                        cityObj.CityName = cityVM.CityName;
                        cityObj.Flag = "A";
                        cityObj.UpdateDate = GetCurrentSession.CurrentDateTime();
                        cityObj.UpdateUser = (int)GetCurrentSession.CurrentUser();
                        await Task.Run(() => db.Entry(cityObj).State = System.Data.Entity.EntityState.Modified);
                        await Task.Run(() => db.SaveChanges());
                    }

                }
            }
            
            return Json(loginResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int Id)
        {
            City Obj = db.Cities.Find(Id);

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