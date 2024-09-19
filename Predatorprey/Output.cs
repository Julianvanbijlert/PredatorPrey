
////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Text;
////using System.Threading.Tasks;

////namespace Project1
////{
////    public static class Output
////    {

////        public static void PrintWorld(World world)
////        {
////            //Entities per set gridsize

////            Console.Clear();
////            EntityManager[,] grid = world.GetGrid;
////            world.PrintStats();

////            PrintGrid(grid);
           
////        }

////        public static void PrintGrid(EntityManager[,] grid)
////        {

////            Console.Clear();

////            int tempAmountOfEntities = 0;
////            int[] countArray = new int[Config.worldSize / Config.BlockSize + 1]; //+1 for the last block

////            for (int y = 0; y < Config.worldSize; y++)
////            {
////                for (int x = 0; x < Config.worldSize; x++)
////                {

////                    //checks if there is an entity on the grid
////                    if(grid[x, y] != null)
////                        tempAmountOfEntities++; 
                    


////                    if (x % Config.BlockSize == 0)
////                    {
////                        //collect entities in blocks
////                        int pointer = x / Config.BlockSize;
////                        countArray[pointer] += tempAmountOfEntities;
////                        tempAmountOfEntities = 0;

////                        if (y % Config.BlockSize == 0)
////                        {
////                            //print the amount of entities in the block
////                            Console.Write("|" + countArray[pointer] + "|");
////                            countArray[pointer] = 0;

////                            //make sure the gris is "square"
////                            if(pointer == Config.worldSize / Config.BlockSize)
////                                Console.WriteLine();    


////                        }
////                    }

////                }

////            }
////        }

        

////        public static void PrintList(World world)
////        {
////            HashSet<EntityManager> entities = world.GetEntities;

////            int blockSize = (int)Config.worldSize / Config.BlockSize;
////            int[] countArray = new int[blockSize * blockSize];

////            foreach (EntityManager e in entities)
////            {
////                countArray[e.x / Config.BlockSize + e.y / Config.BlockSize * blockSize]++;
////            }

////            for (int i = 0; i < countArray.Length; i++)
////            {
////                 Console.Write(countArray[i] + "|");
////                 if ((i + 1) % blockSize == 0)
////                 {
////                     Console.WriteLine();
////                 } 
////            }
////        }

////    }
////}

