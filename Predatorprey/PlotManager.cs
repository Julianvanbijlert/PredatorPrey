using System;
using ScottPlot;


namespace Project1
{
    public class PlotManager
    {
        Plot _plot = new Plot();
        private int[] preyCount;
        private int[] predatorCount;
        private int[] roundCount;
        private int[] entityCount;

        public PlotManager()
        {
            entityCount = new int[Config.amountOfRounds];
            preyCount = new int[Config.amountOfRounds];
            predatorCount = new int[Config.amountOfRounds];
            roundCount = new int[Config.amountOfRounds];
        }

        public void AddData(int round, int entities, int prey, int predator)
        {
            entityCount[round] = entities;
            preyCount[round] = prey;
            predatorCount[round] = predator;
            roundCount[round] = round;
        }

        public void SavePlot()
        {
            _plot.Clear();
            _plot.Add.Scatter(roundCount, predatorCount);
            _plot.Add.Scatter(roundCount, preyCount);
            _plot.Add.Scatter(roundCount, entityCount);
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            _plot.SavePng($"../../../Graphs/{timestamp}.png", 600, 400);
        }

        





    }
}
