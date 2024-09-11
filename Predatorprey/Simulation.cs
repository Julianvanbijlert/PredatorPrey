using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    internal class Simulation
    {
        private World _world;

        public static void Main()
        {
            Simulation s = new Simulation();
            s.Initialize();
            s.Run();
        }

        private void Initialize()
        {
            _world = new World();
        }

        private void Run()
        {
            for (int i = 0; i < Config.amountOfRounds; i++)
            {
                Round();
            }
        }

        private void Round()
        {
            int amountOfEntities = _world.AmountOfEntities;
            for (int i = 0; i < amountOfEntities; i++)
            {
                Entity entity = _world.GetRandomEntity();
                entity.AttemptActions();
            }
        }
    }
}
