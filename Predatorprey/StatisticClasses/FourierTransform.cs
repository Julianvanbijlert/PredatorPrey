using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;

namespace Project1
{
    internal class FourierTransform
    {
        // this method was made by ChatGPT, I do not know how to check the validity of it
        public static (double, double, double) GetTopFrequency(double[] preyAmounts)
        {
            int n = preyAmounts.Length;

            // Prepare a complex array for FFT (real part is your prey amounts, imaginary part is 0)
            Complex[] preyData = new Complex[n];
            for (int i = 0; i < n; i++)
            {
                preyData[i] = new Complex(preyAmounts[i], 0);  // Initialize imaginary part to 0
            }

            // Apply FFT to the data
            Fourier.Forward(preyData, FourierOptions.Matlab);

            // Find the dominant frequency (the index of the highest magnitude)
            double[] magnitudes = new double[n];
            for (int i = 0; i < n; i++)
            {
                magnitudes[i] = preyData[i].Magnitude;  // Compute magnitude for each frequency
            }

            // Find the index of the peak frequency (ignoring the zero frequency component at index 0)
            int peakIndex = 1;
            double maxMagnitude = magnitudes[1];
            for (int i = 2; i < n / 2; i++)  // Only look at the first half (positive frequencies)
            {
                if (magnitudes[i] > maxMagnitude)
                {
                    maxMagnitude = magnitudes[i];
                    peakIndex = i;
                }
            }

            // Calculate the period from the dominant frequency
            double frequency = (double)peakIndex / n; // Normalized frequency
            double period = 1 / frequency;

            Console.WriteLine($"Dominant frequency index: {peakIndex}");
            Console.WriteLine($"Dominant frequency: {frequency}");
            Console.WriteLine($"Estimated period: {period}");

            return (peakIndex, frequency, period);
        }
    }
}
