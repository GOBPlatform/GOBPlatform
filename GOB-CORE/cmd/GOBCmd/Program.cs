﻿using System;
using GOBCommon;

namespace GOBCmd
{
    class Program
    {
        static void Main(string[] args)
        {
            Address oAddr = Address.GetInstance();
            Address.ArrAddress = new byte[] {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01};
        
            byte[] code = new byte[] {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04};
            var addr = oAddr.AddressFromVmCode(code);
            
            Console.WriteLine(BitConverter.ToString(addr));


            Console.WriteLine(oAddr.ToBase58());

        }
    }
}
