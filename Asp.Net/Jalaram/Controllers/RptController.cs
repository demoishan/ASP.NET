using CrystalDecisions.CrystalReports.Engine;
using JalaramTravels.Filters;
using JalaramTravels.Models;
using JalaramTravels.Reports;
using JalaramTravels.ViewModel;
using PdfSample.Pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JalaramTravels.Controllers
{
    public class RptController : Controller
    {
        JalaramDBEntities db = new JalaramDBEntities();
        public ActionResult Index()
        {
            return View();
        }
        public CrystalReportPdfResult Pdf()
        {
            List<CustomerTest> model = new List<CustomerTest>();
            model.Add(new CustomerTest { CompanyName = "Blah Inc.", ContactName = "Joe Blogs" });
            string reportPath = Path.Combine(Server.MapPath("~/Reports"), "rptTest.rpt");
            return new CrystalReportPdfResult(reportPath, model);
        }

        public ActionResult Te()
        {

            int Tempoid = 1;
            DateTime CurrentDate = new DateTime();
            CurrentDate = GetCurrentSession.CurrentDateTime();
            #region Print 
            string joindata = string.Join(", ", "1,2,3");

                SqlParameter param1 = new SqlParameter("@Var", joindata);
                List<rptCustomer> rptCustomerList = db.Database.SqlQuery<rptCustomer>("SP_PickUpboy @Var", param1).ToList();
                var LoginList = db.Logins.ToList();
                string PrintBy = string.Empty;
                int CurrentUser = (int)GetCurrentSession.CurrentUser();
                var login = LoginList.FirstOrDefault(t => t.LoginID == CurrentUser);
                if (login != null)
                {
                    PrintBy = login.FirstName + " " + login.LastName;
                }

                //List<rptCustomer> rptCustomerList = context.Database.SqlQuery<rptCustomer>("SP_PickUpboy @Var", param1).ToList<rptCustomer>();
                DataTable dtBoyName = new DataTable();
                dtBoyName.Columns.Add("Name", typeof(string));


                var boy = db.PickUpBoys.FirstOrDefault(t => t.PickUpBoyID == Tempoid);
                if (boy != null)
                {
                    dtBoyName.Rows.Add("Delivery Boy:-  " + boy.PickUpBoyName + "\t Print By:- " + PrintBy);
                }
                else
                {
                    dtBoyName.Rows.Add("");
                }

                DataTable dtCustomer = new DataTable();
                dtCustomer.Columns.Add("LRNo", typeof(string));
                dtCustomer.Columns.Add("CustomerName", typeof(string));
                dtCustomer.Columns.Add("PayTypeStatus", typeof(string));
                dtCustomer.Columns.Add("Amount", typeof(decimal));
                dtCustomer.Columns.Add("Total", typeof(decimal));
                dtCustomer.Columns.Add("Damrage", typeof(decimal));
                dtCustomer.Columns.Add("Hamali", typeof(decimal));
                dtCustomer.Columns.Add("NoOfParcel", typeof(int));
                dtCustomer.Columns.Add("CustomerNumber", typeof(string));
                foreach (var item in rptCustomerList)
                {
                    dtCustomer.Rows.Add(item.LRNo, item.CustomerName, item.PayTypeStatus, item.Amount,
                        item.Total, item.Damrage, item.Hamali * item.NoOfParcel, item.NoOfParcel, item.CustomerNumber);
                }

                DataSet1 myds = new DataSet1();
                myds.Tables["DtPickupBoy"].Merge(dtCustomer);
                myds.Tables["DtBoyName"].Merge(dtBoyName);

                ReportDocument rptH = new ReportDocument();
                rptH.Load(Server.MapPath("~/Reports/rptPickUpboy.rpt"));
                rptH.SetDataSource(myds);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                string htmlfilename = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string path = Server.MapPath("~/PDF");
                AllClass.CreateDirectory(path);
                string fileName = path + "/" + htmlfilename + ".pdf";
                string fileName2 = htmlfilename + ".pdf";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                rptH.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, fileName);

                JsonResult result = new JsonResult();
                result.Data = fileName2;
                return result;
           
            #endregion

        }




    }
}