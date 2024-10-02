using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    public enum Direction { Null, Up, Right, Down, Left }


    public class TracksMap
    {
        public int[,] _tracksMap;

        private Direction[,] _directionsMap;

        private Random rnd;

        public TracksMap(Random rnd)
        {
            _tracksMap = new int[Config.worldSize, Config.worldSize];
            _directionsMap = new Direction[Config.worldSize, Config.worldSize];
            this.rnd = rnd;
        }

        public void AddTrack(int x, int y, Direction direction)
        {
                _tracksMap[x, y] = Config.tracksStrength;
            _directionsMap[x, y] = direction;
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
        /// Get the direction stored on the given location
        /// </summary>
        public Direction GetDirection(int x, int y)
        {
            return _directionsMap[x, y];
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
