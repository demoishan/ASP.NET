using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JalaramTravels.ViewModel
{
    public class PickUpBoyVM
    {
        public Int32 PickUpBoyID { get; set; }
        public String PickUpBoyName { get; set; }
        public Decimal? PickUpBoyNumber { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Int32? CreateUser { get; set; }
        public Int32? UpdateUser { get; set; }
        public String Flag { get; set; }
    }
}