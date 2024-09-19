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

        public void AddSmell(int x, int y, int smell)
        {
            _smellMap[x, y] = smell;
        }

        public int GetSmell(int x, int y)
        {
            return _smellMap[x, y];
        }

        public void DecreaseSmell(int x, int y)
        {
            _smellMap[x, y]--;
        }

        public void DecreaseSmells()
        {
            for (int y = 0; y < Config.worldSize; y++)
            {
                for (int x = 0; x < Config.worldSize; x++)
                {
                    if (_smellMap[x, y] > 0)
                    {
                        _smellMap[x, y]--;
                    }
                }
            }
        }

        public (int, int) GetSurroundingSmells(int x, int y)
        {
            if (World.IsWithinGrid(x + 1, y) && _smellMap[x + 1, y] > 0) return (x + 1, y);
            if (World.IsWithinGrid(x - 1, y) && _smellMap[x - 1, y] > 0) return (x - 1, y);
            if (World.IsWithinGrid(x, y + 1) && _smellMap[x, y + 1] > 0) return (x, y + 1);
            if (World.IsWithinGrid(x, y - 1) &&_smellMap[x, y - 1] > 0) return (x, y - 1);

            return (x,y);
        }
    }   
}
