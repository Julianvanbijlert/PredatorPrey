using System;

namespace Project1
{
    public abstract class Entity
    {
        protected double birthRate;
        protected double deathRate;
        protected double walkRate;

        protected Entity()
        {
            this.birthRate = Config.birthRate;
            this.deathRate = Config.deathRate;
            this.walkRate = Config.walkRate;
        }

    }

    public class Predator : Entity
    {
        public Predator() : base()
        {
            
        }
    }

    public class Prey : Entity
    {
        public Prey() : base()
        {
           
        }
    }



}

