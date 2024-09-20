using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    public class SmellMap
    {
        public int[,] _smellMap;

        public SmellMap()
        {
            _smellMap = new int[Config.worldSize, Config.worldSize];
        }

        public void AddSmell(int x, int y)
        {
            _smellMap[x, y] = Config.smellStrength;
        }

        public int GetSmell(int x, int y)
        {
            return _smellMap[x, y];
        }

        private void DecreaseSmell(int x, int y)
        {
            _smellMap[x, y]--;
        }

        /// <summary>
        /// Decrease all smells in the world by 1
        /// </summary>
        public void DecreaseSmells()
        {
            for (int y = 0; y < Config.worldSize; y++)
            {
                for (int x = 0; x < Config.worldSize; x++)
                {
                    if (_smellMap[x, y] > 0)
                    {
                        DecreaseSmell(x, y);
                    }
                }
            }
        }

        /// <summary>
        /// Return a list of coordinates with locations, all of which have the
        /// same, highest, smell number
        /// </summary>
        /// <param name="x">X of the queried location</param>
        /// <param name="y">Y of the queried location</param>
        /// <returns>List of locations with the highest smell numbers</returns>
        public List<(int, int)> GetHighestSurroundingSmells(int x, int y)
        {
            // keep track of highest smell value and smell list
            int maxSmell = int.MinValue;
            List<(int, int)> result = new List<(int, int)>();

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int locationX = x + dx;
                    int locationY = y + dy;

                    // skip if location is outside grid
                    if (!World.IsWithinGrid(locationX, locationY))
                        continue;

                    // new highest sell is found. Reset smell value and list.
                    if (_smellMap[locationX, locationY] > maxSmell)
                    {
                        maxSmell = _smellMap[locationX, locationY];
                        result.Clear();
                        result.Add((locationX, locationY));
                    }
                    // location has (not unique) highest smell
                    else if (_smellMap[locationX, locationY] == maxSmell)
                    {
                        result.Add((locationX, locationY));
                    }
                }
            }

            return result;
        }

        public bool AnyWithSmell()
        {
            foreach (int x in _smellMap)
            {
                if (x > 0) return true;
            }

            return false;
        }
    }   
}
