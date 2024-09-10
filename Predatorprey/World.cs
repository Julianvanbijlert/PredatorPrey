using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    public class World
    {
        Random rnd = new Random();

        Entity[,] grid;
        List<Entity> entities = new List<Entity>();

        public World(int amountPrey, int amountPreditor)
        {
            for (int i = 0; i < amountPrey; i++)
            {
                AddPrey();
            }

            for (int i = 0; i < amountPreditor; i++)
            {
                AddPreditor();
            }


            grid = new Entity[10, 10];
        }


        /// <summary>
        /// Adds one prey to the world
        /// </summary>
        public void AddPrey()
        {
            entities.Add(new Prey());
        }


        /// <summary>
        /// Adds one preditor to the world
        /// </summary>
        public void AddPreditor()
        {
            entities.Add(new Predator());
        }
    }
}
