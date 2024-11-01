using MetuTrade.Core.TechnicalAnalysis;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetuTrade.Core.ArtificialIntelligence
{
    public class NeuralNetwork
    {
        [JsonProperty("weights")]
        public List<List<List<double>>>? Weights { get; set; }

        [JsonProperty("biases")]
        public List<List<double>>? Biases { get; set; }

        [JsonProperty("settings")]
        public NeuralNetworkSettings Settings { get; set; }

        public int Size1 => Weights?.Count ?? 0;
        public int Size2 => Size1 == 0 ? 0 : (Weights?[0].Count ?? 0);
        public int Size3 => Size2 == 0 ? 0 : (Weights?[0][0].Count ?? 0);

        public List<double> Predict(List<double> input)
        {
            List<double> temp = Mathematics.Generate(input.Count);

            for (int i = 0; i < temp.Count; i++)
            {
                temp[i] = input[i];
            }

            if (Weights != null && Biases != null && Settings != null)
            {
                for (int i = 0; i < Weights.Count; i++)
                {
                    temp = Mathematics.DotProduct(Weights[i], temp, Biases[i]);
                    if (i != Weights.Count - 1) temp = Mathematics.Tanh(temp);
                }
            }

            temp = Mathematics.Clamp(temp, -1.0, 1.0);

            return temp;
        }

        public double? Process(Chart chart)
        {
            if (Weights == null || Biases == null || Settings == null) return null;

            var high = chart.GetHighValues();
            var low = chart.GetLowValues();
            var close = chart.GetCloseValues();
            var volume = chart.GetVolumes();

            List<double> input = Mathematics.Generate(Settings.Indicators.Count);

            for(int i = 0; i < Settings.Indicators.Count; i++)
            {
                List<double> ind = Indicators.Solve(Settings.Indicators[i], high, low, close, volume);
                input[i] = ind[ind.Count - 2] / Settings.StandardDeviations[i];
            }

            input = Mathematics.Clamp(input, -4.0, 4.0);

            var predict = Predict(input);

            return predict[0];

            /*Console.Write(chart.Symbol + " " + chart.Interval + " predicted action: ");
            Console.ForegroundColor = predict[0] >= 0 ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine(predict[0]);
            Console.ResetColor();*/
        }
    }
}
