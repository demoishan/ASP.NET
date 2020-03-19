using JalaramTravels.Models;
using JalaramTravels.ViewModel;
using PdfSample.Pdf;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JalaramTravels.Controllers
{
    public class ReportController : Controller
    {
        JalaramDBEntities db = new JalaramDBEntities();

        public async Task<ActionResult> Transaction()
        {
            var transationList = await Task.Run(() => db.TransactionDetails.ToList());

            TransactionDetailVM transationsObj = new TransactionDetailVM();
            List<TransactionDetailsVM> transations = new List<TransactionDetailsVM>();

            var LoginList = await Task.Run(() => db.Logins.ToList());

            var busActiveList = await Task.Run(() => db.Buses.ToList().OrderBy(m => m.BusName));
            transationsObj.BusList = await Task.Run(() => busActiveList.Select(s => new SelectListItem() { Text = s.BusName + "-" + s.BusNumber, Value = s.BusID.ToString() }));

            var TempoActiveList = await Task.Run(() => db.PickUpBoys.ToList().OrderBy(m => m.PickUpBoyName));
            transationsObj.PickUpBoyList = await Task.Run(() => TempoActiveList.Select(s => new SelectListItem() { Text = s.PickUpBoyName, Value = s.PickUpBoyID.ToString() }));


            var ParcelContainerList = await Task.Run(() => db.ParcelContainerMasters.ToList().OrderBy(m => m.ParcelContainerName));

            var ParcelTypeList = await Task.Run(() => db.ParcelTypeMasters.ToList().OrderBy(m => m.ParcelTypeName));

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
                
                //Enum to String name
                var DeliverdStatusVar = item.DeliverdStatus;
                obj.DeliverdStatusString = Enum.GetName(typeof(JalaramTravels.Models.Enums.DeliverdStatus), DeliverdStatusVar);

                var PayTypevar = item.PayType;
                obj.PayTypeString = Enum.GetName(typeof(JalaramTravels.Models.Enums.PayTypes), PayTypevar);

                var busD = await Task.Run(() => busActiveList.FirstOrDefault(t => t.BusID == item.BusID));
                if (busD != null)
                {
                    obj.BusName = busD.BusName;
                    obj.BusNumber = busD.BusNumber;
                }

                var Par = await Task.Run(() => ParcelTypeList.FirstOrDefault(t => t.ParcelTypeID == item.ParcelTypeID));
                if (Par != null)
                {
                    obj.ParcelTypeString = Par.ParcelTypeName;
                }

                var ParC = await Task.Run(() => ParcelContainerList.FirstOrDefault(t => t.ParcelContainerID == item.ParcelContainerID));
                if (ParC != null)
                {
                    obj.ParcelContainerString = ParC.ParcelContainerName;
                }


                if (item.TransactionDate.HasValue)
                {
                    obj.TransactionDateS = item.TransactionDate.Value.ToString("dd/MM/yyyy");
                }
                if (item.CreateUser != null)
                {
                    var login = await Task.Run(() => LoginList.FirstOrDefault(t => t.LoginID == item.CreateUser));
                    if (login != null)
                    {
                        obj.CreateUserString = login.FirstName + " " + login.LastName;
                    }
                }
                transations.Add(obj);
            }
            transationsObj.TransactionDetailList = transations;

            return View(transationsObj);
        }

        [HttpPost]
        public async Task<JsonResult> TransactionFilter(TransactionDetailFiler transactionDetailFiler)
        {
            var transationList = await Task.Run(() => db.TransactionDetails.Where( m=> m.TransactionDate.Value >= transactionDetailFiler.TransactionDate.Value && m.TransactionDate.Value <= transactionDetailFiler.TransactionDateEnd.Value).ToList());

            if (transactionDetailFiler.BusID != 0)
            {
                transationList = transationList.Where(m => m.BusID == transactionDetailFiler.BusID).ToList();
            }

            TransactionDetailVM transationsObj = new TransactionDetailVM();
            List<TransactionDetailsVM> transations = new List<TransactionDetailsVM>();
            var LoginList = await Task.Run(() => db.Logins.ToList());

            var busActiveList = await Task.Run(() => db.Buses.ToList().OrderBy(m => m.BusName));
            transationsObj.BusList = await Task.Run(() => busActiveList.Select(s => new SelectListItem() { Text = s.BusName, Value = s.BusID.ToString() }));

            var TempoActiveList = await Task.Run(() => db.PickUpBoys.ToList().OrderBy(m => m.PickUpBoyName));
            transationsObj.PickUpBoyList = await Task.Run(() => TempoActiveList.Select(s => new SelectListItem() { Text = s.PickUpBoyName, Value = s.PickUpBoyID.ToString() }));

            var ParcelContainerList = await Task.Run(() => db.ParcelContainerMasters.ToList().OrderBy(m => m.ParcelContainerName));

            var ParcelTypeList = await Task.Run(() => db.ParcelTypeMasters.ToList().OrderBy(m => m.ParcelTypeName));

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

                //Enum to String name
                var DeliverdStatusVar = item.DeliverdStatus;
                obj.DeliverdStatusString = Enum.GetName(typeof(JalaramTravels.Models.Enums.DeliverdStatus), DeliverdStatusVar);


                var PayTypevar = item.PayType;
                obj.PayTypeString = Enum.GetName(typeof(JalaramTravels.Models.Enums.PayTypes), PayTypevar);
                var busD = await Task.Run(() => busActiveList.FirstOrDefault(t => t.BusID == item.BusID));
                if (busD != null)
                {
                    obj.BusName = busD.BusName;
                    obj.BusNumber = busD.BusNumber;
                }

                var Par = await Task.Run(() => ParcelTypeList.FirstOrDefault(t => t.ParcelTypeID == item.ParcelTypeID));
                if (Par != null)
                {
                    obj.ParcelTypeString = Par.ParcelTypeName;
                }

                var ParC = await Task.Run(() => ParcelContainerList.FirstOrDefault(t => t.ParcelContainerID == item.ParcelContainerID));
                if (ParC != null)
                {
                    obj.ParcelContainerString = ParC.ParcelContainerName;
                }

                if (item.TransactionDate.HasValue)
                {
                    obj.TransactionDateS = item.TransactionDate.Value.ToString("dd/MM/yyyy");
                }

                if (item.CreateUser != null)
                {
                    var login = await Task.Run(() => LoginList.FirstOrDefault(t => t.LoginID == item.CreateUser));
                    if (login != null)
                    {
                        obj.CreateUserString = login.FirstName + " " + login.LastName;
                    }
                }
                transations.Add(obj);
            }
            transationsObj.TransactionDetailList = transations;

            return Json(transations, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Undelivered()
        {
            var transationList = await Task.Run(() => db.TransactionDetails.Where(m => m.DeliverdStatus == (int)JalaramTravels.Models.Enums.DeliverdStatus.Undelivered).ToList());

            TransactionDetailVM transationsObj = new TransactionDetailVM();
            List<TransactionDetailsVM> transations = new List<TransactionDetailsVM>();

            var LoginList = await Task.Run(() => db.Logins.ToList());

            var busActiveList = await Task.Run(() => db.Buses.ToList().OrderBy(m => m.BusName));
            transationsObj.BusList = await Task.Run(() => busActiveList.Select(s => new SelectListItem() { Text = s.BusName + "-" + s.BusNumber, Value = s.BusID.ToString() }));

            var TempoActiveList = await Task.Run(() => db.PickUpBoys.ToList().OrderBy(m => m.PickUpBoyName));
            transationsObj.PickUpBoyList = await Task.Run(() => TempoActiveList.Select(s => new SelectListItem() { Text = s.PickUpBoyName, Value = s.PickUpBoyID.ToString() }));


            var ParcelContainerList = await Task.Run(() => db.ParcelContainerMasters.ToList().OrderBy(m => m.ParcelContainerName));

            var ParcelTypeList = await Task.Run(() => db.ParcelTypeMasters.ToList().OrderBy(m => m.ParcelTypeName));

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

                var Par = await Task.Run(() => ParcelTypeList.FirstOrDefault(t => t.ParcelTypeID == item.ParcelTypeID));
                if (Par !=null)
                {
                    obj.ParcelTypeString = Par.ParcelTypeName;
                }

                var ParC = await Task.Run(() => ParcelContainerList.FirstOrDefault(t => t.ParcelContainerID == item.ParcelContainerID));
                if (ParC != null)
                {
                    obj.ParcelContainerString = ParC.ParcelContainerName;
                }


                if (item.TransactionDate.HasValue)
                {
                    obj.TransactionDateS = item.TransactionDate.Value.ToString("dd/MM/yyyy");
                }
                if (item.CreateUser != null)
                {
                    var login = await Task.Run(() => LoginList.FirstOrDefault(t => t.LoginID == item.CreateUser));
                    if (login != null)
                    {
                        obj.CreateUserString = login.FirstName + " " + login.LastName;
                    }
                }
                transations.Add(obj);
            }
            transationsObj.TransactionDetailList = transations;

            return View(transationsObj);
        }

        [HttpPost]
        public async Task<JsonResult> UndeliveredFilter(TransactionDetailFiler transactionDetailFiler)
        {
            var transationList = await Task.Run(() => db.TransactionDetails.Where(m => m.DeliverdStatus == (int)JalaramTravels.Models.Enums.DeliverdStatus.Undelivered && (m.TransactionDate.Value >= transactionDetailFiler.TransactionDate.Value && m.TransactionDate.Value <= transactionDetailFiler.TransactionDateEnd.Value)).ToList());

            if (transactionDetailFiler.BusID !=0)
            {
                transationList = transationList.Where(m => m.BusID == transactionDetailFiler.BusID).ToList();
            }

            TransactionDetailVM transationsObj = new TransactionDetailVM();
            List<TransactionDetailsVM> transations = new List<TransactionDetailsVM>();
            var LoginList = await Task.Run(() => db.Logins.ToList());

            var busActiveList = await Task.Run(() => db.Buses.ToList().OrderBy(m => m.BusName));
            transationsObj.BusList = await Task.Run(() => busActiveList.Select(s => new SelectListItem() { Text = s.BusName, Value = s.BusID.ToString() }));

            var TempoActiveList = await Task.Run(() => db.PickUpBoys.ToList().OrderBy(m => m.PickUpBoyName));
            transationsObj.PickUpBoyList = await Task.Run(() => TempoActiveList.Select(s => new SelectListItem() { Text = s.PickUpBoyName, Value = s.PickUpBoyID.ToString() }));

            var ParcelContainerList = await Task.Run(() => db.ParcelContainerMasters.ToList().OrderBy(m => m.ParcelContainerName));

            var ParcelTypeList = await Task.Run(() => db.ParcelTypeMasters.ToList().OrderBy(m => m.ParcelTypeName));

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

                var Par = await Task.Run(() => ParcelTypeList.FirstOrDefault(t => t.ParcelTypeID == item.ParcelTypeID));
                if (Par != null)
                {
                    obj.ParcelTypeString = Par.ParcelTypeName;
                }

                var ParC = await Task.Run(() => ParcelContainerList.FirstOrDefault(t => t.ParcelContainerID == item.ParcelContainerID));
                if (ParC != null)
                {
                    obj.ParcelContainerString = ParC.ParcelContainerName;
                }

                if (item.TransactionDate.HasValue)
                {
                    obj.TransactionDateS = item.TransactionDate.Value.ToString("dd/MM/yyyy");
                }

                if (item.CreateUser != null)
                {
                    var login = await Task.Run(() => LoginList.FirstOrDefault(t => t.LoginID == item.CreateUser));
                    if (login != null)
                    {
                        obj.CreateUserString = login.FirstName + " " + login.LastName;
                    }
                }
                transations.Add(obj);
            }
            transationsObj.TransactionDetailList = transations;

            return Json(transations, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> InTransit()
        {
            InTransitVM inTransitVM = new InTransitVM();
            inTransitVM.DeliverdStatus = Enums.DeliverdStatus.Delivered;
            List<TransactionDetailsVM> toPayVM = new List<TransactionDetailsVM>();

            var LoginList = await Task.Run(() => db.Logins.ToList());

            var PickUpBoysList = await Task.Run(() => db.PickUpBoys.ToList());

            var TempoActiveList = await Task.Run(() => db.PickUpBoys.ToList().OrderBy(m => m.PickUpBoyName));
            inTransitVM.PickUpBoyList = await Task.Run(() => TempoActiveList.Select(s => new SelectListItem() { Text = s.PickUpBoyName, Value = s.PickUpBoyID.ToString() }));

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

                if (item.CreateUser != null)
                {
                    var login = await Task.Run(() => LoginList.FirstOrDefault(t => t.LoginID == item.CreateUser));
                    if (login != null)
                    {
                        ToPayVMObj.CreateUserString = login.FirstName + " " + login.LastName;
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
            inTransitVM.DeliverdStatus = Enums.DeliverdStatus.Delivered;
            List<TransactionDetailsVM> toPayVM = new List<TransactionDetailsVM>();

            var LoginList = await Task.Run(() => db.Logins.ToList());

            var PickUpBoysList = await Task.Run(() => db.PickUpBoys.ToList());

            var TempoActiveList = await Task.Run(() => db.PickUpBoys.ToList().OrderBy(m => m.PickUpBoyName));
            inTransitVM.PickUpBoyList = await Task.Run(() => TempoActiveList.Select(s => new SelectListItem() { Text = s.PickUpBoyName, Value = s.PickUpBoyID.ToString() }));

            var TransactionMasters1 = await Task.Run(() => db.TransactionDetails.ToList().Where(t => t.DeliverdStatus.Value == (int)Enums.DeliverdStatus.InTransit));
            var busActiveList = await Task.Run(() => db.Buses.ToList().OrderBy(m => m.BusName));
            inTransitVM.BusList = await Task.Run(() => busActiveList.Select(s => new SelectListItem() { Text = s.BusName, Value = s.BusID.ToString() }));

            if (transactionDetailFiler.BusID.HasValue)
            {
                TransactionMasters1 = TransactionMasters1.ToList().Where(t => t.BusID == transactionDetailFiler.BusID);
            }
            if (transactionDetailFiler.PickUpBoyID.HasValue)
            {
                TransactionMasters1 = TransactionMasters1.ToList().Where(t => t.PickUpBoyID == transactionDetailFiler.PickUpBoyID);
            }
            if (transactionDetailFiler.TransactionDate.HasValue)
            {
                TransactionMasters1 = TransactionMasters1.ToList().Where(t => t.TransactionDate == transactionDetailFiler.TransactionDate);
            }
            if (transactionDetailFiler.PickUpDate.HasValue && transactionDetailFiler.PickUpDateEnd.HasValue)
            {
                TransactionMasters1 = TransactionMasters1.ToList().Where(t=>t.PickUpDate.Value >= transactionDetailFiler.PickUpDate.Value && t.PickUpDate.Value <= transactionDetailFiler.PickUpDateEnd.Value);
            }
            foreach (var item in TransactionMasters1)
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
                if (item.CreateUser != null)
                {
                    var login = await Task.Run(() => LoginList.FirstOrDefault(t => t.LoginID == item.CreateUser));
                    if (login != null)
                    {
                        ToPayVMObj.CreateUserString = login.FirstName + " " + login.LastName;
                    }
                }
                ToPayVMObj.DriverName = item.DriverName;
                toPayVM.Add(ToPayVMObj);
                inTransitVM.TransactionDetailList = toPayVM;

            }
            return Json(toPayVM, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Delivered()
        {
            var transationList = await Task.Run(() => db.TransactionDetails.Where(m => m.DeliverdStatus == (int)JalaramTravels.Models.Enums.DeliverdStatus.Delivered).ToList());

            TransactionDetailVM transationsObj = new TransactionDetailVM();
            List<TransactionDetailsVM> transations = new List<TransactionDetailsVM>();

            var LoginList = await Task.Run(() => db.Logins.ToList());

            var PickUpBoysList = await Task.Run(() => db.PickUpBoys.ToList());

            var busActiveList = await Task.Run(() => db.Buses.ToList().OrderBy(m => m.BusName));
            transationsObj.BusList = await Task.Run(() => busActiveList.Select(s => new SelectListItem() { Text = s.BusName, Value = s.BusID.ToString() }));

            var PickUpBoyList = await Task.Run(() => db.PickUpBoys.ToList().OrderBy(m => m.PickUpBoyName));
            transationsObj.PickUpBoyList = await Task.Run(() => PickUpBoyList.Select(s => new SelectListItem() { Text = s.PickUpBoyName, Value = s.PickUpBoyID.ToString() }));

            foreach (var item in transationList)
            {
                TransactionDetailsVM obj = new TransactionDetailsVM();
                obj.TransactionDetailID = (Int64)item.TransactionDetailID;
                obj.LRNo = item.LRNo;
                obj.SenderName = item.SenderName;
                obj.ReceiverName = item.ReceiverName;
                obj.Amount = (decimal)item.Amount;
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

               
                if (item.TransactionDate.HasValue)
                {
                    obj.TransactionDateS = item.TransactionDate.Value.ToString("dd/MM/yyyy");
                }
                if (item.PickUpDate.HasValue)
                {
                    obj.PickUpDateS = item.PickUpDate.Value.ToString("dd/MM/yyyy");
                }
                if (item.PickUpBoyID != null)
                {
                    var boy = await Task.Run(() => PickUpBoysList.FirstOrDefault(t => t.PickUpBoyID == item.PickUpBoyID));
                    if (PickUpBoysList != null)
                    {
                        obj.PickUpBoy = boy.PickUpBoyName;
                    }
                }
                if (item.PickUpByUserID != null)
                {
                    var login = await Task.Run(() => LoginList.FirstOrDefault(t => t.LoginID == item.PickUpByUserID));
                    if (login != null)
                    {
                        obj.PickUpCreateBy = login.FirstName + " " + login.LastName;
                    }
                }
                if (item.CreateUser != null)
                {
                    var login = await Task.Run(() => LoginList.FirstOrDefault(t => t.LoginID == item.CreateUser));
                    if (login != null)
                    {
                        obj.CreateUserString = login.FirstName + " " + login.LastName;
                    }
                }

                transations.Add(obj);
            }
            transationsObj.TransactionDetailList = transations;

            return View(transationsObj);
        }

        [HttpPost]
        public async Task<JsonResult> DeliveredFilter(TransactionDetailFiler transactionDetailFiler)
        {
            var transationList = await Task.Run(() => db.TransactionDetails.Where(m => m.DeliverdStatus == (int)JalaramTravels.Models.Enums.DeliverdStatus.Delivered && (m.TransactionDate.Value >= transactionDetailFiler.TransactionDate.Value && m.TransactionDate.Value <= transactionDetailFiler.TransactionDateEnd.Value)).ToList());

            if (transactionDetailFiler.DeliveryDate.HasValue)
            {
                transationList = transationList.Where(t => t.DeliveryDate == transactionDetailFiler.DeliveryDate).ToList();
            }
            TransactionDetailVM transationsObj = new TransactionDetailVM();
            List<TransactionDetailsVM> transations = new List<TransactionDetailsVM>();

            var LoginList = await Task.Run(() => db.Logins.ToList());

            var PickUpBoysList = await Task.Run(() => db.PickUpBoys.ToList());

            var busActiveList = await Task.Run(() => db.Buses.ToList().OrderBy(m => m.BusName));
            transationsObj.BusList = await Task.Run(() => busActiveList.Select(s => new SelectListItem() { Text = s.BusName, Value = s.BusID.ToString() }));

            var PickUpBoyList = await Task.Run(() => db.PickUpBoys.ToList().OrderBy(m => m.PickUpBoyName));
            transationsObj.PickUpBoyList = await Task.Run(() => PickUpBoyList.Select(s => new SelectListItem() { Text = s.PickUpBoyName, Value = s.PickUpBoyID.ToString() }));

            foreach (var item in transationList)
            {
                TransactionDetailsVM obj = new TransactionDetailsVM();
                obj.TransactionDetailID = (Int64)item.TransactionDetailID;
                obj.LRNo = item.LRNo;
                obj.SenderName = item.SenderName;
                obj.ReceiverName = item.ReceiverName;
                obj.Amount = (decimal)item.Amount;
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


                if (item.TransactionDate.HasValue)
                {
                    obj.TransactionDateS = item.TransactionDate.Value.ToString("dd/MM/yyyy");
                }
                if (item.PickUpDate.HasValue)
                {
                    obj.PickUpDateS = item.PickUpDate.Value.ToString("dd/MM/yyyy");
                }
                if (item.PickUpBoyID != null)
                {
                    var boy = await Task.Run(() => PickUpBoysList.FirstOrDefault(t => t.PickUpBoyID == item.PickUpBoyID));
                    if (PickUpBoysList != null)
                    {
                        obj.PickUpBoy = boy.PickUpBoyName;
                    }
                }
                if (item.PickUpByUserID != null)
                {
                    var login = await Task.Run(() => LoginList.FirstOrDefault(t => t.LoginID == item.PickUpByUserID));
                    if (login != null)
                    {
                        obj.PickUpCreateBy = login.FirstName + " " + login.LastName;
                    }
                }
                if (item.CreateUser != null)
                {
                    var login = await Task.Run(() => LoginList.FirstOrDefault(t => t.LoginID == item.CreateUser));
                    if (login != null)
                    {
                        obj.CreateUserString = login.FirstName + " " + login.LastName;
                    }
                }
                transations.Add(obj);
            }
            transationsObj.TransactionDetailList = transations;

            return Json(transations, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Customer()
        {
            var customersList = await Task.Run(() => db.Customers.ToList());

            CustomerVM transationsObj = new CustomerVM();
            List<CustomerVM> transations = new List<CustomerVM>();

            foreach (var item in customersList)
            {
                CustomerVM obj = new CustomerVM();
                obj.CustomerName =  item.CustomerName;
                obj.CustomerNumber = item.CustomerNumber;

                transations.Add(obj);
            }

            return View(transations);
        }


        public async Task<ActionResult> Payments()
        {
            ToPayVML toPayVML = new ToPayVML();

            var LoginList = await Task.Run(() => db.Logins.ToList());
             
            var TransactionMasters1 = await Task.Run(() => db.TransactionMasters.ToList());
            var busActiveList = await Task.Run(() => db.Buses.ToList().OrderBy(m => m.BusName));
            var BusList1 = await Task.Run(() => busActiveList.Select(s => new SelectListItem() { Text = s.BusName, Value = s.BusID.ToString() }));
            toPayVML.BusList = BusList1;

            List<ToPayVM> lst = new List<ToPayVM>();
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

                if (item.CreateUser != null)
                {
                    var login = await Task.Run(() => LoginList.FirstOrDefault(t => t.LoginID == item.CreateUser));
                    if (login != null)
                    {
                        ToPayVMObj.CreateUserString = login.FirstName + " " + login.LastName;
                    }
                }

                lst.Add(ToPayVMObj);
            }
            toPayVML.ToPayVMList=lst;
            return View(toPayVML);
        }

        [HttpPost]
        public async Task<JsonResult> PaymentsFilter(ToPayVML toPayObj)
        {
            ToPayVML toPayVML = new ToPayVML();

            var LoginList = await Task.Run(() => db.Logins.ToList());

            
            var TransactionMasters1 = await Task.Run(() => db.TransactionMasters.ToList());

            if (toPayObj.BusID != 0)
            {
                TransactionMasters1= TransactionMasters1.Where(m => m.BusID==toPayObj.BusID).ToList();
            }
            if (toPayObj.TransactionDate.HasValue && toPayObj.TransactionDateEnd.HasValue)
            {
                TransactionMasters1=TransactionMasters1.Where(m => m.TransactionDate.Value >= toPayObj.TransactionDate.Value && m.TransactionDate.Value <= toPayObj.TransactionDateEnd.Value).ToList();
            }
            var busActiveList = await Task.Run(() => db.Buses.ToList().OrderBy(m => m.BusName));
            var BusList1 = await Task.Run(() => busActiveList.Select(s => new SelectListItem() { Text = s.BusName, Value = s.BusID.ToString() }));
            toPayVML.BusList = BusList1;

            List<ToPayVM> lst = new List<ToPayVM>();
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

                if (item.CreateUser != null)
                {
                    var login = await Task.Run(() => LoginList.FirstOrDefault(t => t.LoginID == item.CreateUser));
                    if (login != null)
                    {
                        ToPayVMObj.CreateUserString = login.FirstName + " " + login.LastName;
                    }
                }

                //var TransactionStatusvar = item.TransactionStatus;
                //ToPayVMObj.TransactionStatusString = Enum.GetName(typeof(JalaramTravels.Models.Enums.TransactionStatus), TransactionStatusvar);

                if (item.TransactionStatus==(int)JalaramTravels.Models.Enums.TransactionStatus.Completed)
                {
                    ToPayVMObj.TransactionStatusString = "Paid";
                }
                else
                {
                    ToPayVMObj.TransactionStatusString = "Not Paid";

                }
               

                lst.Add(ToPayVMObj);
            }
            toPayVML.ToPayVMList = lst;

            return Json(toPayVML, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> User()
        {
            decimal total = 0;

            TransactionDetailVM transationsObj = new TransactionDetailVM();
            List<TransactionDetailsVM> transations = new List<TransactionDetailsVM>();

            var LoginList = await Task.Run(() => db.Logins.ToList());

            var PickUpBoysList = await Task.Run(() => db.PickUpBoys.ToList());

            var busActiveList = await Task.Run(() => db.Buses.ToList().OrderBy(m => m.BusName));
            transationsObj.BusList = await Task.Run(() => busActiveList.Select(s => new SelectListItem() { Text = s.BusName, Value = s.BusID.ToString() }));

            var PickUpBoyList = await Task.Run(() => db.PickUpBoys.ToList().OrderBy(m => m.PickUpBoyName));
             
            transationsObj.DeliveredByList = await Task.Run(() => LoginList.Select(s => new SelectListItem() { Text = s.FirstName + " "  +s.LastName, Value = s.LoginID.ToString() }));

            var transationList = await Task.Run(() => db.TransactionDetails.Where(m => m.DeliverdStatus == (int)JalaramTravels.Models.Enums.DeliverdStatus.Delivered).ToList());
             
            foreach (var item in transationList)
            {
                TransactionDetailsVM obj = new TransactionDetailsVM();
                obj.TransactionDetailID = (Int64)item.TransactionDetailID;
                obj.LRNo = item.LRNo;
                obj.SenderName = item.SenderName;
                obj.ReceiverName = item.ReceiverName;                
                obj.Cartage = (decimal)item.Cartage;
                //obj.Amount = (decimal)item.Amount;
                obj.Amount = (decimal) AllClass.GetFinalAmout(item.Amount, item.Damrage, item.Hamali, item.NoOfParcel, item.PayType);
                total += obj.Amount;

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


                if (item.TransactionDate.HasValue)
                {
                    obj.TransactionDateS = item.TransactionDate.Value.ToString("dd/MM/yyyy");
                }
                if (item.PickUpDate.HasValue)
                {
                    obj.PickUpDateS = item.PickUpDate.Value.ToString("dd/MM/yyyy");
                }
                if (item.PickUpBoyID != null)
                {
                    var boy = await Task.Run(() => PickUpBoysList.FirstOrDefault(t => t.PickUpBoyID == item.PickUpBoyID));
                    if (PickUpBoysList != null)
                    {
                        obj.PickUpBoy = boy.PickUpBoyName;
                    }
                }
                if (item.PickUpByUserID != null)
                {
                    var login = await Task.Run(() => LoginList.FirstOrDefault(t => t.LoginID == item.PickUpByUserID));
                    if (login != null)
                    {
                        obj.PickUpCreateBy = login.FirstName + " " + login.LastName;
                    }
                }
                if (item.CreateUser != null)
                {
                    var login = await Task.Run(() => LoginList.FirstOrDefault(t => t.LoginID == item.CreateUser));
                    if (login != null)
                    {
                        obj.CreateUserString = login.FirstName + " " + login.LastName;
                    }
                }

                if (item.DeliveryByUserID != null)
                {
                    var login = await Task.Run(() => LoginList.FirstOrDefault(t => t.LoginID == item.DeliveryByUserID));
                    if (login != null)
                    {
                        obj.DeliveryByUserIDString = login.FirstName + " " + login.LastName;
                    }
                }

                if (item.DeliveryByCustomer)
                {
                    obj.ReceiverDetails = item.ReceiverDetails;
                }

                transations.Add(obj);
            }
            transationsObj.TotalAmount = total;
            transationsObj.TransactionDetailList = transations;

            return View(transationsObj);
        }

        [HttpPost]
        public async Task<JsonResult> UserFilter(TransactionDetailFiler transactionDetailFiler)
        {
            decimal total = 0;

            TransactionDetailVM transationsObj = new TransactionDetailVM();
            List<TransactionDetailsVM> transations = new List<TransactionDetailsVM>();

            var LoginList = await Task.Run(() => db.Logins.ToList());

            var PickUpBoysList = await Task.Run(() => db.PickUpBoys.ToList());

            var busActiveList = await Task.Run(() => db.Buses.ToList().OrderBy(m => m.BusName));
            transationsObj.BusList = await Task.Run(() => busActiveList.Select(s => new SelectListItem() { Text = s.BusName, Value = s.BusID.ToString() }));

            var PickUpBoyList = await Task.Run(() => db.PickUpBoys.ToList().OrderBy(m => m.PickUpBoyName));
            transationsObj.PickUpBoyList = await Task.Run(() => LoginList.Select(s => new SelectListItem() { Text = s.FirstName + " " + s.LastName, Value = s.LoginID.ToString() }));

            //  var transationList = await Task.Run(() => db.TransactionDetails.Where(m=>m.DeliveryDate.HasValue==true && m.DeliveryDate.Value==transactionDetailFiler.DeliveryDate.Value ));
            int PickUpBoyID = 0;
            if (transactionDetailFiler.PickUpBoyID.HasValue)
            {
                PickUpBoyID = transactionDetailFiler.PickUpBoyID.Value;
            }
            SqlParameter param1 = new SqlParameter("@ToDate", transactionDetailFiler.DeliveryDate.Value);
            SqlParameter param2 = new SqlParameter("@PickUpBoyID", PickUpBoyID);
            var transationList = db.Database.SqlQuery<TransactionDetail>("SP_TransationDetails_Delivered @ToDate , @PickUpBoyID", param1, param2).ToList();

            foreach (var item in transationList)
            {
                TransactionDetailsVM obj = new TransactionDetailsVM();
                obj.TransactionDetailID = (Int64)item.TransactionDetailID;
                obj.LRNo = item.LRNo;
                obj.SenderName = item.SenderName;
                obj.ReceiverName = item.ReceiverName;
                obj.Amount = (decimal)item.Amount;
                obj.Cartage = (decimal)item.Cartage;
                total += obj.Amount;
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
                 
                if (item.TransactionDate.HasValue)
                {
                    obj.TransactionDateS = item.TransactionDate.Value.ToString("dd/MM/yyyy");
                }
                if (item.PickUpDate.HasValue)
                {
                    obj.PickUpDateS = item.PickUpDate.Value.ToString("dd/MM/yyyy");
                }
                if (item.PickUpBoyID != null)
                {
                    var boy = await Task.Run(() => PickUpBoysList.FirstOrDefault(t => t.PickUpBoyID == item.PickUpBoyID));
                    if (PickUpBoysList != null)
                    {
                        obj.PickUpBoy = boy.PickUpBoyName;
                    }
                }
                if (item.PickUpByUserID != null)
                {
                    var login = await Task.Run(() => LoginList.FirstOrDefault(t => t.LoginID == item.PickUpByUserID));
                    if (login != null)
                    {
                        obj.PickUpCreateBy = login.FirstName+ " "+ login.LastName;
                    }
                }
                if (item.CreateUser != null)
                {
                    var login = await Task.Run(() => LoginList.FirstOrDefault(t => t.LoginID == item.CreateUser));
                    if (login != null)
                    {
                        obj.CreateUserString = login.FirstName+ " "+ login.LastName;
                    }
                }
                if (item.DeliveryByUserID != null)
                {
                    var login = await Task.Run(() => LoginList.FirstOrDefault(t => t.LoginID == item.DeliveryByUserID));
                    if (login != null)
                    {
                        obj.DeliveryByUserIDString = login.FirstName+ " "+ login.LastName;
                    }
                }
                if (item.DeliveryByCustomer)
                {
                    obj.ReceiverDetails = item.ReceiverDetails;
                }
                transations.Add(obj);
            }
            transationsObj.TotalAmount = total;
            transationsObj.TransactionDetailList = transations;

            return Json(transations, JsonRequestBehavior.AllowGet);
        } 
    }
}