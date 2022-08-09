using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var port = Environment.GetEnvironmentVariable("PORT");
            DedicatedServer.Start(port == null ? 26950 : int.Parse(port));

            DedicatedServer.ConsoleCommand();
            // var a = test("babad");
            // Console.WriteLine(Solution.LongestPalindrome("abbcccbbbcaaccbababcbcabca"));
            //Console.WriteLine('c' != 'd');
        }

    }
}
