using System;
using System.Linq;
using GOBCommon;
using GOBCrypto;
using System.Text;

using GOBCommon;

using System.Security.Cryptography;
using System.Collections.Generic;

namespace GOBCmd
{
    class Program
    {
        static void Main(string[] args)
        {
            //====주소 체크썸 생성====
            //Address oAddr = Address.GetInstance();
            //Address.ArrAddress = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 };

            //byte[] code = new byte[] {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            //    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04};
            //var addr = oAddr.AddressFromVmCode(code);

            //Console.WriteLine(BitConverter.ToString(addr));
            //Console.WriteLine(oAddr.ToBase58());
            //Console.WriteLine("==========================");
            //====주소 체크썸 생성====

            //==== 개인키,공개키 생성====
            //var priKey = Crypto.GetInstance().PrivateKey;
            //var pubKey = Crypto.GetInstance().PublicKey;
            //Console.WriteLine("Private Key =====> " + priKey);
            //Console.WriteLine("Public Key =====> " + pubKey);

            //Crypto.GetInstance().InitKeyGen();
            //Console.WriteLine("Private Key =====> " + Crypto.GetInstance().PrivateKey);
            //Console.WriteLine("Public Key =====> " + Crypto.GetInstance().PublicKey);

            //Console.WriteLine(Crypto.GetInstance().VerifySignature("이것이냐?", Crypto.GetInstance().PrivateKey, pubKey));
            //==== 개인키,공개키 생성====

            ////==== 주소 생성====
            //byte proto = 0;
            //var priKey = Crypto.GetInstance().PrivateKey;
            //var pubKey = Crypto.GetInstance().PublicKey;
            //string addrStr = Crypto.GetInstance().GetAddress(proto, pubKey);
            //////외부에서 쓰는거
            //Console.WriteLine("Address : " + addrStr);
            //Console.WriteLine("Address Length : " + Crypto.GetInstance().GetAddress(proto, pubKey).Length);
            //////내부에서 쓰는거
            //Console.WriteLine(BitConverter.ToString(Crypto.GetInstance().GetHash160FromAddress(addrStr)).Replace("-", ""));
            //Console.WriteLine(Crypto.GetInstance().GetHash160FromAddress(addrStr).Length);
            ////==== 주소 생성====

            //==== Nonce 값====
            //Console.WriteLine(Common.GetNonce());
            //Console.WriteLine(Common.GetNonce());
            //Console.WriteLine(Common.GetNonce());
            //Console.WriteLine(Common.GetNonce());
            //==== Nonce 값====

            //SHA256 sha = new SHA256Managed();
            //string test = "Hello world~!";
            //string hash = GOBCommon.Hellper.ExtSHA256.StringHash(test).Replace("-", "");
            //Console.WriteLine(hash);
            //Console.WriteLine(hash.Length);

            Dictionary<string, string> myDic = new Dictionary<string, string>();
            myDic.Add("1", "one");
            myDic.Add("2", "two");
            string result = "";
            myDic.TryGetValue("3", out result);
            if (result == null) result = "Null";
            Console.WriteLine(result);
        }
    }
}
