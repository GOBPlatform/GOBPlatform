using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GOBBlockchain.Block
{
    public class BlockHeader : IBlockHeader
    {
        private const string BLOCK_VER_1 = "v.1.9.4.16";

        public BlockHeader(string sign)
        {
            Sign = sign;
            TimeStamp = DateTime.Now;
        }

        #region Implements IBlockHeader interface

        public string Version => BLOCK_VER_1;

        public int BlockSize { get; set; }

        public byte[] PrevHash { get; set; }

        public DateTime TimeStamp { get; }

        public byte[] MerkleRootHash { get; set; }

        public string Sign { get; }

        public byte[] ToByteArray()
        {
            using (MemoryStream st = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(st))
            {
                bw.Write(Version);
                bw.Write(BlockSize);
                bw.Write(MerkleRootHash);
                bw.Write(TimeStamp.ToBinary());
                bw.Write(PrevHash);
                bw.Write(Sign);
                return st.ToArray();
            }
        }

        #endregion

    }
}
