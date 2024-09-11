using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    public class World
    {
        public Random rnd { get; private set; }

        List<Entity>[,] grid;
        List<Predator> predators = new List<Predator>();
        List<Prey> prey = new List<Prey>();

        public World()
        {
            rnd = new Random();

            //Make the Entities
            int amountPrey = (int)(Config.worldSize * Config.preyDensity);
            int amountPreditor = (int)(Config.worldSize * Config.preditorDensity);

            for (int i = 0; i < amountPrey; i++)
            {
                AddPrey();
            }

            for (int i = 0; i < amountPreditor; i++)
            {
                AddPredator();
            }


            //Make the world grid
            int size = Config.worldSize;
            grid = new List<Entity>[size, size];



            // This is under the assumption that the size does not change and that it will always be a square
            for (var index0 = 0; index0 < size; index0++)
                for (var index1 = 0; index1 < size; index1++)
                {
                    grid[index0, index1] = new List<Entity>();
                }

        }


        /// <summary>
        /// Adds one prey to the world
        /// </summary>
        public void AddPrey()
        {
            prey.Add(new Prey());
        }


        /// <summary>
        /// Adds one preditor to the world
        /// </summary>
        public void AddPredator()
        {
            predators.Add(new Predator());
        }

        public Predator GetRandomPredator()
        {
            return predators[rnd.Next(predators.Count)];
        }

        public Prey GetRandomPrey()
        {
            return prey[rnd.Next(prey.Count)];
        }

        void MoveEntity(Entity entity, int x, int y)
        {
            grid[entity.x, entity.y].Remove(entity);
            grid[x, y].Add(entity);

            entity.Move(x, y);

        }

        public int AmountOfPredators => predators.Count;

        public int AmountOfPrey => prey.Count;
    }
}
