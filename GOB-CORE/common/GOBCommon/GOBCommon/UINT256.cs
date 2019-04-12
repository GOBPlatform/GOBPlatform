using GOBCommon.Hellper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GOBCommon
{
    public class UINT256
    {
        const int UINT16_SIZE = 2;
        const int UINT32_SIZE = 4;
        const int UINT64_SIZE = 8;
        const int UINT256_SIZE = 32;

        private static UINT256 _uint256;
        private Uint256Type uint256;


        private UINT256()
        {
            Uint256 = new Uint256Type();
        }

        public Uint256Type Uint256 { get => uint256; set => uint256 = value; }

        public static UINT256 GetInstance()
        {
            if (_uint256 == null) _uint256 = new UINT256();
            return _uint256;
        }

        public Uint256Type Uint256ParseFromBytes(byte[] f)
        {
            if (f.Length != UINT256_SIZE) return new Uint256Type();
            Uint256Type hash = new Uint256Type();
            Buffer.BlockCopy(f, 0, hash.UInt256, 0, UINT256_SIZE);
            return hash;
        }

        public Uint256Type Uint256FromHexString(string s)
        {
            try
            {
                var hx = Hex.HexToBytes(s);
                return Uint256ParseFromBytes(hx);
            }
            catch
            {
                throw;
            }
        }

        public class Uint256Type
        {
            private byte[] uInt256 = null;

            public Uint256Type()
            {
                uInt256 = new byte[UINT256_SIZE];
            }

            public byte[] UInt256
            {
                get => uInt256;
                set
                {
                    if (value.Length != UINT256_SIZE) Buffer.BlockCopy(value, 0, UInt256, 0, UINT256_SIZE);
                    else uInt256 = value;
                }
            }

            public byte[] ToArray()
            {
                byte[] buf = new byte[UINT256_SIZE];
                Buffer.BlockCopy(buf, 0, UInt256, 0, UINT256_SIZE);
                return buf;
            }

            public string ToHexString()
            {
                return Encoding.UTF8.GetString(Common.ToArrayReverse(uInt256));
            }

            public void Serialize(StreamWriter writer)
            {
                //TODO : 실행되는 로직에 따라 코드를 작성해야함. 일단 보류
            }

            public void Deserialize(StreamReader reader)
            {
                //TODO : 실행되는 로직에 따라 코드를 작성해야함. 일단 보류
            }


        }
    }
}
