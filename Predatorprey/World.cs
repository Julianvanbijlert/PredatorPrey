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
        public Random rnd { get; private set; }

        private List<Entity>[,] grid;
        private HashSet<Predator> predators = new HashSet<Predator>();
        private HashSet<Prey> prey = new HashSet<Prey>();
        private HashSet<Entity> entities = new HashSet<Entity>();

        public World()
        {
            rnd = new Random();

            InitializeGrid();

            InitializeEntities();
        }

        private void InitializeGrid()
        {
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

        private void InitializeEntities()
        {
            int size = Config.worldSize;
            //Make the Entities
            int amountPrey = (int)(Config.worldSize * Config.preyDensity);
            int amountPredator = (int)(Config.worldSize * Config.preditorDensity);

            for (int i = 0; i < amountPrey; i++)
            {
                AddPrey(rnd.Next(size), rnd.Next(size));
            }

            for (int i = 0; i < amountPredator; i++)
            {
                AddPredator(rnd.Next(size), rnd.Next(size));
            }
        }


        /// <summary>
        /// Adds one prey to the world
        /// </summary>
        public void AddPrey(int x, int y)
        {
            Prey newPrey = new Prey(this, x, y);
            entities.Add(newPrey);
            prey.Add(newPrey);
        }


        /// <summary>
        /// Adds one preditor to the world
        /// </summary>
        public void AddPredator(int x, int y)
        {
            Predator newPredator = new Predator(this, x, y);
            entities.Add(newPredator);
            predators.Add(newPredator);
        }

        public Entity GetRandomEntity()
        {
            return entities.ElementAt(rnd.Next(entities.Count));
        }

        public void MoveEntity(Entity entity, int x, int y)
        {
            grid[entity.x, entity.y].Remove(entity);
            grid[x, y].Add(entity);

            entity.ChangeLocation(x, y);
        }

        public void RemoveEntity(Entity e)
        {
            entities.Remove(e);
        }

        public void RemovePredator(Predator p)
        {
            RemoveEntity(p);
            predators.Remove(p);
        }

        public void RemovePrey(Prey p)
        {
            RemoveEntity(p);
            prey.Remove(p);
        }

        /// <summary>
        /// Get all prey that is on the specified location
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public List<Entity> PreyOnLocation(int x, int y)
        {
            if (IsWithinGrid(x, y))
            {
                return grid[x, y].FindAll((entity => entity is Prey));
            }
            return new List<Entity>();
        }

        private bool IsWithinGrid(int x, int y)
        {
            return x >= 0 && x < Config.worldSize && y >= 0 && y < Config.worldSize;
        }

        public int AmountOfPredators => predators.Count;

        public int AmountOfPrey => prey.Count;

        public int AmountOfEntities => entities.Count;
    }
}
