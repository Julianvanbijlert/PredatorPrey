using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    public struct Config
    {
        public const int worldSize = 512;

        public const double deathRate = 0.2;
        public const double birthRate = 0.3;
        public const double walkRate = 0.3;
        public static int walkDistance = 5;


        public const double predationRate = 0.6;

        public static int tracksStrength = 5;

        public const double predatorDensity = 0.2;
        public const double preyDensity = 0.2;

        public const int amountOfRounds = 500;

        public const int BlockSize = worldSize / 10;

        /// <summary>
        /// True if the simulation has tracking mechanic enabled, else false
        /// </summary>
        public static bool WithTracks = true;
        public static bool WithPrint = true;

        /// <summary>
        /// The seed which is used for the Random object.
        /// -1 if you want a random seed to be used.
        /// </summary>

        public const int rndSeed = -1;
    }
}