using System;
using System.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Crypto.Signers;
using System.Text;

namespace GOBCrypto
{
    public class Crypto
    {
        private static Crypto _instance = null;
        private string _privateKey;
        private string _publicKey;
        private string _unCompressPublicKey;

        private X9ECParameters _ecParams = null;
        private ECDomainParameters _domainParams = null;
        private SecureRandom _random = null;

        private ECKeyPairGenerator _keyGen = null;
        private ECKeyGenerationParameters _keyParams = null;
        private AsymmetricCipherKeyPair _keyPair = null;

        private ECPrivateKeyParameters _privateKeyParams = null;
        private ECPublicKeyParameters _publicKeyParams = null;

        public string PublicKey { get => GetPublicKey(); }
        public string PrivateKey { get => GetPrivateKey(); }
        //public string UnCompressPublicKey { get => GetUnCompressPublicKey(); set => _unCompressPublicKey = value; }

        public static Crypto GetInstance()
        {
            if (_instance == null) _instance = new Crypto();
            return _instance;
        }

        public void InitKeyGen()
        {
            _ecParams = SecNamedCurves.GetByName("secp256k1");
            _domainParams = new ECDomainParameters(GetECParams().Curve, GetECParams().G, GetECParams().N, GetECParams().H);
            _random = new SecureRandom();
            _keyGen = new ECKeyPairGenerator();
            _keyParams = new ECKeyGenerationParameters(_domainParams, _random);
            _keyGen.Init(_keyParams);
            _keyPair = _keyGen.GenerateKeyPair();
            _privateKeyParams = _keyPair.Private as ECPrivateKeyParameters;
            _publicKeyParams = _keyPair.Public as ECPublicKeyParameters;
        }

        public X9ECParameters GetECParams()
        {
            if (_ecParams == null) _ecParams = SecNamedCurves.GetByName("secp256k1");
            return _ecParams;
        }

        public ECDomainParameters GetDomainParams()
        {
            if (_domainParams == null) _domainParams = new ECDomainParameters(GetECParams().Curve, GetECParams().G, GetECParams().N, GetECParams().H);
            return _domainParams;
        }

        public SecureRandom GetSecureRandom()
        {
            if (_random == null) _random = new SecureRandom();
            return _random;
        }

        public ECKeyPairGenerator GetECKeyPairGen()
        {
            if (_keyGen == null) _keyGen = new ECKeyPairGenerator();
            return _keyGen;
        }

        public ECKeyGenerationParameters GetECKeyGenParams()
        {
            if (_keyParams == null) _keyParams = new ECKeyGenerationParameters(GetDomainParams(), GetSecureRandom());
            return _keyParams;
        }

        public AsymmetricCipherKeyPair GetAsymmetricCipher()
        {
            if (_keyPair == null) _keyPair = GetECKeyPairGen().GenerateKeyPair();
            return _keyPair;
        }

        public void GenerateKeyPair()
        {
            _keyPair = GetAsymmetricCipher();
        }

        private string GetPrivateKey()
        {
            if (_privateKeyParams == null) InitKeyGen();
            BigInteger privD = _privateKeyParams.D;
            byte[] privBytes = privD.ToByteArray();

            if (privBytes.Length == 33)
            {
                var temp = new byte[32];
                Array.Copy(privBytes, 1, temp, 0, 32);
                privBytes = temp;
            }
            else if (privBytes.Length < 32)
            {
                var temp = Enumerable.Repeat<byte>(0x00, 32).ToArray();
                Array.Copy(privBytes, 0, temp, 32 - privBytes.Length, privBytes.Length);
                privBytes = temp;
            }
            return BitConverter.ToString(privBytes).Replace("-", "");
        }

        private string GetPublicKey()
        {
            if (_publicKeyParams == null) InitKeyGen();
            ECPoint q = _publicKeyParams.Q;
            FpPoint fp = new FpPoint(GetECParams().Curve, q.AffineXCoord, q.AffineYCoord);
            byte[] enc = fp.GetEncoded(true);
            return BitConverter.ToString(enc).Replace("-", "");
        }

        public bool VerifySignature(string message, string _privateKey, string _publicKey)
        {
            string privateKey = _privateKey;
            string publicKey = _publicKey;
            bool verified = false;
            try
            {
                BigInteger priv = new BigInteger(privateKey, 16);
                X9ECParameters ec = SecNamedCurves.GetByName("secp256k1");
                ECDomainParameters domainParams = new ECDomainParameters(ec.Curve, ec.G, ec.N, ec.H);
                ECPrivateKeyParameters privateKeyParams = new ECPrivateKeyParameters(priv, domainParams);

                BigInteger pub = new BigInteger(publicKey, 16);
                ECPoint q = domainParams.Curve.DecodePoint(pub.ToByteArray());
                ECPublicKeyParameters publicKeyParams = new ECPublicKeyParameters(q, domainParams);

                ECDsaSigner privSigner = new ECDsaSigner();
                privSigner.Init(true, privateKeyParams);

                var msg = Encoding.ASCII.GetBytes(message);
                BigInteger[] signature = privSigner.GenerateSignature(msg);
                BigInteger r = signature[0];
                BigInteger s = signature[1];

                ECDsaSigner pubSigner = new ECDsaSigner();
                pubSigner.Init(false, publicKeyParams);
                verified = pubSigner.VerifySignature(msg, r, s);
            }
            catch (Exception e)
            {

            }

            return verified;
        }
    }
}
