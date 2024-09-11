using System;

namespace Project1
{
    public abstract class Entity
    {
        protected static World world;
        protected static Random rnd;

        protected double birthRate;
        protected double deathRate;
        protected double walkRate;

        public int x { get; protected set; }
        public int y { get; protected set; }

        protected Entity(World _world)
        {
            world = _world;
            rnd = world.rnd;

            this.birthRate = Config.birthRate;
            this.deathRate = Config.deathRate;
            this.walkRate = Config.walkRate;
        }


        public void Move(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public abstract void AttemptActions();

        /// <summary>
        /// Attempt to move to a different location
        /// </summary>
        protected void AttemptWalk()
        {
            if (Attempt.Success(rnd, walkRate))
            {
                int newX = rnd.Next(Config.worldSize);
                int newY = rnd.Next(Config.worldSize);

                Move(newX, newY);
            }
        }

        protected void AttemptBirth() {}

        protected void AttemptDeath() {}
    }

    public class Predator : Entity
    {
        public Predator(World world) : base(world){ }

        public override void AttemptActions()
        {
            throw new NotImplementedException();
        }
    }

    public class Prey : Entity
    {
        public Prey(World world) : base(world){ }
        public override void AttemptActions()
        {
            AttemptWalk();
            AttemptBirth();
            AttemptDeath();
        }
    }
}

