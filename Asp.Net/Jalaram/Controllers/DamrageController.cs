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
    public class DamrageController : Controller
    {
        JalaramDBEntities db = new JalaramDBEntities();
        public async Task<ActionResult> Index()
        {
            DamrageVM damrageVM = new DamrageVM();
            var damrage = await Task.Run(() => db.DamrageMasters.OrderByDescending(t => t.ID).FirstOrDefault()) ; 

            if (damrage != null)
            {
                damrageVM.ID = damrage.ID;
                damrageVM.Damrage = (decimal)damrage.Damrage;
            }
            else
            {
                damrageVM.Damrage = 0;
            }
            return View(damrageVM);
        }

        [HttpPost]
        public async Task<JsonResult> CreatePost(DamrageVM damrageVM)
        {
            DamrageMaster damrageObj = new DamrageMaster();

            db.DamrageMasters.RemoveRange(db.DamrageMasters);
            damrageObj.Damrage = damrageVM.Damrage;
            await Task.Run(() => db.DamrageMasters.Add(damrageObj));
            await Task.Run(() => db.SaveChanges());

            return Json(damrageObj, JsonRequestBehavior.AllowGet);
        }
    }
}