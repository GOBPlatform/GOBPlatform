using System;
using System.IO;

namespace GOBCrypto
{
    public class ARIAEngine
    {
        private static readonly char[] HEX_DIGITS = new char[16] {
            '0','1','2','3','4','5','6','7','8','9','a','b','c','d','e','f'
        };

        private static readonly uint[,] KRK = new uint[,] {
            {0x517cc1b7, 0x27220a94, 0xfe13abe8, 0xfa9a6ee0},
            {0x6db14acc, 0x9e21c820, 0xff28b1d5, 0xef5de2b0},
            {0xdb92371d, 0x2126e970, 0x03249775, 0x04e8c90e}
        };

        private static readonly byte[] S1 = new byte[256];
        private static readonly byte[] S2 = new byte[256];
        private static readonly byte[] X1 = new byte[256];
        private static readonly byte[] X2 = new byte[256];

        private static readonly uint[] TS1 = new uint[256];
        private static readonly uint[] TS2 = new uint[256];
        private static readonly uint[] TX1 = new uint[256];
        private static readonly uint[] TX2 = new uint[256];

        static ARIAEngine()
        {
            uint[] exp = new uint[256];
            uint[] log = new uint[256];
            exp[0] = 1;
            for (int i = 1; i < 256; i++)
            {
                uint j = (exp[i - 1] << 1) ^ exp[i - 1];
                if ((j & 0x100) != 0) j ^= 0x11b;
                exp[i] = j;
            }
            for (int i = 1; i < 255; i++)
                log[exp[i]] = (uint)i;

            uint[,] A = new uint[,] {
                {1, 0, 0, 0, 1, 1, 1, 1},
                {1, 1, 0, 0, 0, 1, 1, 1},
                {1, 1, 1, 0, 0, 0, 1, 1},
                {1, 1, 1, 1, 0, 0, 0, 1},
                {1, 1, 1, 1, 1, 0, 0, 0},
                {0, 1, 1, 1, 1, 1, 0, 0},
                {0, 0, 1, 1, 1, 1, 1, 0},
                {0, 0, 0, 1, 1, 1, 1, 1}
            };

            uint[,] B = new uint[,] {
                {0, 1, 0, 1, 1, 1, 1, 0},
                {0, 0, 1, 1, 1, 1, 0, 1},
                {1, 1, 0, 1, 0, 1, 1, 1},
                {1, 0, 0, 1, 1, 1, 0, 1},
                {0, 0, 1, 0, 1, 1, 0, 0},
                {1, 0, 0, 0, 0, 0, 0, 1},
                {0, 1, 0, 1, 1, 1, 0, 1},
                {1, 1, 0, 1, 0, 0, 1, 1}
            };

            for (int i = 0; i < 256; i++)
            {
                uint t = 0, p;
                if (i == 0)
                    p = 0;
                else
                    p = exp[255 - log[i]];
                for (int j = 0; j < 8; j++)
                {
                    uint s = 0;
                    for (int k = 0; k < 8; k++)
                    {
                        if (((p >> (7 - k)) & 0x01) != 0)
                            s ^= A[k, j];
                    }
                    t = (t << 1) ^ s;
                }
                t ^= 0x63;
                S1[i] = (byte)t;
                X1[t] = (byte)i;
            }
            for (int i = 0; i < 256; i++)
            {
                uint t = 0, p;
                if (i == 0)
                    p = 0;
                else
                    p = exp[(247 * log[i]) % 255];
                for (int j = 0; j < 8; j++)
                {
                    uint s = 0;
                    for (int k = 0; k < 8; k++)
                    {
                        if (((p >> k) & 0x01) != 0)
                            s ^= B[7 - j, k];
                    }
                    t = (t << 1) ^ s;
                }
                t ^= 0xe2;
                S2[i] = (byte)t;
                X2[t] = (byte)i;
            }

            for (int i = 0; i < 256; i++)
            {
                TS1[i] = (uint)0x00010101 * (S1[i] & (uint)0xff);
                TS2[i] = (uint)0x01000101 * (S2[i] & (uint)0xff);
                TX1[i] = (uint)0x01010001 * (X1[i] & (uint)0xff);
                TX2[i] = (uint)0x01010100 * (X2[i] & (uint)0xff);
            }
        }

