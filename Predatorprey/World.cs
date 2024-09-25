using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    public class World
    {
        /// <summary>
        /// A random generator used throughout the whole project
        /// </summary>
        public Random rnd { get; private set; }

        /// <summary>
        /// The 'map' of the world, the integers represent the index of the
        /// predator in the entity list
        /// </summary>
        public int [,] grid { get; private set; }

        /// <summary>
        /// Contains all active entities in the world
        /// </summary>
        public EntityList entities { get; private set; }


        public SmellMap smellMap { get; private set; }

        /// <summary>
        /// Represents the world with a grid and the entities on it.
        /// Is responsible for all operations over the entities, like moving, adding and removing
        /// </summary>
        public World(Random rnd)
        {
            this.rnd = rnd;

            entities = new EntityList(rnd, this);

            if(Config.WithSmell) this.smellMap = new SmellMap();

            InitializeGrid();
        }

        /// <summary>
        /// Initialize the grid of the world by making an empty 2D array of Entities
        /// </summary>
        private void InitializeGrid()
        {
            //Make the world grid
            int size = Config.worldSize;
            grid = new int[size, size];
            // Fill the grid with empty spots (-1 signals empty)
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    grid[x, y] = -1;
                }
            }
        }

        /// <summary>
        /// Returns whether the given cell is empty
        /// </summary>
        /// <param name="x">X coordinate of the cell</param>
        /// <param name="y">Y coordinate of the cell</param>
        /// <returns>True if the cell is empty, else false</returns>
        public bool IsEmptyCell(int x, int y)
        {
            return grid[x, y] == -1;
        }

        /// <summary>
        /// Return the type of the entity in a cell. This function is unsafe
        /// and must only be used if the cell is not empty.
        /// </summary>
        /// <param name="x">X coordinate of the cell</param>
        /// <param name="y">Y coordinate of the cell</param>
        /// <returns>The type of the entity in the cell</returns>
        public EntityType EntityTypeOnLocation(int x, int y)
        {
            return entities.GetEntityType(grid[x, y]);
        }

        public List<int> GetSurroundingPrey(int x, int y)
        {
            List<int> result = new List<int>();

            // up
            AddIfPreyOnLocation(result, x, y + 1);
            // right
            AddIfPreyOnLocation(result, x + 1, y);
            // down
            AddIfPreyOnLocation(result, x, y - 1);
            // left
            AddIfPreyOnLocation(result, x - 1, y);

            return result;
        }

        /// <summary>
        /// Add prey index to list if location has a prey
        /// </summary>
        private void AddIfPreyOnLocation(List<int> l, int x, int y)
        {
            int index = PreyOnLocation(x, y);
            if(index != -1) l.Add(index);
        }

        /// <summary>
        /// Get the (index of the) prey that is on the specified location, if any.
        /// Else, -1 is returned.
        /// </summary>
        /// <param name="x">x coordinate of the location</param>
        /// <param name="y">y coordinate of the location</param>
        /// <returns>The index of the prey in the entity list</returns>
        public int PreyOnLocation(int x, int y)
        {
            if (IsWithinGrid(x, y) && !IsEmptyCell(x, y) && EntityTypeOnLocation(x, y) == EntityType.Prey)
            {
                return grid[x, y];
            }
            return -1;
        }

        /// <summary>
        /// Return whether the given location is viable and available for an Entity
        /// </summary>
        /// <param name="x">The x coordinate of the queried location</param>
        /// <param name="y">The x coordinate of the queried location</param>
        /// <returns>True if the location is available for the Entity, otherwise false</returns>
        public bool IsAvailableLocation(int x, int y)
        {
            return IsWithinGrid(x, y) && IsEmptyCell(x, y);
        }

        /// <summary>
        /// Return whether a location is on the grid
        /// </summary>
        /// <param name="x">The x of the queried location</param>
        /// <param name="y">The y of the queried location</param>
        public static bool IsWithinGrid(int x, int y)
        {
            return x >= 0 && x < Config.worldSize && y >= 0 && y < Config.worldSize;
        }

        /// <summary>
        /// Get a random location to birth a new entity on, given location of parent
        /// </summary>
        /// <param name="x">x coordinate of the parent</param>
        /// <param name="y">y coordinate of the parent</param>
        /// <returns>The location to be birthed on</returns>
        public (int, int)? GetBirthLocation(int x, int y)
        {
            // make a list of available locations
            List<(int, int)> availableLocations = new List<(int, int)>(4);
            // up
            AddBirthLocationIfAvailable(availableLocations, x, y + 1);
            // right
            AddBirthLocationIfAvailable(availableLocations, x + 1, y);
            // down
            AddBirthLocationIfAvailable(availableLocations, x, y - 1);
            // left
            AddBirthLocationIfAvailable(availableLocations, x - 1, y);

            // choose one randomly
            if (availableLocations.Count > 0)
                return availableLocations[rnd.Next(availableLocations.Count)];

            return null;
        }

        private void AddBirthLocationIfAvailable(List<(int, int)> available, int x, int y)
        {
            if (IsAvailableLocation(x, y))
                available.Add((x, y));
        }

        /// <summary>
        /// Get a random empty location from the grid
        /// </summary>
        /// <returns></returns>
        public (int, int) GetRandomEmptyLocation()
        {
            int x, y;
            do
            {
                x = rnd.Next(Config.worldSize);
                y = rnd.Next(Config.worldSize);
            } while (!IsEmptyCell(x, y)); // do not use IsAvailableLocation(),
                                          // because this checks whether the
                                          // location is in the grid, which is
                                          // already guaranteed

            return (x, y);
        }

        /// <summary>
        /// Get a random available location within the range of the given location
        /// </summary>
        /// <returns>The coordinates of the random location</returns>
        public (int, int) GetLocationNext(int x, int y)
        {
            //get all locations around the current location that are not off the board
            List<(int, int)> availableLocations = GetAvailableLocations(x, y);

            //pick one of the locations randomly
            if(availableLocations.Count > 0)
                return availableLocations[rnd.Next(availableLocations.Count)];  
            

            //no squares around the current location are available
            return (x, y);

        }

        public (int, int) GetLocationFast(int x, int y)
        {
            if (IsAvailableLocation(x + 1, y)) return (x + 1, y);
            if (IsAvailableLocation(x - 1, y)) return (x - 1, y);
            if (IsAvailableLocation(x, y + 1)) return (x, y + 1);
            if (IsAvailableLocation(x, y - 1)) return (x, y - 1);

            return (x, y);
        }

        /// <summary>
        /// Get all locations around the current location that are not off the board.
        /// Could be changed into only up, down, left, right
        /// </summary>
        private List<(int, int)> GetAvailableLocations(int x, int y)
        {
            int newx, newy;
            //how far the entity can walk
            int dist = Config.walkDistance;

            List<(int, int)> availableLocations = new List<(int, int)>();


            for (int i = -dist; i <= dist; i++)
            {
                for (int j = -dist; j <= dist; j++)
                {
                    newy = y + i;
                    newx = x + j;

                    if (IsAvailableLocation(newx, newy))
                    {
                        availableLocations.Add((newx, newy));
                    }
                }
            }
            return availableLocations;
        }

        /// <summary>
        /// Prints the amount of entities, predators and prey in the world
        /// </summary>
        public void PrintStats()
        {
            Console.Clear();

            int tempAmountOfEntities = 0;
            int[] countArray = new int[Config.worldSize / Config.BlockSize + 1]; //+1 for the last block

            for (int y = 0; y < Config.worldSize; y++)
            {
                for (int x = 0; x < Config.worldSize; x++)
                {

                    //checks if there is an entity on the grid
                    if(grid[x, y] != -1)
                        tempAmountOfEntities++; 



                    if (x % Config.BlockSize == 0)
                    {
                       //collect entities in blocks
                        int pointer = x / Config.BlockSize;
                        countArray[pointer] += tempAmountOfEntities;
                        tempAmountOfEntities = 0;

                        if (y % Config.BlockSize == 0)
                        {
                            //print the amount of entities in the block
                            Console.Write("|" + countArray[pointer] + "|");
                            countArray[pointer] = 0;

                            //make sure the gris is "square"
                            if(pointer == Config.worldSize / Config.BlockSize)
                                Console.WriteLine();    


                        }
                    }

                }

           }

            Console.WriteLine("Predators: " + entities.AmountOfPredators);
            Console.WriteLine("Prey: " + entities.AmountOfPrey);
            Console.WriteLine("Entities: " + AmountOfEntities);
        }

        public void AddStatsToPM(PlotManager pm, int round)
        {
            pm.AddData(round, AmountOfEntities, AmountOfPredators, AmountOfPrey);
        }


        /// <summary>
        /// The total amount of entities currently active in the world.
        /// Birthed entities do not count yet.
        /// </summary>
        public int AmountOfEntities => entities.Count;

        public int AmountOfPredators => entities.AmountOfPredators;

        public int AmountOfPrey => entities.AmountOfPrey;

        /// <summary>
        /// Calculates the amount of entities in the grid. This must match
        /// with the amount of AmountOfEntities. Use this method for debugging.
        /// </summary>
        /// <returns>The total amount of entities in the grid</returns>
        public int CalculateEntitiesInGrid()
        {
            int sum = 0;

            for (int x = 0; x < Config.worldSize; x++)
            {
                for (int y = 0; y < Config.worldSize; y++)
                {
                    if (grid[x, y] != -1) sum++;
                }
            }

            return sum;
        }

        
    }
}
