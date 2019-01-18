using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automate;

namespace eNKA2DKA
{
    class Program
    {
        static void Main()
        {
            eNKA enka = new eNKA("2");

            DKA dka = AutomateFunction.toDKA(enka);

            dka.PrintToFile("dka");

        }
    }
}