        private int keySize = 0;
        private int numberOfRounds = 0;
        private byte[] masterKey = null;
        private uint[] encRoundKeys = null, decRoundKeys = null;

        public ARIAEngine(int keySize)
        {
            SetKeySize(keySize);
        }

        void Reset()
        {
            this.keySize = 0;
            this.numberOfRounds = 0;
            this.masterKey = null;
            this.encRoundKeys = null;
            this.decRoundKeys = null;
        }

        int GetKeySize()
        {
            return this.keySize;
        }

        void SetKeySize(int keySize)
        {
            this.Reset();
            if (keySize != 128 && keySize != 192 && keySize != 256)
                throw new InvalidKeyException("keySize=" + keySize);
            this.keySize = keySize;
            switch (keySize)
            {
                case 128:
                    this.numberOfRounds = 12;
                    break;
                case 192:
                    this.numberOfRounds = 14;
                    break;
                case 256:
                    this.numberOfRounds = 16;
                    break;
            }
        }

        public void SetKey(byte[] masterKey)
        {
            if (masterKey.Length * 8 < keySize)
                throw new InvalidKeyException("masterKey size=" + masterKey.Length);
            this.decRoundKeys = null;
            this.encRoundKeys = null;
            this.masterKey = (byte[])masterKey.Clone();
        }

        void SetupEncRoundKeys()
        {
            if (this.keySize == 0)
                throw new InvalidKeyException("keySize");
            if (this.masterKey == null)
                throw new InvalidKeyException("masterKey");
            if (this.encRoundKeys == null)
                this.encRoundKeys = new uint[4 * (this.numberOfRounds + 1)];
            this.decRoundKeys = null;
            DoEncKeySetup(this.masterKey, this.encRoundKeys, this.keySize);
        }

        void SetupDecRoundKeys()
        {
            if (this.keySize == 0)
                throw new InvalidKeyException("keySize");
            if (this.encRoundKeys == null)
                if (this.masterKey == null)
                    throw new InvalidKeyException("masterKey");
                else
                    SetupEncRoundKeys();

            this.decRoundKeys = (uint[])encRoundKeys.Clone();
            DoDecKeySetup(this.masterKey, this.decRoundKeys, this.keySize);
        }

        public void SetupRoundKeys()
        {
            SetupDecRoundKeys();
        }

