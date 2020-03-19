using JalaramTravels.Models;
using JalaramTravels.ViewModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using JalaramTravels.Reports;
using CrystalDecisions.CrystalReports.Engine;
using System.Data.Sql;
using System.Data;
using System.Data.SqlClient;
using JalaramTravels.Filters;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Collections;
using System.Data.Common;

namespace JalaramTravels.Controllers
{
    [CheckSessionTimeOut]
    public class HomeController : Controller
    {
        JalaramDBEntities db = new JalaramDBEntities();

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            if (Session["LoginID"] == null)
            {
                return RedirectToAction("Index", "Login");

            }
            DashboardVM dashboad = new DashboardVM();

            var busList = db.Buses.ToList();
            var customersList = db.Customers.ToList();
            var transactionMastersList = db.TransactionMasters.ToList();
            var transactionDetailsList = db.TransactionDetails.ToList();

            transactionMastersList = transactionMastersList.Where(t => t.TransactionDate.Value.Month == DateTime.Today.Month).ToList();
            transactionDetailsList = transactionDetailsList.Where(t => t.TransactionDate.Value.Month == DateTime.Today.Month).ToList();

            dashboad.TotalBus = busList.Count();
            dashboad.TotalCustomer = customersList.Count();
            var TotalPaid = transactionMastersList.Sum(t => t.TotalPaid.Value);
            dashboad.TotalPaid = Convert.ToInt32(TotalPaid);

            var TotalParcel = transactionMastersList.Sum(t => t.TotalParcel.Value);
            dashboad.TotalParcel = Convert.ToInt32(TotalParcel);

            var TotalTopay = transactionMastersList.Sum(t => t.TotalTopay.Value);
            dashboad.TotalTopay = Convert.ToInt32(TotalTopay);


            var TotalDeliverd = transactionDetailsList.Where
                (t => t.DeliverdStatus == (int)JalaramTravels.Models.Enums.DeliverdStatus.Delivered)
                .Count();
            dashboad.TotalDelivered = Convert.ToInt32(TotalDeliverd);



            var TotalUnDeliverd = transactionDetailsList.Where
                (t => t.DeliverdStatus == (int)JalaramTravels.Models.Enums.DeliverdStatus.Undelivered)
                .Count();
            dashboad.TotalUndelivered = Convert.ToInt32(TotalUnDeliverd);


            var TotalInTransit = transactionDetailsList.Where
                (t => t.DeliverdStatus == (int)JalaramTravels.Models.Enums.DeliverdStatus.InTransit)
                .Sum(t => t.TransactionDetailID);
            dashboad.TotalInTransit = Convert.ToInt32(TotalInTransit);

            List<CustomerVM> rptCustomerList = db.Database.SqlQuery<CustomerVM>("SP_DashBoard_Customers").ToList();

            dashboad.CustomerList = rptCustomerList;
            return View(dashboad);

        }

        public FileResult DownloadTemplate()
        {
            string filepath = Server.MapPath("~/UploadFiles") + @"\Template.xlsx";
            byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SampleTemplate.xlsx");
        }

        public ActionResult UploadFiles()
        {
            if (Session["LoginID"] == null)
            {
                return RedirectToAction("Index", "Login");

            }
            UploadVM uploadVMobj = new UploadVM();

            uploadVMobj.TransactionDate = GetCurrentSession.CurrentDateTime();

            var busActiveList = db.Buses.ToList().Where(m => m.Flag.Equals("A")).OrderBy(m => m.BusName);
            List<BusVM> buslist = new List<BusVM>();
            uploadVMobj.BusList = busActiveList.Select(s => new SelectListItem() { Text = s.BusName + "-" + s.BusNumber, Value = s.BusID.ToString() });

            var SenderCityList = db.Cities.ToList().Where(m => m.Flag.Equals("A")).OrderBy(m => m.CityName);
            uploadVMobj.SenderCityList = SenderCityList.Select(s => new SelectListItem() { Text = s.CityName, Value = s.CityID.ToString() });

            foreach (var item in busActiveList)
            {

                BusVM busVM = new BusVM();
                busVM.BusID = item.BusID;
                busVM.BusName = item.BusName;
                busVM.BusNumber = item.BusNumber;
                buslist.Add(busVM);
            }
            return View(uploadVMobj);
        }

        public async Task<ActionResult> Preview()
        {
            var ParcelTypeMasterList = await Task.Run(() => db.ParcelTypeMasters.ToList());
            var ParcelContainerMasterList = await Task.Run(() => db.ParcelContainerMasters.ToList());
            Decimal Hamali = 0;
            var hamalidb = await Task.Run(() => db.HamaliMasters.OrderByDescending(t => t.ID).FirstOrDefault());
            if (hamalidb != null)
            {
                Hamali = (decimal)hamalidb.Hamali;
            }

            var ExcelTableList = await Task.Run(() => db.ExcelTables.ToList()); ;
            List<TransactionDetailsVM> transactionDetailsVM = new List<ViewModel.TransactionDetailsVM>();
            foreach (var item in ExcelTableList)
            {
                TransactionDetailsVM detail = new TransactionDetailsVM();
                detail.StatusFlag = "Y";
                detail.Id = item.Id;
                detail.LRNo = item.LRNo;
                detail.NoOfParcel = item.NoOfParcel.Value;
                detail.SenderName = item.SenderName;
                detail.SenderCityID = item.SenderCityID.Value;
                detail.SenderNumber = item.SenderNumber;
                detail.ReceiverName = item.ReceiverName;
                detail.ReceiverNumber = item.ReceiverNumber;
                detail.ReceiverCity = item.ReceiverCity;
                detail.Amount = item.Amount.Value;
                detail.Cartage = item.Cartage.Value;
                detail.PayType = (JalaramTravels.Models.Enums.PayTypes)Enum.ToObject(typeof(JalaramTravels.Models.Enums.PayTypes), item.PayType.Value);
                detail.BusID = item.BusID.Value;
                detail.DriverName = item.DriverName;
                detail.TransactionDate = item.TransactionDate.Value;
                detail.Hamali = Hamali;

                detail.ParcelTypeList = ParcelTypeMasterList.Select(s => new SelectListItem() { Text = s.ParcelTypeName, Value = s.ParcelTypeID.ToString() });
                detail.ParcelContainerList = ParcelContainerMasterList.Select(s => new SelectListItem() { Text = s.ParcelContainerName, Value = s.ParcelContainerID.ToString() });
                detail.ParcelTypeID = 1;
                detail.ParcelContainerID = 1;
                transactionDetailsVM.Add(detail);

            }
            return View(transactionDetailsVM);
        }

