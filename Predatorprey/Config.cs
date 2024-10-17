﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    public struct Config
    {
        public const int worldSize = 512;

        public const double deathRate = 0.03575;
        public const double birthRate = 0.20;
        public const double walkRate = 0.3;
        public const int walkDistance = 5;


        public const double predationRate = 0.8;

        public const int tracksStrength = 5;

        public const double predatorDensity = 0.15;
        public const double preyDensity = 0.15;

        public const int amountOfRounds = 210;

        public const int BlockSize = worldSize / 10;

        /// <summary>
        /// True if the simulation has tracking mechanic enabled, else false
        /// </summary>
        public static bool WithTracks = true;
        public const bool WithPrint = true;

        /// <summary>
        /// The seed which is used for the Random object.
        /// -1 if you want a random seed to be used.
        /// </summary>

        public const int rndSeed = -1;
    }
}