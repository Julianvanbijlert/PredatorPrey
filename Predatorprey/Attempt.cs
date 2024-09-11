using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    /// <summary>
    /// Class made for the Success function
    /// </summary>
    public class Attempt
    {
        /// <summary>
        /// Returns whether a random draw failed or succeeded
        /// </summary>
        /// <param name="rnd">A random generator</param>
        /// <param name="chance">The chance that the random draw succeeds</param>
        /// <returns>Whether the draw succeeded or not</returns>
        public static bool Success(Random rnd, double chance) => rnd.NextDouble() < chance;
    }
}