        [HttpPost]
        public JsonResult PreviewPost(InTransitVM inTransitVM)
        {
            Response<bool> response = new Response<bool>();
            DateTime CurrentDate = new DateTime();
            CurrentDate = GetCurrentSession.CurrentDateTime();
            decimal Hamali = 0;
            decimal Damrage = 0;
            Int64 TransactionMasterID;

            var scopelevel = new TransactionScope();
            using (var context = ApplicationDbContext.Create())
            {
                using (scopelevel)
                {
                    try
                    {
                        var hamali = db.HamaliMasters.OrderByDescending(t => t.ID).FirstOrDefault();
                        if (hamali != null)
                        {
                            Hamali = (decimal)hamali.Hamali;
                        }

                        foreach (var item in inTransitVM.TransactionDetailList)
                        {
                            if (item.StatusFlag == "Y")
                            {
                                var transactionMasterObj = db.TransactionMasters.Where(t => t.TransactionDate == item.TransactionDate && t.BusID == item.BusID).FirstOrDefault();
                                if (transactionMasterObj != null)
                                {
                                    TransactionMasterID = transactionMasterObj.TransactionMasterID;
                                }
                                else
                                {
                                    TransactionMaster transactionMaster = new TransactionMaster();
                                    transactionMaster.BusID = item.BusID;
                                    transactionMaster.TransactionDate = item.TransactionDate;
                                    transactionMaster.DriverName = item.DriverName;
                                    transactionMaster.TransactionStatus = (int)Enums.TransactionStatus.Incomplete;
                                    transactionMaster.CreateDate = GetCurrentSession.CurrentDateTime();
                                    transactionMaster.CreateUser = (int)GetCurrentSession.CurrentUser();

                                    db.TransactionMasters.Add(transactionMaster);
                                    db.SaveChanges();
                                    TransactionMasterID = transactionMaster.TransactionMasterID;
                                }
                                TransactionDetail transationdetail = new TransactionDetail();

                                transationdetail.TransactionMasterID = TransactionMasterID;
                                transationdetail.LRNo = Convert.ToString(item.LRNo);
                                transationdetail.NoOfParcel = item.NoOfParcel;

                                #region Customer Sender
                                transationdetail.SenderName = Convert.ToString(item.SenderName);
                                transationdetail.SenderNumber = Convert.ToString(0);
                                transationdetail.SenderCityID = item.SenderCityID;
                                Customer custSender = new Customer();
                                custSender = db.Customers.FirstOrDefault(m => m.CustomerName == transationdetail.SenderName);

                                if (custSender == null)
                                {
                                    custSender = new Customer();
                                    custSender.CustomerName = transationdetail.SenderName;
                                    custSender.CustomerNumber = transationdetail.SenderNumber;
                                    custSender.CustomerCityID = transationdetail.SenderCityID;
                                    db.Customers.Add(custSender);
                                    db.SaveChanges();
                                    transationdetail.SenderID = custSender.CustomerID;
                                }
                                else
                                {
                                    transationdetail.SenderID = custSender.CustomerID;
                                }
                                #endregion

                                #region Customer Receiver
                                string RCity = string.Empty;
                                RCity = Convert.ToString(item.ReceiverCity);

                                City city = new City();
                                city = db.Cities.FirstOrDefault(m => m.CityName == RCity);
                                if (city == null)
                                {
                                    city = new City();
                                    city.CityName = RCity;
                                    city.CreateDate = CurrentDate;
                                    city.Flag = "A";
                                    db.Cities.Add(city);
                                    db.SaveChanges();
                                    transationdetail.ReceiverCityID = city.CityID;
                                }
                                else
                                {
                                    transationdetail.ReceiverCityID = city.CityID;
                                }
                                transationdetail.ReceiverName = Convert.ToString(item.ReceiverName);
                                transationdetail.ReceiverNumber = Convert.ToString(item.ReceiverNumber);

                                Customer custRecever = new Customer();
                                custRecever = db.Customers.FirstOrDefault(m => m.CustomerName == transationdetail.ReceiverName && m.CustomerNumber == transationdetail.ReceiverNumber);

                                if (custRecever == null)
                                {
                                    custRecever = new Customer();
                                    custRecever.CustomerName = transationdetail.ReceiverName;
                                    custRecever.CustomerNumber = transationdetail.ReceiverNumber;
                                    custRecever.CustomerCityID = transationdetail.ReceiverCityID;
                                    db.Customers.Add(custRecever);
                                    db.SaveChanges();
                                    transationdetail.ReceiverID = custRecever.CustomerID;
                                }
                                else
                                {
                                    transationdetail.ReceiverID = custRecever.CustomerID;
                                }

                                #endregion

                                transationdetail.Cartage = item.Cartage;
                                transationdetail.Hamali = Hamali;
                                transationdetail.Damrage = Damrage;
                                if (item.PayType != 0)
                                {
                                    transationdetail.Amount = item.Amount + transationdetail.Cartage;
                                    transationdetail.PayType = (int)JalaramTravels.Models.Enums.PayTypes.ToPay;
                                }
                                else
                                {
                                    transationdetail.Amount = item.Amount + transationdetail.Cartage;
                                    transationdetail.PayType = (int)JalaramTravels.Models.Enums.PayTypes.Paid;
                                }
                                transationdetail.PaymentType = (int)JalaramTravels.Models.Enums.PaymentTypes.None;
                                transationdetail.ParcelContainerID = item.ParcelContainerID;
                                transationdetail.ParcelTypeID = item.ParcelTypeID;
                                transationdetail.TransactionDate = item.TransactionDate;
                                transationdetail.CreateDate = GetCurrentSession.CurrentDateTime();
                                transationdetail.CreateUser = (int)GetCurrentSession.CurrentUser();
                                transationdetail.Flag = "A";
                                transationdetail.BusID = item.BusID;
                                transationdetail.DriverName = item.DriverName;
                                transationdetail.DeliverdStatus = (int)JalaramTravels.Models.Enums.DeliverdStatus.Undelivered;

                                db.TransactionDetails.Add(transationdetail);
                                db.SaveChanges();


                                var TotalTopaid = db.TransactionDetails.ToList().Where(t => t.TransactionMasterID == TransactionMasterID && t.PayType == (int)JalaramTravels.Models.Enums.PayTypes.ToPay).Sum(t => t.Amount);
                                var TotalPaid = db.TransactionDetails.ToList().Where(t => t.TransactionMasterID == TransactionMasterID && t.PayType == (int)JalaramTravels.Models.Enums.PayTypes.Paid).Sum(t => t.Amount);
                                var TotalCartage = db.TransactionDetails.ToList().Where(t => t.TransactionMasterID == TransactionMasterID).Sum(t => t.Cartage);
                                var TotalDamrage = db.TransactionDetails.ToList().Where(t => t.TransactionMasterID == TransactionMasterID).Sum(t => t.Damrage);
                                var NoOfParcel1 = db.TransactionDetails.ToList().Where(t => t.TransactionMasterID == TransactionMasterID).Sum(t => t.NoOfParcel);

                                TransactionMaster transactionMaster1 = new TransactionMaster();
                                transactionMaster1 = db.TransactionMasters.FirstOrDefault(t => t.TransactionMasterID == TransactionMasterID);

                                if (transactionMaster1 != null)
                                {
                                    transactionMaster1.TotalTopay = TotalTopaid;
                                    transactionMaster1.TotalPaid = TotalPaid;
                                    transactionMaster1.TotalCartage = TotalCartage;
                                    transactionMaster1.TotalDamrage = TotalDamrage;
                                    transactionMaster1.TotalParcel = NoOfParcel1;
                                    db.Entry(transactionMaster1).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();

                                }
                            }
                            ExcelTable ObjExcelTable = db.ExcelTables.Find(item.Id);

                            if (ObjExcelTable != null)
                            {
                                db.ExcelTables.Remove(ObjExcelTable);
                                db.SaveChanges();
                            }
                        }
                        scopelevel.Complete();

                        response.Result = true;
                        response.Status = HttpStatusCode.OK;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        response.Error = ex.Message;
                        response.Status = HttpStatusCode.BadRequest;
                    }
                    finally
                    {
                        scopelevel.Dispose();
                    }
                }
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadFilesPost(UploadVM uploadVM, HttpPostedFileBase fileUpload)
        {
            Response<bool> response = new Response<bool>();
            var scopelevel = new TransactionScope();

            using (var context = ApplicationDbContext.Create())
            {
                using (scopelevel)
                {
                    string fileText = string.Empty;
                    if (fileUpload != null && fileUpload.ContentLength > 0)
                    {
                        string path = string.Empty;
                        DateTime CurrentDate = new DateTime();
                        Int64 SenderCityID = uploadVM.SenderCityID;
                        CurrentDate = GetCurrentSession.CurrentDateTime();

                        try
                        {
                            path = Path.Combine(Server.MapPath("~/UploadFiles"),
                            Path.GetFileName(fileUpload.FileName));

                            fileUpload.SaveAs(path);

                            var extension = fileUpload.FileName.Substring(fileUpload.FileName.LastIndexOf('.'));

                            XSSFWorkbook hssfworkbook;
                            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                            {
                                hssfworkbook = new XSSFWorkbook(fs);
                            }

                            //excel and word from here
                            ISheet sheet = hssfworkbook.GetSheetAt(0);
                            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

                            IRow headerRow = sheet.GetRow(0);
                            int cellCount = headerRow.Cells.Count;
                            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                            {
                                
                                IRow row = sheet.GetRow(i);
                                if (row == null)
                                {
                                    continue;
                                }
                                ExcelTable transationdetail = new ExcelTable();

                                transationdetail.LRNo = Convert.ToString(row.GetCell(1));


                                if (transationdetail.LRNo =="")
                                {
                                    continue;
                                }
                                string NoOfParcel = Convert.ToString(row.GetCell(2));
                                transationdetail.NoOfParcel = NoOfParcel == null ? 0 : Convert.ToInt32(NoOfParcel);

                                transationdetail.SenderName = Convert.ToString(row.GetCell(3));
                                transationdetail.SenderNumber = Convert.ToString("0");
                                transationdetail.SenderCityID = uploadVM.SenderCityID;
                                transationdetail.ReceiverName = Convert.ToString(row.GetCell(4));
                                transationdetail.ReceiverNumber = Convert.ToString(row.GetCell(6));
                                string RCity = string.Empty;
                                RCity = Convert.ToString(row.GetCell(5));
                                transationdetail.ReceiverCity = RCity;

                                string Cartage = Convert.ToString(row.GetCell(9));
                                transationdetail.Cartage = Cartage == null ? 0 : Convert.ToDecimal(Cartage);

                                string Paid = Convert.ToString(row.GetCell(7));
                                string ToPay = Convert.ToString(row.GetCell(8));
                                decimal PaidD = Convert.ToDecimal(Paid);
                                decimal ToPayD = Convert.ToDecimal(ToPay);

                                if (PaidD != 0)
                                {
                                    transationdetail.Amount = PaidD;
                                    transationdetail.PayType = (int)JalaramTravels.Models.Enums.PayTypes.Paid;
                                }
                                else
                                {
                                    transationdetail.Amount = ToPayD;  //+ transationdetail.Cartage
                                    transationdetail.PayType = (int)JalaramTravels.Models.Enums.PayTypes.ToPay;
                                }

                                transationdetail.TransactionDate = uploadVM.TransactionDate;
                                transationdetail.CreateDate = CurrentDate;
                                transationdetail.BusID = uploadVM.BusID;
                                transationdetail.DriverName = uploadVM.DriverName;

                                db.ExcelTables.Add(transationdetail);
                                db.SaveChanges();
                            }

                            scopelevel.Complete();

                            response.Result = true;
                            response.Status = HttpStatusCode.OK;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            response.Error = ex.Message;
                            response.Status = HttpStatusCode.BadRequest;
                        }
                        finally
                        {
                            System.IO.File.Delete(path);
                            scopelevel.Dispose();
                        }
                    }
                }
            }
            return RedirectToAction("Preview");
        }

        [HttpPost]
        public JsonResult TransactionPost(TransactionVM uploadVM)
        {

            var response = new LoginResult();
            response.IsError = true;

          //  Response<bool> response = new Response<bool>();
            Int64 TransactionMasterID;
            DateTime CurrentDate = new DateTime();
            CurrentDate = GetCurrentSession.CurrentDateTime();
            var scopelevel = new TransactionScope();

            using (var context = ApplicationDbContext.Create())
            {
                using (scopelevel)
                {
                    try
                    {
                        #region TransactionMasters
                        if (uploadVM.TransactionMasterID == 0)
                        {
                            var transactionMasterObj = db.TransactionMasters.Where(t => t.TransactionDate == uploadVM.TransactionDate && t.BusID == uploadVM.BusID).FirstOrDefault();
                            if (transactionMasterObj != null)
                            {
                                TransactionMasterID = transactionMasterObj.TransactionMasterID;
                            }
                            else
                            {
                                TransactionMaster transactionMaster = new TransactionMaster();
                                transactionMaster.BusID = uploadVM.BusID;
                                transactionMaster.TransactionDate = uploadVM.TransactionDate;
                                transactionMaster.DriverName = uploadVM.DriverName;
                                transactionMaster.TransactionStatus = (int)Enums.TransactionStatus.Incomplete;
                                transactionMaster.CreateDate = GetCurrentSession.CurrentDateTime();
                                transactionMaster.CreateUser = (int)GetCurrentSession.CurrentUser();

                                db.TransactionMasters.Add(transactionMaster);
                                db.SaveChanges();
                                TransactionMasterID = transactionMaster.TransactionMasterID;
                            }
                        }
                        else
                        {
                            TransactionMasterID = uploadVM.TransactionMasterID; ;
                        }
                        #endregion
                        TransactionDetail transationdetail;
                        if (uploadVM.TransactionDetailID == 0)
                        {
                            transationdetail = new TransactionDetail();
                        }
                        else
                        {
                            transationdetail = db.TransactionDetails.FirstOrDefault(t => t.TransactionDetailID == uploadVM.TransactionDetailID); 
                        }


                        transationdetail.TransactionMasterID = TransactionMasterID;
                        transationdetail.LRNo = Convert.ToString(uploadVM.LRNo);
                        string NoOfParcel = Convert.ToString(uploadVM.NoOfParcel);
                        transationdetail.NoOfParcel = NoOfParcel == null ? 0 : Convert.ToInt32(NoOfParcel);
                        transationdetail.Amount = uploadVM.Amount;

                        #region Customer Sender
                        #region City
                        if (uploadVM.SenderCityID == 0)
                        {
                            string RCity = string.Empty;
                            RCity = Convert.ToString(uploadVM.SenderCity);

                            City city = new City();
                            city = db.Cities.FirstOrDefault(m => m.CityName == RCity);
                            if (city == null)
                            {
                                city = new City();
                                city.CityName = RCity;
                                city.CreateDate = CurrentDate;
                                city.Flag = "A";
                                db.Cities.Add(city);
                                db.SaveChanges();
                                transationdetail.SenderCityID = city.CityID;
                            }
                            else
                            {
                                transationdetail.SenderCityID = city.CityID;
                            }
                        }
                        else
                        {
                            transationdetail.SenderCityID = uploadVM.SenderCityID;
                        }
                        #endregion
                        transationdetail.SenderName = Convert.ToString(uploadVM.SenderName);
                        transationdetail.SenderNumber = Convert.ToString(uploadVM.SenderNumber);
                        if (uploadVM.SenderID == 0)
                        {
                            Customer custSender = new Customer();
                            custSender = db.Customers.FirstOrDefault(m => m.CustomerName == transationdetail.SenderName);

                            if (custSender == null)
                            {
                                custSender = new Customer();
                                custSender.CustomerName = transationdetail.SenderName;
                                custSender.CustomerNumber = transationdetail.SenderNumber;
                                custSender.CustomerCityID = transationdetail.SenderCityID;
                                db.Customers.Add(custSender);
                                db.SaveChanges();
                                transationdetail.SenderID = custSender.CustomerID;
                            }
                            else
                            {
                                transationdetail.SenderID = custSender.CustomerID;
                            }
                        }
                        else
                        {
                            transationdetail.SenderID = uploadVM.SenderID;
                        }

                        #endregion

                        #region Customer Receiver
                        #region City
                        if (uploadVM.ReceiverCityID == 0)
                        {
                            string RCity = string.Empty;
                            RCity = Convert.ToString(uploadVM.ReceiverCity); ;

                            City city = new City();
                            city = db.Cities.FirstOrDefault(m => m.CityName == RCity);
                            if (city == null)
                            {
                                city = new City();
                                city.CityName = RCity;
                                city.CreateDate = CurrentDate;
                                city.Flag = "A";
                                db.Cities.Add(city);
                                db.SaveChanges();
                                transationdetail.ReceiverCityID = city.CityID;
                            }
                            else
                            {
                                transationdetail.ReceiverCityID = city.CityID;
                            }
                        }
                        else
                        {
                            transationdetail.ReceiverCityID = uploadVM.ReceiverCityID;
                        }
                        #endregion
                        transationdetail.ReceiverName = Convert.ToString(uploadVM.ReceiverName);
                        transationdetail.ReceiverNumber = Convert.ToString(uploadVM.ReceiverNumber);

                        if (uploadVM.ReceiverID == 0)
                        {
                            Customer custRecever = new Customer();
                            custRecever = db.Customers.FirstOrDefault(m => m.CustomerName == transationdetail.ReceiverName && m.CustomerNumber == transationdetail.ReceiverNumber);

                            if (custRecever == null)
                            {
                                custRecever = new Customer();
                                custRecever.CustomerName = transationdetail.ReceiverName;
                                custRecever.CustomerNumber = transationdetail.ReceiverNumber;
                                custRecever.CustomerCityID = transationdetail.ReceiverCityID;
                                db.Customers.Add(custRecever);
                                db.SaveChanges();
                                transationdetail.ReceiverID = custRecever.CustomerID;
                            }
                            else
                            {
                                transationdetail.ReceiverID = custRecever.CustomerID;
                            }
                        }
                        else
                        {
                            transationdetail.ReceiverID = uploadVM.ReceiverID;
                        }
                        #endregion


                        if (uploadVM.PayType == 0)
                        {
                            transationdetail.PayType = (int)JalaramTravels.Models.Enums.PayTypes.Paid;
                        }
                        else
                        {
                            transationdetail.PayType = (int)JalaramTravels.Models.Enums.PayTypes.ToPay;
                        }

                        transationdetail.PaymentType = (int)JalaramTravels.Models.Enums.PaymentTypes.None;
                        string Cartage = Convert.ToString(uploadVM.Cartage);
                        transationdetail.Cartage = Cartage == null ? 0 : Convert.ToDecimal(Cartage);

                        transationdetail.TransactionDate = uploadVM.TransactionDate;
                        transationdetail.CreateDate = GetCurrentSession.CurrentDateTime();
                        transationdetail.CreateUser = (int)GetCurrentSession.CurrentUser();
                        transationdetail.Flag = "A";
                        transationdetail.BusID = uploadVM.BusID;
                        transationdetail.DriverName = uploadVM.DriverName;


                        transationdetail.DeliverdStatus = (int)uploadVM.DeliverdStatus;

                        if (transationdetail.DeliverdStatus == (int)JalaramTravels.Models.Enums.DeliverdStatus.InTransit)
                        {
                            transationdetail.PickUpBoyID = uploadVM.PickUpBoyID;
                            transationdetail.PickUpDate = uploadVM.PickUpDate;
                        }
                        if (transationdetail.DeliverdStatus == (int)JalaramTravels.Models.Enums.DeliverdStatus.Delivered)
                        {
                            transationdetail.DeliveryDate = uploadVM.DeliveryDate;
                        }

                        if (uploadVM.DeliveryByCustomer)
                        {
                            transationdetail.DeliveryByCustomer = true;
                            transationdetail.ReceiverDetails = uploadVM.ReceiverDetails;
                        }
                        transationdetail.Hamali = uploadVM.Hamali;
                        transationdetail.Damrage = uploadVM.Damrage;

                        transationdetail.ParcelTypeID = uploadVM.ParcelTypeID;
                        transationdetail.ParcelContainerID = uploadVM.ParcelContainerID;
                        if (uploadVM.TransactionDetailID == 0)
                        {
                            db.TransactionDetails.Add(transationdetail);
                            var last = db.TransactionDetails.OrderByDescending(t => t.TransactionDetailID).FirstOrDefault();
                            if (last != null)
                            {
                                response.Url = last.TransactionDetailID.ToString();
                            }
                        }
                        else
                        {
                            db.Entry(transationdetail).State = System.Data.Entity.EntityState.Modified;
                            response.Url = transationdetail.TransactionDetailID.ToString();
                        }
                           db.SaveChanges();
                        
                       

                        var TotalTopaid = db.TransactionDetails.ToList().Where(t => t.TransactionMasterID == TransactionMasterID && t.PayType == (int)JalaramTravels.Models.Enums.PayTypes.ToPay).Sum(t => t.Amount);
                        var TotalPaid = db.TransactionDetails.ToList().Where(t => t.TransactionMasterID == TransactionMasterID && t.PayType == (int)JalaramTravels.Models.Enums.PayTypes.Paid).Sum(t => t.Amount);
                        var TotalCartage = db.TransactionDetails.ToList().Where(t => t.TransactionMasterID == TransactionMasterID).Sum(t => t.Cartage);
                        var TotalDamrage = db.TransactionDetails.ToList().Where(t => t.TransactionMasterID == TransactionMasterID).Sum(t => t.Damrage);
                        var NoOfParcel1 = db.TransactionDetails.ToList().Where(t => t.TransactionMasterID == TransactionMasterID).Sum(t => t.NoOfParcel);

                        TransactionMaster transactionMaster1 = new TransactionMaster();
                        transactionMaster1 = db.TransactionMasters.FirstOrDefault(t => t.TransactionMasterID == TransactionMasterID);

                        if (transactionMaster1 != null)
                        {
                            transactionMaster1.TotalTopay = TotalTopaid;
                            transactionMaster1.TotalPaid = TotalPaid;
                            transactionMaster1.TotalCartage = TotalCartage;
                            transactionMaster1.TotalDamrage = TotalDamrage;
                            transactionMaster1.TotalParcel = NoOfParcel1;
                            db.Entry(transactionMaster1).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }

                        #region DeliverdStatus Delivered
                        if (uploadVM.DeliverdStatus == Enums.DeliverdStatus.Delivered)
                        {
                            #region Print Using CR
                            //DataTable dtSlip = new DataTable();
                            //dtSlip.Columns.Add("LRNo", typeof(string));
                            //dtSlip.Columns.Add("CustomerName", typeof(string));
                            //dtSlip.Columns.Add("From", typeof(string));
                            //dtSlip.Columns.Add("Amount", typeof(decimal));
                            //dtSlip.Columns.Add("Damrage", typeof(decimal));
                            //dtSlip.Columns.Add("Hamali", typeof(decimal));
                            //dtSlip.Columns.Add("Total", typeof(decimal));
                            //dtSlip.Columns.Add("NoOfParcel", typeof(int));
                            //dtSlip.Columns.Add("PayTypeStatus", typeof(string));

                            //var PayTypevar = uploadVM.PayType;
                            //string PayTypeString = Enum.GetName(typeof(JalaramTravels.Models.Enums.PayTypes), PayTypevar);

                            //decimal Total = (decimal)uploadVM.Amount + (decimal)uploadVM.Damrage + (decimal)uploadVM.Cartage + ((decimal)uploadVM.Hamali * (int)uploadVM.NoOfParcel);
                            //string SenderCity = string.Empty;
                            //var CityData = db.Cities.FirstOrDefault(t => t.CityID == uploadVM.SenderCityID);
                            //if (CityData != null)
                            //{
                            //    SenderCity = CityData.CityName;
                            //}
                            //dtSlip.Rows.Add(uploadVM.LRNo, uploadVM.ReceiverName, SenderCity, uploadVM.Amount, uploadVM.Damrage, uploadVM.Hamali, Total, uploadVM.NoOfParcel, PayTypeString);

                            //DataSet1 myds = new DataSet1();
                            //myds.Tables["DtSlip"].Merge(dtSlip);


                            //ReportDocument rptH = new ReportDocument();
                            //rptH.Load(Server.MapPath("~/Reports/rptSlip.rpt"));
                            //rptH.SetDataSource(myds);
                            //Response.Buffer = false;
                            //Response.ClearContent();
                            //Response.ClearHeaders();

                            //string htmlfilename = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                            //string path = Server.MapPath("~/PDF");
                            //AllClass.CreateDirectory(path);
                            //string fileName = path + "/" + htmlfilename + ".pdf";

                            //string fileName2 = htmlfilename + ".pdf";
                            //if (!Directory.Exists(path))
                            //{
                            //    Directory.CreateDirectory(path);
                            //}
                            //rptH.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, fileName);
                            //scopelevel.Complete();
                            //JsonResult result = new JsonResult();
                            //result.Data = fileName2;
                            //return result; 
                            #endregion

                            scopelevel.Complete();
                            //JsonResult result = new JsonResult();
                            //result.Data = true;
                            //return result;
                            response.IsError = false;
                        }
                        else
                        {
                            scopelevel.Complete();
                            //JsonResult result = new JsonResult();
                            //result.Data = true;
                            //return result;
                            //response.Result = true;
                            //response.Status = HttpStatusCode.OK;
                            response.IsError = false;
                        }
                        #endregion
                        
                        //response.Result = false;
                        //response.Status = HttpStatusCode.OK;
                    }
                    catch (Exception ex)
                    {
                        response.IsError = true;
                        //response.Result = false;
                        //response.Status = HttpStatusCode.OK;
                        //return null;
                        //JsonResult result = new JsonResult();
                        //result.Data = ex.InnerException;
                        //return result;
                    }
                    finally
                    {
                        scopelevel.Dispose();
                    }
                }
            }
            // return Json(true, JsonRequestBehavior.AllowGet);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TransactionPrint(int ID)
        {
            TransactionVM transactionVM = new TransactionVM();
            TransactionDetail transationdetail = db.TransactionDetails.FirstOrDefault(t => t.TransactionDetailID == ID);
            if (transationdetail != null)
            {
                transactionVM.LRNo = transationdetail.LRNo;
                transactionVM.SenderName = transationdetail.SenderName;
                transactionVM.ReceiverName = transationdetail.ReceiverName;

                string SenderCity = string.Empty;
                if (transationdetail.SenderCityID.HasValue)
                {
                    var CityData = db.Cities.FirstOrDefault(t => t.CityID == transationdetail.SenderCityID);
                    if (CityData != null)
                    { 
                        transactionVM.SenderCity = CityData.CityName;
                    }
                   
                }

                string RCity = string.Empty;
                if (transationdetail.ReceiverCityID.HasValue)
                {
                    var CityData = db.Cities.FirstOrDefault(t => t.CityID == transationdetail.ReceiverCityID);
                    if (CityData != null)
                    {
                        transactionVM.ReceiverCity = CityData.CityName;
                    }
                   
                }

                transactionVM.NoOfParcel = transationdetail.NoOfParcel;
                transactionVM.Damrage = transationdetail.Damrage;
                transactionVM.Hamali = transationdetail.Hamali;

                transactionVM.Amount = transationdetail.Amount;
                transactionVM.FinalAmount = AllClass.GetFinalAmout(transationdetail.Amount, transationdetail.Damrage, transationdetail.Hamali, transationdetail.NoOfParcel, transationdetail.PayType);



                var PayTypevar = transationdetail.PayType;
                string PayTypeString = Enum.GetName(typeof(JalaramTravels.Models.Enums.PayTypes), PayTypevar);
                transactionVM.PayTypeString = PayTypeString;

                if (transationdetail.PayType == (int)JalaramTravels.Models.Enums.PayTypes.ToPay)
                {
                    transactionVM.PayTypeString = transactionVM.PayTypeString+ " Amount: "+ transationdetail.Amount.ToString();
                }
            }
            Company CompanyDetails = db.Companies.FirstOrDefault();

            transactionVM.Cmp = new CompnayDetail();
            if (CompanyDetails != null)
            {
                transactionVM.Cmp.CompnayName = CompanyDetails.CompnayName;
                transactionVM.Cmp.CompnayAddress = CompanyDetails.CompnayAddress;
                transactionVM.Cmp.CompnayNumber = CompanyDetails.CompnayNumber;
                transactionVM.Cmp.CompnayEmail = CompanyDetails.CompnayEmail;
            }
            DateTime CurrentDate = new DateTime();
            CurrentDate = GetCurrentSession.CurrentDateTime();
            transactionVM.Cmp.PrintDate = CurrentDate;
            return View(transactionVM);
        }

        public ActionResult Transaction(Int64? ID)
        {
            int CurrentLogin = (int)GetCurrentSession.CurrentUser();

            TransactionVM transationDetailVM = new TransactionVM();

            var LoginResult = db.Logins.FirstOrDefault(t => t.LoginID == CurrentLogin);
            if (LoginResult != null)
            {
                transationDetailVM.RoleID = (int)LoginResult.RoleID;
            }

            transationDetailVM.TransactionDate = GetCurrentSession.CurrentDateTime();
            transationDetailVM.PickUpDate = GetCurrentSession.CurrentDateTime();
            transationDetailVM.DeliveryDate = GetCurrentSession.CurrentDateTime();

            var busActiveList = db.Buses.ToList().Where(m => m.Flag.Equals("A")).OrderBy(m => m.BusName);
            var cityActiveList = db.Cities.ToList().Where(m => m.Flag.Equals("A")).OrderBy(m => m.CityName);
            var pickupboyActiveList = db.PickUpBoys.ToList().Where(m => m.Flag.Equals("A")).OrderBy(m => m.PickUpBoyName);
            List<BusVM> buslist = new List<BusVM>();
            transationDetailVM.BusList = busActiveList.Select(s => new SelectListItem() { Text = s.BusName + "-" + s.BusNumber, Value = s.BusID.ToString() });
            transationDetailVM.SenderCityList = cityActiveList.Select(s => new SelectListItem() { Text = s.CityName, Value = s.CityID.ToString() });
            transationDetailVM.ReceiverCityList = cityActiveList.Select(s => new SelectListItem() { Text = s.CityName, Value = s.CityID.ToString() });
            transationDetailVM.PickUpBoyList = pickupboyActiveList.Select(s => new SelectListItem() { Text = s.PickUpBoyName, Value = s.PickUpBoyID.ToString() });

            var ParcelTypeMasterList = db.ParcelTypeMasters.ToList();
            transationDetailVM.ParcelTypeList = ParcelTypeMasterList.Select(s => new SelectListItem() { Text = s.ParcelTypeName, Value = s.ParcelTypeID.ToString() });

            var ParcelContainerMasterList = db.ParcelContainerMasters.ToList();
            transationDetailVM.ParcelContainerList = ParcelContainerMasterList.Select(s => new SelectListItem() { Text = s.ParcelContainerName, Value = s.ParcelContainerID.ToString() });

            var transationDetail1 = db.TransactionDetails.FirstOrDefault(m => m.TransactionDetailID == ID);

            if (transationDetail1 != null)
            {
                transationDetailVM.TransactionDetailID = (Int64)transationDetail1.TransactionDetailID;
                transationDetailVM.TransactionMasterID = (Int64)transationDetail1.TransactionMasterID;
                transationDetailVM.BusID = (int)transationDetail1.BusID;
                transationDetailVM.LRNo = transationDetail1.LRNo;
                transationDetailVM.NoOfParcel = (int)transationDetail1.NoOfParcel;
                transationDetailVM.SenderName = transationDetail1.SenderName;
                transationDetailVM.SenderNumber = transationDetail1.SenderNumber;
                transationDetailVM.SenderCityID = transationDetail1.SenderCityID.HasValue ? (Int64)transationDetail1.SenderCityID.Value : 0;
                transationDetailVM.ReceiverName = transationDetail1.ReceiverName;
                transationDetailVM.ReceiverNumber = transationDetail1.ReceiverNumber;
                transationDetailVM.ReceiverCityID = (Int64)transationDetail1.ReceiverCityID;
                transationDetailVM.ReceiverID = (Int64)transationDetail1.ReceiverID;
                transationDetailVM.Amount = (decimal)transationDetail1.Amount;
                transationDetailVM.Cartage = transationDetail1.Cartage.HasValue ? (decimal)transationDetail1.Cartage.Value : 0;
                transationDetailVM.Hamali = transationDetail1.Hamali.HasValue ? (decimal)transationDetail1.Hamali.Value : 0;
                transationDetailVM.Damrage = transationDetail1.Damrage.HasValue ? (decimal)transationDetail1.Damrage.Value : 0;

                transationDetailVM.ParcelTypeID = transationDetail1.ParcelTypeID;
                transationDetailVM.ParcelContainerID = transationDetail1.ParcelContainerID;
                if (transationDetail1.DeliveryDate.HasValue)
                {
                    transationDetailVM.DeliveryDate = (DateTime)transationDetail1.DeliveryDate.Value;
                }

                transationDetailVM.DriverName = transationDetail1.DriverName;

                if (transationDetail1.DeliverdStatus == (int)JalaramTravels.Models.Enums.DeliverdStatus.InTransit)
                {
                    if (transationDetail1.PickUpBoyID.HasValue)
                    {
                        var Boy = db.PickUpBoys.FirstOrDefault(t => t.PickUpBoyID == transationDetail1.PickUpBoyID.Value);
                        transationDetailVM.PickUpBy = Boy.PickUpBoyName;
                        transationDetailVM.PickUpBoyID = (int)transationDetail1.PickUpBoyID;

                    }
                    if (transationDetail1.PickUpDate.HasValue)
                    {
                        transationDetailVM.PickUpDateS = transationDetail1.PickUpDate.Value.ToString("dd/MM/yyyy");
                        transationDetailVM.PickUpDate = transationDetail1.PickUpDate;
                    }
                }
                if (transationDetail1.DeliverdStatus == (int)JalaramTravels.Models.Enums.DeliverdStatus.Delivered)
                {
                    if (transationDetail1.DeliveryDate.HasValue)
                    {
                        transationDetailVM.DeliveryDateS = transationDetail1.DeliveryDate.Value.ToString("dd/MM/yyyy");
                        transationDetailVM.DeliveryDate = transationDetail1.DeliveryDate;
                    }
                }
                transationDetailVM.TransactionDate = transationDetail1.TransactionDate;

                transationDetailVM.PaymentType = (int)JalaramTravels.Models.Enums.PaymentTypes.None;

                transationDetailVM.PayType = (JalaramTravels.Models.Enums.PayTypes)Enum.ToObject(typeof(JalaramTravels.Models.Enums.PayTypes), transationDetail1.PayType);
                transationDetailVM.DeliverdStatus = (JalaramTravels.Models.Enums.DeliverdStatus)Enum.ToObject(typeof(JalaramTravels.Models.Enums.DeliverdStatus), transationDetail1.DeliverdStatus);

                transationDetailVM.DeliveryByCustomer = transationDetail1.DeliveryByCustomer;
                transationDetailVM.ReceiverDetails = transationDetail1.ReceiverDetails;
            }
            else
            {
                var hamali = db.HamaliMasters.OrderByDescending(t => t.ID).FirstOrDefault();
                if (hamali != null)
                {
                    transationDetailVM.Hamali = hamali.Hamali;
                }

                var damrage = db.DamrageMasters.OrderByDescending(t => t.ID).FirstOrDefault();
                if (damrage != null)
                {
                    transationDetailVM.Damrage = damrage.Damrage;
                }
                transationDetailVM.ParcelContainerID = 0;
            }
            return View(transationDetailVM);
        }

        [HttpPost]
        public async Task<JsonResult> TransactionDetail(Int64 ID)
        {
            TransactionVM transationDetailVM = new TransactionVM();
            var busActiveList = await Task.Run(() => db.Buses.ToList().OrderBy(m => m.BusName));
            var cityActiveList = await Task.Run(() => db.Cities.ToList().OrderBy(m => m.CityName));
            var pickupboyActiveList = db.PickUpBoys.ToList().OrderBy(m => m.PickUpBoyName);
            var LoginList = await Task.Run(() => db.Logins.ToList());
            var ParcelTypeMasterList = await Task.Run(() => db.ParcelTypeMasters.ToList());
            var ParcelContainerMasterList = await Task.Run(() => db.ParcelContainerMasters.ToList());

            var transationDetail1 = await Task.Run(() => db.TransactionDetails.FirstOrDefault(m => m.TransactionDetailID == ID));

            if (transationDetail1 != null)
            {
                transationDetailVM.BusID = (int)transationDetail1.BusID;

                var bus = await Task.Run(() => busActiveList.FirstOrDefault(t => t.BusID == transationDetail1.BusID));
                if (bus != null)
                {
                    transationDetailVM.BusNumber = bus.BusName + " " + bus.BusNumber;
                }
                transationDetailVM.TransactionDetailID = (Int64)transationDetail1.TransactionDetailID;
                transationDetailVM.TransactionMasterID = (Int64)transationDetail1.TransactionMasterID;
                transationDetailVM.LRNo = transationDetail1.LRNo;
                transationDetailVM.NoOfParcel = (int)transationDetail1.NoOfParcel;
                transationDetailVM.SenderName = transationDetail1.SenderName;
                transationDetailVM.SenderNumber = transationDetail1.SenderNumber;
                transationDetailVM.SenderCityID = transationDetail1.SenderCityID.HasValue ? (Int64)transationDetail1.SenderCityID.Value : 0;
                if (transationDetailVM.SenderCityID != 0)
                {
                    var city = await Task.Run(() => cityActiveList.FirstOrDefault(t => t.CityID == transationDetailVM.SenderCityID));
                    if (city != null)
                    {
                        transationDetailVM.SenderCity = city.CityName;
                    }
                }
                transationDetailVM.ReceiverName = transationDetail1.ReceiverName;
                transationDetailVM.ReceiverNumber = transationDetail1.ReceiverNumber;
                transationDetailVM.ReceiverCityID = (Int64)transationDetail1.ReceiverCityID;
                if (transationDetailVM.ReceiverCityID != 0)
                {
                    var city = await Task.Run(() => cityActiveList.FirstOrDefault(t => t.CityID == transationDetailVM.ReceiverCityID));
                    if (city != null)
                    {
                        transationDetailVM.ReceiverCity = city.CityName;
                    }
                }
                transationDetailVM.ReceiverID = (Int64)transationDetail1.ReceiverID;
                transationDetailVM.Amount = transationDetail1.Amount;
                transationDetailVM.Cartage = transationDetail1.Cartage.HasValue ? (decimal)transationDetail1.Cartage.Value : 0;
                transationDetailVM.Hamali = transationDetail1.Hamali.HasValue ? (decimal)transationDetail1.Hamali.Value : 0;
                transationDetailVM.Damrage = transationDetail1.Damrage.HasValue ? (decimal)transationDetail1.Damrage.Value : 0;

                if (transationDetail1.TransactionDate.HasValue)
                {
                    transationDetailVM.TransactionDateS = transationDetail1.TransactionDate.Value.ToString("dd/MM/yyyy");
                }

                if (transationDetail1.DeliveryDate.HasValue)
                {
                    transationDetailVM.DeliveryDate = (DateTime)transationDetail1.DeliveryDate.Value;
                }

                transationDetailVM.DriverName = transationDetail1.DriverName;

                if (transationDetail1.PickUpBoyID.HasValue)
                {
                    var Boy = await Task.Run(() => db.PickUpBoys.FirstOrDefault(t => t.PickUpBoyID == transationDetail1.PickUpBoyID.Value));
                    transationDetailVM.PickUpBy = Boy.PickUpBoyName;
                    transationDetailVM.PickUpBoyID = (int)transationDetail1.PickUpBoyID;

                }
                if (transationDetail1.PickUpDate.HasValue)
                {
                    transationDetailVM.PickUpDateS = transationDetail1.PickUpDate.Value.ToString("dd/MM/yyyy");
                }

                if (transationDetail1.DeliveryDate.HasValue)
                {
                    transationDetailVM.DeliveryDateS = transationDetail1.DeliveryDate.Value.ToString("dd/MM/yyyy"); ;
                }

                transationDetailVM.TransactionDate = transationDetail1.TransactionDate;

                transationDetailVM.PaymentType = (int)JalaramTravels.Models.Enums.PaymentTypes.None;

                transationDetailVM.PayType = (JalaramTravels.Models.Enums.PayTypes)Enum.ToObject(typeof(JalaramTravels.Models.Enums.PayTypes), transationDetail1.PayType);
                transationDetailVM.DeliverdStatus = (JalaramTravels.Models.Enums.DeliverdStatus)Enum.ToObject(typeof(JalaramTravels.Models.Enums.DeliverdStatus), transationDetail1.DeliverdStatus);

                 

                //Enum to String name
                var DeliverdStatusVar = transationDetail1.DeliverdStatus;
                transationDetailVM.DeliverdStatusString = Enum.GetName(typeof(JalaramTravels.Models.Enums.DeliverdStatus), DeliverdStatusVar);

                var PayTypevar = transationDetail1.PayType;
                transationDetailVM.PayTypeString = Enum.GetName(typeof(JalaramTravels.Models.Enums.PayTypes), PayTypevar);

                var PaymentTypesvar = transationDetail1.PaymentType;
                transationDetailVM.PaymentTypesString = Enum.GetName(typeof(JalaramTravels.Models.Enums.PaymentTypes), PaymentTypesvar);


                if (transationDetail1.CreateUser != null)
                {
                    var login = await Task.Run(() => LoginList.FirstOrDefault(t => t.LoginID == transationDetail1.CreateUser));
                    if (login != null)
                    {
                        transationDetailVM.CreateUserS = login.FirstName+ " "+ login.LastName;
                    }
                }

                if (transationDetail1.ParcelTypeID != 0)
                {
                    var par = await Task.Run(() => ParcelTypeMasterList.FirstOrDefault(t => t.ParcelTypeID == transationDetail1.ParcelTypeID));
                    if (par != null)
                    {
                        transationDetailVM.ParcelTypeS = par.ParcelTypeName;
                        transationDetailVM.ParcelTypeID = transationDetail1.ParcelTypeID;
                    }
                }
                if (transationDetail1.ParcelContainerID != 0)
                {
                    var par = await Task.Run(() => ParcelContainerMasterList.FirstOrDefault(t => t.ParcelContainerID == transationDetail1.ParcelContainerID));
                    if (par != null)
                    {
                        transationDetailVM.ParcelContainerS = par.ParcelContainerName;
                        transationDetailVM.ParcelContainerID = transationDetail1.ParcelContainerID;
                    }
                }

            }
            return Json(transationDetailVM, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Undelivered()
        {
            var transationList = await Task.Run(() => db.TransactionDetails.Where(m => m.DeliverdStatus == (int)JalaramTravels.Models.Enums.DeliverdStatus.Undelivered).ToList());

            TransactionDetailVM transationsObj = new TransactionDetailVM();
            List<TransactionDetailsVM> transations = new List<TransactionDetailsVM>();
            transationsObj.TransactionDate = GetCurrentSession.CurrentDateTime();

            var busActiveList = await Task.Run(() => db.Buses.ToList().OrderBy(m => m.BusName));
            transationsObj.BusList = await Task.Run(() => busActiveList.Where(m => m.Flag.Equals("A")).Select(s => new SelectListItem() { Text = s.BusName + "-" + s.BusNumber, Value = s.BusID.ToString() }));


            var PickUpBoyListActiveList = await Task.Run(() => db.PickUpBoys.ToList().OrderBy(m => m.PickUpBoyName));
            transationsObj.PickUpBoyList = await Task.Run(() => PickUpBoyListActiveList.Where(m => m.Flag.Equals("A")).Select(s => new SelectListItem() { Text = s.PickUpBoyName, Value = s.PickUpBoyID.ToString() }));

            var TransactionMastersData = db.TransactionMasters.ToList();

            foreach (var item in transationList)
            {
                TransactionDetailsVM obj = new TransactionDetailsVM();
                obj.TransactionDetailID = (Int64)item.TransactionDetailID;
                obj.LRNo = item.LRNo;
                obj.NoOfParcel = (int)item.NoOfParcel;
                obj.SenderName = item.SenderName;
                obj.ReceiverName = item.ReceiverName;
                obj.Amount = (decimal)item.Amount;
                obj.Cartage = (decimal)item.Cartage;

                obj.PaymentType = (int)JalaramTravels.Models.Enums.PaymentTypes.None;
                obj.PayType = (JalaramTravels.Models.Enums.PayTypes)Enum.ToObject(typeof(JalaramTravels.Models.Enums.PayTypes), item.PayType);
                obj.DeliverdStatus = (JalaramTravels.Models.Enums.DeliverdStatus)Enum.ToObject(typeof(JalaramTravels.Models.Enums.DeliverdStatus), item.DeliverdStatus);

                var PayTypevar = item.PayType;
                obj.PayTypeString = Enum.GetName(typeof(JalaramTravels.Models.Enums.PayTypes), PayTypevar);

                var busD = await Task.Run(() => busActiveList.FirstOrDefault(t => t.BusID == item.BusID));
                if (busD != null)
                {
                    obj.BusName = busD.BusName;
                    obj.BusNumber = busD.BusNumber;
                }
                if (item.TransactionDate.HasValue)
                {
                    obj.TransactionDateS = item.TransactionDate.Value.ToString("dd/MM/yyyy");
                }
                transations.Add(obj);
            }
            transationsObj.TransactionDetailList = transations;

            return View(transationsObj);
        }

        [HttpPost]
        public async Task<JsonResult> UndeliveredFilter(TransactionDetailFiler transactionDetailFiler)
        {
            var transationList = await Task.Run(() => db.TransactionDetails.Where(m => m.DeliverdStatus == (int)JalaramTravels.Models.Enums.DeliverdStatus.Undelivered && m.BusID == transactionDetailFiler.BusID && m.TransactionDate == transactionDetailFiler.TransactionDate).ToList());

            TransactionDetailVM transationsObj = new TransactionDetailVM();
            List<TransactionDetailsVM> transations = new List<TransactionDetailsVM>();

            var busActiveList = await Task.Run(() => db.Buses.ToList().OrderBy(m => m.BusName));
            transationsObj.BusList = await Task.Run(() => busActiveList.Where(m => m.Flag.Equals("A")).Select(s => new SelectListItem() { Text = s.BusName + "-" + s.BusNumber, Value = s.BusID.ToString() }));


            var PickUpBoyListActiveList = await Task.Run(() => db.PickUpBoys.ToList().OrderBy(m => m.PickUpBoyName));
            transationsObj.PickUpBoyList = await Task.Run(() => PickUpBoyListActiveList.Where(m => m.Flag.Equals("A")).Select(s => new SelectListItem() { Text = s.PickUpBoyName, Value = s.PickUpBoyID.ToString() }));

            var TransactionMastersData = db.TransactionMasters.ToList();

            foreach (var item in transationList)
            {
                TransactionDetailsVM obj = new TransactionDetailsVM();
                obj.TransactionDetailID = (Int64)item.TransactionDetailID;
                obj.LRNo = item.LRNo;
                obj.NoOfParcel = (int)item.NoOfParcel;
                obj.SenderName = item.SenderName;
                obj.ReceiverName = item.ReceiverName;
                obj.Amount = (decimal)item.Amount;
                obj.Cartage = (decimal)item.Cartage;

                obj.PaymentType = (int)JalaramTravels.Models.Enums.PaymentTypes.None;
                obj.PayType = (JalaramTravels.Models.Enums.PayTypes)Enum.ToObject(typeof(JalaramTravels.Models.Enums.PayTypes), item.PayType);
                obj.DeliverdStatus = (JalaramTravels.Models.Enums.DeliverdStatus)Enum.ToObject(typeof(JalaramTravels.Models.Enums.DeliverdStatus), item.DeliverdStatus);

                var PayTypevar = item.PayType;
                obj.PayTypeString = Enum.GetName(typeof(JalaramTravels.Models.Enums.PayTypes), PayTypevar);

                var busD = await Task.Run(() => busActiveList.FirstOrDefault(t => t.BusID == item.BusID));
                if (busD != null)
                {
                    obj.BusName = busD.BusName;
                    obj.BusNumber = busD.BusNumber;
                }
                if (item.TransactionDate.HasValue)
                {
                    obj.TransactionDateS = item.TransactionDate.Value.ToString("dd/MM/yyyy");
                }
                transations.Add(obj);
            }
            transationsObj.TransactionDetailList = transations;

            return Json(transations, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> PaymentToPay()
        {
            List<ToPayVM> toPayVM = new List<ToPayVM>();

            var TransactionMasters1 = await Task.Run(() => db.TransactionMasters.ToList());
            var busActiveList = await Task.Run(() => db.Buses.ToList().OrderBy(m => m.BusName));
            foreach (var item in TransactionMasters1)
            {
                ToPayVM ToPayVMObj = new ToPayVM();
                ToPayVMObj.TransactionMasterID = (Int64)item.TransactionMasterID;
                ToPayVMObj.BusID = (int)item.BusID;
                if (item.BusID.HasValue)
                {
                    var busD = await Task.Run(() => busActiveList.FirstOrDefault(t => t.BusID == item.BusID.Value));
                    if (busD != null)
                    {
                        ToPayVMObj.BusName = busD.BusName;
                        ToPayVMObj.BusNumber = busD.BusNumber;
                    }
                }
                ToPayVMObj.TotalTopay = (decimal)item.TotalTopay;
                ToPayVMObj.TotalPaid = (decimal)item.TotalPaid;
                ToPayVMObj.TotalCartage = (decimal)item.TotalCartage;
                ToPayVMObj.TotalDamrage = (decimal)item.TotalDamrage;
                ToPayVMObj.TransactionStatus = (JalaramTravels.Models.Enums.TransactionStatus)Enum.ToObject(typeof(JalaramTravels.Models.Enums.TransactionStatus), item.TransactionStatus);
                if (item.TransactionDate.HasValue)
                {
                    ToPayVMObj.TransactionDateS = item.TransactionDate.Value.ToString("dd/MM/yyyy");
                }

                toPayVM.Add(ToPayVMObj);
            }

            return View(toPayVM);
        }

        public async Task<ActionResult> Payment(Int64 ID)
        {
            TransactionPaymentVM transactionPaymentVM = new TransactionPaymentVM();
            var busActiveList = await Task.Run(() => db.Buses.ToList().OrderBy(m => m.BusName));
            List<BusVM> buslist = new List<BusVM>();
            transactionPaymentVM.BusList = await Task.Run(() => busActiveList.Where(m => m.Flag.Equals("A")).Select(s => new SelectListItem() { Text = s.BusName, Value = s.BusID.ToString() }));

            var data = await Task.Run(() => db.TransactionMasters.FirstOrDefault(t => t.TransactionMasterID == ID));
            if (data != null)
            {
                transactionPaymentVM.TransactionMasterID = data.TransactionMasterID;
                transactionPaymentVM.TotalParcel = (int)data.TotalParcel;
                transactionPaymentVM.TotalTopay = (decimal)data.TotalTopay;
                transactionPaymentVM.TotalPaid = (decimal)data.TotalPaid;
                transactionPaymentVM.TotalCartage = (decimal)data.TotalCartage;
                transactionPaymentVM.TotalDamrage = (decimal)data.TotalDamrage;
                transactionPaymentVM.TopayDate = GetCurrentSession.CurrentDateTime();
            }
            return View(transactionPaymentVM);
        }

        [HttpPost]
        public async Task<JsonResult> PaymentPaymentPost(TransactionPaymentVM transactionPaymentVM)
        {
            DateTime CurrentDate = new DateTime();
            CurrentDate = GetCurrentSession.CurrentDateTime();
            try
            {
                var data = await Task.Run(() => db.TransactionMasters.FirstOrDefault(t => t.TransactionMasterID == transactionPaymentVM.TransactionMasterID));

                if (data != null)
                {
                    data.TopayDriverName = transactionPaymentVM.TopayDriverName;
                    data.TopayBusID = transactionPaymentVM.BusID;
                    data.TopayDate = transactionPaymentVM.TopayDate;
                    data.PaymentReceiveDate = transactionPaymentVM.TopayDate;
                    data.TransactionStatus = (int)JalaramTravels.Models.Enums.TransactionStatus.Completed;
                    data.PaymentReceiveDate = transactionPaymentVM.TopayDate;
                    data.PaymentReceiverBy = (int)GetCurrentSession.CurrentUser();
                    await Task.Run(() => db.Entry(data).State = System.Data.Entity.EntityState.Modified);
                    await Task.Run(() => db.SaveChanges());
                }
            }
            catch (Exception ex)
            {
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> InTransit()
        {
            InTransitVM inTransitVM = new InTransitVM();
            inTransitVM.DeliverdStatus = Enums.DeliverdStatus.Delivered;
            List<TransactionDetailsVM> toPayVM = new List<TransactionDetailsVM>();

            inTransitVM.PickUpDate = GetCurrentSession.CurrentDateTime();

            var LoginList = await Task.Run(() => db.Logins.ToList());

            var PickUpBoysList = await Task.Run(() => db.PickUpBoys.ToList());

            var PickUpBoyList = await Task.Run(() => db.PickUpBoys.ToList().OrderBy(m => m.PickUpBoyName));
            inTransitVM.PickUpBoyList = await Task.Run(() => PickUpBoyList.Where(m => m.Flag.Equals("A")).Select(s => new SelectListItem() { Text = s.PickUpBoyName, Value = s.PickUpBoyID.ToString() }));

            var TransactionMasters1 = await Task.Run(() => db.TransactionDetails.ToList().Where(t => t.DeliverdStatus.Value == (int)Enums.DeliverdStatus.InTransit));
            var busActiveList = await Task.Run(() => db.Buses.ToList().OrderBy(m => m.BusName));
            inTransitVM.BusList = await Task.Run(() => busActiveList.Select(s => new SelectListItem() { Text = s.BusName, Value = s.BusID.ToString() }));


            foreach (var item in TransactionMasters1)
            {
                TransactionDetailsVM ToPayVMObj = new TransactionDetailsVM();
                ToPayVMObj.TransactionMasterID = (Int64)item.TransactionMasterID;
                ToPayVMObj.TransactionDetailID = (Int64)item.TransactionDetailID;
                ToPayVMObj.BusID = (int)item.BusID;
                if (item.BusID.HasValue)
                {
                    var busD = await Task.Run(() => busActiveList.FirstOrDefault(t => t.BusID == item.BusID.Value));

                    if (busD != null)
                    {
                        ToPayVMObj.BusName = busD.BusName;
                        ToPayVMObj.BusNumber = busD.BusNumber;
                    }

                }
                ToPayVMObj.LRNo = item.LRNo;
                ToPayVMObj.NoOfParcel = (int)item.NoOfParcel;
                ToPayVMObj.SenderName = item.SenderName;
                ToPayVMObj.SenderCityID = item.SenderCityID.HasValue ? (Int64)item.SenderCityID.Value : 0;
                ToPayVMObj.ReceiverName = item.ReceiverName;
                ToPayVMObj.ReceiverNumber = item.ReceiverNumber;
                ToPayVMObj.ReceiverCityID = (Int64)item.ReceiverCityID;
                ToPayVMObj.ReceiverID = (Int64)item.ReceiverID;
                ToPayVMObj.Amount = item.Amount.HasValue ? (decimal)item.Amount.Value : 0;
                ToPayVMObj.Cartage = item.Cartage.HasValue ? (decimal)item.Cartage.Value : 0;
                ToPayVMObj.Hamali = item.Hamali.HasValue ? (decimal)item.Hamali.Value : 0;
                ToPayVMObj.Damrage = item.Damrage.HasValue ? (decimal)item.Damrage.Value : 0;

                if (item.DeliveryDate.HasValue)
                {
                    ToPayVMObj.DeliveryDate = (DateTime)item.DeliveryDate.Value;
                }
                if (item.TransactionDate.HasValue)
                {
                    ToPayVMObj.TransactionDateS = item.TransactionDate.Value.ToString("dd/MM/yyyy");
                }
                if (item.PickUpDate.HasValue)
                {
                    ToPayVMObj.PickUpDateS = item.PickUpDate.Value.ToString("dd/MM/yyyy");
                }
                if (item.PickUpBoyID != null)
                {
                    var boy = await Task.Run(() => PickUpBoysList.FirstOrDefault(t => t.PickUpBoyID == item.PickUpBoyID));
                    if (PickUpBoysList != null)
                    {
                        ToPayVMObj.PickUpBoy = boy.PickUpBoyName;
                    }
                }
                if (item.PickUpByUserID != null)
                {
                    var login = await Task.Run(() => LoginList.FirstOrDefault(t => t.LoginID == item.PickUpByUserID));
                    if (login != null)
                    {
                        ToPayVMObj.PickUpCreateBy = login.FirstName + " " + login.LastName;
                    }
                }
                ToPayVMObj.DriverName = item.DriverName;
                toPayVM.Add(ToPayVMObj);
                inTransitVM.TransactionDetailList = toPayVM;

            }

            return View(inTransitVM);
        }

        [HttpPost]
        public async Task<JsonResult> InTransitFilter(InTransitVM transactionDetailFiler)
        {
            InTransitVM inTransitVM = new InTransitVM();
            List<TransactionDetailsVM> toPayVM = new List<TransactionDetailsVM>();

            var PickUpBoysList = await Task.Run(() => db.PickUpBoys.ToList());

            var LoginList = await Task.Run(() => db.Logins.ToList());

            var PickUpBoyList = await Task.Run(() => db.PickUpBoys.ToList().OrderBy(m => m.PickUpBoyName));
            inTransitVM.PickUpBoyList = await Task.Run(() => PickUpBoyList.Where(m => m.Flag.Equals("A")).Select(s => new SelectListItem() { Text = s.PickUpBoyName, Value = s.PickUpBoyID.ToString() }));

            var busActiveList = await Task.Run(() => db.Buses.ToList().OrderBy(m => m.BusName));
            inTransitVM.BusList = await Task.Run(() => busActiveList.Select(s => new SelectListItem() { Text = s.BusName, Value = s.BusID.ToString() }));

            int PickUpBoyID = 0;
            if (transactionDetailFiler.PickUpBoyID.HasValue)
            {
                PickUpBoyID = transactionDetailFiler.PickUpBoyID.Value;
            }
            SqlParameter param1 = new SqlParameter("@ToDate", transactionDetailFiler.PickUpDate.Value);
            SqlParameter param2 = new SqlParameter("@PickUpBoyID", PickUpBoyID);
            var TransactionDetails1 = db.Database.SqlQuery<TransactionDetail>("SP_TransationDetails_InTransit @ToDate , @PickUpBoyID", param1, param2).ToList();

            foreach (var item in TransactionDetails1)
            {
                TransactionDetailsVM ToPayVMObj = new TransactionDetailsVM();
                ToPayVMObj.TransactionMasterID = (Int64)item.TransactionMasterID;
                ToPayVMObj.TransactionDetailID = (Int64)item.TransactionDetailID;
                ToPayVMObj.BusID = (int)item.BusID;
                if (item.BusID.HasValue)
                {
                    var busD = busActiveList.FirstOrDefault(t => t.BusID == item.BusID.Value);
                    ToPayVMObj.BusName = busD.BusName;
                    ToPayVMObj.BusNumber = busD.BusNumber;
                }
                ToPayVMObj.LRNo = item.LRNo;
                ToPayVMObj.NoOfParcel = (int)item.NoOfParcel;
                ToPayVMObj.SenderName = item.SenderName;
                ToPayVMObj.SenderCityID = item.SenderCityID.HasValue ? (Int64)item.SenderCityID.Value : 0;
                ToPayVMObj.ReceiverName = item.ReceiverName;
                ToPayVMObj.ReceiverNumber = item.ReceiverNumber;
                ToPayVMObj.ReceiverCityID = (Int64)item.ReceiverCityID;
                ToPayVMObj.ReceiverID = (Int64)item.ReceiverID;
                ToPayVMObj.Amount = item.Amount.HasValue ? (decimal)item.Amount.Value : 0;
                ToPayVMObj.Cartage = item.Cartage.HasValue ? (decimal)item.Cartage.Value : 0;
                ToPayVMObj.Hamali = item.Hamali.HasValue ? (decimal)item.Hamali.Value : 0;
                ToPayVMObj.Damrage = item.Damrage.HasValue ? (decimal)item.Damrage.Value : 0;

                if (item.DeliveryDate.HasValue)
                {
                    ToPayVMObj.DeliveryDate = (DateTime)item.DeliveryDate.Value;
                }
                if (item.TransactionDate.HasValue)
                {
                    ToPayVMObj.TransactionDateS = item.TransactionDate.Value.ToString("dd/MM/yyyy");
                }
                if (item.PickUpDate.HasValue)
                {
                    ToPayVMObj.PickUpDateS = item.PickUpDate.Value.ToString("dd/MM/yyyy");
                }
                if (item.PickUpBoyID != null)
                {
                    var boy = await Task.Run(() => PickUpBoysList.FirstOrDefault(t => t.PickUpBoyID == item.PickUpBoyID));
                    if (PickUpBoysList != null)
                    {
                        ToPayVMObj.PickUpBoy = boy.PickUpBoyName;
                    }
                }
                if (item.PickUpByUserID != null)
                {
                    var login = await Task.Run(() => LoginList.FirstOrDefault(t => t.LoginID == item.PickUpByUserID));
                    if (login != null)
                    {
                        ToPayVMObj.PickUpCreateBy = login.FirstName + " " + login.LastName;
                    }
                }
                ToPayVMObj.DriverName = item.DriverName;
                toPayVM.Add(ToPayVMObj);
                inTransitVM.TransactionDetailList = toPayVM;

            }
            return Json(toPayVM, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> DoorService(string Transactions)
        {

            Int64[] nums = Array.ConvertAll(Transactions.Split(','), Int64.Parse);

            InTransitCharges inTransitCharges = new InTransitCharges();

            inTransitCharges.PickUpDate = GetCurrentSession.CurrentDateTime();
            List<InTransitChargesDetails> Details = new List<InTransitChargesDetails>();
            var ParcelTypeMasterList = await Task.Run(() => db.ParcelTypeMasters.ToList());
            var ParcelContainerMasterList = await Task.Run(() => db.ParcelContainerMasters.ToList());

            foreach (var item in nums)
            {
                var transactionDetail = await Task.Run(() => db.TransactionDetails.FirstOrDefault(t => t.TransactionDetailID == item));
                if (transactionDetail != null)
                {
                    InTransitChargesDetails obj = new InTransitChargesDetails();
                    obj.TransactionDetailID = (Int64)transactionDetail.TransactionDetailID;
                    obj.TransactionMasterID = (Int64)transactionDetail.TransactionMasterID;
                    obj.LRNo = transactionDetail.LRNo;
                    obj.Amount = (decimal)transactionDetail.Amount;
                    obj.NoOfParcel = transactionDetail.NoOfParcel == null ? 0 : (int)transactionDetail.NoOfParcel;

                    obj.ParcelTypeList = ParcelTypeMasterList.Select(s => new SelectListItem() { Text = s.ParcelTypeName, Value = s.ParcelTypeID.ToString() });
                    obj.ParcelContainerList = ParcelContainerMasterList.Select(s => new SelectListItem() { Text = s.ParcelContainerName, Value = s.ParcelContainerID.ToString() });

                    obj.PayType = (JalaramTravels.Models.Enums.PayTypes)Enum.ToObject(typeof(JalaramTravels.Models.Enums.PayTypes), transactionDetail.PayType);
                    obj.ParcelTypeID = transactionDetail.ParcelTypeID;
                    obj.ParcelContainerID = transactionDetail.ParcelContainerID;
                    obj.Hamali = transactionDetail.Hamali == null ? 0 : (decimal)transactionDetail.Hamali;
                    obj.Damrage = transactionDetail.Damrage == null ? 0 : (decimal)transactionDetail.Damrage;
                    Details.Add(obj);
                }
            }
            inTransitCharges.inTransitChargesDetails = Details;
            var TempoActiveList = await Task.Run(() => db.PickUpBoys.ToList().Where(m => m.Flag.Equals("A")).OrderBy(m => m.PickUpBoyName));
            inTransitCharges.PickUpBoyList = await Task.Run(() => TempoActiveList.Select(s => new SelectListItem() { Text = s.PickUpBoyName, Value = s.PickUpBoyID.ToString() }));

            return View(inTransitCharges);
        }

        [HttpPost]
        public JsonResult DoorServicePost(InTransitCharges inTransitCharges)
        {
            JsonResult result = new JsonResult();
            result.Data = false;

            int Tempoid = inTransitCharges.TempoID;
            DateTime CurrentDate = new DateTime();
            if (inTransitCharges.PickUpDate.HasValue)
            {
                CurrentDate = inTransitCharges.PickUpDate.Value;
            }
            else
            {
                CurrentDate = GetCurrentSession.CurrentDateTime();
            }
            var scopelevel = new TransactionScope();

            using (var context = ApplicationDbContext.Create())
            {
                using (scopelevel)
                {
                    #region Process
                    try
                    {
                        foreach (var item in inTransitCharges.inTransitChargesDetails)
                        {
                            var data = db.TransactionDetails.FirstOrDefault(t => t.TransactionDetailID == item.TransactionDetailID);
                            if (data != null)
                            {
                                data.PickUpBoyID = Tempoid;
                                data.PayType = (int)item.PayType;
                                data.Amount = item.Amount;
                                data.Hamali = item.Hamali;
                                data.NoOfParcel = item.NoOfParcel;
                                data.Damrage = item.Damrage;
                                data.PickUpDate = CurrentDate;
                                data.PickUpByUserID = (int)GetCurrentSession.CurrentUser();
                                data.DeliverdStatus = (int)JalaramTravels.Models.Enums.DeliverdStatus.InTransit;
                                db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();

                                var TotalTopaid = db.TransactionDetails.ToList().Where(t => t.TransactionMasterID == item.TransactionMasterID && t.PayType == (int)JalaramTravels.Models.Enums.PayTypes.ToPay).Sum(t => t.Amount);
                                var TotalPaid = db.TransactionDetails.ToList().Where(t => t.TransactionMasterID == item.TransactionMasterID && t.PayType == (int)JalaramTravels.Models.Enums.PayTypes.Paid).Sum(t => t.Amount);
                                var TotalCartage = db.TransactionDetails.ToList().Where(t => t.TransactionMasterID == item.TransactionMasterID).Sum(t => t.Cartage);
                                var TotalDamrage = db.TransactionDetails.ToList().Where(t => t.TransactionMasterID == item.TransactionMasterID).Sum(t => t.Damrage);
                                var NoOfParcel1 = db.TransactionDetails.ToList().Where(t => t.TransactionMasterID == item.TransactionMasterID).Sum(t => t.NoOfParcel);

                                TransactionMaster transactionMaster1 = new TransactionMaster();
                                transactionMaster1 = db.TransactionMasters.FirstOrDefault(t => t.TransactionMasterID == item.TransactionMasterID);

                                if (transactionMaster1 != null)
                                {
                                    transactionMaster1.TotalTopay = TotalTopaid;
                                    transactionMaster1.TotalPaid = TotalPaid;
                                    transactionMaster1.TotalCartage = TotalCartage;
                                    transactionMaster1.TotalDamrage = TotalDamrage;
                                    transactionMaster1.TotalParcel = NoOfParcel1;
                                    db.Entry(transactionMaster1).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
                        scopelevel.Complete();
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                    finally
                    {
                        scopelevel.Dispose();
                    }
                    #endregion

                    result.Data = true;
                    return result;
                    #region Print using For CR
                    //try
                    //{
                    //    string joindata = string.Join(", ", inTransitCharges.inTransitChargesDetails.Select(d => d.TransactionDetailID));

                    //    SqlParameter param1 = new SqlParameter("@Var", joindata);
                    //    List<rptCustomer> rptCustomerList = db.Database.SqlQuery<rptCustomer>("SP_PickUpboy @Var", param1).ToList();
                    //    var LoginList = db.Logins.ToList();
                    //    string PrintBy = string.Empty;
                    //    int CurrentUser = (int)GetCurrentSession.CurrentUser();
                    //    var login = LoginList.FirstOrDefault(t => t.LoginID == CurrentUser);
                    //    if (login != null)
                    //    {
                    //        PrintBy = login.FirstName+ " "+ login.LastName;
                    //    }

                    //    //List<rptCustomer> rptCustomerList = context.Database.SqlQuery<rptCustomer>("SP_PickUpboy @Var", param1).ToList<rptCustomer>();
                    //    DataTable dtBoyName = new DataTable();
                    //    dtBoyName.Columns.Add("Name", typeof(string));


                    //    var boy = db.PickUpBoys.FirstOrDefault(t => t.PickUpBoyID == Tempoid);
                    //    if (boy != null)
                    //    {
                    //        dtBoyName.Rows.Add("Delivery Boy:-  " + boy.PickUpBoyName + "\t Print By:- " + PrintBy);
                    //    }
                    //    else
                    //    {
                    //        dtBoyName.Rows.Add("");
                    //    }

                    //    DataTable dtCustomer = new DataTable();
                    //    dtCustomer.Columns.Add("LRNo", typeof(string));
                    //    dtCustomer.Columns.Add("CustomerName", typeof(string));
                    //    dtCustomer.Columns.Add("PayTypeStatus", typeof(string));
                    //    dtCustomer.Columns.Add("Amount", typeof(decimal));
                    //    dtCustomer.Columns.Add("Total", typeof(decimal));
                    //    dtCustomer.Columns.Add("Damrage", typeof(decimal));
                    //    dtCustomer.Columns.Add("Hamali", typeof(decimal));
                    //    dtCustomer.Columns.Add("NoOfParcel", typeof(int));
                    //    dtCustomer.Columns.Add("CustomerNumber", typeof(string));
                    //    foreach (var item in rptCustomerList)
                    //    {
                    //        dtCustomer.Rows.Add(item.LRNo, item.CustomerName, item.PayTypeStatus, item.Amount,
                    //            item.Total, item.Damrage, item.Hamali * item.NoOfParcel, item.NoOfParcel, item.CustomerNumber);
                    //    }

                    //    DataSet1 myds = new DataSet1();
                    //    myds.Tables["DtPickupBoy"].Merge(dtCustomer);
                    //    myds.Tables["DtBoyName"].Merge(dtBoyName);

                    //    ReportDocument rptH = new ReportDocument();
                    //    rptH.Load(Server.MapPath("~/Reports/rptPickUpboy.rpt"));
                    //    rptH.SetDataSource(myds);
                    //    Response.Buffer = false;
                    //    Response.ClearContent();
                    //    Response.ClearHeaders();

                    //    string htmlfilename = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    //    string path = Server.MapPath("~/PDF");
                    //    AllClass.CreateDirectory(path);
                    //    string fileName = path + "/" + htmlfilename + ".pdf";
                    //    string fileName2 = htmlfilename + ".pdf";
                    //    if (!Directory.Exists(path))
                    //    {
                    //        Directory.CreateDirectory(path);
                    //    }
                    //    rptH.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, fileName);

                    //    JsonResult result = new JsonResult();
                    //    result.Data = fileName2;
                    //    return result;
                    //}
                    //catch (Exception)
                    //{
                    //    JsonResult result = new JsonResult();
                    //    result.Data = null;
                    //    return result;
                    //}
                    #endregion

                }
            }
        }

        public async Task<ActionResult> DoorServicePrint(string Transactions, int pickBoy)
        {
            Decimal GrandTotal = 0;
            Int64[] nums = Array.ConvertAll(Transactions.Split(','), Int64.Parse);

            InTransitCharges inTransitCharges = new InTransitCharges();
            var TransactionDetailList = db.TransactionDetails.ToList();
            var PickUpBoyList = db.PickUpBoys.ToList();
            var LoginList = await Task.Run(() => db.Logins.ToList());

            DateTime CurrentDate = new DateTime();
            CurrentDate = GetCurrentSession.CurrentDateTime();

            String PrintBy = string.Empty;
            int CurrentUser = (int)GetCurrentSession.CurrentUser();
            var login = LoginList.FirstOrDefault(t => t.LoginID == CurrentUser);
            if (login != null)
            {
                PrintBy = login.FirstName + " " + login.LastName;
            }

            var pickupBoy = PickUpBoyList.FirstOrDefault(t => t.PickUpBoyID == pickBoy);
            if (pickupBoy != null)
            {
                inTransitCharges.ReportHeder = "Delivery Boy:-  " + pickupBoy.PickUpBoyName + "   Print By:- " + PrintBy;
            }
            List<InTransitChargesDetails> InTransitChargesList = new List<InTransitChargesDetails>();
            foreach (var item in nums)
            {
                InTransitChargesDetails InTransitChargesDetails1 = new InTransitChargesDetails();

                var data = TransactionDetailList.FirstOrDefault(t => t.TransactionDetailID == item);
                if (data != null)
                {
                    InTransitChargesDetails1.LRNo = data.LRNo;
                    InTransitChargesDetails1.Amount = (decimal)data.Amount;
                    InTransitChargesDetails1.Damrage = (decimal)data.Damrage;
                    InTransitChargesDetails1.NoOfParcel = (int)data.NoOfParcel;
                    InTransitChargesDetails1.Hamali = (int)data.Hamali * (int)data.NoOfParcel;
                    InTransitChargesDetails1.ReceiverName = data.ReceiverName;
                    InTransitChargesDetails1.ReceiverNumber = data.ReceiverNumber;
                    InTransitChargesDetails1.SenderName = data.SenderName;
                    InTransitChargesDetails1.SenderNumber = data.SenderNumber;
                    InTransitChargesDetails1.NoOfParcel = (int)data.NoOfParcel;

                    decimal total = 0;
                    total = (decimal)InTransitChargesDetails1.Damrage + (decimal)InTransitChargesDetails1.Hamali ;

                    if (data.PayType == (int)JalaramTravels.Models.Enums.PayTypes.ToPay)
                    {
                        total = total + (decimal)InTransitChargesDetails1.Amount ;
                    }
                   
                    GrandTotal += total;
                    InTransitChargesDetails1.Total = total;


                    var PayTypevar = data.PayType;
                    string PayTypeString = Enum.GetName(typeof(JalaramTravels.Models.Enums.PayTypes), PayTypevar);
                    InTransitChargesDetails1.PayTypeString = PayTypeString;
                    InTransitChargesList.Add(InTransitChargesDetails1);
                }

            }
            inTransitCharges.inTransitChargesDetails = InTransitChargesList;
            Company CompanyDetails = db.Companies.FirstOrDefault();
            inTransitCharges.Cmp = new CompnayDetail();
            if (CompanyDetails != null)
            {
                inTransitCharges.Cmp.CompnayName = CompanyDetails.CompnayName;
                inTransitCharges.Cmp.CompnayAddress = CompanyDetails.CompnayAddress;
                inTransitCharges.Cmp.CompnayNumber = CompanyDetails.CompnayNumber;
                inTransitCharges.Cmp.CompnayEmail = CompanyDetails.CompnayEmail;
            }
            inTransitCharges.Cmp.PrintDate = CurrentDate;
            inTransitCharges.GrandTotal = GrandTotal;
            return View(inTransitCharges);
        }

        [HttpPost]
        public async Task<JsonResult> Delivery(List<Int64> Ids)
        {
            //, int dStatus
            // Int64[] nums = Array.ConvertAll(Ids.Split(','), Int64.Parse);
            DateTime CurrentDate = new DateTime();
            CurrentDate = GetCurrentSession.CurrentDateTime();
            try
            {
                foreach (var item in Ids)
                {
                    var transactionDetail = await Task.Run(() => db.TransactionDetails.FirstOrDefault(t => t.TransactionDetailID == item));
                    if (transactionDetail != null)
                    {
                        transactionDetail.DeliveryDate = CurrentDate;
                        transactionDetail.DeliverdStatus = (int)Enums.DeliverdStatus.Delivered;
                        transactionDetail.DeliveryByUserID = (int)GetCurrentSession.CurrentUser();
                        await Task.Run(() => db.Entry(transactionDetail).State = System.Data.Entity.EntityState.Modified);
                        await Task.Run(() => db.SaveChanges());
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Search(string Prefix)
        {
            List<CustomerVM> CustomerListResult = new List<CustomerVM>();

            var result = db.Customers.Where(u => u.CustomerName.Contains(Prefix) || u.CustomerNumber.Contains(Prefix)).ToList();

            foreach (var item in result)
            {
                CustomerVM userVN = new CustomerVM();
                userVN.CustomerID = item.CustomerID;
                userVN.CustomerName = item.CustomerName + " " + item.CustomerNumber;
                userVN.CustomerNumber = item.CustomerNumber;
                CustomerListResult.Add(userVN);
            }
            return Json(CustomerListResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SelectById(Int64 CustomerID)
        {
            CustomerVM CustomerDetails = new CustomerVM();
            var result = db.Customers.Where(u => u.CustomerID == CustomerID).FirstOrDefault();

            if (result != null)
            {
                CustomerDetails.CustomerID = result.CustomerID;
                CustomerDetails.CustomerName = result.CustomerName;
                CustomerDetails.CustomerNumber = result.CustomerNumber;
            }
            return Json(CustomerDetails, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchLR()
        {
            SearchLRVM searchLRVM = new SearchLRVM();
            return View(searchLRVM);
        }

        [HttpPost]
        public async Task<JsonResult> SearchLRPost(string LRNo)
        {
            TransactionVM transationDetailVM = new TransactionVM();
            var busActiveList = await Task.Run(() => db.Buses.ToList().OrderBy(m => m.BusName));
            var cityActiveList = await Task.Run(() => db.Cities.ToList().OrderBy(m => m.CityName));
            var pickupboyActiveList = db.PickUpBoys.ToList().OrderBy(m => m.PickUpBoyName);
            var LoginList = await Task.Run(() => db.Logins.ToList());
            var ParcelTypeMasterList = await Task.Run(() => db.ParcelTypeMasters.ToList());
            var ParcelContainerMasterList = await Task.Run(() => db.ParcelContainerMasters.ToList());

            var transationDetail1 = await Task.Run(() => db.TransactionDetails.FirstOrDefault(m => m.LRNo == LRNo));

            if (transationDetail1 != null)
            {
                transationDetailVM.BusID = (int)transationDetail1.BusID;

                var bus = await Task.Run(() => busActiveList.FirstOrDefault(t => t.BusID == transationDetail1.BusID));
                if (bus != null)
                {
                    transationDetailVM.BusNumber = bus.BusName + " " + bus.BusNumber;
                }
                transationDetailVM.TransactionDetailID = (Int64)transationDetail1.TransactionDetailID;
                transationDetailVM.TransactionMasterID = (Int64)transationDetail1.TransactionMasterID;
                transationDetailVM.LRNo = transationDetail1.LRNo;
                transationDetailVM.NoOfParcel = (int)transationDetail1.NoOfParcel;
                transationDetailVM.SenderName = transationDetail1.SenderName;
                transationDetailVM.SenderNumber = transationDetail1.SenderNumber;
                transationDetailVM.SenderCityID = transationDetail1.SenderCityID.HasValue ? (Int64)transationDetail1.SenderCityID.Value : 0;
                if (transationDetailVM.SenderCityID != 0)
                {
                    var city = await Task.Run(() => cityActiveList.FirstOrDefault(t => t.CityID == transationDetailVM.SenderCityID));
                    if (city != null)
                    {
                        transationDetailVM.SenderCity = city.CityName;
                    }
                }
                transationDetailVM.ReceiverName = transationDetail1.ReceiverName;
                transationDetailVM.ReceiverNumber = transationDetail1.ReceiverNumber;
                transationDetailVM.ReceiverCityID = (Int64)transationDetail1.ReceiverCityID;
                if (transationDetailVM.ReceiverCityID != 0)
                {
                    var city = await Task.Run(() => cityActiveList.FirstOrDefault(t => t.CityID == transationDetailVM.ReceiverCityID));
                    if (city != null)
                    {
                        transationDetailVM.ReceiverCity = city.CityName;
                    }
                }
                transationDetailVM.ReceiverID = (Int64)transationDetail1.ReceiverID;
                transationDetailVM.Amount = transationDetail1.Amount;
                transationDetailVM.Cartage = transationDetail1.Cartage.HasValue ? (decimal)transationDetail1.Cartage.Value : 0;
                transationDetailVM.Hamali = transationDetail1.Hamali.HasValue ? (decimal)transationDetail1.Hamali.Value : 0;
                transationDetailVM.Damrage = transationDetail1.Damrage.HasValue ? (decimal)transationDetail1.Damrage.Value : 0;


                transationDetailVM.FinalAmount = AllClass.GetFinalAmout(transationDetailVM.Amount, transationDetailVM.Damrage, transationDetailVM.Hamali, transationDetailVM.NoOfParcel, transationDetail1.PayType);


                if (transationDetail1.TransactionDate.HasValue)
                {
                    transationDetailVM.TransactionDateS = transationDetail1.TransactionDate.Value.ToString("dd/MM/yyyy");
                }

                if (transationDetail1.DeliveryDate.HasValue)
                {
                    transationDetailVM.DeliveryDate = (DateTime)transationDetail1.DeliveryDate.Value;
                }

                transationDetailVM.DriverName = transationDetail1.DriverName;

                if (transationDetail1.PickUpBoyID.HasValue)
                {
                    var Boy = await Task.Run(() => db.PickUpBoys.FirstOrDefault(t => t.PickUpBoyID == transationDetail1.PickUpBoyID.Value));
                    transationDetailVM.PickUpBy = Boy.PickUpBoyName;
                    transationDetailVM.PickUpBoyID = (int)transationDetail1.PickUpBoyID;

                }
                if (transationDetail1.PickUpDate.HasValue)
                {
                    transationDetailVM.PickUpDateS = transationDetail1.PickUpDate.Value.ToString("dd/MM/yyyy");
                }

                if (transationDetail1.DeliveryDate.HasValue)
                {
                    transationDetailVM.DeliveryDateS = transationDetail1.DeliveryDate.Value.ToString("dd/MM/yyyy"); ;
                }

                transationDetailVM.TransactionDate = transationDetail1.TransactionDate;

                transationDetailVM.PaymentType = (int)JalaramTravels.Models.Enums.PaymentTypes.None;

                transationDetailVM.PayType = (JalaramTravels.Models.Enums.PayTypes)Enum.ToObject(typeof(JalaramTravels.Models.Enums.PayTypes), transationDetail1.PayType);
                transationDetailVM.DeliverdStatus = (JalaramTravels.Models.Enums.DeliverdStatus)Enum.ToObject(typeof(JalaramTravels.Models.Enums.DeliverdStatus), transationDetail1.DeliverdStatus);


                //Enum to String name
                var DeliverdStatusVar = transationDetail1.DeliverdStatus;
                transationDetailVM.DeliverdStatusString = Enum.GetName(typeof(JalaramTravels.Models.Enums.DeliverdStatus), DeliverdStatusVar);

                var PayTypevar = transationDetail1.PayType;
                transationDetailVM.PayTypeString = Enum.GetName(typeof(JalaramTravels.Models.Enums.PayTypes), PayTypevar);

                var PaymentTypesvar = transationDetail1.PaymentType;
                transationDetailVM.PaymentTypesString = Enum.GetName(typeof(JalaramTravels.Models.Enums.PaymentTypes), PaymentTypesvar);


                if (transationDetail1.CreateUser != null)
                {
                    var login = await Task.Run(() => LoginList.FirstOrDefault(t => t.LoginID == transationDetail1.CreateUser));
                    if (login != null)
                    {
                        transationDetailVM.CreateUserS = login.FirstName + " " + login.LastName;
                    }
                }

                if (transationDetail1.DeliveryByUserID != null)
                {
                    var login = await Task.Run(() => LoginList.FirstOrDefault(t => t.LoginID == transationDetail1.DeliveryByUserID));
                    if (login != null)
                    {
                        transationDetailVM.DeliveryByUserIDString = login.FirstName + " " + login.LastName;
                    }
                }

                if (transationDetail1.DeliveryByCustomer)
                {
                    transationDetailVM.DeliveryByUserIDString = transationDetail1.ReceiverDetails;
                }

                if (transationDetail1.ParcelTypeID != 0)
                {
                    var par = await Task.Run(() => ParcelTypeMasterList.FirstOrDefault(t => t.ParcelTypeID == transationDetail1.ParcelTypeID));
                    if (par != null)
                    {
                        transationDetailVM.ParcelTypeS = par.ParcelTypeName;
                        transationDetailVM.ParcelTypeID = transationDetail1.ParcelTypeID;
                    }
                }
                if (transationDetail1.ParcelContainerID != 0)
                {
                    var par = await Task.Run(() => ParcelContainerMasterList.FirstOrDefault(t => t.ParcelContainerID == transationDetail1.ParcelContainerID));
                    if (par != null)
                    {
                        transationDetailVM.ParcelContainerS = par.ParcelContainerName;
                        transationDetailVM.ParcelContainerID = transationDetail1.ParcelContainerID;
                    }
                }

            }

            return Json(transationDetailVM, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Delivered()
        {
            var transationList = await Task.Run(() => db.TransactionDetails.Where(m => m.DeliverdStatus == (int)JalaramTravels.Models.Enums.DeliverdStatus.Delivered).ToList());

            TransactionDetailVM transationsObj = new TransactionDetailVM();
            List<TransactionDetailsVM> transations = new List<TransactionDetailsVM>();

            transationsObj.TransactionDate = GetCurrentSession.CurrentDateTime();

            var busActiveList = await Task.Run(() => db.Buses.ToList().OrderBy(m => m.BusName));
            transationsObj.BusList = await Task.Run(() => busActiveList.Where(m => m.Flag.Equals("A")).Select(s => new SelectListItem() { Text = s.BusName, Value = s.BusID.ToString() }));

            var PickUpBoysList = await Task.Run(() => db.PickUpBoys.ToList().OrderBy(m => m.PickUpBoyName));
            transationsObj.PickUpBoyList = await Task.Run(() => PickUpBoysList.Where(m => m.Flag.Equals("A")).Select(s => new SelectListItem() { Text = s.PickUpBoyName, Value = s.PickUpBoyID.ToString() }));

            foreach (var item in transationList)
            {
                TransactionDetailsVM obj = new TransactionDetailsVM();
                obj.TransactionDetailID = (Int64)item.TransactionDetailID;
                obj.LRNo = item.LRNo;
                obj.SenderName = item.SenderName;
                obj.ReceiverName = item.ReceiverName;
                //obj.Amount = (decimal)item.Amount;
                obj.Amount = (decimal)AllClass.GetFinalAmout(item.Amount, item.Damrage, item.Hamali, item.NoOfParcel, item.PayType);
                obj.Cartage = (decimal)item.Cartage;

                obj.PaymentType = (int)JalaramTravels.Models.Enums.PaymentTypes.None;
                obj.PayType = (JalaramTravels.Models.Enums.PayTypes)Enum.ToObject(typeof(JalaramTravels.Models.Enums.PayTypes), item.PayType);
                obj.DeliverdStatus = (JalaramTravels.Models.Enums.DeliverdStatus)Enum.ToObject(typeof(JalaramTravels.Models.Enums.DeliverdStatus), item.DeliverdStatus);

                var busD = await Task.Run(() => db.Buses.FirstOrDefault(t => t.BusID == item.BusID));
                if (busD != null)
                {
                    obj.BusName = busD.BusName;
                    obj.BusNumber = busD.BusNumber;
                }
                if (item.TransactionDate.HasValue)
                {
                    obj.TransactionDateS = item.TransactionDate.Value.ToString("dd/MM/yyyy");
                }
                if (item.DeliveryDate.HasValue)
                {
                    obj.DeliveryDateS = item.DeliveryDate.Value.ToString("dd/MM/yyyy");
                }

                transations.Add(obj);
            }
            transationsObj.TransactionDetailList = transations;

            return View(transationsObj);
        }

        [HttpPost]
        public async Task<JsonResult> DeliveredFilter(TransactionDetailFiler transactionDetailFiler)
        {
            var transationList = await Task.Run(() => db.TransactionDetails.Where(m => m.DeliverdStatus == (int)JalaramTravels.Models.Enums.DeliverdStatus.Delivered && m.TransactionDate == transactionDetailFiler.TransactionDate).ToList());

            TransactionDetailVM transationsObj = new TransactionDetailVM();
            List<TransactionDetailsVM> transations = new List<TransactionDetailsVM>();

            var busActiveList = await Task.Run(() => db.Buses.ToList().OrderBy(m => m.BusName));
            transationsObj.BusList = await Task.Run(() => busActiveList.Where(m => m.Flag.Equals("A")).Select(s => new SelectListItem() { Text = s.BusName, Value = s.BusID.ToString() }));

            var PickUpBoysList = await Task.Run(() => db.PickUpBoys.ToList().OrderBy(m => m.PickUpBoyName));
            transationsObj.PickUpBoyList = await Task.Run(() => PickUpBoysList.Where(m => m.Flag.Equals("A")).Select(s => new SelectListItem() { Text = s.PickUpBoyName, Value = s.PickUpBoyID.ToString() }));

            foreach (var item in transationList)
            {
                TransactionDetailsVM obj = new TransactionDetailsVM();
                obj.TransactionDetailID = (Int64)item.TransactionDetailID;
                obj.LRNo = item.LRNo;
                obj.SenderName = item.SenderName;
                obj.ReceiverName = item.ReceiverName;
                //obj.Amount = (decimal)item.Amount;
                obj.Amount = (decimal)AllClass.GetFinalAmout(item.Amount, item.Damrage, item.Hamali, item.NoOfParcel, item.PayType);
                obj.Cartage = (decimal)item.Cartage;

                obj.PaymentType = (int)JalaramTravels.Models.Enums.PaymentTypes.None;
                obj.PayType = (JalaramTravels.Models.Enums.PayTypes)Enum.ToObject(typeof(JalaramTravels.Models.Enums.PayTypes), item.PayType);
                obj.DeliverdStatus = (JalaramTravels.Models.Enums.DeliverdStatus)Enum.ToObject(typeof(JalaramTravels.Models.Enums.DeliverdStatus), item.DeliverdStatus);

                var busD = db.Buses.FirstOrDefault(t => t.BusID == item.BusID);
                if (busD != null)
                {
                    obj.BusName = busD.BusName;
                    obj.BusNumber = busD.BusNumber;
                }
                if (item.TransactionDate.HasValue)
                {
                    obj.TransactionDateS = item.TransactionDate.Value.ToString("dd/MM/yyyy");
                }
                if (item.DeliveryDate.HasValue)
                {
                    obj.DeliveryDateS = item.DeliveryDate.Value.ToString("dd/MM/yyyy");
                }
                transations.Add(obj);
            }
            transationsObj.TransactionDetailList = transations;

            return Json(transations, JsonRequestBehavior.AllowGet);
        }

    }
}
