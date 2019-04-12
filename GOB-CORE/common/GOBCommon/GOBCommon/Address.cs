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
    public class Address
    {
        private const int ADDR_LEN = 20;

        private static byte[] _arrAddress;

        public static Address _address = null;
        public static byte[] ADDRESS_EMPTY = new byte[ADDR_LEN];

        public static byte[] ArrAddress { get => _arrAddress; set => _arrAddress = value; }

        /// <summary>
        /// Constrcutor for singleton pattern
        /// </summary>
        private Address()
        {
            _arrAddress = new byte[ADDR_LEN];
        }

        /// <summary>
        /// singleton pattern
        /// </summary>
        /// <returns></returns>
        public static Address GetInstance()
        {
            if (_address == null) _address = new Address();
            return _address;
        }

        public string ToHexString()
        {
            //string key = X509Certificate2.CreateFromCertFile("aaaaa").GetPublicKeyString();
            //string aa = Encoding.UTF8.GetBytes(key).ToBase58();
            return Encoding.UTF8.GetString(Common.ToArrayReverse(_arrAddress));
        }

        public void Serialize(StreamWriter writer)
        {
            try
            {
                writer.Write(_arrAddress);
            }
            catch
            {
                throw;
            }
        }

        public void Deserialize(StreamReader reader)
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

        //CheckSum Data가 붙은...24바이트 데이터를 String으로 인코딩하여 리턴한다.
        public string ToBase58()
        {
            byte[] data = Base58Encoding.AddCheckSum(_arrAddress);
            return Base58Encoding.Encode(data);
        }

        public byte[] AddressParseFrombytes(byte[] f)
        {
            if(f.Length != ADDR_LEN)
            {
                return ADDRESS_EMPTY;
            }
            Buffer.BlockCopy(f, 0, _arrAddress, 0, ADDR_LEN);
            return _arrAddress;
        }

        public byte[] AddressFromHexString(string s)
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

        public byte[] AddressFromBase58(string encoded)
        {
            if(encoded == "") return new byte[ADDR_LEN];
            var decoded = Base58Encoding.Decode(encoded);
            if(decoded.Length != 1 + ADDR_LEN + 4 || decoded[0] != (byte)23)
            {
                return ADDRESS_EMPTY;
            }
            byte[] buf = new byte[ADDR_LEN];
            Buffer.BlockCopy(decoded, 1, buf, 0, ADDR_LEN);
            AddressParseFrombytes(buf);
            var sAddr = ToBase58();
            if (sAddr != encoded) return ADDRESS_EMPTY;
            return _arrAddress;
        }

        public byte[] AddressFromVmCode(byte[] code)
        {
            var temp = GOBCommon.Hellper.ExtSHA256.Hash(code);
            RIPEMD160 myRIPEMD160 = RIPEMD160Managed.Create();
            return myRIPEMD160.ComputeHash(temp);
        }
         
    }
}
