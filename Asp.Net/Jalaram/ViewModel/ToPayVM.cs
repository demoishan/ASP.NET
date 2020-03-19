using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static JalaramTravels.Models.Enums;

namespace JalaramTravels.ViewModel
{
    public class ToPayVM
    {
        public Int64 TransactionMasterID { get; set; }
        public Int32 BusID { get; set; }
        public string BusName { get; set; }
        public string BusNumber { get; set; }
        public decimal TotalTopay { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalCartage { get; set; }
        public decimal TotalDamrage { get; set; }
        public String TransactionDateS { get; set; }
        public TransactionStatus TransactionStatus { get; set; }

        public string TransactionStatusString { get; set; }
        public string CreateUserString { get; set; }


        public IEnumerable<SelectListItem> BusList { get; set; }

        public DateTime? TransactionDate { get; set; }
        public DateTime? TransactionDateEnd { get; set; }
    }

    public class ToPayVML
    {
        public List<ToPayVM> ToPayVMList { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? TransactionDateEnd { get; set; }
        public IEnumerable<SelectListItem> PickUpBoyList { get; set; }
        public IEnumerable<SelectListItem> BusList { get; set; }
        public Int32 BusID { get; set; }
        public Int32 TempoID { get; set; }

    }
}