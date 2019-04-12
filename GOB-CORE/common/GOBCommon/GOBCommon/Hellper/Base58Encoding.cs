using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GOBCommon.Hellper
{
    public static class Base58Encoding
    {
        #region Old

        //public const int CheckSumSizeInBytes = 4;

        //public static byte[] AddCheckSum(byte[] data)
        //{
        //    Contract.Requires<ArgumentNullException>(data != null);
        //    Contract.Ensures(Contract.Result<byte[]>().Length == data.Length + CheckSumSizeInBytes);
        //    byte[] checkSum = GetCheckSum(data);
        //    byte[] dataWithCheckSum = ArrayHelpers.ConcatArrays(data, checkSum);
        //    return dataWithCheckSum;
        //}

        ////Returns null if the checksum is invalid
        //public static byte[] VerifyAndRemoveCheckSum(byte[] data)
        //{
        //    Contract.Requires<ArgumentNullException>(data != null);
        //    Contract.Ensures(Contract.Result<byte[]>() == null || Contract.Result<byte[]>().Length + CheckSumSizeInBytes == data.Length);
        //    byte[] result = ArrayHelpers.SubArray(data, 0, data.Length - CheckSumSizeInBytes);
        //    byte[] givenCheckSum = ArrayHelpers.SubArray(data, data.Length - CheckSumSizeInBytes);
        //    byte[] correctCheckSum = GetCheckSum(result);
        //    if (givenCheckSum.SequenceEqual(correctCheckSum))
        //        return result;
        //    else
        //        return null;
        //}

        //private const string Digits = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        //public static string Encode(byte[] data)
        //{
        //    Contract.Requires<ArgumentNullException>(data != null);
        //    Contract.Ensures(Contract.Result<string>() != null);

        //    // Decode byte[] to BigInteger
        //    BigInteger intData = 0;
        //    for (int i = 0; i < data.Length; i++)
        //    {
        //        intData = intData * 256 + data[i];
        //    }

        //    // Encode BigInteger to Base58 string
        //    string result = "";
        //    while (intData > 0)
        //    {
        //        int remainder = (int)(intData % 58);
        //        intData /= 58;
        //        result = Digits[remainder] + result;
        //    }

        //    // Append `1` for each leading 0 byte
        //    for (int i = 0; i < data.Length && data[i] == 0; i++)
        //    {
        //        result = '1' + result;
        //    }
        //    return result;
        //}

        //public static string EncodeWithCheckSum(byte[] data)
        //{
        //    Contract.Requires<ArgumentNullException>(data != null);
        //    Contract.Ensures(Contract.Result<string>() != null);
        //    return Encode(AddCheckSum(data));
        //}

        //public static byte[] Decode(string s)
        //{
        //    Contract.Requires<ArgumentNullException>(s != null);
        //    Contract.Ensures(Contract.Result<byte[]>() != null);

        //    // Decode Base58 string to BigInteger 
        //    BigInteger intData = 0;
        //    for (int i = 0; i < s.Length; i++)
        //    {
        //        int digit = Digits.IndexOf(s[i]); //Slow
        //        if (digit < 0)
        //            throw new FormatException(string.Format("Invalid Base58 character `{0}` at position {1}", s[i], i));
        //        intData = intData * 58 + digit;
        //    }

        //    // Encode BigInteger to byte[]
        //    // Leading zero bytes get encoded as leading `1` characters
        //    int leadingZeroCount = s.TakeWhile(c => c == '1').Count();
        //    var leadingZeros = Enumerable.Repeat((byte)0, leadingZeroCount);
        //    var bytesWithoutLeadingZeros =
        //        intData.ToByteArray()
        //        .Reverse()// to big endian
        //        .SkipWhile(b => b == 0);//strip sign byte
        //    var result = leadingZeros.Concat(bytesWithoutLeadingZeros).ToArray();
        //    return result;
        //}

        //// Throws `FormatException` if s is not a valid Base58 string, or the checksum is invalid
        //public static byte[] DecodeWithCheckSum(string s)
        //{
        //    Contract.Requires<ArgumentNullException>(s != null);
        //    Contract.Ensures(Contract.Result<byte[]>() != null);
        //    var dataWithCheckSum = Decode(s);
        //    var dataWithoutCheckSum = VerifyAndRemoveCheckSum(dataWithCheckSum);
        //    if (dataWithoutCheckSum == null)
        //        throw new FormatException("Base58 checksum is invalid");
        //    return dataWithoutCheckSum;
        //}

        //private static byte[] GetCheckSum(byte[] data)
        //{
        //    Contract.Requires<ArgumentNullException>(data != null);
        //    Contract.Ensures(Contract.Result<byte[]>() != null);

        //    System.Security.Cryptography.SHA256 sha256 = new SHA256Managed();
        //    byte[] hash1 = sha256.ComputeHash(data);
        //    byte[] hash2 = sha256.ComputeHash(hash1);

        //    var result = new byte[CheckSumSizeInBytes];
        //    Buffer.BlockCopy(hash2, 0, result, 0, result.Length);

        //    return result;
        //}

        #endregion

        #region New

        private const int CHECK_SUM_SIZE = 4;
        private const string DIGITS = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        /// <summary>
        /// Encodes data with a 4-byte checksum
        /// </summary>
        /// <param name="data">Data to be encoded</param>
        /// <returns></returns>
        public static string Encode(byte[] data)
        {
            return EncodePlain(AddCheckSum(data));
        }

        /// <summary>
        /// Encodes data in plain Base58, without any checksum.
        /// </summary>
        /// <param name="data">The data to be encoded</param>
        /// <returns></returns>
        public static string EncodePlain(byte[] data)
        {
            // Decode byte[] to BigInteger
            var intData = data.Aggregate<byte, BigInteger>(0, (current, t) => current * 256 + t);

            // Encode BigInteger to Base58 string
            var result = string.Empty;
            while (intData > 0)
            {
                var remainder = (int)(intData % 58);
                intData /= 58;
                result = DIGITS[remainder] + result;
            }

            // Append `1` for each leading 0 byte
            for (var i = 0; i < data.Length && data[i] == 0; i++)
            {
                result = '1' + result;
            }

            return result;
        }

        /// <summary>
        /// Decodes data in Base58Check format (with 4 byte checksum)
        /// </summary>
        /// <param name="data">Data to be decoded</param>
        /// <returns>Returns decoded data if valid; throws FormatException if invalid</returns>
        public static byte[] Decode(string data)
        {
            var dataWithCheckSum = DecodePlain(data);
            var dataWithoutCheckSum = _VerifyAndRemoveCheckSum(dataWithCheckSum);

            if (dataWithoutCheckSum == null)
            {
                throw new FormatException("Base58 checksum is invalid");
            }

            return dataWithoutCheckSum;
        }

        /// <summary>
        /// Decodes data in plain Base58, without any checksum.
        /// </summary>
        /// <param name="data">Data to be decoded</param>
        /// <returns>Returns decoded data if valid; throws FormatException if invalid</returns>
        public static byte[] DecodePlain(string data)
        {
            // Decode Base58 string to BigInteger 
            BigInteger intData = 0;
            for (var i = 0; i < data.Length; i++)
            {
                var digit = DIGITS.IndexOf(data[i]); //Slow

                if (digit < 0)
                {
                    throw new FormatException(string.Format("Invalid Base58 character `{0}` at position {1}", data[i], i));
                }

                intData = intData * 58 + digit;
            }

            // Encode BigInteger to byte[]
            // Leading zero bytes get encoded as leading `1` characters
            var leadingZeroCount = data.TakeWhile(c => c == '1').Count();
            var leadingZeros = Enumerable.Repeat((byte)0, leadingZeroCount);
            var bytesWithoutLeadingZeros =
              intData.ToByteArray()
              .Reverse()// to big endian
              .SkipWhile(b => b == 0);//strip sign byte
            var result = leadingZeros.Concat(bytesWithoutLeadingZeros).ToArray();

            return result;
        }

        public static byte[] AddCheckSum(byte[] data)
        {
            byte[] checkSum = _GetCheckSum(data);
            var dataWithCheckSum = ArrayHelpers.ConcatArrays(data, checkSum);

            return dataWithCheckSum;
        }

        //Returns null if the checksum is invalid
        private static byte[] _VerifyAndRemoveCheckSum(byte[] data)
        {
            var result = ArrayHelpers.SubArray(data, 0, data.Length - CHECK_SUM_SIZE);
            var givenCheckSum = ArrayHelpers.SubArray(data, data.Length - CHECK_SUM_SIZE);
            var correctCheckSum = _GetCheckSum(result);

            return givenCheckSum.SequenceEqual(correctCheckSum) ? result : null;
        }

        private static byte[] _GetCheckSum(byte[] data)
        {
            System.Security.Cryptography.SHA256 sha256 = new SHA256Managed();
            var hash1 = sha256.ComputeHash(data);
            var hash2 = sha256.ComputeHash(hash1);

            var result = new byte[CHECK_SUM_SIZE];
            Buffer.BlockCopy(hash2, 0, result, 0, result.Length);

             return result;
        }

        #endregion
    }
}
