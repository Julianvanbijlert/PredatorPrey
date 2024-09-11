using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{ 
    public struct Config
    {
        public const int worldSize = 50;

        public const double deathRate = 0.025;
        public const double birthRate = 1;
        public const double walkRate = 0;

        public const double predationRate = 0.25;

        public const double preditorDensity = 0.3;
        public const double preyDensity = 0.3;

        public const int amountOfRounds = 1500;

        public const int BlockSize = 10;
    }
}
