using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GOBCommon;
using GOBCommon.Hellper;
using System.Security.Cryptography;
using System.Linq;
using System.Numerics;

namespace GOBCommon
{
    public class Address
    {
        private const int ADDR_LEN = 20;
        public byte[] address = new byte[ADDR_LEN];

        public string ToHexString()
        {
            return Encoding.Default.GetString(Common.ToArrayReverse(address));
        }

        public void Serialize(StreamWriter writer)
        {
            try
            {
                writer.Write(address);
            }
            catch
            {
                throw;
            }
        }

        public void Deserialize(StreamReader reader)
        {
            //reader.Read(address, 0, 9);
            //TODO : 기능확인 해보고 구현하자. 일단은 빈 메소드
            try
            {

            }
            catch
            {
                throw;
            }
        }

        public string ToBase58()
        {
            byte[] data = Base58Encoding.AddCheckSum(address);
            return Base58Encoding.Encode(data);
        }


        //    // AddressParseFromBytes returns parsed Address
        //    func AddressParseFromBytes(f[]byte) (Address, error) {
        //	if len(f) != ADDR_LEN {
        //		return ADDRESS_EMPTY, errors.New("[Common]: AddressParseFromBytes err, len != 20")
        //	}

        //var addr Address
        //copy(addr[:], f)
        //	return addr, nil
        //}

        //// AddressParseFromHexString returns parsed Address
        //func AddressFromHexString(s string) (Address, error) {
        //	hx, err := HexToBytes(s)
        //	if err != nil {
        //		return ADDRESS_EMPTY, err
        //	}
        //	return AddressParseFromBytes(ToArrayReverse(hx))
        //}

        //// AddressFromBase58 returns Address from encoded base58 string
        //func AddressFromBase58(encoded string) (Address, error) {
        //	if encoded == "" {
        //		return ADDRESS_EMPTY, errors.New("invalid address")
        //	}
        //	decoded, err := base58.BitcoinEncoding.Decode([]byte (encoded))
        //	if err != nil {
        //		return ADDRESS_EMPTY, err
        //	}

        //	x, ok := new(big.Int).SetString(string(decoded), 10)
        //	if !ok {
        //		return ADDRESS_EMPTY, errors.New("invalid address")
        //	}

        //	buf := x.Bytes()
        //	if len(buf) != 1+ADDR_LEN+4 || buf[0] != byte (23) {
        //		return ADDRESS_EMPTY, errors.New("wrong encoded address")
        //	}

        //	ph, err := AddressParseFromBytes(buf[1:21])
        //	if err != nil {
        //		return ADDRESS_EMPTY, err
        //	}

        //	addr := ph.ToBase58()

        //	if addr != encoded {
        //		return ADDRESS_EMPTY, errors.New("[AddressFromBase58]: decode encoded verify failed.")
        //	}

        //	return ph, nil
        //}

        //func AddressFromVmCode(code[]byte) Address {
        //	var addr Address
        //    temp := sha256.Sum256(code)
        //    md := ripemd160.New()
        //    md.Write(temp[:])

        //    md.Sum(addr[:0])

        //	return addr
        //}
    }
}
