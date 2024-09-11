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

        void Round()
        {
            Entity entity;
            for (int i = 0; i < _world.AmountOfEntities; i++)
            {
                entity = _world.GetRandomEntity();

            }
        }
    }
}
