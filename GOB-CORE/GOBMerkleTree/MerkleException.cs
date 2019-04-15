using System;
using System.Collections.Generic;
using System.Text;

namespace GOBMerkleTree
{
    public class MerkleException : ApplicationException
    {
        public MerkleException(string msg) : base(msg)
        {
        }
    }
}
