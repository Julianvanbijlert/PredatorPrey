using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace Project1
{
    public class EntityManager
    {
        // the world of the simulation
        public World world { get; set; }
        // the same random as the worlds random
        protected Random rnd;
        // keep track of the total amount of predators born since starting simulation
        public double AmountOfPredatorsBorn { get; private set; }

        public EntityManager(World world, Random rnd)
        {
            this.world = world;
            this.rnd = world.rnd;
        }

        /// <summary>
        /// Randomly place Entities on the grid for the starting configuration
        /// </summary>
        public void InitializeEntities()
        {
            //Make the Entities
            int amountPrey = (int)(Config.worldSize * Config.worldSize * Config.preyDensity);
            int amountPredator = (int)(Config.worldSize * Config.worldSize * Config.predatorDensity);

            for (int i = 0; i < amountPrey; i++)
            {
                (int x, int y) = world.GetRandomEmptyLocation();
                AddEntity((EntityType.Prey, x, y));
            }

            for (int i = 0; i < amountPredator; i++)
            {
                (int x, int y) = world.GetRandomEmptyLocation();
                AddEntity((EntityType.Predator, x, y));
            }
        }

        /// <summary>
        /// Add the given entity to the world and its entity list
        /// </summary>
        /// <param name="entity">The entity to add</param>
        public void AddEntity((EntityType, int x, int y) entity)
        {
            if (entity.Item1 == EntityType.Predator) AmountOfPredatorsBorn++;
            int newIndex = world.entities.BirthEntity(entity);
            world.grid[entity.x, entity.y] = newIndex;
        }

        /// <summary>
        /// Add a new prey on a location near the parent with the same type
        /// as the parent
        /// </summary>
        /// <param name="parent">The parent which is birthing</param>
        public void BirthEntity((EntityType type, int x, int y) parent)
        {
            (int, int)? location = world.GetBirthLocation(parent.x, parent.y);
            if (!location.HasValue) return;
            (int x, int y) = location.Value;

            if (!world.IsAvailableLocation(x, y))
                throw new Exception("Birth location is not available.");

            AddEntity((parent.type, x, y));
        }

        /// <summary>
        /// Change the location of an entity
        /// </summary>
        /// <param name="entity">The entity to move</param>
        /// <param name="index">The index of the entity</param>
        /// <param name="x">The new x coordinate</param>
        /// <param name="y">The new y coordinate</param>
        public void ChangeLocation((EntityType type, int oldX, int oldY) entity, int index, int x, int y)
        {
            if (!world.IsAvailableLocation(x, y))
                throw new ArgumentException("Location to move to is not available.");

            world.entities.ChangeLocationData(index, x, y);
            world.OnMoveEntity(entity.oldX, entity.oldY, x, y);
        }

        /// <summary>
        /// Remove entity from the world (the grid and the Entity list)
        /// </summary>
        /// <param name="index">The index of the entity to remove in the list</param>
        public void RemoveEntity(int index)
        {
            ((EntityType, int, int) deadEntity, (EntityType, int, int) shiftEntity, int shiftIndex) = 
                world.entities.KillEntity(index);
            world.OnRemoveEntity(deadEntity, shiftEntity, shiftIndex);
        }
    }
}

