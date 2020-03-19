using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JalaramTravels.ViewModel
{
    public class CustomerVM
    {
        public Int64 CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNumber { get; set; }
        public List<CustomerVM> CustomerList1 { get; set; }

        public IEnumerable<SelectListItem> CityList { get; set; }

        public Int64 CustomerCityID { get; set; }
        public string CustomerCityName { get; set; }
    }
}