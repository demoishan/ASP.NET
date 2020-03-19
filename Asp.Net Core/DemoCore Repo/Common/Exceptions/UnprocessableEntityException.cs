using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Exceptions
{
    public class UnprocessableEntityException : Exception
    {
        public UnprocessableEntityException() { }
        public UnprocessableEntityException(string message) : base(message) { }
        public UnprocessableEntityException(string message, Exception inner) : base(message, inner) { }
    }
}
