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
            Program p = new OscilationProgram();
            Simulation sim = new Simulation(p, GetNewRandom());
            p.simulation = sim;
            p.Run(50);
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

    /// <summary>
    /// Abstract class for all research about amount of rounds until
    /// extinction using some change.
    /// </summary>
    internal abstract class RoundsUntilExtinctionAbstract : Program
    {
        private List<int> roundsWOChange = new List<int>();
        private List<int> roundsWChange  = new List<int>();

        private bool _changeIsActive = false;

        protected override void Run(int amountOfRuns)
        {
            SetInitial();
            base.Run(amountOfRuns);

            Change();
            _changeIsActive = true;
            base.Run(amountOfRuns);

            double[] withoutChange = roundsWOChange.Select(x => (double)x).ToArray();
            double[] withChange = roundsWChange.Select(x => (double)x).ToArray();


            MeanDifference md = new MeanDifference();

            Console.WriteLine("\nAmount of successes without change:");
            Console.WriteLine(withoutChange.Length);

            Console.WriteLine("Amount of successes with change:");
            Console.WriteLine(withChange.Length);

            Console.WriteLine("Mean and sd without change:");
            Console.WriteLine(md.GetMeanAndSd(withoutChange));

            Console.WriteLine("Mean and sd with change:");
            Console.WriteLine(md.GetMeanAndSd(withChange));

            Console.WriteLine("p value more rounds without change:");
            Console.WriteLine(md.GetPDifferenceTwoIsGreater(withChange, withoutChange));

            Console.WriteLine("p value more rounds with change:");
            Console.WriteLine(md.GetPDifferenceTwoIsGreater(withoutChange, withChange));

            Console.WriteLine("\nConfidence interval [without - with]:");
            Console.WriteLine(md.GetConfidenceIntervalDifference(withoutChange, withChange));
        }

        public override void OnSimulationEnd()
        {
            if (simulation.Extinction())
            {
                if(_changeIsActive) roundsWChange.Add(simulation.roundNumber);
                else roundsWOChange.Add(simulation.roundNumber);
            }
        }

        /// <summary>
        /// Makes sure the change is not active
        /// </summary>
        protected abstract void SetInitial();
        /// <summary>
        /// Applies the change the research is about
        /// </summary>
        protected abstract void Change();
    }

    internal class RoundsUntilExtinctionWHuntingProgram : RoundsUntilExtinctionAbstract
    {
        protected override void SetInitial()
        {
            Config.WithTracks = false;
        }

        protected override void Change()
        {
            Config.WithTracks = true;
        }
    }

    internal class RoundsUntilExtinctionWDiffTracksStrength : RoundsUntilExtinctionAbstract
    {
        private readonly int _normalStrength;
        private readonly int _normalWalkDistance;
        private readonly int _diffStrength;
        private readonly int _diffWalkDistance;
        public RoundsUntilExtinctionWDiffTracksStrength(int normalStrength, int normalWalkDistance, int diffStrength, int diffWalkDistance)
        {
            _normalStrength = normalStrength;
            _normalWalkDistance = normalWalkDistance;
            _diffStrength = diffStrength;
            _diffWalkDistance = diffWalkDistance;
        }

        protected override void SetInitial()
        {
            Config.WithTracks = true;
            Config.tracksStrength = _normalStrength;
            Config.walkDistance = _normalWalkDistance;
        }

        protected override void Change()
        {
            Config.tracksStrength = _diffStrength;
            Config.walkDistance = _diffWalkDistance;
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

    internal class DiffTrackStrengthWithWalkDistanceProgram : Program
    {
        private readonly int[] _tracksStrengths;
        private int _currentIndex = 0;
        private List<int>[] _roundsPerStrength;
        public DiffTrackStrengthWithWalkDistanceProgram(int[] tracksStrengths)
        {
            this._tracksStrengths = tracksStrengths;
            _roundsPerStrength = new List<int>[tracksStrengths.Length];
            for (int i = 0; i < tracksStrengths.Length; i++) _roundsPerStrength[i] = new List<int>();
        }

        protected override void Run(int amountOfRuns)
        {
            for (; _currentIndex < _tracksStrengths.Length; _currentIndex++)
            {
                Config.tracksStrength = _tracksStrengths[_currentIndex];
                Config.walkDistance   = _tracksStrengths[_currentIndex];
                base.Run(amountOfRuns);
            }

            Console.WriteLine($"\nDone after {amountOfRuns} simulations per variant.");
            MeanDifference md = new MeanDifference();

            for (int i = 0; i < _tracksStrengths.Length; i++)
            {
                Console.WriteLine($"\nMean and sd between {_roundsPerStrength[i].Count} extinctions for trackStrength = walkDistance = {_tracksStrengths[i]}:");
                Console.WriteLine(md.GetMeanAndSd(_roundsPerStrength[i].Select(x => (double)x).ToArray()));
            }
        }

        public override void OnSimulationEnd()
        {
            if (simulation.Extinction())
            {
                _roundsPerStrength[_currentIndex].Add(simulation.roundNumber);
            }
        }
    }

    internal class ExtinctionRateFindProgram : Program
    {
        private int amountOfExtinctions = 0;
        private int amountOfNoExtinctions = 0;

        protected override void Run(int amountOfTimes)
        {
            base.Run(amountOfTimes);
            Console.WriteLine("Amount of extinctions: ");
            Console.WriteLine(amountOfExtinctions);
        }

        public override void OnSimulationEnd()
        {
            if (simulation.Extinction()) amountOfExtinctions++;
            else amountOfNoExtinctions++;
        }
    }

    internal class TrackDifferenceGraphProgram : Program
    {
        protected override void Run(int amountOfRuns)
        {
            Config.WithTracks = false;
            Config.WithPrint = false;

                simulation.Run();
            PlotManager pmwo = simulation.GetPlotManager();
            pmwo.Fourier();
            


            Config.WithTracks = true;
                simulation.Run();

            PlotManager pmw = simulation.GetPlotManager();
            pmw.Fourier();

            PlotManager.SaveTwoPlots(pmwo, pmw);

            
        }

    }

    internal class OscilationProgram : Program
    {
        protected override void Run(int amountOfRuns)
        {
            Config.WithTracks = false;
            Config.WithPrint = false;
            double[] withoutChange = new double[Config.amountOfRounds];
            double[] withChange = new double[Config.amountOfRounds];

            for (int i = 0; i < amountOfRuns; i++)
            {
                simulation.Run();
                PlotManager pmwo = simulation.GetPlotManager();
                withoutChange[i] = pmwo.Fourier().Item3;

            }

           

            Config.WithTracks = true;

            for (int i = 0; i < amountOfRuns; i++)
            {
                simulation.Run();
                PlotManager pmw = simulation.GetPlotManager();
                withChange[i] = pmw.Fourier().Item3;
            }

            MeanDifference md = new MeanDifference();

            Console.WriteLine("Mean and sd without change:");
            Console.WriteLine(md.GetMeanAndSd(withoutChange));

            Console.WriteLine("Mean and sd with change:");
            Console.WriteLine(md.GetMeanAndSd(withChange));

            Console.WriteLine("p value more rounds without change:");
            Console.WriteLine(md.GetPDifferenceTwoIsGreater(withChange, withoutChange));

            Console.WriteLine("p value more rounds with change:");
            Console.WriteLine(md.GetPDifferenceTwoIsGreater(withoutChange, withChange));

            Console.WriteLine("\nConfidence interval [without - with]:");
            Console.WriteLine(md.GetConfidenceIntervalDifference(withoutChange, withChange));
        }
    }

}
