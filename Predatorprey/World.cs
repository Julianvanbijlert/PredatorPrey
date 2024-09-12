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
        private Entity[,] grid;

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
        private List<Entity> birthingEntities = new List<Entity>();

        /// <summary>
        /// Represents the world with a grid and the entities on it.
        /// Is responsible for all operations over the entities, like moving, adding and removing
        /// </summary>
        public World()
        {
            rnd = new Random(201);

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
            grid = new Entity[size, size];
        }

        /// <summary>
        /// Randomly place Entities on the grid for the starting configuration
        /// </summary>
        private void InitializeEntities()
        {
            //Make the Entities
            int amountPrey = (int)(Config.worldSize * Config.worldSize * Config.preyDensity);
            int amountPredator = (int)(Config.worldSize * Config.worldSize * Config.preditorDensity);

            for (int i = 0; i < amountPrey; i++)
            {
                (int x, int y) = GetRandomEmptyLocation();
                AddPrey(new Prey(this, x, y));
            }

            for (int i = 0; i < amountPredator; i++)
            {
                (int x, int y) = GetRandomEmptyLocation();
                AddPredator(new Predator(this, x, y));
            }
        }


        /// <summary>
        /// Adds one prey to the world
        /// </summary>
        public void AddPrey(Prey p)
        {
            if (grid[p.x, p.y] != null && grid[p.x, p.y] != p) throw new Exception("Entity count will exceed capacity");

            entities.Add(p);
            prey.Add(p);
            grid[p.x, p.y] = p;
        }


        /// <summary>
        /// Adds one predator to the world
        /// </summary>
        public void AddPredator(Predator p)
        {
            if (grid[p.x, p.y] != null && grid[p.x, p.y] != p) throw new Exception("Entity count will exceed capacity");

            entities.Add(p);
            predators.Add(p);
            grid[p.x, p.y] = p;
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
            if (grid[x, y] != null) throw new Exception("Location capacity would be exceeded");

            grid[entity.x, entity.y] = null;
            grid[x, y] = entity;

            entity.ChangeLocation(x, y);
        }

        /// <summary>
        /// Remove the specified entity from the world
        /// </summary>
        public void RemoveEntity(Entity e)
        {
            grid[e.x, e.y] = null;
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
        /// Get the prey that is on the specified location, if any
        /// </summary>
        /// <param name="x">x coordinate of the location</param>
        /// <param name="y">y coordinate of the location</param>
        /// <returns>The list of all prey on the location as Entities, because
        /// casting to Prey is costly and not necessary</returns>
        public Prey PreyOnLocation(int x, int y)
        {
            if (IsWithinGrid(x, y))
            {
                // will return null if grid[x, y] is empty or not Prey
                return grid[x, y] as Prey;
            }
            return null;
        }

        /// <summary>
        /// Return whether the given location is available for an Entity
        /// </summary>
        /// <param name="x">The x coordinate of the queried location</param>
        /// <param name="y">The x coordinate of the queried location</param>
        /// <returns>True if the location is available for the Entity, otherwise false</returns>
        public bool IsAvailableLocation(int x, int y)
        {
            return IsWithinGrid(x, y) && grid[x, y] == null;
        }

        /// <summary>
        /// Return whether a location is on the grid
        /// </summary>
        /// <param name="x">The x of the queried location</param>
        /// <param name="y">The y of the queried location</param>
        public bool IsWithinGrid(int x, int y)
        {
            return x >= 0 && x < Config.worldSize && y >= 0 && y < Config.worldSize;
        }

        /// <summary>
        /// Add an entity to the list of birthing entities
        /// </summary>
        /// <param name="p">The entity to add to the list of
        /// birthing entities</param>
        public void AddBirthingEntity(Entity e)
        {
            birthingEntities.Add(e);
        }

        /// <summary>
        /// Set all birthed Entities in the world and empty the birth lists
        /// </summary>
        public void ReleaseBirthingEntities()
        {
            foreach (Entity e in birthingEntities)
            {
                e.GiveBirth();
            }
        }

        public (int, int) GetRandomEmptyLocation()
        {
            int x, y;
            do
            {
                x = rnd.Next(Config.worldSize);
                y = rnd.Next(Config.worldSize);
            } while (grid[x, y] != null);

            return (x, y);
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
                    if (grid[x, y] != null) sum++;
                }
            }

            return sum;
        }


        public Entity[,] GetGrid => grid;

        public HashSet<Entity> GetEntities => entities;
    }
}
