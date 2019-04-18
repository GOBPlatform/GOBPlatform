using System;
using System.Security.Cryptography;
using System.Text;

namespace GOBCrypto
{
    public class ARIACipher
    {
        private static readonly int BLOCK_SIZE = 16;

        private ARIAEngine engine = null;

        public ARIACipher(byte[] key)
        {
            Init(key);
        }

        public ARIACipher(String key)
        {
            Init(CreateKey(key));
        }

        private void Init(byte[] key)
        {
            engine = new ARIAEngine(key.Length * 8);
            engine.SetKey(key);
            engine.SetupRoundKeys();
        }

	    protected byte[] CreateKey(string key)
        {
            MD5 md5Hasher = MD5.Create();

            byte[] hashData = md5Hasher.ComputeHash(Encoding.Default.GetBytes(key));
            return hashData;
        }

        public byte[] Encrypt(byte[] data)
        {
            byte[] indata = PKCS5Padding.Pad(data, BLOCK_SIZE);
            byte[] outdata = new byte[indata.Length];

            for (int i = 0; i < indata.Length; i += BLOCK_SIZE)
            {
                engine.Encrypt(indata, i, outdata, i);
            }

            return outdata;
        }

        public byte[] Decrypt(byte[] data)
        {
            byte[] outdata = new byte[data.Length];

            for (int i = 0; i < data.Length; i += BLOCK_SIZE)
            {
                engine.Decrypt(data, i, outdata, i);
            }

            return PKCS5Padding.UnPad(outdata, BLOCK_SIZE);
        }

        public String EncryptString(string data, Encoding encoding)
        {
            byte[] bytes = Encrypt(encoding.GetBytes(data));
            return Convert.ToBase64String(bytes);
        }

        public String DecryptString(string data, Encoding encoding)
        {
            byte[] bytes = Decrypt(Convert.FromBase64String(data));
            if (bytes == null)
                return null;

            return encoding.GetString(bytes);
        }

        public String encryptString(String data)
        {
            byte[] bytes = Encrypt(Encoding.Default.GetBytes(data));
            return Convert.ToBase64String(bytes);
        }

        public String decryptString(String data)
        {
            byte[] bytes = Decrypt(Convert.FromBase64String(data));
            if (bytes == null)
                return null;

            return Encoding.Default.GetString(bytes);
        }
    }
}
