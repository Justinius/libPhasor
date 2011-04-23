using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libPhasor;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            phasor my_phasor = new phasor(1, 1, true);
            phasor my_phasor2 = new phasor(0, 10, true);
            Console.WriteLine("{0} {1}", my_phasor.mag, my_phasor.angle_deg);
            Console.WriteLine("{0}", my_phasor.ToString());
            Console.WriteLine("--------------------------------");
            Console.WriteLine("{0} {1}", my_phasor2.mag, my_phasor2.angle_deg);
            Console.WriteLine("{0}", my_phasor2.ToString());
            Console.WriteLine("--------------------------------");
            phasor add_phas = my_phasor + my_phasor2;
            Console.WriteLine("{0} {1}", add_phas.mag, add_phas.angle_deg);
            Console.WriteLine("{0}", add_phas.ToString());
            Console.ReadLine();
        }
    }
}
