using Common.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class HomesUser : ITransactionType
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int SecurityLevel { get; set; }
        public decimal ProfitCenter { get; set; }
        public decimal? Spins { get; set; }
        public string ActiveRecord { get; set; }
        public string SalesPersonId { get; set; }
    }
}
