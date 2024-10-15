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
            Simulation sim = new Simulation(GetNewRandom());
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

        /// <summary>
        /// Get a Random object based on the seed in Config
        /// </summary>
        private static Random GetNewRandom()
        {
            if (Config.rndSeed != -1)
                return new Random(Config.rndSeed);

            // -1 signals you want a random seed, so do not specify the seed
            // when making the Random object.
            return new Random();
        }
    }
}
