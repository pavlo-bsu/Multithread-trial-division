using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;


namespace Pavlo.DivisorsSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            BigInteger theNumber = 12345678998765;
            TrialDivisionMT td = new TrialDivisionMT(theNumber, 1, "result.txt");
            td.Start();
            Console.WriteLine("Calculation has been completed. Output file has been written.");
            Console.ReadLine();
        }
    }

}
