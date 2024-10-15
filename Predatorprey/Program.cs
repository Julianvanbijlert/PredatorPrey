using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    internal class Program
    {
        protected Simulation simulation;

        public static void Main()
        {
            Simulation sim = new Simulation();
            sim.Initialize();
            Program p = new Program(sim);
            p.Run();
        }

        public Program(Simulation s)
        {
            this.simulation = s;
        }

        protected virtual void Run()
        {
            simulation.Run();
        }
    }
}
