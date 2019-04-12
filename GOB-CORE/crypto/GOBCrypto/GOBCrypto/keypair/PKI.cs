using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using GOBCrypto.base58;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace GOBCrypto.keypair
{
    public class PKI
    {
        private static PKI _instance = null;
        private RSACryptoServiceProvider csp;
        public PKI()
        {
        }

        public static PKI GetInstance()
        {
            if (_instance == null) _instance = new PKI();
            return _instance;
        }

        public void GenerateKey()
        {
            csp = new RSACryptoServiceProvider();
            RSAParameters privateKey = RSA.Create().ExportParameters(true);
            csp.ImportParameters(privateKey);
        }

        public string ExportPrivateKey()
        {
            using (TextWriter outputStream = new StringWriter())
            {
                if (csp.PublicOnly) throw new ArgumentException("CSP does not contain a private key", "csp");
                var parameters = csp.ExportParameters(true);
                using (var stream = new MemoryStream())
                {
                    var writer = new BinaryWriter(stream);
                    writer.Write((byte)0x30); // SEQUENCE
                    using (var innerStream = new MemoryStream())
                    {
                        var innerWriter = new BinaryWriter(innerStream);
                        EncodeIntegerBigEndian(innerWriter, new byte[] { 0x00 }); // Version
                        EncodeIntegerBigEndian(innerWriter, parameters.Modulus);
                        EncodeIntegerBigEndian(innerWriter, parameters.Exponent);
                        EncodeIntegerBigEndian(innerWriter, parameters.D);
                        EncodeIntegerBigEndian(innerWriter, parameters.P);
                        EncodeIntegerBigEndian(innerWriter, parameters.Q);
                        EncodeIntegerBigEndian(innerWriter, parameters.DP);
                        EncodeIntegerBigEndian(innerWriter, parameters.DQ);
                        EncodeIntegerBigEndian(innerWriter, parameters.InverseQ);
                        var length = (int)innerStream.Length;
                        EncodeLength(writer, length);
                        writer.Write(innerStream.GetBuffer(), 0, length);
                    }

                    var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
                    outputStream.WriteLine("-----BEGIN RSA PRIVATE KEY-----");
                    // Output as Base64 with lines chopped at 64 characters
                    for (var i = 0; i < base64.Length; i += 64)
                    {
                        outputStream.WriteLine(base64, i, Math.Min(64, base64.Length - i));
                    }
                    outputStream.WriteLine("-----END RSA PRIVATE KEY-----");
                }
                return outputStream.ToString();
            }
        }

        private void EncodeLength(BinaryWriter stream, int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException("length", "Length must be non-negative");
            if (length < 0x80)
            {
                // Short form
                stream.Write((byte)length);
            }
            else
            {
                // Long form
                var temp = length;
                var bytesRequired = 0;
                while (temp > 0)
                {
                    temp >>= 8;
                    bytesRequired++;
                }
                stream.Write((byte)(bytesRequired | 0x80));
                for (var i = bytesRequired - 1; i >= 0; i--)
                {
                    stream.Write((byte)(length >> (8 * i) & 0xff));
                }
            }
        }

        private void EncodeIntegerBigEndian(BinaryWriter stream, byte[] value, bool forceUnsigned = true)
        {
            stream.Write((byte)0x02); // INTEGER
            var prefixZeros = 0;
            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] != 0) break;
                prefixZeros++;
            }
            if (value.Length - prefixZeros == 0)
            {
                EncodeLength(stream, 1);
                stream.Write((byte)0);
            }
            else
            {
                if (forceUnsigned && value[prefixZeros] > 0x7f)
                {
                    EncodeLength(stream, value.Length - prefixZeros + 1);
                    stream.Write((byte)0);
                }
                else
                {
                    EncodeLength(stream, value.Length - prefixZeros);
                }
                for (var i = prefixZeros; i < value.Length; i++)
                {
                    stream.Write(value[i]);
                }
            }
        }

        public String ExportPublicKey()
        {
            using (TextWriter outputStream = new StringWriter())
            {
                var parameters = csp.ExportParameters(false);
                using (var stream = new MemoryStream())
                {
                    var writer = new BinaryWriter(stream);
                    writer.Write((byte)0x30);
                    using (var innerStream = new MemoryStream())
                    {
                        var innerWriter = new BinaryWriter(innerStream);
                        EncodeIntegerBigEndian(innerWriter, new byte[] { 0x00 });
                        EncodeIntegerBigEndian(innerWriter, parameters.Modulus);
                        EncodeIntegerBigEndian(innerWriter, parameters.Exponent);

                        EncodeIntegerBigEndian(innerWriter, parameters.Exponent);
                        EncodeIntegerBigEndian(innerWriter, parameters.Exponent);
                        EncodeIntegerBigEndian(innerWriter, parameters.Exponent);
                        EncodeIntegerBigEndian(innerWriter, parameters.Exponent);
                        EncodeIntegerBigEndian(innerWriter, parameters.Exponent);
                        EncodeIntegerBigEndian(innerWriter, parameters.Exponent);

                        var length = (int)innerStream.Length;
                        EncodeLength(writer, length);
                        writer.Write(innerStream.GetBuffer(), 0, length);
                    }

                    var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
                    outputStream.WriteLine("-----BEGIN RSA PUBLIC KEY-----");

                    for (var i = 0; i < base64.Length; i += 64)
                    {
                        outputStream.WriteLine(base64, i, Math.Min(64, base64.Length - i));
                    }
                    outputStream.WriteLine("-----END RSA PUBLIC KEY-----");

                    return outputStream.ToString();
                }
            }
        }

        public string GeneratePrivateKey()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            RSAParameters privateKey = RSA.Create().ExportParameters(true);
            rsa.ImportParameters(privateKey);

            return RSAKeyExtention.ToXmlString(rsa, true);
        }

        public string GetPublicKeyFromPrivateKeyEx(string privateKey)
        {
            var curve = SecNamedCurves.GetByName("secp256k1");
            var domain = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);

            var d = new Org.BouncyCastle.Math.BigInteger(privateKey);
            var q = domain.G.Multiply(d);

            var publicKey = new ECPublicKeyParameters(q, domain);
            return Base58Encoding.Encode(publicKey.Q.GetEncoded());
        }


        private string GetPublicKeyFromPrivateKey(string privateKey)
        {
            var p = BigInteger.Parse("0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFC2F", NumberStyles.HexNumber);
            var b = (BigInteger)7;
            var a = BigInteger.Zero;
            var Gx = BigInteger.Parse("79BE667EF9DCBBAC55A06295CE870B07029BFCDB2DCE28D959F2815B16F81798", NumberStyles.HexNumber);
            var Gy = BigInteger.Parse("483ADA7726A3C4655DA4FBFC0E1108A8FD17B448A68554199C47D08FFB10D4B8", NumberStyles.HexNumber);

            CurveFp curve256 = new CurveFp(p, a, b);
            Point generator256 = new Point(curve256, Gx, Gy);

            var secret = BigInteger.Parse(privateKey, NumberStyles.HexNumber);
            var pubkeyPoint = generator256 * secret;
            return pubkeyPoint.X.ToString("X") + pubkeyPoint.Y.ToString("X");
        }
    }

    class Point
    {
        public static readonly Point INFINITY = new Point(null, default(BigInteger), default(BigInteger));
        public CurveFp Curve { get; private set; }
        public BigInteger X { get; private set; }
        public BigInteger Y { get; private set; }

        public Point(CurveFp curve, BigInteger x, BigInteger y)
        {
            this.Curve = curve;
            this.X = x;
            this.Y = y;
        }
        public Point Double()
        {
            if (this == INFINITY)
                return INFINITY;

            BigInteger p = this.Curve.p;
            BigInteger a = this.Curve.a;
            BigInteger l = ((3 * this.X * this.X + a) * InverseMod(2 * this.Y, p)) % p;
            BigInteger x3 = (l * l - 2 * this.X) % p;
            BigInteger y3 = (l * (this.X - x3) - this.Y) % p;
            return new Point(this.Curve, x3, y3);
        }
        public override string ToString()
        {
            if (this == INFINITY)
                return "infinity";
            return string.Format("({0},{1})", this.X, this.Y);
        }
        public static Point operator +(Point left, Point right)
        {
            if (right == INFINITY)
                return left;
            if (left == INFINITY)
                return right;
            if (left.X == right.X)
            {
                if ((left.Y + right.Y) % left.Curve.p == 0)
                    return INFINITY;
                else
                    return left.Double();
            }

            var p = left.Curve.p;
            var l = ((right.Y - left.Y) * InverseMod(right.X - left.X, p)) % p;
            var x3 = (l * l - left.X - right.X) % p;
            var y3 = (l * (left.X - x3) - left.Y) % p;
            return new Point(left.Curve, x3, y3);
        }
        public static Point operator *(Point left, BigInteger right)
        {
            var e = right;
            if (e == 0 || left == INFINITY)
                return INFINITY;
            var e3 = 3 * e;
            var negativeLeft = new Point(left.Curve, left.X, -left.Y);
            var i = LeftmostBit(e3) / 2;
            var result = left;
            while (i > 1)
            {
                result = result.Double();
                if ((e3 & i) != 0 && (e & i) == 0)
                    result += left;
                if ((e3 & i) == 0 && (e & i) != 0)
                    result += negativeLeft;
                i /= 2;
            }
            return result;
        }

        private static BigInteger LeftmostBit(BigInteger x)
        {
            BigInteger result = 1;
            while (result <= x)
                result = 2 * result;
            return result / 2;
        }
        private static BigInteger InverseMod(BigInteger a, BigInteger m)
        {
            while (a < 0) a += m;
            if (a < 0 || m <= a)
                a = a % m;
            BigInteger c = a;
            BigInteger d = m;

            BigInteger uc = 1;
            BigInteger vc = 0;
            BigInteger ud = 0;
            BigInteger vd = 1;

            while (c != 0)
            {
                BigInteger r;
                //q, c, d = divmod( d, c ) + ( c, );
                var q = BigInteger.DivRem(d, c, out r);
                d = c;
                c = r;

                //uc, vc, ud, vd = ud - q*uc, vd - q*vc, uc, vc;
                var uct = uc;
                var vct = vc;
                var udt = ud;
                var vdt = vd;
                uc = udt - q * uct;
                vc = vdt - q * vct;
                ud = uct;
                vd = vct;
            }
            if (ud > 0) return ud;
            else return ud + m;
        }
    }

    class CurveFp
    {
        public BigInteger p { get; private set; }
        public BigInteger a { get; private set; }
        public BigInteger b { get; private set; }
        public CurveFp(BigInteger p, BigInteger a, BigInteger b)
        {
            this.p = p;
            this.a = a;
            this.b = b;
        }
    }
}