        private static void DoCrypt(byte[] i, int ioffset, uint[] rk, int nr, byte[] o, int ooffset)
        {
            uint t0, t1, t2, t3, j = 0;

            t0 = ToInt(i[0 + ioffset], i[1 + ioffset], i[2 + ioffset], i[3 + ioffset]);
            t1 = ToInt(i[4 + ioffset], i[5 + ioffset], i[6 + ioffset], i[7 + ioffset]);
            t2 = ToInt(i[8 + ioffset], i[9 + ioffset], i[10 + ioffset], i[11 + ioffset]);
            t3 = ToInt(i[12 + ioffset], i[13 + ioffset], i[14 + ioffset], i[15 + ioffset]);

            for (int r = 1; r < nr / 2; r++)
            {
                t0 ^= rk[j++]; t1 ^= rk[j++]; t2 ^= rk[j++]; t3 ^= rk[j++];
                t0 = TS1[(t0 >> 24) & 0xff] ^ TS2[(t0 >> 16) & 0xff] ^ TX1[(t0 >> 8) & 0xff] ^ TX2[t0 & 0xff];
                t1 = TS1[(t1 >> 24) & 0xff] ^ TS2[(t1 >> 16) & 0xff] ^ TX1[(t1 >> 8) & 0xff] ^ TX2[t1 & 0xff];
                t2 = TS1[(t2 >> 24) & 0xff] ^ TS2[(t2 >> 16) & 0xff] ^ TX1[(t2 >> 8) & 0xff] ^ TX2[t2 & 0xff];
                t3 = TS1[(t3 >> 24) & 0xff] ^ TS2[(t3 >> 16) & 0xff] ^ TX1[(t3 >> 8) & 0xff] ^ TX2[t3 & 0xff];
                t1 ^= t2; t2 ^= t3; t0 ^= t1; t3 ^= t1; t2 ^= t0; t1 ^= t2;
                t1 = Badc(t1); t2 = Cdab(t2); t3 = Dcba(t3);
                t1 ^= t2; t2 ^= t3; t0 ^= t1; t3 ^= t1; t2 ^= t0; t1 ^= t2;

                t0 ^= rk[j++]; t1 ^= rk[j++]; t2 ^= rk[j++]; t3 ^= rk[j++];
                t0 = TX1[(t0 >> 24) & 0xff] ^ TX2[(t0 >> 16) & 0xff] ^ TS1[(t0 >> 8) & 0xff] ^ TS2[t0 & 0xff];
                t1 = TX1[(t1 >> 24) & 0xff] ^ TX2[(t1 >> 16) & 0xff] ^ TS1[(t1 >> 8) & 0xff] ^ TS2[t1 & 0xff];
                t2 = TX1[(t2 >> 24) & 0xff] ^ TX2[(t2 >> 16) & 0xff] ^ TS1[(t2 >> 8) & 0xff] ^ TS2[t2 & 0xff];
                t3 = TX1[(t3 >> 24) & 0xff] ^ TX2[(t3 >> 16) & 0xff] ^ TS1[(t3 >> 8) & 0xff] ^ TS2[t3 & 0xff];
                t1 ^= t2; t2 ^= t3; t0 ^= t1; t3 ^= t1; t2 ^= t0; t1 ^= t2;
                t3 = Badc(t3); t0 = Cdab(t0); t1 = Dcba(t1);
                t1 ^= t2; t2 ^= t3; t0 ^= t1; t3 ^= t1; t2 ^= t0; t1 ^= t2;
            }
            t0 ^= rk[j++]; t1 ^= rk[j++]; t2 ^= rk[j++]; t3 ^= rk[j++];
            t0 = TS1[(t0 >> 24) & 0xff] ^ TS2[(t0 >> 16) & 0xff] ^ TX1[(t0 >> 8) & 0xff] ^ TX2[t0 & 0xff];
            t1 = TS1[(t1 >> 24) & 0xff] ^ TS2[(t1 >> 16) & 0xff] ^ TX1[(t1 >> 8) & 0xff] ^ TX2[t1 & 0xff];
            t2 = TS1[(t2 >> 24) & 0xff] ^ TS2[(t2 >> 16) & 0xff] ^ TX1[(t2 >> 8) & 0xff] ^ TX2[t2 & 0xff];
            t3 = TS1[(t3 >> 24) & 0xff] ^ TS2[(t3 >> 16) & 0xff] ^ TX1[(t3 >> 8) & 0xff] ^ TX2[t3 & 0xff];
            t1 ^= t2; t2 ^= t3; t0 ^= t1; t3 ^= t1; t2 ^= t0; t1 ^= t2;
            t1 = Badc(t1); t2 = Cdab(t2); t3 = Dcba(t3);
            t1 ^= t2; t2 ^= t3; t0 ^= t1; t3 ^= t1; t2 ^= t0; t1 ^= t2;

            t0 ^= rk[j++]; t1 ^= rk[j++]; t2 ^= rk[j++]; t3 ^= rk[j++];
            o[0 + ooffset] = (byte)(X1[0xff & (t0 >> 24)] ^ (rk[j] >> 24));
            o[1 + ooffset] = (byte)(X2[0xff & (t0 >> 16)] ^ (rk[j] >> 16));
            o[2 + ooffset] = (byte)(S1[0xff & (t0 >> 8)] ^ (rk[j] >> 8));
            o[3 + ooffset] = (byte)(S2[0xff & (t0)] ^ (rk[j]));
            o[4 + ooffset] = (byte)(X1[0xff & (t1 >> 24)] ^ (rk[j + 1] >> 24));
            o[5 + ooffset] = (byte)(X2[0xff & (t1 >> 16)] ^ (rk[j + 1] >> 16));
            o[6 + ooffset] = (byte)(S1[0xff & (t1 >> 8)] ^ (rk[j + 1] >> 8));
            o[7 + ooffset] = (byte)(S2[0xff & (t1)] ^ (rk[j + 1]));
            o[8 + ooffset] = (byte)(X1[0xff & (t2 >> 24)] ^ (rk[j + 2] >> 24));
            o[9 + ooffset] = (byte)(X2[0xff & (t2 >> 16)] ^ (rk[j + 2] >> 16));
            o[10 + ooffset] = (byte)(S1[0xff & (t2 >> 8)] ^ (rk[j + 2] >> 8));
            o[11 + ooffset] = (byte)(S2[0xff & (t2)] ^ (rk[j + 2]));
            o[12 + ooffset] = (byte)(X1[0xff & (t3 >> 24)] ^ (rk[j + 3] >> 24));
            o[13 + ooffset] = (byte)(X2[0xff & (t3 >> 16)] ^ (rk[j + 3] >> 16));
            o[14 + ooffset] = (byte)(S1[0xff & (t3 >> 8)] ^ (rk[j + 3] >> 8));
            o[15 + ooffset] = (byte)(S2[0xff & (t3)] ^ (rk[j + 3]));
        }

