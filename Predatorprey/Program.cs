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
            Program p = new Program();
            Simulation sim = new Simulation(p, GetNewRandom());
            p.simulation = sim;
            p.Run();
        }

        public Program()
        {
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

        public virtual void OnSimulationEnd(){}
    }

    internal class RoundsUntilExtinctionProgram : Program
    {
        private List<int> roundsWOTracks = new List<int>();
        private List<int> roundsWTracks = new List<int>();

        /// <summary>
        /// Run the simulation one time
        /// </summary>
        protected override void Run()
        {
            Config.WithTracks = false;
            List<double> roundWithoutTracks = GetRoundUntilExtinctionSample();

            Config.WithTracks = true;
            List<double> roundWithTracks = GetRoundUntilExtinctionSample();

            MeanDifference md = new MeanDifference();
            Console.WriteLine("p value:");
            Console.WriteLine(md.GetPDifferenceTwoIsGreater(roundWithTracks.ToArray(), roundWithoutTracks.ToArray()));
            Console.WriteLine("\nConfidence interval:");
            Console.WriteLine(md.GetConfidenceIntervalDifference(roundWithoutTracks.ToArray(), roundWithTracks.ToArray()));
        }

        private List<double> GetRoundUntilExtinctionSample()
        {
            int amountOfSimulations = 20;
            List<double> roundsUntilExtinction = new List<double>(amountOfSimulations);

            for (int x = 0; x < amountOfSimulations; x++)
            {
                simulation.Run();
            }

            return roundsUntilExtinction;
        }

        public override void OnSimulationEnd()
        {
            if (Config.WithTracks) roundsWTracks.Add(simulation.roundNumber);
            else roundsWOTracks.Add(simulation.roundNumber);
        }
    }
}
