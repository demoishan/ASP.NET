using Common.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class Role : ITransactionType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
