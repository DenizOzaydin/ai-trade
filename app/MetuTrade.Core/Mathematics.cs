using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetuTrade.Core
{
    public class Mathematics
    {
        public static List<double> Generate(int n)
        {
            List<double> list = new List<double>();
            for (int i = 0; i < n; i++)
            {
                list.Add(0);
            }
            return list;
        }

        public static List<double> DotProduct(List<List<double>> weight, List<double> input, List<double> bias)
        {
            int rows = weight.Count;
            if (rows == 0) return new List<double>();
            int cols = weight[0].Count;
            List<double> result = Generate(rows);

            for (int i = 0; i < rows; i++)
            {
                result[i] = 0;
                for (int j = 0; j < cols; j++)
                {
                    result[i] += weight[i][j] * input[j];
                }
                result[i] += bias[i];
            }

            return result;
        }

        public static List<double> Multiply(List<double> l1, List<double> l2)
        {
            int n = l1.Count;
            List<double> l3 = Generate(n);

            for (int i = 0; i < n; i++)
            {
                l3[i] = l1[i] * l2[i];
            }

            return l3;
        }

        public static List<double> Multiply(List<double> l1, double d)
        {
            int n = l1.Count;
            List<double> l3 = Generate(n);

            for (int i = 0; i < n; i++)
            {
                l3[i] = l1[i] * d;
            }

            return l3;
        }

        public static List<double> Divide(List<double> l1, List<double> l2)
        {
            int n = l1.Count;
            List<double> l3 = Generate(n);

            for (int i = 0; i < n; i++)
            {
                l3[i] = l2[i] == 0 ? 0 : l1[i] / l2[i];
            }

            return l3;
        }

        public static List<double> Divide(List<double> l1, double d)
        {
            int n = l1.Count;
            List<double> l3 = Generate(n);

            for (int i = 0; i < n; i++)
            {
                l3[i] = d == 0 ? 0 : l1[i] / d;
            }

            return l3;
        }

        public static List<double> Subtract(List<double> l1, List<double> l2)
        {
            int n = l1.Count;
            List<double> l3 = Generate(n);

            for (int i = 0; i < n; i++)
            {
                l3[i] = l1[i] - l2[i];
            }

            return l3;
        }

        public static List<double> Subtract(List<double> l1, double d)
        {
            int n = l1.Count;
            List<double> l3 = Generate(n);

            for (int i = 0; i < n; i++)
            {
                l3[i] = l1[i] - d;
            }

            return l3;
        }

        public static List<double> Sum(List<double> l1, List<double> l2)
        {
            int n = l1.Count;
            List<double> l3 = Generate(n);

            for (int i = 0; i < n; i++)
            {
                l3[i] = l1[i] + l2[i];
            }

            return l3;
        }

        public static List<double> Sum(List<double> l1, double d)
        {
            int n = l1.Count;
            List<double> l3 = Generate(n);

            for (int i = 0; i < n; i++)
            {
                l3[i] = l1[i] + d;
            }

            return l3;
        }

        public static List<double> Log(List<double> list)
        {
            List<double> log = Generate(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                log[i] = Math.Log(list[i]);
            }
            return log;
        }

        public static List<double> Tanh(List<double> list)
        {
            List<double> tanh = Generate(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                tanh[i] = Math.Tanh(list[i]);
            }
            return tanh;
        }

        public static List<double> Clamp(List<double> list, double minValue, double maxValue)
        {
            List<double> clamped = Generate(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                clamped[i] = list[i];
                if (clamped[i] < minValue) clamped[i] = minValue;
                if (clamped[i] > maxValue) clamped[i] = maxValue;
            }
            return clamped;
        }
    }
}
