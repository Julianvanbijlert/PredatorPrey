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

        protected TracksMap tracksMap { get; private set; }

        public EntitySimulator(EntityManager entityManager, Random rnd)
        {
            this.entityManager = entityManager;
            this.world = entityManager.world;
            this.rnd = rnd;
            this.tracksMap = world.tracksMap;
        }

        /// <summary>
        /// Set the currently used entity in the variables
        /// </summary>
        /// <param name="entityIndex">The index of the entity in the entity list</param>
        public void SetCurrentEntity(int entityIndex)
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

        /// <summary>
        /// Do a random walk with walkDistance (from config) steps
        /// </summary>
        private void Walk()
        {
            for (int i = 0; i < Config.walkDistance; i++)
            {
                (int newX, int newY) = GetNextSpot();
                if(!(newX == _currentEntity.x && newY == _currentEntity.y)) 
                    WalkOneStep(newX, newY);
            }
        }

        /// <summary>
        /// Let entity pick a next, neighboring spot to walk to
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
        /// Method is handy for deploying tracks if needed
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

        /// <summary>
        /// Attempt to predate on the prey on this location if there is a prey there
        /// </summary>
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
            if(!Config.WithTracks) 
                return base.GetNextSpot();

            (int newX, int newY) = world.tracksMap.GetSurroundingHigherTrack(_currentEntity.x, _currentEntity.y);
            if (!(newX == _currentEntity.x && newY == _currentEntity.y))
                return (newX, newY);

            Direction direction = world.tracksMap.GetDirection(_currentEntity.x, _currentEntity.y);

            switch (direction)
            {
                case Direction.Null: // no direction, go to a random other available location
                    return base.GetNextSpot();
                case Direction.Up:
                    return (_currentEntity.x, _currentEntity.y + 1);
                case Direction.Right:
                    return (_currentEntity.x + 1, _currentEntity.y);
                case Direction.Down:
                    return (_currentEntity.x, _currentEntity.y - 1);
                case Direction.Left:
                    return (_currentEntity.x - 1, _currentEntity.y);
                default:
                    throw new Exception("Something went wrong with interpreting the direction");
            }
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

        protected override void WalkOneStep(int newX, int newY)
        {
            if(Config.WithTracks) world.tracksMap.AddTrack(_currentEntity.x, _currentEntity.y);
            base.WalkOneStep(newX, newY);
        }

        private void AttemptBirth()
        {
            if(Attempt.Success(rnd, Config.birthRate)) 
                entityManager.BirthEntity(_currentEntity);
        }
    }
}
