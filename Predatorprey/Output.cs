using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    public static class Output
    {

        public static void PrintWorld(World world)
        {
            //Entities per set gridsize

            //Console.Clear();
            HashSet<Entity>[,] grid = world.GetGrid;
            Console.WriteLine("Entities: " + world.AmountOfEntities);
            Console.WriteLine("Predators: " + world.AmountOfPredators);
            Console.WriteLine("Prey: " + world.AmountOfPrey);

            PrintGrid(grid);
           
        }

        public static void PrintGrid(HashSet<Entity>[,] grid)
        {
            int amountOfEntities = 0;

            for (int y = 0; y < Config.worldSize; y++)
            {
                for (int x = 0; x < Config.worldSize; x++)
                {
                    amountOfEntities += grid[x, y].Count;
                    if (x % Config.BlockSize == 0)
                    {
                        Console.Write("|" + amountOfEntities + "|");
                        amountOfEntities = 0;
                    }

                }

                Console.WriteLine();
            }
        }

        

        public static void PrintList(World world)
        {
            HashSet<Entity> entities = world.GetEntities;

            int blockSize = (int)Config.worldSize / Config.BlockSize;
            int[] countArray = new int[blockSize * blockSize];

            foreach (Entity e in entities)
            {
                countArray[e.x / Config.BlockSize + e.y / Config.BlockSize * blockSize]++;
            }

            for (int i = 0; i < countArray.Length; i++)
            {
                 Console.Write(countArray[i] + "|");
                 if ((i + 1) % blockSize == 0)
                 {
                     Console.WriteLine();
                 } 
            }
        }

    }
}
