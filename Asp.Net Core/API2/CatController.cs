using DemoDapper.Abstractions;
using DemoDapper.Models;
using DemoDapper.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoDapper.Controllers
{
    public class CatController : Controller
    {
        ICategoryService _salesOrderService;
        IAbstractService<CategoryMaster> _salesOrderLineService;

        public CatController(ICategoryService salesOrderService, IAbstractService<CategoryMaster> salesOrderLineService)
        {
            _salesOrderService = salesOrderService ;
            _salesOrderLineService = salesOrderLineService;
        }
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            var salesOrderSettings = await _salesOrderService.GetAllAsync();
            return View();
        }
    }
}