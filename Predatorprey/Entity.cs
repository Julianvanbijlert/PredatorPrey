using System;

namespace Project1
{
    public abstract class Entity
    {
        protected double birthRate;
        protected double deathRate;
        protected double walkRate;

        public int x { get; protected set; }
        public int y { get; protected set; }

        protected Entity()
        {
            this.birthRate = Config.birthRate;
            this.deathRate = Config.deathRate;
            this.walkRate = Config.walkRate;
        }


        public void Move(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public abstract Action ChooseAction();
    }

    public class Predator : Entity
    {
        public Predator() : base()
        {
            
        }

        public override Action ChooseAction()
        {
            throw new NotImplementedException();
        }
    }

    public class Prey : Entity
    {
        public Prey() : base()
        {
           
        }

        public override Action ChooseAction()
        {
            throw new NotImplementedException();
        }
    }



}

