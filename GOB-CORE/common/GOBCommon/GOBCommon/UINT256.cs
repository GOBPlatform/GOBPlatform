using GOBCommon.Hellper;
using NBitcoin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GOBCommon
{
    //public class UINT256
    //{
    //    const int UINT16_SIZE = 2;
    //    const int UINT32_SIZE = 4;
    //    const int UINT64_SIZE = 8;
    //    const int UINT256_SIZE = 32;

    //    private static UINT256 _uint256;
    //    private Uint256Type uint256;


    //    private UINT256()
    //    {
    //        Uint256 = new Uint256Type();
    //    }

    //    public Uint256Type Uint256 { get => uint256; set => uint256 = value; }

    //    public static UINT256 GetInstance()
    //    {
    //        if (_uint256 == null) _uint256 = new UINT256();
    //        return _uint256;
    //    }

    //    public Uint256Type Uint256ParseFromBytes(byte[] f)
    //    {
    //        if (f.Length != UINT256_SIZE) return new Uint256Type();
    //        Uint256Type hash = new Uint256Type();
    //        Buffer.BlockCopy(f, 0, hash.UInt256, 0, UINT256_SIZE);
    //        return hash;
    //    }

    //    public Uint256Type Uint256FromHexString(string s)
    //    {
    //        try
    //        {
    //            var hx = Hex.HexToBytes(s);
    //            return Uint256ParseFromBytes(hx);
    //        }
    //        catch
    //        {
    //            throw;
    //        }
    //    }

    //    public class Uint256Type
    //    {
    //        private byte[] uInt256 = null;

    //        public Uint256Type()
    //        {
    //            uInt256 = new byte[UINT256_SIZE];
    //        }

    //        public byte[] UInt256
    //        {
    //            get => uInt256;
    //            set
    //            {
    //                if (value.Length != UINT256_SIZE) Buffer.BlockCopy(value, 0, UInt256, 0, UINT256_SIZE);
    //                else uInt256 = value;
    //            }
    //        }

    //        public byte[] ToArray()
    //        {
    //            byte[] buf = new byte[UINT256_SIZE];
    //            Buffer.BlockCopy(buf, 0, UInt256, 0, UINT256_SIZE);
    //            return buf;
    //        }

    //        public string ToHexString()
    //        {
    //            return Encoding.UTF8.GetString(Common.ToArrayReverse(uInt256));
    //        }

    //        public void Serialize(StreamWriter writer)
    //        {
    //            //TODO : 실행되는 로직에 따라 코드를 작성해야함. 일단 보류
    //        }

    //        public void Deserialize(StreamReader reader)
    //        {
    //            //TODO : 실행되는 로직에 따라 코드를 작성해야함. 일단 보류
    //        }


    //    }
    //}

    public class uint256 : IEquatable<uint256>
    {
        public static Hex hex;
        private const int WIDTH_BYTE = 256 / 8;
        static readonly uint256 _Zero = new uint256();
        static readonly uint256 _One = new uint256();
        internal readonly UInt32 pn0;
        internal readonly UInt32 pn1;
        internal readonly UInt32 pn2;
        internal readonly UInt32 pn3;
        internal readonly UInt32 pn4;
        internal readonly UInt32 pn5;
        internal readonly UInt32 pn6;
        internal readonly UInt32 pn7;

        /// <summary>
        /// uint256 default Constructor
        /// </summary>
        public uint256()
        {
            hex = new Hex();
        }

        public uint256(uint256 b)
        {
            pn0 = b.pn0;
            pn1 = b.pn1;
            pn2 = b.pn2;
            pn3 = b.pn3;
            pn4 = b.pn4;
            pn5 = b.pn5;
            pn6 = b.pn6;
            pn7 = b.pn7;
        }

        public uint256(ulong b)
        {
            pn0 = (uint)b;
            pn1 = (uint)(b >> 32);
            pn2 = 0;
            pn3 = 0;
            pn4 = 0;
            pn5 = 0;
            pn6 = 0;
            pn7 = 0;
        }

        public uint256(string str)
        {
            pn0 = 0;
            pn1 = 0;
            pn2 = 0;
            pn3 = 0;
            pn4 = 0;
            pn5 = 0;
            pn6 = 0;
            pn7 = 0;
            str = str.Trim();

            if (str.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                str = str.Substring(2);

            var bytes = hex.DecodeData(str);
            Array.Reverse(bytes);
            if (bytes.Length != WIDTH_BYTE)
                throw new FormatException("Invalid hex length");
            pn0 = Utils.ToUInt32(bytes, 4 * 0, true);
            pn1 = Utils.ToUInt32(bytes, 4 * 1, true);
            pn2 = Utils.ToUInt32(bytes, 4 * 2, true);
            pn3 = Utils.ToUInt32(bytes, 4 * 3, true);
            pn4 = Utils.ToUInt32(bytes, 4 * 4, true);
            pn5 = Utils.ToUInt32(bytes, 4 * 5, true);
            pn6 = Utils.ToUInt32(bytes, 4 * 6, true);
            pn7 = Utils.ToUInt32(bytes, 4 * 7, true);

        }

        public uint256(byte[] vch, int offset, int length, bool lendian = true)
        {
            if (length != WIDTH_BYTE)
            {
                throw new FormatException("the byte array should be 32 bytes long");
            }

            if (!lendian)
            {
                if (length != vch.Length)
                    vch = vch.Take(32).ToArray();
                vch = vch.Reverse().ToArray();
            }

            pn0 = Utils.ToUInt32(vch, offset + 4 * 0, true);
            pn1 = Utils.ToUInt32(vch, offset + 4 * 1, true);
            pn2 = Utils.ToUInt32(vch, offset + 4 * 2, true);
            pn3 = Utils.ToUInt32(vch, offset + 4 * 3, true);
            pn4 = Utils.ToUInt32(vch, offset + 4 * 4, true);
            pn5 = Utils.ToUInt32(vch, offset + 4 * 5, true);
            pn6 = Utils.ToUInt32(vch, offset + 4 * 6, true);
            pn7 = Utils.ToUInt32(vch, offset + 4 * 7, true);

        }

        public uint256(byte[] vch, bool lendian = true) : this(vch, 0, vch.Length, lendian)
        {

        }

        public uint256(byte[] vch)
            : this(vch, true)
        {
        }

        #region Properties

        public static uint256 Zero => _Zero;

        public static uint256 One => _One;

        #endregion

        public static uint256 Parse(string hex)
        {
            return new uint256(hex);
        }

        public static bool TryParse(string hex, out uint256 result)
        {
            if (hex == null)
                throw new ArgumentNullException(nameof(hex));
            if (hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                hex = hex.Substring(2);
            result = null;
            if (hex.Length != WIDTH_BYTE * 2)
                return false;
            if (!Hex.IsValid(hex))
                return false;
            result = new uint256(hex);
            return true;
        }

        public override bool Equals(object obj)
        {
            var item = obj as uint256;
            return Equals(item);
        }

        /// <summary>
        /// IEquatable 인터페이스 구현
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(uint256 other)
        {
            if (other is null)
            {
                return false;
            }

            bool equals = true;
            equals &= pn0 == other.pn0;
            equals &= pn1 == other.pn1;
            equals &= pn2 == other.pn2;
            equals &= pn3 == other.pn3;
            equals &= pn4 == other.pn4;
            equals &= pn5 == other.pn5;
            equals &= pn6 == other.pn6;
            equals &= pn7 == other.pn7;
            return equals;
        }

        public static bool operator ==(uint256 a, uint256 b)
        {
            if (System.Object.ReferenceEquals(a, b))
                return true;
            if (((object)a == null) || ((object)b == null))
                return false;

            bool equals = true;
            equals &= a.pn0 == b.pn0;
            equals &= a.pn1 == b.pn1;
            equals &= a.pn2 == b.pn2;
            equals &= a.pn3 == b.pn3;
            equals &= a.pn4 == b.pn4;
            equals &= a.pn5 == b.pn5;
            equals &= a.pn6 == b.pn6;
            equals &= a.pn7 == b.pn7;
            return equals;
        }

        public static bool operator <(uint256 a, uint256 b)
        {
            return Comparison(a, b) < 0;
        }

        public static bool operator >(uint256 a, uint256 b)
        {
            return Comparison(a, b) > 0;
        }

        public static bool operator <=(uint256 a, uint256 b)
        {
            return Comparison(a, b) <= 0;
        }

        public static bool operator >=(uint256 a, uint256 b)
        {
            return Comparison(a, b) >= 0;
        }

        private static int Comparison(uint256 a, uint256 b)
        {
            if (a.pn7 < b.pn7)
                return -1;
            if (a.pn7 > b.pn7)
                return 1;
            if (a.pn6 < b.pn6)
                return -1;
            if (a.pn6 > b.pn6)
                return 1;
            if (a.pn5 < b.pn5)
                return -1;
            if (a.pn5 > b.pn5)
                return 1;
            if (a.pn4 < b.pn4)
                return -1;
            if (a.pn4 > b.pn4)
                return 1;
            if (a.pn3 < b.pn3)
                return -1;
            if (a.pn3 > b.pn3)
                return 1;
            if (a.pn2 < b.pn2)
                return -1;
            if (a.pn2 > b.pn2)
                return 1;
            if (a.pn1 < b.pn1)
                return -1;
            if (a.pn1 > b.pn1)
                return 1;
            if (a.pn0 < b.pn0)
                return -1;
            if (a.pn0 > b.pn0)
                return 1;
            return 0;
        }

        public static bool operator !=(uint256 a, uint256 b)
        {
            return !(a == b);
        }

        public static bool operator ==(uint256 a, ulong b)
        {
            return (a == new uint256(b));
        }

        public static bool operator !=(uint256 a, ulong b)
        {
            return !(a == new uint256(b));
        }

        public static implicit operator uint256(ulong value)
        {
            return new uint256(value);
        }


        public byte[] ToBytes(bool lendian = true)
        {
            var arr = new byte[WIDTH_BYTE];
            ToBytes(arr);
            if (!lendian)
                Array.Reverse(arr);
            return arr;
        }

        public void ToBytes(byte[] output)
        {
            Buffer.BlockCopy(Utils.ToBytes(pn0, true), 0, output, 4 * 0, 4);
            Buffer.BlockCopy(Utils.ToBytes(pn1, true), 0, output, 4 * 1, 4);
            Buffer.BlockCopy(Utils.ToBytes(pn2, true), 0, output, 4 * 2, 4);
            Buffer.BlockCopy(Utils.ToBytes(pn3, true), 0, output, 4 * 3, 4);
            Buffer.BlockCopy(Utils.ToBytes(pn4, true), 0, output, 4 * 4, 4);
            Buffer.BlockCopy(Utils.ToBytes(pn5, true), 0, output, 4 * 5, 4);
            Buffer.BlockCopy(Utils.ToBytes(pn6, true), 0, output, 4 * 6, 4);
            Buffer.BlockCopy(Utils.ToBytes(pn7, true), 0, output, 4 * 7, 4);
        }

        public byte GetByte(int index)
        {
            var uintIndex = index / sizeof(uint);
            var byteIndex = index % sizeof(uint);
            UInt32 value;
            switch (uintIndex)
            {
                case 0:
                    value = pn0;
                    break;
                case 1:
                    value = pn1;
                    break;
                case 2:
                    value = pn2;
                    break;
                case 3:
                    value = pn3;
                    break;
                case 4:
                    value = pn4;
                    break;
                case 5:
                    value = pn5;
                    break;
                case 6:
                    value = pn6;
                    break;
                case 7:
                    value = pn7;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("index");
            }
            return (byte)(value >> (byteIndex * 8));
        }

        public override string ToString()
        {
            var bytes = ToBytes();
            Array.Reverse(bytes);
            return hex.EncodeData(bytes, 0, bytes.Length);
        }

        public MutableUint256 AsBitcoinSerializable()
        {
            return new MutableUint256(this);
        }

        public int GetSerializeSize(int nType = 0, uint? protocolVersion = null)
        {
            return WIDTH_BYTE;
        }

        public int Size
        {
            get
            {
                return WIDTH_BYTE;
            }
        }

        public ulong GetLow64()
        {
            return pn0 | (ulong)pn1 << 32;
        }

        public uint GetLow32()
        {
            return pn0;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            unchecked
            {
                hash = hash * 31 + (int)pn0;
                hash = hash * 31 + (int)pn1;
                hash = hash * 31 + (int)pn2;
                hash = hash * 31 + (int)pn3;
                hash = hash * 31 + (int)pn4;
                hash = hash * 31 + (int)pn5;
                hash = hash * 31 + (int)pn6;
                hash = hash * 31 + (int)pn7;
            }
            return hash;
        }

        public class MutableUint256
        {
            uint256 _Value;
            public uint256 Value
            {
                get
                {
                    return _Value;
                }
                set
                {
                    _Value = value;
                }
            }
            public MutableUint256()
            {
                _Value = uint256.Zero;
            }
            public MutableUint256(uint256 value)
            {
                _Value = value;
            }

            public void ReadWrite(BitcoinStream stream)
            {
                if (stream.Serializing)
                {
                    var b = Value.ToBytes();
                    stream.ReadWrite(ref b);
                }
                else
                {
                    byte[] b = new byte[WIDTH_BYTE];
                    stream.ReadWrite(ref b);
                    _Value = new uint256(b);
                }
            }
        }
    }
}
