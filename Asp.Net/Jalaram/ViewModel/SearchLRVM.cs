using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JalaramTravels.ViewModel
{
    public class SearchLRVM
    {
        public string LRNo { get; set; }
        public List<TransactionVM> TransactionsList { get; set; }
    }
}