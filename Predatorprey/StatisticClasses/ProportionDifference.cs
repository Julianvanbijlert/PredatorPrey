using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;

namespace Project1
{
    public class ProportionDifferenceCI
    {
        public (double, double) ConfidenceIntervalProp1MinProp2(int success1, int success2, int n1, int n2)
        {
            if (!CheckConditions(success1, success2)) throw new ArgumentException("Data does not check conditions for valid CI.");

            double prop1 = (double)success1 / (double)n1;
            double prop2 = (double)success2 / (double)n2;

            double pointEstimate = prop1 - prop2;
            double se = CalculateSe(prop1, prop2, n1, n2);
            double errorMargin = 1.96 * se;

            return (pointEstimate - errorMargin, pointEstimate + errorMargin);
        }

        private bool CheckConditions(int success1, int success2)
        {
            return success1 >= 10 && success2 >= 10;
        }

        private double CalculateSe(double prop1, double prop2, int n1, int n2)
        {
            double seSquared = prop1 * (1-prop1) / (double)n1 + prop2 * (1-prop2) / (double)n2;
            return Math.Sqrt(seSquared);
        }   
    }

    public class ProportionDifferencePValue
    {
        public double CalculatePValuePropTwoIsGreater(int success1, int success2, int n1, int n2)
        {
            double pooledProp = (double)(success1 + success2) / (double)(n1 + n2);

            if(!CheckConditions(pooledProp, n1, n2)) throw new ArgumentException("Data does not check conditions for valid p-value.");

            double pointEstimate = (double)success1 / (double)n1 - (double)success2 / (double)n2;
            double se = CalculateSe(pooledProp, n1, n2);
            double zScore = pointEstimate / se;

            double pValue = Normal.CDF(0, 1, zScore);
            return pValue;
        }

        private bool CheckConditions(double pooledProp, int n1, int n2)
        {
            return pooledProp * n1 >= 10 && pooledProp * n2 >= 10;
        }

        private double CalculateSe(double pooledProp, int n1, int n2) 
        {
            double seSquared = pooledProp * (1 - pooledProp) / (double)n1 + pooledProp * (1 - pooledProp) / (double)n2;
            return Math.Sqrt(seSquared);
        }
    }
}
