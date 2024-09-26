﻿using Project1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    /// <summary>
    /// Class for the actions the entities can simulate
    /// </summary>
    public abstract class EntitySimulator
    {
        /// <summary>
        /// The entity which is currently attempting actions
        /// </summary>
        protected (EntityType type, int x, int y) _currentEntity => world.entities.GetEntity(_currentEntityIndex);

        /// <summary>
        /// The index of the current entity
        /// </summary>
        protected int _currentEntityIndex { get; private set; }

        /// <summary>
        /// Object for changing entities in the world
        /// </summary>
        protected EntityManager entityManager;

        protected World world { get; private set; }

        protected Random rnd { get; private set; }

        protected SmellMap smellMap { get; private set; }

        public EntitySimulator(EntityManager entityManager, Random rnd)
        {
            this.entityManager = entityManager;
            this.world = entityManager.world;
            this.rnd = rnd;
            this.smellMap = world.smellMap;
        }

        /// <summary>
        /// Set the currently used entity in the variables
        /// </summary>
        /// <param name="entity">The entity to use</param>
        /// <param name="entityIndex">The index of the entity in the entity list</param>
        public void SetCurrentEntity((EntityType, int, int) entity, int entityIndex)
        {
            _currentEntityIndex = entityIndex;
        }

        public abstract void AttemptActions();


        protected void AttemptWalk()
        {
            if (Attempt.Success(rnd, Config.walkRate))
            {
                Walk();
            }
        }

        private void Walk()
        {
            for (int i = 0; i < Config.walkDistance; i++)
            {
                (int newX, int newY) = GetNextSpot();
                if(newX != _currentEntity.x && newY != _currentEntity.y) 
                    WalkOneStep(newX, newY);
            }
        }

        /// <summary>
        /// Let predator or prey pick a next, neighboring spot to walk to
        /// </summary>
        protected virtual (int, int) GetNextSpot()
        {
            List<(int, int)> possibleLocations = new List<(int, int)>();

            // up
            AddIfAvailableLocation(possibleLocations, _currentEntity.x, _currentEntity.y + 1);
            // right
            AddIfAvailableLocation(possibleLocations, _currentEntity.x + 1, _currentEntity.y);
            // down
            AddIfAvailableLocation(possibleLocations, _currentEntity.x, _currentEntity.y - 1);
            // left
            AddIfAvailableLocation(possibleLocations, _currentEntity.x - 1, _currentEntity.y);

            // if no possible locations, return current location
            if (possibleLocations.Count == 0)
                return (_currentEntity.x, _currentEntity.y);

            // else pick a random option
            return possibleLocations[rnd.Next(possibleLocations.Count)];
        }

        /// <summary>
        /// Add the location to the list if it is available
        /// </summary>
        private void AddIfAvailableLocation(List<(int, int)> l, int x, int y)
        {
            if(world.IsAvailableLocation(x, y)) 
                l.Add((x, y));
        }

        /// <summary>
        /// Let entity walk one step to new location.
        /// Method is handy for deploying smell if needed
        /// </summary>
        protected virtual void WalkOneStep(int newX, int newY)
        {
            entityManager.ChangeLocation(_currentEntity, _currentEntityIndex, newX, newY);
        }
    }














    public class PredatorSimulator : EntitySimulator
    {
        public PredatorSimulator(EntityManager entityManager, Random rnd) : base(entityManager, rnd)
        {
            
        }

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
            // up
            AttemptPredationOnOneLocation(_currentEntity.x, _currentEntity.y + 1);
            // right
            AttemptPredationOnOneLocation(_currentEntity.x + 1, _currentEntity.y);
            // down
            AttemptPredationOnOneLocation(_currentEntity.x, _currentEntity.y - 1);
            // left
            AttemptPredationOnOneLocation(_currentEntity.x - 1, _currentEntity.y);
        }

        private void AttemptPredationOnOneLocation(int x, int y)
        {
            // get the index of the prey on the location. If no prey exists there, -1 is returned.
            int index = world.PreyOnLocation(x, y);

            // check whether there is a prey on index and see if predation is successful
            if (index != -1 && Attempt.Success(rnd, Config.predationRate))
            {
                Predation(index);
            }
        }

        protected override (int, int) GetNextSpot()
        {
            if(!Config.WithSmell) 
                return base.GetNextSpot();

            // get neighbor locations with highest smell values
            List<(int x, int y)> highestSmells = smellMap.GetHighestSurroundingSmells(_currentEntity.x, _currentEntity.y);

            List<(int, int)> possibleLocations = highestSmells.FindAll(location => world.IsEmptyCell(location.x, location.y));

            // if you have no options, return current location
            if (possibleLocations.Count == 0)
                return (_currentEntity.x , _currentEntity.y);

            // else, choose a random option
            return possibleLocations[rnd.Next(possibleLocations.Count)];
        }

        /// <summary>
        /// Do predation on a prey 
        /// </summary>
        /// <param name="p">The prey to kill</param>
        private void Predation(int preyIndex)
        {
            // check whether the assumption holds that the entity is a prey
            if (world.entities.GetEntityType(preyIndex) != EntityType.Prey)
                throw new ArgumentException("A non-prey entity is being preyed on.");

            // kill Prey and birth new Predator
            entityManager.RemoveEntity(preyIndex);
            entityManager.BirthEntity(_currentEntity);
        }

        /// <summary>
        /// Attempt to let this entity die
        /// </summary>
        protected void AttemptDeath()
        {
            if (Attempt.Success(rnd, Config.deathRate)) 
                entityManager.RemoveEntity(_currentEntityIndex);
        }

    }















    public class PreySimulator : EntitySimulator
    {
        public PreySimulator(EntityManager entityManager, Random rnd) : base(entityManager, rnd)
        {
        }

        public override void AttemptActions()
        {
            AttemptWalk();
            AttemptBirth();
        }

        protected override (int, int) GetNextSpot()
        {
            List<(int, int)> possibleLocations = new List<(int, int)>();
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int locationX = _currentEntity.x + dx;
                    int locationY = _currentEntity.y + dy;

                    if(world.IsAvailableLocation(locationX, locationY))
                        possibleLocations.Add((locationX, locationY));
                }
            }

            // if no possible locations, return current location
            if(possibleLocations.Count == 0) 
                return (_currentEntity.x, _currentEntity.y);

            // else pick up random option
            return possibleLocations[rnd.Next(possibleLocations.Count)];
        }

        protected override void WalkOneStep(int newX, int newY)
        {
            if(Config.WithSmell) world.smellMap.AddSmell(_currentEntity.x, _currentEntity.y);
            base.WalkOneStep(newX, newY);
        }

        private void AttemptBirth()
        {
            if(Attempt.Success(rnd, Config.birthRate)) entityManager.BirthEntity(_currentEntity);
        }
    }
}
