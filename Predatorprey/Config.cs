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

        public const double deathRate = 0.025;
        public const double birthRate = 1;
        public const double walkRate = 0.1;
        public const int walkDistance = 5;
        public const int trackDistance = 1;


        public const double predationRate = 0.1;

        public const int tracksStrength = 5;

        public const double preditorDensity = 0.15;
        public const double preyDensity = 0.15;

        public const int amountOfRounds = 5000;

        public const int BlockSize = worldSize / 10;

        /// <summary>
        /// True if the simulation has tracking mechanic enabled, else false
        /// </summary>
        public const bool WithTracks = true;
        public const bool WithPrint = true;
    }
}
