using JalaramTravels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JalaramTravels.ViewModel
{
    public class DashboardVM
    {
        public Int32 TotalBus { get; set; }
        public int TotalCustomer { get; set; }
        public Int32 TotalParcel { get; set; }
        public Int32 TotalPaid { get; set; }
        public Int32 TotalTopay { get; set; }


        public Int32 TotalDelivered { get; set; }
        public Int32 TotalUndelivered { get; set; }
        public Int32 TotalInTransit { get; set; }

        public List<CustomerVM> CustomerList { get; set; }
    }


}