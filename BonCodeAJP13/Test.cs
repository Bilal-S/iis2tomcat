/*
 * this is a test class
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using BonCodeAJP13;

namespace BonCodeAJP13
{
    public class Test
    {

        public string fMyStuff(string sInput)
        {
            return "Hello " + sInput;
        }

        public string fOtherStuff()
        {

            return "wow";
        }

        public void fConfTest()
        {
            
            int a = 2;

            a = a + 2;
        }

        public void fHeaderTest()
        {
            byte[] hBytes = BonCodeAJP13PacketHeaders.GetHeaderBytes("HTTP_ACCEPT");
            byte aByte = BonCodeAJP13PacketHeaders.GetAttributeByte("REMOTE_USER");
            byte mByte = BonCodeAJP13PacketHeaders.GetMethodByte("PUT");

            Console.WriteLine("A:" + aByte);
            Console.WriteLine("M:" + mByte);

            int a = 0;

            a = a + 2;
        }
    }
}
