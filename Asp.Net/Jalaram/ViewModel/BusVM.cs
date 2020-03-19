using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JalaramTravels.ViewModel
{
    public class BusVM
    {
        public Int32 BusID { get; set; }
        public String BusName { get; set; }
        public String BusNumber { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Int32? CreateUser { get; set; }
        public Int32? UpdateUser { get; set; }
        public String Flag { get; set; }
    }
}