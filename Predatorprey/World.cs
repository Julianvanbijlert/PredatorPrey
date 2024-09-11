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
        /// The 'map' of the world
        /// </summary>
        private List<Entity>[,] grid;

        // Note that entities = predators ++ prey. This is to get the total
        // number of predators and prey fast. It remains to be seen if this
        // really is more efficient, because maintaining more sets is also
        // more work.

        /// <summary>
        /// Contains all active entities in the world
        /// </summary>
        private HashSet<Entity> entities = new HashSet<Entity>();
        /// <summary>
        /// Contains all active predators in the world
        /// </summary>
        private HashSet<Predator> predators = new HashSet<Predator>();
        /// <summary>
        /// Contains all active prey in the world
        /// </summary>
        private HashSet<Prey> prey = new HashSet<Prey>();

        // These lists contain birthed entities. They are added to the world upon round completion to 
        // let each entity at the start of the round be selected once on average, while still having
        // round completions. 
        private HashSet<Predator> birthedPredators = new HashSet<Predator>();
        private HashSet<Prey> birthedPrey = new HashSet<Prey>();

        /// <summary>
        /// Represents the world with a grid and the entities on it.
        /// Is responsible for all operations over the entities, like moving, adding and removing
        /// </summary>
        public World()
        {
            rnd = new Random();

            InitializeGrid();

            InitializeEntities();
        }

        /// <summary>
        /// Initialize the grid of the world by making a 2D array of Entities
        /// </summary>
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

        /// <summary>
        /// Randomly place Entities on the grid for the starting configuration
        /// </summary>
        private void InitializeEntities()
        {
            int size = Config.worldSize;
            //Make the Entities
            int amountPrey = (int)(Config.worldSize * Config.worldSize * Config.preyDensity);
            int amountPredator = (int)(Config.worldSize * Config.worldSize * Config.preditorDensity);

            for (int i = 0; i < amountPrey; i++)
            {
                AddPrey(new Prey(this, rnd.Next(size), rnd.Next(size)));
            }

            for (int i = 0; i < amountPredator; i++)
            {
                AddPredator(new Predator(this, rnd.Next(size), rnd.Next(size)));
            }
        }


        /// <summary>
        /// Adds one prey to the world
        /// </summary>
        public void AddPrey(Prey p)
        {
            entities.Add(p);
            prey.Add(p);
        }


        /// <summary>
        /// Adds one predator to the world
        /// </summary>
        public void AddPredator(Predator p)
        {
            entities.Add(p);
            predators.Add(p);
        }

        /// <summary>
        /// Retrieve one random Entity (Predator or Prey) from the world
        /// </summary>
        /// <returns>The random Entity</returns>
        public Entity GetRandomEntity()
        {
            return entities.ElementAt(rnd.Next(entities.Count));
        }

        /// <summary>
        /// Move an Entity on the grid and change its coordinates known to itself
        /// </summary>
        /// <param name="entity">The entity to move</param>
        /// <param name="x">The new x coordinate</param>
        /// <param name="y">The new y coordinate</param>
        public void MoveEntity(Entity entity, int x, int y)
        {
            grid[entity.x, entity.y].Remove(entity);
            grid[x, y].Add(entity);

            entity.ChangeLocation(x, y);
        }

        /// <summary>
        /// Remove the specified entity from the world
        /// </summary>
        public void RemoveEntity(Entity e)
        {
            entities.Remove(e);
        }

        /// <summary>
        /// Remove a Predator from the world.
        /// </summary>
        /// <param name="p">The predator to remove</param>
        public void RemovePredator(Predator p)
        {
            RemoveEntity(p);
            predators.Remove(p);
        }

        /// <summary>
        /// Remove a prey from the world.
        /// </summary>
        /// <param name="p">The prey to remove</param>
        public void RemovePrey(Prey p)
        {
            RemoveEntity(p);
            prey.Remove(p);
        }

        /// <summary>
        /// Get all prey that is on the specified location
        /// </summary>
        /// <param name="x">x coordinate of the location</param>
        /// <param name="y">y coordinate of the location</param>
        /// <returns>The list of all prey on the location as Entities, because
        /// casting to Prey is costly and not necessary</returns>
        public List<Entity> PreyOnLocation(int x, int y)
        {
            if (IsWithinGrid(x, y))
            {
                return grid[x, y].FindAll((entity => entity is Prey));
            }
            return new List<Entity>();
        }

        /// <summary>
        /// Return whether a location is on the grid
        /// </summary>
        /// <param name="x">The x of the queried location</param>
        /// <param name="y">The y of the queried location</param>
        private bool IsWithinGrid(int x, int y)
        {
            return x >= 0 && x < Config.worldSize && y >= 0 && y < Config.worldSize;
        }

        /// <summary>
        /// Add a predator to the list of birthed predators
        /// </summary>
        /// <param name="p">The predator to add to the list of
        /// birthed predators</param>
        public void AddBirthPredator(Predator p)
        {
            birthedPredators.Add(p);
        }

        /// <summary>
        /// Add a prey to the list of birthed prey
        /// </summary>
        /// <param name="p">The prey to add to the list of
        /// birthed prey</param>
        public void AddBirthPrey(Prey p)
        {
            birthedPrey.Add(p);
        }

        /// <summary>
        /// Set all birthed Entities in the world and empty the birth lists
        /// </summary>
        public void ReleaseBirthedEntities()
        {
            foreach (Predator p in birthedPredators)
            {
                AddPredator(p);
            }

            foreach (Prey p in birthedPrey)
            {
                AddPrey(p);
            }

            birthedPredators.Clear();
            birthedPrey.Clear();
        }

        /// <summary>
        /// The amount of predators currently active in the world.
        /// Birthed predators do not count yet.
        /// </summary>
        public int AmountOfPredators => predators.Count;


        /// <summary>
        /// The amount of prey currently active in the world.
        /// Birthed prey do not count yet.
        /// </summary>
        public int AmountOfPrey => prey.Count;


        /// <summary>
        /// The total amount of entities currently active in the world.
        /// Birthed entities do not count yet.
        /// </summary>
        public int AmountOfEntities => entities.Count;
    }
}
