using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ScottPlot;


namespace Project1
{
    public class PlotManager
    {
        Plot _plot = new Plot();
        private Queue<int> preyCount;
        private Queue<int> predatorCount;
        private int[] roundCount;
        private int[] entityCount;
        private int[][] grid;


        public PlotManager()
        {
            entityCount = new int[Config.amountOfRounds];
            preyCount = new Queue<int>(Config.worldSize);
            predatorCount = new Queue<int>(Config.worldSize);
            roundCount = new int[Config.amountOfRounds];
        }

        public void AddData(int round, int entities, int prey, int predator, EntityList entity)
        {
            entityCount[round] = entities;

            preyCount.Enqueue(prey);
            predatorCount.Enqueue(predator);
            
            roundCount[round] = round;


            int[][] tempGrid = new int[Config.worldSize][];

            for (int i = 0; i < Config.worldSize; i++)
            {
                tempGrid[i] = new int[Config.worldSize];
            }

            foreach ((EntityType,int, int) e in entity)
            {
                int color = e.Item1 == EntityType.Prey ? 1 : 2;
                tempGrid[e.Item2][ e.Item3] = color;
            }

            grid = tempGrid;

        }

        public void SavePlot()
        {
            _plot.Add.Scatter(roundCount, predatorCount.ToArray());
            _plot.Add.Scatter(roundCount, preyCount.ToArray());
            //_plot.Add.Scatter(roundCount, entityCount);


            string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            _plot.SavePng($"../../../Graphs/{timestamp}.png", 600, 400);
        }

        /// <summary>
        /// First plot is without hunting, second plot is with hunting
        /// </summary>
        public static void SaveTwoPlots(PlotManager plot1, PlotManager plot2)
        {

            Plot plt = new Plot();
           var a = plt.Add.Scatter(plot1.roundCount, plot1.predatorCount.ToArray());
           var b = plt.Add.Scatter(plot1.roundCount, plot1.preyCount.ToArray());

           var c =  plt.Add.Scatter(plot2.roundCount, plot2.predatorCount.ToArray());
           var d =  plt.Add.Scatter(plot2.roundCount, plot2.preyCount.ToArray());

           a.Color = Colors.Red;  //pred
           b.Color = Colors.Green; //prey

           c.Color = Colors.DarkOrange;  //pred
           c.LinePattern =  LinePattern.Dashed;
            d.Color = Colors.LightGreen;  //prey
            d.LinePattern = LinePattern.Dashed;



            plt.XLabel("Rounds");
           plt.YLabel( "Amount of entities");

            plt.SavePng($"../../../Graphs/{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.png", 600, 400);
        }


        public void SaveJson()
        {
            var data = new DrawingData { prey = preyCount.ToArray(), predator = predatorCount.ToArray(), Egrid = grid };
            var jsonString = JsonSerializer.Serialize(data);
            File.WriteAllText("../../../JSON/output.json", jsonString);
        }

        public (double, double, double) Fourier()
        {
           return FourierTransform.GetTopFrequency(preyCount.Select(x => (double) x).ToArray());
        }

        public class DrawingData
        {
            public int[] prey { get; set; }
            public int[] predator { get; set; }

            public int[][] Egrid { get; set; }
        }





    }
}
