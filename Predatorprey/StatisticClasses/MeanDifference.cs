using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;

namespace Project1
{
    public class MeanDifference
    {
        public (double, double) GetConfidenceIntervalDifference(double[] class1, double[] class2)
        {
            (double meanDiff, double se) = GetMeanDiffAndSe(class1, class2);
            return (meanDiff - se * 1.96, meanDiff + se * 1.96);
        }

        public double GetPDifferenceTwoIsGreater(double[] class1, double[] class2)
        {
            // H0: mean1 - mean2 == 0
            // Ha: mean1 - mean2 <  0

            (double meanDiff, double se) = GetMeanDiffAndSe(class1, class2);

            double T = (meanDiff - 0) / se;

            var df = Math.Min(class1.Length - 1, class2.Length - 1);

            double cdf = GetCumDistr(T, df);

            return cdf;
        }

        public (double mean, double sd) GetMeanAndSd(double[] ints)
        {
            // https://stackoverflow.com/questions/5336457/how-to-calculate-a-standard-deviation-array
            double mean = ints.Average();
            double sumOfSquaresOfDifferences = ints.Select(val => (val - mean) * (val - mean)).Sum();
            double sd = Math.Sqrt(sumOfSquaresOfDifferences / ints.Length);

            return (mean, sd);
        }

        private (double meanDiff, double se) GetMeanDiffAndSe(double[] class1, double[] class2)
        {
            (double mean1, double sd1) = GetMeanAndSd(class1);
            (double mean2, double sd2) = GetMeanAndSd(class2);

            double meanDiff = mean1 - mean2;

            double se = Math.Sqrt(sd1 * sd1 / (double)class1.Length + sd2 * sd2 / (double)class2.Length);

            return (meanDiff, se);
        }

        public double GetCumDistr(double t, int df)
        {
            return new StudentT(0, 1, df).CumulativeDistribution(t);
        }
    }
}
