using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MetuTrade.Core.Collections;

namespace MetuTrade.Core.TechnicalAnalysis
{
    public static class Indicators
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

        public static List<double> Multiply(List<double> l1, List<double> l2)
        {
            int n = l1.Count;
            List<double> l3 = Generate(n);

            for(int i = 0; i < n; i++)
            {
                l3[i] = l1[i] * l2[i];
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

        public static List<double> EMA(List<double> close, int period)
        {
            int n = close.Count;
            double a = 2.0 / (period + 1.0);
            List<double> ind = Generate(n);
            if (n == 0) return ind;
            ind[0] = close[0];
            for(int i = 1; i < n; i++)
            {
                ind[i] = a * close[i] + (1 - a) * close[i - 1];
            }
            return ind;
        }

        public static List<double> RMA(List<double> close, int period)
        {
            int n = close.Count;
            double a = 1.0 / period;
            List<double> ind = Generate(n);
            if (n == 0) return ind;
            ind[0] = close[0];
            for (int i = 1; i < n; i++)
            {
                ind[i] = a * close[i] + (1 - a) * close[i - 1];
            }
            return ind;
        }

        public static List<double> TR(List<double> high, List<double> low, List<double> close)
        {
            int n = close.Count;
            List<double> tr = Generate(n);
            if (n == 0) return tr;
            tr[0] = high[0] - low[0];
            for(int i = 1; i < n; i++)
            {
                tr[i] = high[i] - low[i];
                tr[i] = Math.Max(tr[i], Math.Abs(high[i] - close[i - 1]));
                tr[i] = Math.Max(tr[i], Math.Abs(close[i - 1] - low[i]));
            }
            return tr;
        }

        public static List<double> ATR(List<double> high, List<double> low, List<double> close, int period)
        {
            return RMA(TR(high, low, close), period);
        }

        public static (List<double>, List<double>, List<double>) ADX(List<double> high, List<double> low, List<double> close, int period)
        {
            int n = close.Count;
            List<double> atr = ATR(high, low, close, period);

            List<double> up = Generate(n);
            List<double> down = Generate(n);
            List<double> k = Generate(n);

            up[0] = 0;
            down[0] = 0;

            if (atr[0] != 0) k[0] = 100.0 / atr[0];
            else k[0] = 0;

            for(int i = period; i < n; i++) 
            {
                double pos = high[i] - high[i - 1];
                double neg = low[i - 1] - low[i];

                if (pos > neg && pos > 0) up[i] = pos;
                else up[i] = 0;

                if (neg > pos && neg > 0) down[i] = neg;
                else down[i] = 0;

                if (atr[i] != 0) k[i] = 100.0 / atr[i];
                else k[i] = 0;
            }

            up = Multiply(RMA(up, period), k);
            down = Multiply(RMA(down, period), k);

            List<double> dx = Generate(n);

            for (int i = 0; i < n; i++)
            {
                if (up[i] + down[i] != 0) dx[i] = 100.0 * (Math.Abs(up[i] - down[i]) / Math.Abs(up[i] + down[i]));
                else dx[i] = 0;
            }

            return (dx, up, down);
        }

        public static (List<double>, List<double>, List<double>) MACD(List<double> close, int fast, int slow, int signal)
        {
            List<double> fastLine = EMA(close, fast);
            List<double> slowLine = EMA(close, slow);
            List<double> macdLine = Subtract(fastLine, slowLine);
            List<double> signalLine = EMA(macdLine, signal);
            List<double> macdHist = Subtract(macdLine, signalLine);
            return (macdLine, macdHist, signalLine);
        }

        public static List<double> HHV(List<double> H, int p)
        {
            int n = H.Count;

            List<double> hhv = Generate(n);
            Deque<int> q = new Deque<int>(100000);

            for (int i = 0; i < n; i++)
            {
                while (q.Size != 0 && i - q.GetFront() > p)
                {
                    q.PopFront();
                }
                while (q.Size != 0 && H[q.GetBack()] < H[i])
                {
                    q.PopBack();
                }
                q.PushBack(i);
                hhv[i] = H[q.GetFront()];
            }

            return hhv;
        }

        public static List<double> LLV(List<double> L, int p)
        {
            int n = L.Count;

            List<double> llv = Generate(n);
            Deque<int> q = new Deque<int>(100000);

            for (int i = 0; i < n; i++)
            {
                while (q.Size != 0 && i - q.GetFront() > p)
                {
                    q.PopFront();
                }
                while (q.Size != 0 && L[q.GetBack()] > L[i])
                {
                    q.PopBack();
                }
                q.PushBack(i);
                llv[i] = L[q.GetFront()];
            }

            return llv;
        }

        public static (List<double>, List<double>) PriceRange(List<double> high, List<double> low, List<double> close, int period)
        {
            int n = close.Count;

            List<double> hhv = HHV(high, period);
            List<double> llv = LLV(low, period);

            var pr = Generate(n);
            var diff = Generate(n);

            for (int i = 0; i < n; i++) {
                if (hhv[i] - llv[i] > 0) pr[i] = (close[i] - llv[i]) / (hhv[i] - llv[i]);
                else pr[i] = 0;
                diff[i] = hhv[i] - llv[i];
            }

            return (pr, diff);
        }
    }
}
