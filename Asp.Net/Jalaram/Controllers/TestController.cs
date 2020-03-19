using PdfSample.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JalaramTravels.Controllers
{
    public class TestController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public CrystalReportPdfResult Pdf()
        {
            List<Customer> model = new List<Customer>();
            model.Add(new Customer { CompanyName = "Blah Inc.", ContactName = "Joe Blogs" });
            string reportPath = Path.Combine(Server.MapPath("~/Reports"), "rptTest.rpt");
            return new CrystalReportPdfResult(reportPath, model);
        }

        public class Customer
        {
            public string CompanyName { get; set; }
            public string ContactName { get; set; }
        }
    }
}