        public void Encrypt(byte[] i, int ioffset, byte[] o, int ooffset)
        {
            if (this.keySize == 0)
                throw new InvalidKeyException("keySize");
            if (this.encRoundKeys == null)
                if (this.masterKey == null)
                    throw new InvalidKeyException("masterKey");
                else
                    SetupEncRoundKeys();
            DoCrypt(i, ioffset, this.encRoundKeys, this.numberOfRounds, o, ooffset);
        }

        public byte[] Encrypt(byte[] i, int ioffset)
        {
            byte[] o = new byte[16];
            this.Encrypt(i, ioffset, o, 0);
            return o;
        }

        public void Decrypt(byte[] i, int ioffset, byte[] o, int ooffset)
        {
            if (this.keySize == 0)
                throw new InvalidKeyException("keySize");
            if (this.decRoundKeys == null)
                if (this.masterKey == null)
                    throw new InvalidKeyException("masterKey");
                else
                    SetupDecRoundKeys();
            DoCrypt(i, ioffset, this.decRoundKeys, this.numberOfRounds, o, ooffset);
        }

        public byte[] Decrypt(byte[] i, int ioffset)
        {
            byte[] o = new byte[16];
            this.Decrypt(i, ioffset, o, 0);
            return o;
        }

        private static void DoEncKeySetup(byte[] mk, uint[] rk, int keyBits)
        {
            uint t0, t1, t2, t3;
            int q, j = 0;
            uint[] w0 = new uint[4];
            uint[] w1 = new uint[4];
            uint[] w2 = new uint[4];
            uint[] w3 = new uint[4];

            w0[0] = ToInt(mk[0], mk[1], mk[2], mk[3]);
            w0[1] = ToInt(mk[4], mk[5], mk[6], mk[7]);
            w0[2] = ToInt(mk[8], mk[9], mk[10], mk[11]);
            w0[3] = ToInt(mk[12], mk[13], mk[14], mk[15]);

            q = (keyBits - 128) / 64;
            t0 = w0[0] ^ KRK[q, 0]; t1 = w0[1] ^ KRK[q, 1];
            t2 = w0[2] ^ KRK[q, 2]; t3 = w0[3] ^ KRK[q, 3];
            t0 = TS1[(t0 >> 24) & 0xff] ^ TS2[(t0 >> 16) & 0xff] ^ TX1[(t0 >> 8) & 0xff] ^ TX2[t0 & 0xff];
            t1 = TS1[(t1 >> 24) & 0xff] ^ TS2[(t1 >> 16) & 0xff] ^ TX1[(t1 >> 8) & 0xff] ^ TX2[t1 & 0xff];
            t2 = TS1[(t2 >> 24) & 0xff] ^ TS2[(t2 >> 16) & 0xff] ^ TX1[(t2 >> 8) & 0xff] ^ TX2[t2 & 0xff];
            t3 = TS1[(t3 >> 24) & 0xff] ^ TS2[(t3 >> 16) & 0xff] ^ TX1[(t3 >> 8) & 0xff] ^ TX2[t3 & 0xff];
            t1 ^= t2; t2 ^= t3; t0 ^= t1; t3 ^= t1; t2 ^= t0; t1 ^= t2;
            t1 = Badc(t1); t2 = Cdab(t2); t3 = Dcba(t3);
            t1 ^= t2; t2 ^= t3; t0 ^= t1; t3 ^= t1; t2 ^= t0; t1 ^= t2;

            if (keyBits > 128)
            {
                w1[0] = ToInt(mk[16], mk[17], mk[18], mk[19]);
                w1[1] = ToInt(mk[20], mk[21], mk[22], mk[23]);
                if (keyBits > 192)
                {
                    w1[2] = ToInt(mk[24], mk[25], mk[26], mk[27]);
                    w1[3] = ToInt(mk[28], mk[29], mk[30], mk[31]);
                }
                else
                {
                    w1[2] = w1[3] = 0;
                }
            }
            else
            {
                w1[0] = w1[1] = w1[2] = w1[3] = 0;
            }
            w1[0] ^= t0; w1[1] ^= t1; w1[2] ^= t2; w1[3] ^= t3;
            t0 = w1[0]; t1 = w1[1]; t2 = w1[2]; t3 = w1[3];

            q = (q == 2) ? 0 : (q + 1);
            t0 ^= KRK[q, 0]; t1 ^= KRK[q, 1]; t2 ^= KRK[q, 2]; t3 ^= KRK[q, 3];
            t0 = TX1[(t0 >> 24) & 0xff] ^ TX2[(t0 >> 16) & 0xff] ^ TS1[(t0 >> 8) & 0xff] ^ TS2[t0 & 0xff];
            t1 = TX1[(t1 >> 24) & 0xff] ^ TX2[(t1 >> 16) & 0xff] ^ TS1[(t1 >> 8) & 0xff] ^ TS2[t1 & 0xff];
            t2 = TX1[(t2 >> 24) & 0xff] ^ TX2[(t2 >> 16) & 0xff] ^ TS1[(t2 >> 8) & 0xff] ^ TS2[t2 & 0xff];
            t3 = TX1[(t3 >> 24) & 0xff] ^ TX2[(t3 >> 16) & 0xff] ^ TS1[(t3 >> 8) & 0xff] ^ TS2[t3 & 0xff];
            t1 ^= t2; t2 ^= t3; t0 ^= t1; t3 ^= t1; t2 ^= t0; t1 ^= t2;
            t3 = Badc(t3); t0 = Cdab(t0); t1 = Dcba(t1);
            t1 ^= t2; t2 ^= t3; t0 ^= t1; t3 ^= t1; t2 ^= t0; t1 ^= t2;
            t0 ^= w0[0]; t1 ^= w0[1]; t2 ^= w0[2]; t3 ^= w0[3];
            w2[0] = t0; w2[1] = t1; w2[2] = t2; w2[3] = t3;

            q = (q == 2) ? 0 : (q + 1);
            t0 ^= KRK[q, 0]; t1 ^= KRK[q, 1]; t2 ^= KRK[q, 2]; t3 ^= KRK[q, 3];
            t0 = TS1[(t0 >> 24) & 0xff] ^ TS2[(t0 >> 16) & 0xff] ^ TX1[(t0 >> 8) & 0xff] ^ TX2[t0 & 0xff];
            t1 = TS1[(t1 >> 24) & 0xff] ^ TS2[(t1 >> 16) & 0xff] ^ TX1[(t1 >> 8) & 0xff] ^ TX2[t1 & 0xff];
            t2 = TS1[(t2 >> 24) & 0xff] ^ TS2[(t2 >> 16) & 0xff] ^ TX1[(t2 >> 8) & 0xff] ^ TX2[t2 & 0xff];
            t3 = TS1[(t3 >> 24) & 0xff] ^ TS2[(t3 >> 16) & 0xff] ^ TX1[(t3 >> 8) & 0xff] ^ TX2[t3 & 0xff];
            t1 ^= t2; t2 ^= t3; t0 ^= t1; t3 ^= t1; t2 ^= t0; t1 ^= t2;
            t1 = Badc(t1); t2 = Cdab(t2); t3 = Dcba(t3);
            t1 ^= t2; t2 ^= t3; t0 ^= t1; t3 ^= t1; t2 ^= t0; t1 ^= t2;
            w3[0] = t0 ^ w1[0]; w3[1] = t1 ^ w1[1]; w3[2] = t2 ^ w1[2]; w3[3] = t3 ^ w1[3];

            Gsrk(w0, w1, 19, rk, j); j += 4;
            Gsrk(w1, w2, 19, rk, j); j += 4;
            Gsrk(w2, w3, 19, rk, j); j += 4;
            Gsrk(w3, w0, 19, rk, j); j += 4;
            Gsrk(w0, w1, 31, rk, j); j += 4;
            Gsrk(w1, w2, 31, rk, j); j += 4;
            Gsrk(w2, w3, 31, rk, j); j += 4;
            Gsrk(w3, w0, 31, rk, j); j += 4;
            Gsrk(w0, w1, 67, rk, j); j += 4;
            Gsrk(w1, w2, 67, rk, j); j += 4;
            Gsrk(w2, w3, 67, rk, j); j += 4;
            Gsrk(w3, w0, 67, rk, j); j += 4;
            Gsrk(w0, w1, 97, rk, j); j += 4;
            if (keyBits > 128)
            {
                Gsrk(w1, w2, 97, rk, j); j += 4;
                Gsrk(w2, w3, 97, rk, j); j += 4;
            }
            if (keyBits > 192)
            {
                Gsrk(w3, w0, 97, rk, j); j += 4;
                Gsrk(w0, w1, 109, rk, j);
            }
        }

