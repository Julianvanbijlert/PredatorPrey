using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Project1
{
    public abstract class Entity
    {
        // the world of the simulation
        public static World world { protected get; set; }
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

        protected Entity(int x, int y)
        {
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
                
                (int newX, int newY) = world.GetLocationNext(x, y);

                world.MoveEntity(this, newX, newY);
            }
        }

        // Get a random location to birth a new entity on
        protected (int, int)? GetBirthLocation()
        {
            // make a list of available locations
            List<(int, int)> availableLocations = new List<(int, int)>(4);
            // up
            AddBirthLocationIfAvailable(availableLocations, this.x, this.y + 1);
            // right
            AddBirthLocationIfAvailable(availableLocations, this.x + 1, this.y);
            // down
            AddBirthLocationIfAvailable(availableLocations, this.x, this.y - 1);
            // left
            AddBirthLocationIfAvailable(availableLocations, this.x - 1, this.y);

            // choose one randomly
            if(availableLocations.Count > 0)
                return availableLocations[rnd.Next(availableLocations.Count)];

            return null;
        }

        private void AddBirthLocationIfAvailable(List<(int, int)> available, int x, int y)
        {
            if (world.IsAvailableLocation(x, y))
                available.Add((x, y));
        }

        /// <summary>
        /// This method handles the process of giving birth
        /// </summary>
        public abstract void GiveBirth();

        /// <summary>
        /// This method lets this entity die
        /// </summary>
        public abstract void Die();
    }

    public class Predator : Entity
    {
        public Predator(int x, int y) : base(x, y){ }
        

        public override void AttemptActions()
        {
            AttemptPredation();
            AttemptWalk();
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
                    Predation(p);
                }
            }
        }

        /// <summary>
        /// Do predation on a prey p
        /// </summary>
        /// <param name="p">The prey to kill</param>
        private void Predation(Prey p)
        {
            // kill Prey and add new Predator
            p.Die();
            world.AddBirthingEntity(this);
        }

        /// <summary>
        /// A list of all prey within reach. This is all prey on the same
        /// location, or the upper, right, lower or left location to this predator.
        /// </summary>
        /// <returns>A list of Entities, only containing Prey which is in reach</returns>
        private IEnumerable<Prey> PossiblePrey()
        {
            Prey location = world.PreyOnLocation(this.x, this.y);
            Prey up = world.PreyOnLocation(this.x, this.y + 1);
            Prey right = world.PreyOnLocation(this.x + 1, this.y);
            Prey down = world.PreyOnLocation(this.x, this.y - 1);
            Prey left = world.PreyOnLocation(this.x - 1, this.y);

            // do not cast to a list of Prey, because this is costly and not necessary
            return new List<Prey>() {location, up, right, down, left}.FindAll((e) => e != null);
        }

       

        /// <summary>
        /// Attempt to let this entity die
        /// </summary>
        protected void AttemptDeath()
        {
            if (Attempt.Success(rnd, Config.deathRate)) Die();
        }

        /// <summary>
        /// Let the predator give birth to a new predator at a random birth location
        /// </summary>
        public override void GiveBirth()
        {
            (int, int)? location = GetBirthLocation();
            if (!location.HasValue) return;
            (int birthX, int birthY) = location.Value;
            world.AddPredator(new Predator(birthX, birthY));
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
        public Prey(int x, int y) : base(x, y){ }

        public override void AttemptActions()
        {
            AttemptWalk();
            AttemptBirth();
        }

        /// <summary>
        /// Let the prey give birth to a new prey at a random birth location
        /// </summary>
        public override void GiveBirth()
        {
            (int, int)? location = GetBirthLocation();
            if (!location.HasValue) return;
            (int birthX, int birthY) = location.Value;
            world.AddPrey(new Prey(birthX, birthY));
        }

        /// <summary>
        /// Attempt to give birth to a new entity
        /// </summary>
        protected void AttemptBirth()
        {
            if (Attempt.Success(rnd, Config.birthRate)) world.AddBirthingEntity(this);
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

