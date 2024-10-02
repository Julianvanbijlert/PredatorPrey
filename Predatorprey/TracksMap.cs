using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    public enum Direction { Up, Right, Down, Left }


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
        /// Get a random surrounding higher track location. If there is no such location, return the current location.
        /// </summary>
        /// <param name="x">The x of the original location</param>
        /// <param name="y">The y of the original location</param>
        /// <returns>A random location with higher track or the given location if no such location exists</returns>
        public (int, int) GetSurroundingHigherTrack(int x, int y)
        {
            int highestTrack = _tracksMap[x, y];

            List<(int, int)> highTrackLocations = new List<(int, int)>();

            // up
            LookForHigherTrack(x, y + 1, highTrackLocations, ref highestTrack);
            // right
            LookForHigherTrack(x + 1, y, highTrackLocations, ref highestTrack);
            // down
            LookForHigherTrack(x, y - 1, highTrackLocations, ref highestTrack);
            // left
            LookForHigherTrack(x - 1, y, highTrackLocations, ref highestTrack);

            // if the original value remained, ensure that the original location is returned
            if (highestTrack == _tracksMap[x, y])
                return (x, y);

            // choose a random location from the list
            return highTrackLocations[rnd.Next(highTrackLocations.Count)];
        }

        /// <summary>
        /// Compare given location with the highest track. Update list with highest track locations.
        /// </summary>
        public void LookForHigherTrack(int x, int y, List<(int, int)> highTracks, ref int highestTrack)
        {
            // Check whether location is on the map
            if (!IsWithinTracksMap(x, y)) return;

            if (_tracksMap[x, y] > highestTrack)
            {
                // new highest track is found, reset list and highestTrack value
                highTracks.Clear();
                highTracks.Add((x, y));
                highestTrack = _tracksMap[x, y];
            }
            else if (_tracksMap[x, y] == highestTrack)
            {
                // equal track is found, add to list
                highTracks.Add((x, y));
            }
        }

        private bool IsWithinTracksMap(int x, int y)
        {
            return x >= 0 && x < Config.worldSize && y >= 0 && y < Config.worldSize;
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