        private static void DoDecKeySetup(byte[] mk, uint[] rk, int keyBits)
        {
            int a = 0, z;
            uint[] t = new uint[4];

            z = 32 + keyBits / 8;
            SwapBlocks(rk, 0, z);
            a += 4; z -= 4;

            for (; a < z; a += 4, z -= 4)
                SwapAndDiffuse(rk, a, z, t);
            Diff(rk, a, t, 0);
            rk[a] = t[0]; rk[a + 1] = t[1]; rk[a + 2] = t[2]; rk[a + 3] = t[3];
        }

        private static uint ToInt(byte b0, byte b1, byte b2, byte b3)
        {
            return (uint)((b0 & 0xff) << 24 ^ (b1 & 0xff) << 16 ^ (b2 & 0xff) << 8 ^ b3 & 0xff);
        }

        private static void ToByteArray(uint i, byte[] b, int offset)
        {
            b[offset] = (byte)(i >> 24);
            b[offset + 1] = (byte)(i >> 16);
            b[offset + 2] = (byte)(i >> 8);
            b[offset + 3] = (byte)(i);
        }

        private static uint M(uint t)
        {
            return 0x00010101 * ((t >> 24) & 0xff) ^ 0x01000101 * ((t >> 16) & 0xff) ^
                0x01010001 * ((t >> 8) & 0xff) ^ 0x01010100 * (t & 0xff);
        }

