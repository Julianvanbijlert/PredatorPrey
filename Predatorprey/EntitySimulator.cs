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


        protected abstract void AttemptWalk();
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
                    (EntityType, int, int) prey = world.entities.GetEntity(preyIndex);
                    Predation(prey, preyIndex);
                }
            }
        }

        protected override void AttemptWalk()
        {
            if (Attempt.Success(rnd, Config.walkRate))
            {
                
                (int newX, int newY) = smellMap.GetSurroundingSmells(_currentEntity.x, _currentEntity.y);

                //check if it does not return same location because that means it did not find any smell
                if(newX == _currentEntity.x && newY == _currentEntity.y)
                    world.GetLocationFast(_currentEntity.x, _currentEntity.y);

                entityManager.ChangeLocation(_currentEntity, _currentEntityIndex, newX, newY);
            }
        }

        /// <summary>
        /// Do predation on a prey 
        /// </summary>
        /// <param name="p">The prey to kill</param>
        private void Predation((EntityType _, int x, int y) prey, int preyIndex)
        {
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

        protected override void AttemptWalk()
        {
            if (Attempt.Success(rnd, Config.walkRate))
            {
                (int newX, int newY) = world.GetLocationFast(_currentEntity.x, _currentEntity.y);

                //add smell in place where it was
                smellMap.AddSmell(_currentEntity.Item2, _currentEntity.Item2,  Config.smellStrength);

                //replace the location of the entity
                entityManager.ChangeLocation(_currentEntity, _currentEntityIndex, newX, newY);

            }
        }

        private void AttemptBirth()
        {
            if(Attempt.Success(rnd, Config.birthRate)) entityManager.BirthEntity(_currentEntity);
        }
    }
}
