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
        private int k; //for knowing the amount of entities, this is the first one that is dead

        private Random rnd;

        public EntityList(Random rnd)
        {
            _entities = new (EntityType, int, int)[Config.worldSize * Config.worldSize];
            this.rnd = rnd;
        }

        public void KillEntity(int index)
        {
            //trades places with the last entity and then kills it by decreasing k
            ChangePlaces(index, --k);
        }

        private void ChangePlaces(int index1, int index2)
        {
            (EntityType, int, int) temp = _entities[index1];
            _entities[index1] = _entities[index2];
            _entities[index2] = temp;
        }

        public int BirthEntity((EntityType, int, int) entity)
        {
            if (k >= _entities.Length)
                throw new IndexOutOfRangeException("The entity list is full");


            _entities[k] = entity;
            return k++; //returns the index of the entity AND THEN increases k (hopefully)
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

        public int Count => k;
    }
}
