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
    public class HamaliController : Controller
    {
        JalaramDBEntities db = new JalaramDBEntities();
        public async Task<ActionResult> Index()
        {
            HamaliVM HamaliVM = new HamaliVM();
            var Hamali = await Task.Run(() => db.HamaliMasters.OrderByDescending(t => t.ID).FirstOrDefault()) ;

            if (Hamali != null)
            {
                HamaliVM.ID = Hamali.ID;
                HamaliVM.Hamali = (decimal)Hamali.Hamali;
            }
            else
            {
                HamaliVM.Hamali = 0;
            }
            return View(HamaliVM);
        }

        [HttpPost]
        public async Task<JsonResult> CreatePost(HamaliVM HamaliVM)
        {
            HamaliMaster HamaliObj = new HamaliMaster();

            //db.Database.ExecuteSqlCommand("TRUNCATE TABLE HamaliMasters ");
            db.HamaliMasters.RemoveRange(db.HamaliMasters);
            HamaliObj.Hamali = HamaliVM.Hamali;
            await Task.Run(() => db.HamaliMasters.Add(HamaliObj));
            await Task.Run(() => db.SaveChanges());

            return Json(HamaliObj, JsonRequestBehavior.AllowGet);
        }
    }
}