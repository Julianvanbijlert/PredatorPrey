using System;
using System.Collections.Generic;
using System.Linq;

namespace Project1
{
    public abstract class Entity
    {
        // the world of the simulation
        protected static World world;
        // the same random as the worlds random
        protected static Random rnd;

        /// <summary>
        /// x coordinate of the entities location
        /// </summary>
        public int x { get; protected set; }

        /// <summary>
        /// y coordinate of the entities location
        /// </summary>
        public int y { get; protected set; }

        protected Entity(World _world, int x, int y)
        {
            world = _world;
            rnd = world.rnd;

            this.x = x;
            this.y = y;
        }


        public void ChangeLocation(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Attempt the actions in order. This is done when this entity is selected.
        /// </summary>
        public abstract void AttemptActions();

        /// <summary>
        /// Attempt to move to a different location.
        /// The new location can be anywhere on the grid with an uniform chance.
        /// </summary>
        protected void AttemptWalk()
        {
            if (Attempt.Success(rnd, Config.walkRate))
            {
                int newX = rnd.Next(Config.worldSize);
                int newY = rnd.Next(Config.worldSize);

                world.MoveEntity(this, newX, newY);
            }
        }

        /// <summary>
        /// Attempt to give birth to a new entity
        /// </summary>
        protected void AttemptBirth()
        {
            if (Attempt.Success(rnd, Config.birthRate)) GiveBirth();
        }

        // Get a random location to birth a new entity on
        protected (int, int) GetBirthLocation()
        {
            // 0: same location
            // 1: up
            // 2: right
            // 3: down
            // 4: left
            int relativeLocation = rnd.Next(5);

            switch (relativeLocation)
            {
                case 0: return (this.x, this.y);
                case 1: return (this.x, this.y + 1);
                case 2: return (this.x + 1, this.y);
                case 3: return (this.x, this.y - 1);
                case 4: return (this.x - 1, this.y);
            }

            throw new Exception("Something went wrong while choosing birth location");
        }

        /// <summary>
        /// Attempt to let this entity die
        /// </summary>
        protected void AttemptDeath()
        {
            if (Attempt.Success(rnd, Config.deathRate)) Die();
        }

        /// <summary>
        /// This method handles the process of giving birth
        /// </summary>
        protected abstract void GiveBirth();

        /// <summary>
        /// This method lets this entity die
        /// </summary>
        public abstract void Die();
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

        /// <summary>
        /// For each prey within reach, attempt predation. Upon success, let prey die.
        /// </summary>
        private void AttemptPredation()
        {
            foreach (Prey p in PossiblePrey())
            {
                if (Attempt.Success(rnd, Config.predationRate))
                {
                    p.Die();
                }
            }
        }

        /// <summary>
        /// A list of all prey within reach. This is all prey on the same
        /// location, or the upper, right, lower or left location to this predator.
        /// </summary>
        /// <returns>A list of Entities, only containing Prey which is in reach</returns>
        private List<Entity> PossiblePrey()
        {
            List<Entity> location = world.PreyOnLocation(this.x, this.y);
            List<Entity> up = world.PreyOnLocation(this.x, this.y + 1);
            List<Entity> right = world.PreyOnLocation(this.x + 1, this.y);
            List<Entity> down = world.PreyOnLocation(this.x, this.y - 1);
            List<Entity> left = world.PreyOnLocation(this.x - 1, this.y);

            // do not cast to a list of Prey, because this is costly and not necessary
            return location.Concat(up).Concat(right).Concat(down).Concat(left).ToList();
        } 

        /// <summary>
        /// Let the predator give birth to a new predator at the same location
        /// </summary>
        protected override void GiveBirth()
        {
            (int newX, int newY) = GetBirthLocation();
            world.AddBirthPredator(new Predator(world, newX, newY));
        }

        /// <summary>
        /// Let the predator remove itself from the world
        /// </summary>
        public override void Die()
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

        /// <summary>
        /// Let the prey give birth to a new prey at the same location
        /// </summary>
        protected override void GiveBirth()
        {
            (int newX, int newY) = GetBirthLocation();
            world.AddBirthPrey(new Prey(world, newX, newY));
        }

        /// <summary>
        /// Let the prey remove itself from the world
        /// </summary>
        public override void Die()
        {
            world.RemovePrey(this);
        }
    }
}