        private static uint Badc(uint t)
        {
            return ((t << 8) & 0xff00ff00) ^ ((t >> 8) & 0x00ff00ff);
        }

        private static uint Cdab(uint t)
        {
            return ((t << 16) & 0xffff0000) ^ ((t >> 16) & 0x0000ffff);
        }

        private static uint Dcba(uint t)
        {
            return (t & 0x000000ff) << 24 ^ (t & 0x0000ff00) << 8 ^ (t & 0x00ff0000) >> 8 ^ (t & 0xff000000) >> 24;
        }

        private static void Gsrk(uint[] x, uint[] y, int rot, uint[] rk, int offset)
        {
            int q = 4 - (rot / 32), r = rot % 32, s = 32 - r;

            rk[offset] = x[0] ^ y[(q) % 4] >> r ^ y[(q + 3) % 4] << s;
            rk[offset + 1] = x[1] ^ y[(q + 1) % 4] >> r ^ y[(q) % 4] << s;
            rk[offset + 2] = x[2] ^ y[(q + 2) % 4] >> r ^ y[(q + 1) % 4] << s;
            rk[offset + 3] = x[3] ^ y[(q + 3) % 4] >> r ^ y[(q + 2) % 4] << s;
        }

        private static void Diff(uint[] i, int offset1, uint[] o, int offset2)
        {
            uint t0, t1, t2, t3;

            t0 = M(i[offset1]); t1 = M(i[offset1 + 1]); t2 = M(i[offset1 + 2]); t3 = M(i[offset1 + 3]);
            t1 ^= t2; t2 ^= t3; t0 ^= t1; t3 ^= t1; t2 ^= t0; t1 ^= t2;
            t1 = Badc(t1); t2 = Cdab(t2); t3 = Dcba(t3);
            t1 ^= t2; t2 ^= t3; t0 ^= t1; t3 ^= t1; t2 ^= t0; t1 ^= t2;
            o[offset2] = t0; o[offset2 + 1] = t1; o[offset2 + 2] = t2; o[offset2 + 3] = t3;
        }

