using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    /// <summary>
    /// Handle the simulation steps, like the rounds
    /// </summary>
    internal class Simulation
    {
        private World _world;

        public static void Main()
        {
            Simulation s = new Simulation();
            s.Initialize();
            s.Run();
        }

        /// <summary>
        /// Set all necessary objects ready for the simulation to start
        /// </summary>
        private void Initialize()
        {
            _world = new World();
        }

        /// <summary>
        /// Run the simulation
        /// </summary>
        private void Run()
        {
            for (int i = 0; i < Config.amountOfRounds; i++)
            {
                Round();
            }
        }

        /// <summary>
        /// Handle one single round of the simulation
        /// </summary>
        private void Round()
        {
            int amountOfEntities = _world.AmountOfEntities;
            for (int i = 0; i < amountOfEntities; i++)
            {
                // choose a random entity
                Entity entity = _world.GetRandomEntity();
                // let the entities do actions
                entity.AttemptActions();
            }

            // add birthed entities to the world
            _world.ReleaseBirthedEntities();
        }
    }
}
