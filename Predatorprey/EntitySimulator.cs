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

        public EntitySimulator(EntityManager entityManager, Random rnd)
        {
            this.entityManager = entityManager;
            this.world = entityManager.world;
            this.rnd = rnd;
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
        protected void AttemptWalk()
        {
            if (Attempt.Success(rnd, Config.walkRate))
            {
                (int newX, int newY) = world.GetLocationNext(_currentEntity.x, _currentEntity.y);

                entityManager.ChangeLocation(_currentEntity, _currentEntityIndex, newX, newY);
            }
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
                    (EntityType, int, int) prey = world.entities.GetEntity(preyIndex);
                    Predation(prey, preyIndex);
                }
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

        private void AttemptBirth()
        {
            if(Attempt.Success(rnd, Config.birthRate)) entityManager.BirthEntity(_currentEntity);
        }
    }
}
