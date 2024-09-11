using System;
using System.Collections.Generic;
using System.Linq;

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

        protected Entity(World _world, int x, int y)
        {
            world = _world;
            rnd = world.rnd;

            this.x = x;
            this.y = y;

            this.birthRate = Config.birthRate;
            this.deathRate = Config.deathRate;
            this.walkRate = Config.walkRate;
        }


        public void ChangeLocation(int x, int y)
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

                world.MoveEntity(this, newX, newY);
            }
        }

        protected void AttemptBirth()
        {
            if (Attempt.Success(rnd, birthRate)) GiveBirth();
        }

        protected void AttemptDeath()
        {
            if (Attempt.Success(rnd, deathRate)) Die();
        }

        protected abstract void GiveBirth();

        protected abstract void Die();
    }

    public class Predator : Entity
    {
        public Predator(World world, int x, int y) : base(world, x, y){ }

        public override void AttemptActions()
        {
            AttemptPredation();
            AttemptWalk();
            AttemptBirth();
            AttemptDeath();
        }

        private void AttemptPredation()
        {
            foreach (Prey p in PossiblePrey())
            {
                if (Attempt.Success(rnd, Config.predationRate))
                {
                    world.RemovePrey(p);
                }
            }
        }

        private List<Entity> PossiblePrey()
        {
            List<Entity> location = world.PreyOnLocation(this.x, this.y);
            List<Entity> up = world.PreyOnLocation(this.x, this.y + 1);
            List<Entity> right = world.PreyOnLocation(this.x + 1, this.y);
            List<Entity> down = world.PreyOnLocation(this.x, this.y - 1);
            List<Entity> left = world.PreyOnLocation(this.x - 1, this.y);

            return location.Concat(up).Concat(right).Concat(down).Concat(left).ToList();
        } 

        protected override void GiveBirth()
        {
            world.AddBirthPredator(new Predator(world, this.x, this.y));
        }

        protected override void Die()
        {
            world.RemovePredator(this);
        }
    }

    public class Prey : Entity
    {
        public Prey(World world, int x, int y) : base(world, x, y){ }
        public override void AttemptActions()
        {
            AttemptWalk();
            AttemptBirth();
            AttemptDeath();
        }

        protected override void GiveBirth()
        {
            world.AddBirthPrey(new Prey(world, this.x, this.y));
        }

        protected override void Die()
        {
            world.RemovePrey(this);
        }
    }
}

