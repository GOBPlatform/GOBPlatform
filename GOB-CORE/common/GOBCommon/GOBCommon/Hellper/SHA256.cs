﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace GOBCommon.Hellper
{
    public class ExtSHA256
    {
        private static readonly SHA256Managed sha256 = new SHA256Managed();

        public static byte[] Hash(byte[] data)
        {
            return sha256.ComputeHash(data);
        }

        public static byte[] DoubleHash(byte[] data)
        {
            return sha256.ComputeHash(sha256.ComputeHash(data));
        }

        public static byte[] DoubleHashCheckSum(byte[] data)
        {
            byte[] checksum = DoubleHash(data);
            Array.Resize(ref checksum, 4);
            return checksum;
        }

        public static byte[] Hash(string hexData)
        {
            byte[] bytes = Hex.HexToBytes(hexData);
            return Hash(bytes);
        }

        public static byte[] DoubleHash(string hexData)
        {
            byte[] bytes = Hex.HexToBytes(hexData);
            return DoubleHash(bytes);
        }

        public static byte[] DoubleHashCheckSum(string hexData)
        {
            byte[] bytes = Hex.HexToBytes(hexData);
            return DoubleHashCheckSum(bytes);
        }

        public static string StringHash(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            return BitConverter.ToString(Hash(bytes)).Replace("-", "");
        }
    }
}
