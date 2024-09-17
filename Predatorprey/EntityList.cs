using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    public abstract class EntityList
    {
        public enum EntityType { Predator, Prey };

        private (EntityType, int, int)[] _entities;
        private int k; //for knowing the amount of entities, this is the first one that is dead

        public EntityList()
        {
            _entities = new (EntityType, int, int)[Config.worldSize * Config.worldSize];
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
    }

    public class PredatorList : EntityList
    {
        public PredatorList() : base()
        {
        }


       
    }

    public class PreyList : EntityList
    {
        public PreyList()
        {

        }
    }
}