        private static void SwapBlocks(uint[] arr, int offset1, int offset2)
        {
            uint t;

            for (int i = 0; i < 4; i++)
            {
                t = arr[offset1 + i];
                arr[offset1 + i] = arr[offset2 + i];
                arr[offset2 + i] = t;
            }
        }

        private static void SwapAndDiffuse(uint[] arr, int offset1, int offset2, uint[] tmp)
        {
            Diff(arr, offset1, tmp, 0);
            Diff(arr, offset2, arr, offset1);
            arr[offset2] = tmp[0]; arr[offset2 + 1] = tmp[1];
            arr[offset2 + 2] = tmp[2]; arr[offset2 + 3] = tmp[3];
        }

        private static void PrintBlock(TextWriter writer, byte[] b)
        {
            for (int i = 0; i < 4; i++)
                ByteToHex(writer, b[i]);
            writer.Write(" ");
            for (int i = 4; i < 8; i++)
                ByteToHex(writer, b[i]);
            writer.Write(" ");
            for (int i = 8; i < 12; i++)
                ByteToHex(writer, b[i]);
            writer.Write(" ");
            for (int i = 12; i < 16; i++)
                ByteToHex(writer, b[i]);
        }

        private static void PrintSBox(TextWriter writer, byte[] box)
        {
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    ByteToHex(writer, box[16 * i + j]);
                    writer.Write(" ");
                }
                writer.WriteLine();
            }
        }

        private static void ByteToHex(TextWriter writer, byte b)
        {
            char[] buf = {
                HEX_DIGITS[(b >> 4) & 0x0F],
                HEX_DIGITS[ b       & 0x0F]
            };
            writer.Write(buf);
        }

        private static void IntToHex(TextWriter writer, uint i)
        {
            byte[] b = new byte[4];
            ToByteArray(i, b, 0);
            ByteToHex(writer, b[0]);
            ByteToHex(writer, b[1]);
            ByteToHex(writer, b[2]);
            ByteToHex(writer, b[3]);
        }

        private static void PrintRoundKeys(TextWriter writer, uint[] roundKeys)
        {
            for (int i = 0; i < roundKeys.Length;)
            {
                writer.Write("* ");
                IntToHex(writer, roundKeys[i++]); writer.Write(" ");
                IntToHex(writer, roundKeys[i++]); writer.Write(" ");
                IntToHex(writer, roundKeys[i++]); writer.Write(" ");
                IntToHex(writer, roundKeys[i++]); writer.WriteLine(" ");
            }
        }

        public static void ARIA_test()
        {
            byte[] p = new byte[16];
            byte[] c = new byte[16];
            byte[] mk = new byte[32];

            bool flag = false;
            TextWriter writer = Console.Out;
            ARIAEngine instance = new ARIAEngine(256);

            for (int i = 0; i < 32; i++)
                mk[i] = 0;
            for (int i = 0; i < 16; i++)
                p[i] = 0;

            writer.WriteLine("BEGIN testing the roundtrip...");
            writer.WriteLine("For key size of 256 bits, starting with " +
                        "the zero plaintext and the zero key, let's see if " +
                        "we may recover the plaintext by decrypting the " +
                        "encrypted ciphertext.");
            instance.SetKey(mk);
            instance.SetupRoundKeys();

            writer.Write("plaintext : "); PrintBlock(writer, p); writer.WriteLine();
            instance.Encrypt(p, 0, c, 0);
            writer.Write("ciphertext: "); PrintBlock(writer, c); writer.WriteLine();
            instance.Decrypt(c, 0, p, 0);
            writer.Write("decrypted : "); PrintBlock(writer, p); writer.WriteLine();
            flag = false;
            for (int i = 0; i < 16; i++)
                if (p[i] != 0)
                    flag = true;
            if (flag)
                writer.WriteLine("The result is incorrect!");
            else
                writer.WriteLine("Okay.  The result is correct.");
            writer.WriteLine("END   testing the roundtrip.\n");

            int TEST_NUM = 0x800000;
            writer.WriteLine("BEGIN speed measurement...");

            for (int i = 0; i < 16; i++) mk[i] = (byte)i;
            writer.WriteLine("  First, EncKeySetup():");
            writer.Write("  masterkey: "); PrintBlock(writer, mk); writer.WriteLine();
            instance.Reset(); instance.SetKeySize(128);
            instance.SetKey(mk);
            for (int i = 0; i < 1000; i++) instance.SetupEncRoundKeys();  // allow the CPU to settle down
            DateTime start = DateTime.Now;
            for (int i = 0; i < TEST_NUM; i++) instance.SetupEncRoundKeys();
            DateTime fin = DateTime.Now;
            float lapse = (float)((fin - start).Milliseconds) / 1000;
            writer.Write("  time lapsed: "); writer.Write(lapse); writer.WriteLine(" sec.");
            writer.Write("  speed      : "); writer.Write(TEST_NUM * 128 / (lapse * 1024 * 1024)); writer.WriteLine(" megabits/sec.\n");

            writer.WriteLine("  Next, Crypt():");
            for (int i = 0; i < 16; i++) p[i] = (byte)((i << 4) ^ i);
            writer.Write("  plaintext : "); PrintBlock(writer, p); writer.WriteLine();
            for (int i = 0; i < 1000; i++) instance.Encrypt(p, 0, c, 0);
            start = DateTime.Now;
            for (int i = 0; i < TEST_NUM; i++) instance.Encrypt(p, 0, c, 0);
            fin = DateTime.Now;
            writer.Write("  ciphertext: "); PrintBlock(writer, c); writer.WriteLine();
            lapse = (float)((fin - start).Milliseconds) / 1000;
            writer.Write("  time lapsed: "); writer.Write(lapse); writer.WriteLine(" sec.");
            writer.Write("  speed      : "); writer.Write(TEST_NUM * 128 / (lapse * 1024 * 1024)); writer.WriteLine(" megabits/sec.\n");

            writer.WriteLine("  Finally, DecKeySetup():");
            for (int i = 0; i < 1000; i++) instance.SetupDecRoundKeys();  // allow the CPU to settle down
            start = DateTime.Now;
            for (int i = 0; i < TEST_NUM; i++) instance.SetupDecRoundKeys();
            fin = DateTime.Now;
            lapse = (float)((fin - start).Milliseconds) / 1000;
            writer.Write("  time lapsed: "); writer.Write(lapse); writer.WriteLine(" sec.");
            writer.Write("  speed      : "); writer.Write(TEST_NUM * 128 / (lapse * 1024 * 1024)); writer.WriteLine(" megabits/sec.");
            writer.WriteLine("END   speed measurement.");
        }
    }
}
