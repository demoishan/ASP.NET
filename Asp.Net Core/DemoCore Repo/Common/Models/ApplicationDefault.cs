using Common.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class ApplicationDefault : ITransactionType
    {
        public int default_id { get; set; }
        public string default_PC { get; set; }
        public string default_ProfitCenter { get; set; }
        public string default_Desc { get; set; }
        public string default_Value { get; set; }
        public bool Encrypted { get; set; }
    }
}
