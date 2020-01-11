using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string str = "-3,85865E-05 -0,009698272 -0,202958992 -0,361067725 2,829697678";

            string res = str.Replace(",", ".").Replace(" ", ", ").Replace("E", "e")+",";

            System.Diagnostics.Debug.WriteLine("\n\n"+ res+ "\n\n");
        }
    }
}
