using Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Abstractions
{
    public interface ISecureResource : ITransactionType
    {
        int Id { get; set; }
        string Name { get; set; }

        IDictionary<string, ICollection<Role>> AccessControl { get; }
    }
}
