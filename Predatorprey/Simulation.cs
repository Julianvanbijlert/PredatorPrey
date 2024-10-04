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
            Random r = GetNewRandom();

            _world = new World(r);
            _entityManager = new EntityManager(_world, r);
            _predatorSimulator = new PredatorSimulator(_entityManager, r);
            _preySimulator = new PreySimulator(_entityManager, r);
            _plotManager = new PlotManager();
            

            _entityManager.InitializeEntities();
        }

        /// <summary>
        /// Get a Random object based on the seed in Config
        /// </summary>
        private Random GetNewRandom()
        {
            if(Config.rndSeed != -1)
                return new Random(Config.rndSeed);

            // -1 signals you want a random seed, so do not specify the seed
            // when making the Random object.
            return new Random();
        }

        /// <summary>
        /// Run the simulation
        /// </summary>
        private void Run()
        {
            for (int i = 0; i < Config.amountOfRounds; i++)
            {
                Round(i);
                
                _plotManager.SaveJson();
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
                    _predatorSimulator.SetCurrentEntity(tuple.index);
                    _predatorSimulator.AttemptActions();
                }
                else
                {
                    _preySimulator.SetCurrentEntity(tuple.index);
                    _preySimulator.AttemptActions();
                }
            }

            if(Config.WithTracks) _world.tracksMap.DecreaseTracks();

            // print the world
            //Output.PrintWorld(_world);
            //Output.PrintList(_world);

            SaveStats(round);
            if(Config.WithPrint)
                _world.PrintStats();
        }

        public void SaveStats(int round)
        {
            _world.AddStatsToPM(_plotManager, round);
        }

        /// <summary>
        /// Checks whether the entity list and the world grid
        /// agree with each other. Can be used for testing.
        /// </summary>
        private bool ListMatchesGrid()
        {
            int k = _world.entities.k;
            for (int i = 0; i < k; i++)
            {
                (EntityType _, int x, int y) entity = _world.entities.GetEntity(i);
                if (_world.grid[entity.x, entity.y] != i)
                {
                    return false;
                }
            }

            for (int x = 0; x < Config.worldSize; x++)
            {
                for (int y = 0; y < Config.worldSize; y++)
                {
                    int index = _world.grid[x, y];
                    if (index == -1) continue;
                    (EntityType type, int x, int y) entities = _world.entities.GetEntity(index);
                    if (entities.x != x || entities.y != y)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
