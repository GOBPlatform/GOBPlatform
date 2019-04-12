using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GOBCommon;
using GOBCommon.Hellper;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System.Numerics;
using GOBCrypto.RIPEMD160;

namespace GOBCommon
{
    public static class Address
    {
        private const int ADDR_LEN = 20;
        //public byte[] address = new byte[ADDR_LEN];

        public static string ToHexString(this byte[] address)
        {
            //string key = X509Certificate2.CreateFromCertFile("aaaaa").GetPublicKeyString();
            //string aa = Encoding.UTF8.GetBytes(key).ToBase58();
            return Encoding.UTF8.GetString(Common.ToArrayReverse(address));
        }

        public static void Serialize(this byte[] address, StreamWriter writer)
        {
            try
            {
                writer.Write(address);
            }
            catch
            {
                throw;
            }
        }

        public static void Deserialize(this byte[] address, StreamReader reader)
        {
            //reader.Read(address, 0, 9);
            //TODO : 기능확인 해보고 구현하자. 일단은 빈 메소드
            try
            {

            }
            catch
            {
                throw;
            }
        }

        public static string ToBase58(this byte[] address)
        {
            byte[] data = Base58Encoding.AddCheckSum(address);
            return Base58Encoding.Encode(data);
        }


        public static byte[] AddressParseFrombytes(byte[] f)
        {
            if(f.Length != ADDR_LEN)
            {
                return new byte[ADDR_LEN];
            }
            var addr = new byte[ADDR_LEN];
            Buffer.BlockCopy(f, 0, addr, 0, ADDR_LEN);
            return addr;
        }

        public static byte[] AddressFromHexString(string s)
        {
            try
            {
                var hx = Hex.HexToBytes(s);
                return AddressParseFrombytes(hx);
            }
            catch
            {
                throw;
            }
        }

        public static byte[] AddressFromBase58(string encoded)
        {
            if(encoded == "") return new byte[ADDR_LEN];
            var decoded = Base58Encoding.Decode(encoded);
            if(decoded.Length != 1 + ADDR_LEN + 4 || decoded[0] != (byte)23)
            {
                return new byte[ADDR_LEN];
            }
            byte[] buf = new byte[ADDR_LEN];
            Buffer.BlockCopy(decoded, 1, buf, 0, ADDR_LEN);
            var ph = AddressParseFrombytes(buf);
            var addr = ToBase58(ph);
            if(addr != encoded) return new byte[ADDR_LEN];
            return ph;
        }

        public static byte[] AddressFromVmCode(byte[] code)
        {
            var temp = GOBCommon.Hellper.SHA256.Hash(code);
            RIPEMD160 myRIPEMD160 = RIPEMD160Managed.Create();
            //myRIPEMD160.ComputeHash();
            return null;
        }
         
        //func AddressFromVmCode(code[]byte) Address {
        //	var addr Address
        //    temp := sha256.Sum256(code)
        //    md := ripemd160.New()
        //    md.Write(temp[:])

        //    md.Sum(addr[:0])

        //	return addr
        //}
    }
}
