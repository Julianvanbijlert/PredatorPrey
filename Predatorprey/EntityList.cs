using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    public enum EntityType { Predator, Prey };

    /// <summary>
    /// A simple class for keeping track of the live entities.
    /// Supports adding, removing and random indexing of the entities.
    /// </summary>
    public class EntityList 
    {
        private (EntityType, int, int)[] _entities;
        public int k { get; private set; } //for knowing the amount of entities, this is the first one that is dead

        private int amountOfPredators;
        private int amountOfPrey;

        private Random rnd;

        public EntityList(Random rnd)
        {
            _entities = new (EntityType, int, int)[Config.worldSize * Config.worldSize];
            this.rnd = rnd;
            k = 0;
            amountOfPredators = 0;
            amountOfPrey = 0;
        }

        /// <summary>
        /// Remove an entity from the list and shift another to avoid empty spots
        /// </summary>
        /// <param name="index">The index of the entity to remove</param>
        /// <returns>The dead entity, the entity that got shifted in the list and the index
        /// it got shifted to</returns>
        public ((EntityType, int, int) deadEntity, (EntityType, int, int) shiftedEntity, 
            int newIndexShiftedEntity) KillEntity(int index)
        {
            // retrieve dead and shifted entities
            (EntityType _, int x, int y) deadEntity = GetEntity(index);
            (EntityType _, int x, int y) shiftEntity = GetEntity(k - 1);

            // shift last living entity to index 
            ShiftEntity(k - 1, index);

            // update entity count
            DecreaseEntityCount(deadEntity);

            // update k to signal there is one living entity less
            k--;

            return (deadEntity, shiftEntity, index);
        }

        private void ShiftEntity(int oldIndex, int newIndex)
        {
            _entities[newIndex] = _entities[oldIndex];
        }

        public int BirthEntity((EntityType, int, int) entity)
        {
            if (k >= _entities.Length)
                throw new IndexOutOfRangeException("The entity list is full");


            _entities[k] = entity;
            
            IncreaseEntityCount(entity);

            k++;

            return k - 1; //returns the index of the entity AND THEN increases k (hopefully)
        }



        /// <summary>
        /// Returns a random (living) entity with uniform chance
        /// </summary>
        /// <returns>The chosen entity and its index in the list</returns>
        public ((EntityType, int, int), int) GetRandomEntity()
        {
            int index = rnd.Next(0, k);
            return (_entities[index], index);
        }

        public (EntityType, int, int) GetEntity(int index)
        {
            return _entities[index]; 
        }

        /// <summary>
        /// Get the type of the entity identified by its index
        /// </summary>
        /// <param name="index">The index of the entity in the list</param>
        /// <returns>The type of the entity</returns>
        public EntityType GetEntityType(int index)
        {
            return _entities[index].Item1;
        }

        /// <summary>
        /// Change some data from an entity, specified by index
        /// </summary>
        /// <param name="index">Index of the entity</param>
        /// <param name="newX">New x coordinate</param>
        /// <param name="newY">New y coordinate</param>
        public void ChangeData(int index, int newX, int newY)
        {
            (EntityType type, int _, int _) = _entities[index];
            _entities[index] = (type, newX, newY);
        }

        public void IncreaseEntityCount((EntityType, int, int) entity)
        {
            //increase the amount of predators or prey
            if (entity.Item1 == EntityType.Predator)
                amountOfPredators++;
            else
                amountOfPrey++;
        }

        public void DecreaseEntityCount((EntityType, int, int) entity)
        {
            //decrease the amount of predators or prey
            if (entity.Item1 == EntityType.Predator)
                amountOfPredators--;
            else
                amountOfPrey--;
        }


        public int Count => k;

        public int AmountOfPredators => amountOfPredators;

        public int AmountOfPrey => amountOfPrey;
    }
}
