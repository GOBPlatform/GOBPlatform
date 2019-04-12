using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GOBCommon
{
    public class Common
    {
        public static UInt64 GetNonce()
        {
            var ByteArray = new byte[8]; //Nonce 값을 만들어낼 byte 배열(uint64)
            using (var Rnd = RandomNumberGenerator.Create())
            {
                Rnd.GetBytes(ByteArray);
            }
            UInt64 nonce = BitConverter.ToUInt64(ByteArray, 0);

            return nonce;
        }

        public static string ToHexString(byte[] barray)
        {
            char[] c = new char[barray.Length * 2];
            byte b;
            for (int i = 0; i < barray.Length; ++i)
            {
                b = ((byte)(barray[i] >> 4));
                c[i * 2] = (char)(b > 9 ? b + 0x37 : b + 0x30);
                b = ((byte)(barray[i] & 0xF));
                c[i * 2 + 1] = (char)(b > 9 ? b + 0x37 : b + 0x30);
            }
            return new string(c);
        }

        public static byte[] HexToByte(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
   
        public static byte[] ToArrayReverse(byte[] arr)
        {
            Array.Reverse(arr);
            return arr;
        }
    }
}
