using Common.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class SecureResource : ISecureResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IDictionary<string, ICollection<Role>> AccessControl { get; set; }
    }
}
