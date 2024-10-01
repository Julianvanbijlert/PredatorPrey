using System;
using System.Collections.Generic;
using System.IO;
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

        public PlotManager()
        {
            entityCount = new int[Config.amountOfRounds];
            preyCount = new Queue<int>(Config.worldSize);
            predatorCount = new Queue<int>(Config.worldSize);
            roundCount = new int[Config.amountOfRounds];
        }

        public void AddData(int round, int entities, int prey, int predator)
        {
            entityCount[round] = entities;

            preyCount.Enqueue(prey);
            predatorCount.Enqueue(predator);

            if (preyCount.Count > Config.worldSize)
            {
                preyCount.Dequeue();
                predatorCount.Dequeue();

            }

            roundCount[round] = round;
        }

        public void SavePlot()
        {
            _plot.Clear();
            _plot.Add.Scatter(roundCount, predatorCount.ToArray());
            _plot.Add.Scatter(roundCount, preyCount.ToArray());
            _plot.Add.Scatter(roundCount, entityCount);


            string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            _plot.SavePng($"../../../Graphs/{timestamp}.png", 600, 400);
        }


        public void SaveJson()
        {
            var data = new DrawingData { prey = preyCount.ToArray(), predator = predatorCount.ToArray() };
            var jsonString = JsonSerializer.Serialize(data);
            File.WriteAllText("../../../JSON/output.json", jsonString);
        }

        public class DrawingData
        {
            public int[] prey { get; set; }
            public int[] predator { get; set; }
        }





    }
}
