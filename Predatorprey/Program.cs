﻿using System;
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
            Program p = new RoundsUntilExtinctionProgram();
            Simulation sim = new Simulation(p, GetNewRandom());
            p.simulation = sim;
            p.Run(25);
        }

        /// <summary>
        /// Run the entire program/experiment once
        /// </summary>
        /// <param name="amountOfRuns">The amount of times each version should run for the experiment</param>
        protected virtual void Run(int amountOfRuns)
        {
            for (int i = 0; i < amountOfRuns; i++)
            {
                simulation.Run();
            }
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
        /// Run the experiment one time
        /// </summary>
        protected override void Run(int amountOfRuns)
        {
            Config.WithTracks = false;
            GetRoundUntilExtinctionSample(amountOfRuns);

            Config.WithTracks = true;
            GetRoundUntilExtinctionSample(amountOfRuns);

            double[] withoutTracks = roundsWOTracks.Select(x => (double)x).ToArray();
            double[] withTracks = roundsWTracks.Select(x => (double)x).ToArray();


            MeanDifference md = new MeanDifference();

            Console.WriteLine("\nAmount of successes without tracks:");
            Console.WriteLine(withoutTracks.Length);

            Console.WriteLine("Amount of successes with tracks:");
            Console.WriteLine(withTracks.Length);

            Console.WriteLine("Mean and sd without tracks:");
            Console.WriteLine(md.GetMeanAndSd(withoutTracks));

            Console.WriteLine("Mean and sd with tracks:");
            Console.WriteLine(md.GetMeanAndSd(withTracks));

            Console.WriteLine("p value more rounds without tracks:");
            Console.WriteLine(md.GetPDifferenceTwoIsGreater(withTracks, withoutTracks));

            Console.WriteLine("\nConfidence interval [without - with]:");
            Console.WriteLine(md.GetConfidenceIntervalDifference(withoutTracks, withTracks));
        }

        private void GetRoundUntilExtinctionSample(int amountOfRuns)
        {
            for (int x = 0; x < amountOfRuns; x++)
            {
                simulation.Run();
            }
        }

        public override void OnSimulationEnd()
        {
            if (Config.WithTracks && simulation.Extinction()) roundsWTracks.Add(simulation.roundNumber);
            else if(simulation.Extinction()) roundsWOTracks.Add(simulation.roundNumber);
        }
    }

    internal class ProportionExinctionProgram : Program
    {
        private int successesWOTracks;
        private int totalWOTracks;

        private int successesWTracks;
        private int totalWTracks;

        protected override void Run(int amountOfRuns)
        {
            Config.WithTracks = false;

            while (!EnoughSimulations(successesWOTracks, totalWOTracks))
            {
                totalWOTracks++;
                simulation.Run();
            }

            Config.WithTracks = true;

            while(!EnoughSimulations(successesWTracks, totalWTracks))
            {
                totalWTracks++; 
                simulation.Run();
            }



            Console.WriteLine("\nConfidence interval [without - with]");
            Console.WriteLine(new ProportionDifferenceCI().
                ConfidenceIntervalProp1MinProp2(successesWOTracks, successesWTracks, 
                    totalWOTracks, totalWTracks));

            Console.WriteLine("P-value proportion with tracks is larger than without tracks");
            Console.WriteLine(new ProportionDifferencePValue().
                CalculatePValuePropTwoIsGreater(successesWOTracks, successesWTracks, 
                    totalWOTracks, totalWTracks));
        }

        private bool EnoughSimulations(int successes, int total)
        {
            return successes >= 10 && total - successes >= 10;
        }

        public override void OnSimulationEnd()
        {
            if (simulation.Extinction())
            {
                if (Config.WithTracks) successesWTracks++;
                else successesWOTracks++;
            }
        }
    }

    /// <summary>
    /// Program for research about mean amount of prey eaten by a predator
    /// </summary>
    internal class MeanPreyEatenProgram : Program
    {
        // keep track of means between all simulation runs
        List<double> woTracks = new List<double>();
        List<double> wTracks = new List<double>();

        protected override void Run(int amountOfRuns)
        {
            Config.WithTracks = false;
            for (int i = 0; i < amountOfRuns; i++)
            {
                simulation.Run();
            }

            Config.WithTracks = true;
            for (int i = 0; i < amountOfRuns; i++)
            {
                simulation.Run();
            }

            // cast to array
            double[] withoutTracks = woTracks.ToArray();
            double[] withTracks = wTracks.ToArray();

            MeanDifference md = new MeanDifference();

            Console.WriteLine("\nMean and sd without tracks:");
            Console.WriteLine(md.GetMeanAndSd(withoutTracks));

            Console.WriteLine("Mean and sd with tracks:");
            Console.WriteLine(md.GetMeanAndSd(withTracks));

            Console.WriteLine("p-value lower predation mean with tracks:");
            Console.WriteLine(md.GetPDifferenceTwoIsGreater(withTracks, withoutTracks));

            Console.WriteLine("\nConfidence interval [without - with]:");
            Console.WriteLine(md.GetConfidenceIntervalDifference(withoutTracks, withTracks));
        }

        public override void OnSimulationEnd()
        {
            if(Config.WithTracks) wTracks.Add(simulation.averagePredation);
            else woTracks.Add(simulation.averagePredation);
        }
    }
}
