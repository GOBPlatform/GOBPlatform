using System;

namespace GOBCrypto
{
    public sealed class PKCS5Padding
    {
        public static byte[] Pad(byte[] indata, int blockSize)
        {
            if (indata == null)
                return null;

            int offset = indata.Length;
            int len = blockSize - (offset % blockSize);

            byte paddingOctet = (byte)(len & 0xff);

            byte[] outdata = new byte[offset + len];

            Array.Copy(indata, 0, outdata, 0, indata.Length);
            for (int i = offset; i < outdata.Length; i++)
            {
                outdata[i] = paddingOctet;
            }

            return outdata;
        }

        public static byte[] UnPad(byte[] indata, int blockSize)
        {
            if (indata == null)
                return null;

            int len = indata.Length;
            byte lastByte = indata[len - 1];
            int padValue = (int)(lastByte & 0xff);

            if ((padValue < 0x01) || (padValue > blockSize))
            {
                return null;
            }

            int offset = len - padValue;

            for (int i = offset; i < len; i++)
            {
                if (indata[i] != padValue)
                    return null;
            }

            byte[] outdata = new byte[offset];

            Array.Copy(indata, 0, outdata, 0, offset);

            return outdata;
        }
    }
}
