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
using Base58Check;
using System.Security.Cryptography;

namespace GOBCrypto
{
    public class Crypto
    {
        private static Crypto _instance = null;
        private string _privateKey;
        private string _publicKey;

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

        private X9ECParameters GetECParams()
        {
            if (_ecParams == null) _ecParams = SecNamedCurves.GetByName("secp256k1");
            return _ecParams;
        }

        private ECDomainParameters GetDomainParams()
        {
            if (_domainParams == null) _domainParams = new ECDomainParameters(GetECParams().Curve, GetECParams().G, GetECParams().N, GetECParams().H);
            return _domainParams;
        }

        private SecureRandom GetSecureRandom()
        {
            if (_random == null) _random = new SecureRandom();
            return _random;
        }

        private ECKeyPairGenerator GetECKeyPairGen()
        {
            if (_keyGen == null) _keyGen = new ECKeyPairGenerator();
            return _keyGen;
        }

        private ECKeyGenerationParameters GetECKeyGenParams()
        {
            if (_keyParams == null) _keyParams = new ECKeyGenerationParameters(GetDomainParams(), GetSecureRandom());
            return _keyParams;
        }

        private AsymmetricCipherKeyPair GetAsymmetricCipher()
        {
            if (_keyPair == null) _keyPair = GetECKeyPairGen().GenerateKeyPair();
            return _keyPair;
        }

        private void GenerateKeyPair()
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
            Org.BouncyCastle.Math.EC.ECPoint q = _publicKeyParams.Q;
            FpPoint fp = new FpPoint(GetECParams().Curve, q.AffineXCoord, q.AffineYCoord);
            byte[] enc = fp.GetEncoded(true);
            return BitConverter.ToString(enc).Replace("-", "");
        }

        public BigInteger[] GenerateSignature(string message, string privateKey)
        {
            BigInteger priv = new BigInteger(privateKey, 16);
            X9ECParameters ec = SecNamedCurves.GetByName("secp256k1");
            ECDomainParameters domainParams = new ECDomainParameters(ec.Curve, ec.G, ec.N, ec.H);
            ECPrivateKeyParameters privateKeyParams = new ECPrivateKeyParameters(priv, domainParams);

            ECDsaSigner privSigner = new ECDsaSigner();
            privSigner.Init(true, privateKeyParams);

            var msg = Encoding.ASCII.GetBytes(message);
            BigInteger[] signature = privSigner.GenerateSignature(msg);

            return signature;
        }

        /// <summary>
        /// PrivateKey와 PublicKey로 Message를 암호화 했다 풀었을때 풀리면 True, 안풀리면 False
        /// </summary>
        /// <param name="message"></param>
        /// <param name="_privateKey"></param>
        /// <param name="_publicKey"></param>
        /// <returns></returns>
        public bool VerifySignature(string message, string _privateKey, string _publicKey)
        {
            string privateKey = _privateKey;
            string publicKey = _publicKey;
            bool verified = false;
            try
            {
                //TODO : GenerateSignature 함수에서 아래 로직을 C&P 했음. 정리 요망.
                BigInteger priv = new BigInteger(privateKey, 16);
                X9ECParameters ec = SecNamedCurves.GetByName("secp256k1");
                ECDomainParameters domainParams = new ECDomainParameters(ec.Curve, ec.G, ec.N, ec.H);
                ECPrivateKeyParameters privateKeyParams = new ECPrivateKeyParameters(priv, domainParams);

                BigInteger pub = new BigInteger(publicKey, 16);
                Org.BouncyCastle.Math.EC.ECPoint q = domainParams.Curve.DecodePoint(pub.ToByteArray());
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
                Console.WriteLine("VerifySignature Exception : " + e.Message);
            }

            return verified;
        }

        /// <summary>
        /// privatekey로 만들었을법한 signature를 가지고 message를 publickey로 풀어봄.
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="message"></param>
        /// <param name="signature"></param>
        /// <returns></returns>
        public bool VerifySignature(string publicKey, string message,  BigInteger[] signature)
        {
            bool verified = false;
            try
            {
                X9ECParameters ec = SecNamedCurves.GetByName("secp256k1");
                ECDomainParameters domainParams = new ECDomainParameters(ec.Curve, ec.G, ec.N, ec.H);
                BigInteger pub = new BigInteger(publicKey, 16);
                Org.BouncyCastle.Math.EC.ECPoint q = domainParams.Curve.DecodePoint(pub.ToByteArray());
                ECPublicKeyParameters publicKeyParams = new ECPublicKeyParameters(q, domainParams);
                ECDsaSigner pubSigner = new ECDsaSigner();
                pubSigner.Init(false, publicKeyParams);
                var msg = Encoding.ASCII.GetBytes(message);
                BigInteger r = signature[0];
                BigInteger s = signature[1];
                verified = pubSigner.VerifySignature(msg, r, s);
            }
            catch (Exception e)
            {
                Console.WriteLine("VerifySignature Exception : " + e.Message);
            }
            return verified;
        }

        public string GetAddress(byte _protocol, string _publicKey)
        {
            string sPubKey = _publicKey;
            BigInteger pubKey = new BigInteger(sPubKey, 16);
            byte[] publicKey = pubKey.ToByteArray();

            var sha256 = SHA256.Create();
            var shaHash = sha256.ComputeHash(publicKey);

            var ripemd160 = RIPEMD160.Create();
            var mdHash = ripemd160.ComputeHash(shaHash);

            var xmdHash = new byte[mdHash.Length + 1];
            xmdHash[0] = _protocol;
            Array.Copy(mdHash, 0, xmdHash, 1, mdHash.Length);

            var doubleHash = sha256.ComputeHash(sha256.ComputeHash(xmdHash));
            var checksum = new byte[4];
            Array.Copy(doubleHash, 0, checksum, 0, 4);

            var binAddr = new byte[25];
            Array.Copy(xmdHash, 0, binAddr, 0, 21);
            Array.Copy(checksum, 0, binAddr, 21, 4);

            return Base58CheckEncoding.EncodePlain(binAddr);
        }

        public byte[] GetHash160FromAddress(string address)
        {
            byte[] addrBytes = Base58CheckEncoding.DecodePlain(address);
            byte[] hash160 = new byte[20];
            Array.Copy(addrBytes, 1, hash160, 0, 20);
            return hash160;
        }
    }

}
