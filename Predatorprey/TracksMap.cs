using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    public class TracksMap
    {
        public int[,] _tracksMap;

        public TracksMap()
        {
            _tracksMap = new int[Config.worldSize, Config.worldSize];
        }

        public void AddTrack(int x, int y)
        {
            _tracksMap[x, y] = Config.tracksStrength;
        }

        public int GetTrack(int x, int y)
        {
            return _tracksMap[x, y];
        }

        private void DecreaseTrack(int x, int y)
        {
            _tracksMap[x, y]--;
        }

        /// <summary>
        /// Decrease all tracks in the world by 1
        /// </summary>
        public void DecreaseTracks()
        {
            for (int y = 0; y < Config.worldSize; y++)
            {
                for (int x = 0; x < Config.worldSize; x++)
                {
                    if (_tracksMap[x, y] > 0)
                    {
                        DecreaseTrack(x, y);
                    }
                }
            }
        }

        /// <summary>
        /// Return a list of coordinates with locations, all of which have the
        /// same, highest, track number
        /// </summary>
        /// <param name="x">X of the queried location</param>
        /// <param name="y">Y of the queried location</param>
        /// <returns>List of locations with the highest track numbers</returns>
        public List<(int, int)> GetHighestSurroundingTracks(int x, int y)
        {
            // keep track of highest track value and track list
            int maxTrack = int.MinValue;
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

                    // new highest sell is found. Reset track value and list.
                    if (_tracksMap[locationX, locationY] > maxTrack)
                    {
                        maxTrack = _tracksMap[locationX, locationY];
                        result.Clear();
                        result.Add((locationX, locationY));
                    }
                    // location has (not unique) highest track
                    else if (_tracksMap[locationX, locationY] == maxTrack)
                    {
                        result.Add((locationX, locationY));
                    }
                }
            }

            return result;
        }

        public bool AnyWithTrack()
        {
            foreach (int x in _tracksMap)
            {
                if (x > 0) return true;
            }

            return false;
        }
    }   
}
