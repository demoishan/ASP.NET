using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Exceptions
{
    public class HomesUserException : Exception
    {
        public HomesUserException() { }
        public HomesUserException(string message) : base(message) { }
    }
}
