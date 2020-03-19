using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static JalaramTravels.Models.Common;

namespace JalaramTravels.ViewModel
{
    public class UploadVM
    {
        //public List<BusVM> BusList { get; set; }
        //public List<KeyValuePair> BusList1 { get; set; }
        //public Int32 BusID2 { get; set; }

        public IEnumerable<SelectListItem> BusList { get; set; }
        public Int32 BusID { get; set; }
        public String DriverName { get; set; }
        public DateTime? TransactionDate { get; set; }

        public IEnumerable<SelectListItem> SenderCityList { get; set; }
        public Int64 SenderCityID { get; set; }
    }
}