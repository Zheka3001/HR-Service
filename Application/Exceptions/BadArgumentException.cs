using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public sealed class BadArgumentException : Exception
    {
        public BadArgumentException()
        {
        }

        public BadArgumentException(string message) : base(message)
        {
        }

        public BadArgumentException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
