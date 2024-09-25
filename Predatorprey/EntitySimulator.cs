using Project1;
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
        protected (EntityType type, int x, int y) _currentEntity { get; private set; }

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
            _currentEntity = entity;
            _currentEntityIndex = entityIndex;
        }

        public abstract void AttemptActions();

        /// <summary>
        /// Attempt to move to a different location.
        /// The new location can be anywhere on the grid with an uniform chance.
        /// </summary>
        protected void AttemptWalkSlow()
        {
            if (Attempt.Success(rnd, Config.walkRate))
            {
                (int newX, int newY) = world.GetLocationNext(_currentEntity.x, _currentEntity.y);

                entityManager.ChangeLocation(_currentEntity, _currentEntityIndex, newX, newY);


            }
        }


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
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int locationX = _currentEntity.x + dx;
                    int locationY = _currentEntity.y + dy;

                    if (world.IsAvailableLocation(locationX, locationY))
                        possibleLocations.Add((locationX, locationY));
                }
            }

            // if no possible locations, return current location
            if (possibleLocations.Count == 0)
                return (_currentEntity.x, _currentEntity.y);

            // else pick up random option
            return possibleLocations[rnd.Next(possibleLocations.Count)];
        }

        /// <summary>
        /// Let entity walk one step to new location.
        /// Method is handy for deploying smell if needed
        /// </summary>
        protected virtual void WalkOneStep(int newX, int newY)
        {
            entityManager.ChangeLocation(_currentEntity, _currentEntityIndex, newX, newY);
            _currentEntity = (_currentEntity.Item1, newX, newY);
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
            foreach (int preyIndex in world.GetSurroundingPrey(_currentEntity.x, _currentEntity.y))
            {
                if (Attempt.Success(rnd, Config.predationRate))
                {
                    (EntityType type, int, int) prey = world.entities.GetEntity(preyIndex);
                    if (prey.type != EntityType.Prey)
                    {
                        int hiwfhwf = 0;
                    }
                    Predation(prey, preyIndex);
                }
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
        private void Predation((EntityType type, int x, int y) prey, int preyIndex)
        {
            if (prey.type != EntityType.Prey)
            {
                int efjiejef = 0;
            }

            if (world.entities.GetEntity(preyIndex).Item1 != EntityType.Prey)
            {
                int haicahsouoih = 0;
            }

            // kill Prey and birth new Predator
            entityManager.RemoveEntity(prey, preyIndex);
            entityManager.BirthEntity(_currentEntity);
        }

        /// <summary>
        /// Attempt to let this entity die
        /// </summary>
        protected void AttemptDeath()
        {
            if (Attempt.Success(rnd, Config.deathRate)) 
                entityManager.RemoveEntity(_currentEntity, _currentEntityIndex);
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
