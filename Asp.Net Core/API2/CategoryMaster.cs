using DemoDapper.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoDapper.Models
{
    public class CategoryMaster : ITransactionType
    {
        public int CatId { get; set; }
        public string  CatName { get; set; }
        public bool Flag { get; set; }
    }
}