using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.Tempus
{
    public class TempusException : Exception
    {
        public TempusException() : base()
        {
        }

        public TempusException(string message) : base(message)
        {
        }

        public TempusException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
