using System;

namespace GOBCrypto
{
    public class InvalidKeyException : Exception
    {
        public InvalidKeyException(string message)
            : base(message)
        {
        }
    }
}
