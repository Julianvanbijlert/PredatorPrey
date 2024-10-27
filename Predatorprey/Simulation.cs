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
        private Program _program;
        private PredatorSimulator _predatorSimulator;
        private PreySimulator _preySimulator;
        private World _world;
        private EntityManager _entityManager;
        private PlotManager _plotManager;
        private Random _random;

        public int roundNumber { get; private set; }

        public double averagePredation => (_predatorSimulator.SuccessfulPredations / _entityManager.AmountOfPredatorsBorn);

        public Simulation(Program p, Random rnd)
        {
            this._program = p;
            this._random = rnd;
        }

        /// <summary>
        /// Set all necessary objects ready for the simulation to start
        /// </summary>
        public void Initialize()
        {
            _world = new World(_random);
            _entityManager = new EntityManager(_world, _random);
            _predatorSimulator = new PredatorSimulator(_entityManager, _random);
            _preySimulator = new PreySimulator(_entityManager, _random);
            _plotManager = new PlotManager();
            

            _entityManager.InitializeEntities();
        }

        /// <summary>
        /// Run the simulation one time
        /// </summary>
        public void Run()
        {
            Initialize();

            for (roundNumber = 0; roundNumber < Config.amountOfRounds && !Extinction(); roundNumber++)
            //for (roundNumber = 0; roundNumber < Config.amountOfRounds; roundNumber++)
            {
                Round();
                
                if(Config.WithGraph) _plotManager.SaveJson();
            }

            if(Config.WithGraph) _plotManager.SavePlot();
            _program.OnSimulationEnd();
        }

        /// <summary>
        /// True if the predators or prey have gone extinct
        /// </summary>
        public bool Extinction()
        {
            return _world.AmountOfPredators == 0 || _world.AmountOfPrey== 0;
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

            SaveStats(roundNumber);
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
        public PlotManager GetPlotManager()
        {
            return _plotManager;
        }
    }

   
}
