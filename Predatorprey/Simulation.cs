using System;
using Microsoft.VisualBasic;
using ScottPlot;



namespace Project1
{
    /// <summary>
    /// Handle the simulation steps, like the rounds
    /// </summary>
    internal class Simulation
    {
        private PredatorSimulator _predatorSimulator;
        private PreySimulator _preySimulator;
        private World _world;
        private EntityManager _entityManager;
        private PlotManager _plotManager;
        //private Forms _forms;

       
        class Program
        {
            static void Maind()
            {
            }
        }

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
            Random r = new Random(201);

            _world = new World(r);
            _entityManager = new EntityManager(_world, r);
            _predatorSimulator = new PredatorSimulator(_entityManager, r);
            _preySimulator = new PreySimulator(_entityManager, r);
            _plotManager = new PlotManager();
            

            _entityManager.InitializeEntities();
        }

        /// <summary>
        /// Run the simulation
        /// </summary>
        private void Run()
        {
            for (int i = 0; i < Config.amountOfRounds; i++)
            {
                Round(i);
            }

            _plotManager.SavePlot();

        }


        /// <summary>
        /// Handle one single round of the simulation
        /// </summary>
        private void Round(int round)
        {
            int amountOfEntities = _world.AmountOfEntities;
            for (int i = 0; i < amountOfEntities; i++)
            {
                // choose a random entity
                ((EntityType type, int, int) entity, int index) tuple = _world.entities.GetRandomEntity();
                // let the entities do actions
                if (tuple.entity.type == EntityType.Predator)
                {
                    _predatorSimulator.SetCurrentEntity(tuple.entity, tuple.index);
                    _predatorSimulator.AttemptActions();
                }
                else
                {
                    _preySimulator.SetCurrentEntity(tuple.entity, tuple.index);
                    _preySimulator.AttemptActions();
                }
            }

            if(Config.WithSmell) _world.smellMap.DecreaseSmells();

            // print the world
            //Output.PrintWorld(_world);
            //Output.PrintList(_world);

            SaveStats(round);
            _world.PrintStats();
        }

        public void SaveStats(int round)
        {
            _world.AddStatsToPM(_plotManager, round);
        }
    }
}
