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

            Console.WriteLine(my_phasor.Equals(my_phasor2).ToString());
            Console.WriteLine(my_phasor == my_phasor);
            Console.WriteLine(my_phasor == my_phasor2);
            Console.WriteLine(my_phasor != my_phasor);
            Console.WriteLine(my_phasor != my_phasor2);

            phasor copy_phas = new phasor(my_phasor);
            Console.WriteLine(copy_phas.ToString());

            phasor my_resist = new phasor().set_resistor(12);
            Console.WriteLine(my_resist.ToString());
            
            //sample of use in circuit type analysis
            //will use examples from http://www.calvin.edu/~svleest/circuitExamples/PhasorDomain/
            //Will work on example 1, 4, and 8

            Console.WriteLine("----------------------------------");
            Console.WriteLine("Engineering Examples");
            Console.WriteLine();
            //Example one
            Console.WriteLine("Example 1 on http://www.calvin.edu/~svleest/circuitExamples/PhasorDomain/");
            phasor source = new phasor().set_source(10, 0);
            Console.WriteLine("Source Phasor: {0}", source.ToString());
            phasor r = new phasor().set_resistor(1000); //needs to be in Ohms
            Console.WriteLine("Resistor Phasor: {0}", r.ToString());
            phasor c = new phasor().set_capacitor(1e-6, 2000); //1 microFarad and the source is 2000
            Console.WriteLine("Capacitor Phasor: {0}", c.ToString());
            
            phasor total_imped = r + c;
            Console.WriteLine("Total Impedence is Resistor + Capacitor = {0}", total_imped.ToString());
            phasor total_curr = source / total_imped;
            Console.WriteLine("Total Current is (Source / Total Impedence) = {0}", total_curr.ToString());

            phasor c_voltage = total_curr * c;
            Console.WriteLine("Capacitor voltage is (Total Current * Capacitor Impedence) = {0}", c_voltage.ToString());

            Console.WriteLine();
            Console.WriteLine("Example 4 on http://www.calvin.edu/~svleest/circuitExamples/PhasorDomain/");
            source = new phasor().set_source(8, 45);
            phasor r1 = new phasor().set_resistor(3000);
            phasor r2 = new phasor().set_resistor(1000);
            c = new phasor().set_capacitor(1e-9, 1e6);
            phasor l = new phasor().set_inductor(2e-3, 1e6);

            Console.WriteLine("Components in circuit");
            Console.WriteLine("Source Phasor: {0}", source.ToString());
            Console.WriteLine("R1 Phasor: {0}", r1.ToString());
            Console.WriteLine("R2 Phasor: {0}", r2.ToString());
            Console.WriteLine("C Phasor: {0}", c.ToString());
            Console.WriteLine("L Phasor: {0}", l.ToString());

            Console.WriteLine("Combine the capacitor and inductor into a combined impedence in series.");
            phasor RsC = r2 + c;
            Console.WriteLine("C + R2 := {0} + {1} = {2}", c.ToString(), r2.ToString(), RsC.ToString());
            Console.WriteLine("Combine the inductor with the previous impedence in parallel.");
            phasor LpRsC = 1 / ((1 / l) + (1 / RsC));
            Console.WriteLine("L in || with (R2+C) := 1/(1/{0} + 1/{1} = {2})", l.ToString(), RsC.ToString(), LpRsC.ToString());
            Console.WriteLine("Finally add the last resistor in series.");
            phasor circ_imped = r1 + LpRsC;
            Console.WriteLine("R1 in series with (L || (R2+C)) := {0} + {1} = {2}", r1.ToString(), LpRsC.ToString(), circ_imped.ToString());
            phasor circ_curr = source / circ_imped;
            Console.WriteLine("Compute total current (Itot)  with Source/TotImped := ({0})/({1}) = {2}", source.ToString(), circ_imped.ToString(), circ_curr.ToString());

            Console.WriteLine("Since L is in || with (C + R2) we can just find");
            Console.WriteLine("the voltage drop across R1 and subtract it from the source.");
            phasor Vx = source - r1*circ_curr;
            Console.WriteLine("(S - R1 * Itot) = {0}", Vx.ToString());

            Console.WriteLine();
            Console.WriteLine("Example 8 on http://www.calvin.edu/~svleest/circuitExamples/PhasorDomain/");
            source = new phasor().set_source(5, 0);
            r1 = new phasor().set_resistor(5000);
            c = new phasor().set_capacitor(70e-12, 2e6);
            l = new phasor().set_inductor(5e-3, 2e6);
            Console.WriteLine("Components in circuit");
            Console.WriteLine("Source Phasor: {0}", source.ToString());
            Console.WriteLine("R1 Phasor: {0}", r1.ToString());
            Console.WriteLine("C Phasor: {0}", c.ToString());
            Console.WriteLine("L Phasor: {0}", l.ToString());

            Console.WriteLine("Need to find current through the inductor.");
            Console.WriteLine("Total current in both loops = {0}", source.ToString());
            phasor L_curr = (r1 / (r1 + c + l)) * source;
            Console.WriteLine("Use current divider L_curr := (R1/(R1 + C+L))*Current = {0}", L_curr.ToString());

            

            Console.ReadLine();
        }
    }
}
