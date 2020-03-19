using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static JalaramTravels.Models.Enums;

namespace JalaramTravels.ViewModel
{
    public class TransactionDetailVM
    {
        public List<TransactionDetailsVM> TransactionDetailList { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? TransactionDateEnd { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public IEnumerable<SelectListItem> PickUpBoyList { get; set; }
        public IEnumerable<SelectListItem> BusList { get; set; }
        public Int32 BusID { get; set; }
        public Int32 TempoID { get; set; }
        public Int32 PickUpBoyID { get; set; }
        public decimal TotalAmount { get; set; }

        public int DeliveryByUserID { get; set; }
        public IEnumerable<SelectListItem> DeliveredByList { get; set; }
    }
    public class TransactionDetailFiler
    {
        public DateTime? TransactionDate { get; set; }
        public DateTime? TransactionDateEnd { get; set; }
        public Int32 BusID { get; set; }

        public Int32 TempoID { get; set; }
        public Int32? PickUpBoyID { get; set; }

        public DateTime? DeliveryDate { get; set; }
    }

    public class InTransitVM
    {
        public Int32? BusID { get; set; }
        public IEnumerable<SelectListItem> BusList { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? PickUpDate { get; set; }
        public DateTime? PickUpDateEnd { get; set; }
        public IEnumerable<SelectListItem> PickUpBoyList { get; set; }
        public DeliverdStatus DeliverdStatus { get; set; }
        public Int32? PickUpBoyID { get; set; }
        public List<TransactionDetailsVM> TransactionDetailList { get; set; }
    }


    public class TransactionDetailsVM
    {
        public Int64 Id { get; set; }
        public string StatusFlag { get; set; }

        public Int64 TransactionDetailID { get; set; }
        public Int64 TransactionMasterID { get; set; }
        public string LRNo { get; set; }
        public int NoOfParcel { get; set; }
        public Int64 SenderID { get; set; }
        public string SenderName { get; set; }
        public string SenderNumber { get; set; }
        public Int64 SenderCityID { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverNumber { get; set; }
        public Int64 ReceiverCityID { get; set; }
        public string ReceiverCity { get; set; }
        public Int64 ReceiverID { get; set; }
        public decimal Amount { get; set; }
        public decimal Cartage { get; set; }
        public decimal Hamali { get; set; }
        public decimal Damrage { get; set; }
        public PayTypes PayType { get; set; }
        public PaymentTypes PaymentType { get; set; }
        public string PayTypeString { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string DeliveryDateS { get; set; }
        public Int32 StaffID { get; set; }
        public Int32 BusID { get; set; }
        public string BusName { get; set; }
        public string BusNumber { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public Int32 CreateUser { get; set; }
        public Int32 UpdateUser { get; set; }
        public string Flag { get; set; }
        public DeliverdStatus DeliverdStatus { get; set; }

        public string DeliverdStatusString { get; set; }
        public string DriverName { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionDateS { get; set; }
        public DateTime? PickUpDate { get; set; }
        public string PickUpDateS { get; set; }
        public string PickUpBoy { get; set; }

        public string CreateUserString { get; set; }

        public string PickUpCreateBy { get; set; }

        public Int32? ParcelTypeID { get; set; }
        public Int32? ParcelContainerID { get; set; }

        public string ParcelTypeString { get; set; }
        public string ParcelContainerString { get; set; }

        public IEnumerable<SelectListItem> ParcelContainerList { get; set; }
        public IEnumerable<SelectListItem> ParcelTypeList { get; set; }

        public int DeliveryByUserID { get; set; }

        public string DeliveryByUserIDString { get; set; }

        public string ReceiverDetails { get; set; }
    }

    public class InTransitCharges
    {
        public IEnumerable<SelectListItem> PickUpBoyList { get; set; }
        public Int32 TempoID { get; set; }
        public DateTime? PickUpDate { get; set; }
        public string PickUpDateS { get; set; }

        public string ReportHeder { get; set; }

        public CompnayDetail Cmp { get; set; }
        public List<InTransitChargesDetails> inTransitChargesDetails { get; set; }

         public decimal GrandTotal { get; set; }
    }
    public class InTransitChargesDetails
    {
        public Int64 TransactionDetailID { get; set; }
        public Int64 TransactionMasterID { get; set; }

        public PayTypes PayType { get; set; }
        public string LRNo { get; set; }
        public decimal Amount { get; set; }
        public decimal Cartage { get; set; }
        public decimal? Hamali { get; set; }
        public decimal? Damrage { get; set; }
        public Int32? ParcelTypeID { get; set; }
        public Int32? ParcelContainerID { get; set; }
        public int NoOfParcel { get; set; }

        public decimal? Total { get; set; }
        public string ReceiverName { get; set; }
        public string SenderName { get; set; }

        public string SenderNumber { get; set; }

        public string ReceiverNumber { get; set; }
         public string PayTypeString { get; set; }
        public IEnumerable<SelectListItem> ParcelContainerList { get; set; }
        public IEnumerable<SelectListItem> ParcelTypeList { get; set; }
    }
    public class TransactionPaymentVM
    {
        public IEnumerable<SelectListItem> BusList { get; set; }
        public Int32 BusID { get; set; }
        public Int64 TransactionMasterID { get; set; }
        public int TotalParcel { get; set; }
        public decimal TotalTopay { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalDamrage { get; set; }
        public decimal TotalCartage { get; set; }
        public decimal Amount { get; set; }
        public string TopayDriverName { get; set; }
        public DateTime TopayDate { get; set; }
        public DeliverdStatus DeliverdStatus { get; set; }
    }

    public class UploadFiles
    {
        public List<TransactionDetailVM> Transactions { get; set; }
    }

    public class TransactionVM
    {
        public IEnumerable<SelectListItem> PickUpBoyList { get; set; }
        public IEnumerable<SelectListItem> BusList { get; set; }
        public IEnumerable<SelectListItem> SenderCityList { get; set; }
        public IEnumerable<SelectListItem> ReceiverCityList { get; set; }
        public IEnumerable<SelectListItem> ParcelContainerList { get; set; }
        public IEnumerable<SelectListItem> ParcelTypeList { get; set; }

        public Int64 TransactionDetailID { get; set; }
        public Int64 TransactionMasterID { get; set; }
        public string LRNo { get; set; }
        public int? NoOfParcel { get; set; }

        public string SenderName { get; set; }
        public string SenderNumber { get; set; }
        public Int64 SenderCityID { get; set; }
        public string SenderCity { get; set; }
        public Int64 SenderID { get; set; }

        public string ReceiverName { get; set; }
        public string ReceiverNumber { get; set; }
        public Int64 ReceiverCityID { get; set; }
        public string ReceiverCity { get; set; }
        public Int64 ReceiverID { get; set; }

        public decimal? FinalAmount { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Cartage { get; set; }
        public decimal? Hamali { get; set; }
        public decimal? Damrage { get; set; }
        public PayTypes PayType { get; set; }
        public string PayTypeString { get; set; }
        public PaymentTypes PaymentType { get; set; }
        public string PaymentTypesString { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string DeliveryDateS { get; set; }
        public Int32 DeliveryUserID { get; set; }
        //public ParcelType ParcelType { get; set; }
        public Int32? ParcelTypeID { get; set; }
        public Int32? ParcelContainerID { get; set; }
        public Int32 StaffID { get; set; }
        public Int32 BusID { get; set; }
        public string BusName { get; set; }
        public string BusNumber { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public Int32 CreateUser { get; set; }
        public Int32 UpdateUser { get; set; }
        public string Flag { get; set; }
        public DeliverdStatus DeliverdStatus { get; set; }
        public string DeliverdStatusString { get; set; }
        public string DriverName { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}")]
        public DateTime? TransactionDate { get; set; }
        public string TransactionDateS { get; set; }
        public DateTime? PickUpDate { get; set; }
        public string PickUpDateS { get; set; }
        public string PickUpBy { get; set; }
        public int PickUpBoyID { get; set; }

        public string CreateUserS { get; set; }

        public string ParcelTypeS { get; set; }
        public string ParcelContainerS { get; set; }

        public CompnayDetail Cmp { get; set; }

        public int RoleID { get; set; }

        public bool DeliveryByCustomer { get; set; }

        public string ReceiverDetails { get; set; }

        public string DeliveryByUserIDString { get; set; }
    }

    public class CompnayDetail
    {
        public string CompnayName { get; set; }
        public string CompnayAddress { get; set; }
        public string CompnayNumber { get; set; }
        public string CompnayEmail { get; set; }

        public DateTime? PrintDate { get; set; }
    }

    public class rptList
    {
        public List<rptCustomer> rptCustomer { get; set; }
    }
    public class rptCustomer
    {
        public string LRNo { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNumber { get; set; }
        public decimal Amount { get; set; }
        public decimal Hamali { get; set; }
        public decimal Total { get; set; }
        public decimal Damrage { get; set; }
        public string PayTypeStatus { get; set; }
        public int NoOfParcel { get; set; }
    }
}