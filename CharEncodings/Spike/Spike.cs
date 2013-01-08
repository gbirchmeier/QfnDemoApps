using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spike
{
    class Spike
    {
        static void Main(string[] args)
        {
            // ole's "é" character:
            // * bytes 0xc3 0xa9
            // * utf \u00e9
            byte[] ole1 = { 0x6f, 0x6c, 0xc3, 0xa9 };
            byte[] ole2 = Encoding.UTF8.GetBytes("olé");

            Console.WriteLine(String.Format("sizes: {0} = {1}", ole1.Length, ole2.Length));
            Console.WriteLine(String.Format("1: {0} / {1}", BitConverter.ToString(ole1), Encoding.UTF8.GetString(ole1)));
            Console.WriteLine(String.Format("2: {0} / {1}", BitConverter.ToString(ole2), Encoding.UTF8.GetString(ole2)));

            if (ole1.Length == ole2.Length)
            {
                for (int i = 0; i < ole1.Length; i++)
                {
                    Console.WriteLine("Char {0}: {1} {2}", i, ole1[i], ole2[i]);
                }
            }


        }
    }
}
