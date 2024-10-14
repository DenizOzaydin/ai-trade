import core
import numpy as np
from collections import deque

class IndicatorSettings:
    def __init__(self):
        self.indicator_type = ""
        self.params = None

def derivative(ls):
    n = len(ls)
    der = core.generate(n)
    for i in range(1, n):
        der[i] = ls[i] - ls[i - 1]
    return der


def ema(ls, p):
    n = len(ls)
    a = 2.0 / (p + 1.0)
    ind = core.generate(n)
    if(n == 0):
        return ind
    ind[0] = ls[0]
    for i in range(1, n):
        ind[i] = a * ls[i] + (1 - a) * ind[i - 1]
    return ind


def rma(ls, p):
    n = len(ls)
    a = 1.0 / p
    ind = core.generate(n)
    if(n == 0):
        return ind
    ind[0] = ls[0]
    for i in range(1, n):
        ind[i] = a * ls[i] + (1 - a) * ind[i - 1]
    return ind


def tr(high, low, close):
    n = len(close)
    tr = core.generate(n)
    if (n == 0):
        return tr
    tr[0] = high[0] - low[0]
    for i in range(1, n):
        t1 = high[i] - low[i]
        t2 = abs(high[i] - close[i - 1])
        t3 = abs(close[i - 1] - low[i])
        tr[i] = max(t1, t2, t3)
    return tr


def atr(high, low, close, length):
    return rma(tr(high, low, close), length)


def adx(high, low, close, length):
    n = len(close)
    _atr = atr(high, low, close, length)

    up = core.generate(n)
    down = core.generate(n)
    k = core.generate(n)

    up[0] = 0
    down[0] = 0
    if(_atr[0] != 0):
        k[0] = 100.0 / _atr[0]
    else:
        k[0] = 0

    for i in range(length, n):
        pos = high[i] - high[i - 1]
        neg = low[i - 1] - low[i]

        up[i] = pos if pos > neg and pos > 0 else 0
        down[i] = neg if neg > pos and neg > 0 else 0
        if(_atr[i] != 0):
            k[i] = 100.0 / _atr[i]
        else:
            k[i] = 0

    up = np.multiply(rma(up, length), k)
    down = np.multiply(rma(down, length), k)

    dx = core.generate(n)
    for i in range(n):
        if(up[i] + down[i] != 0):
            dx[i] = 100.0 * (abs(up[i] - down[i]) / abs(up[i] + down[i]))
        else:
            dx[i] = 0

    return (dx, up, down)


def macd(close, fast, slow, signal):
    fastLine = ema(close, fast)
    slowLine = ema(close, slow)
    macdLine = np.subtract(fastLine, slowLine)
    signalLine = ema(macdLine, signal)
    macdHist = np.subtract(macdLine, signalLine)
    return (macdLine, macdHist, signalLine)


'''
            int n = H.Count;

            List<double> hhv = Generate(n);
            Deque<int> q = new Deque<int>(2000000);

            for(int i = 0; i < n; i++)
            {
                while(q.Size != 0 && i - q.GetFront() > p)
                {
                    q.PopFront();
                }
                while(q.Size != 0 && H[q.GetBack()] < H[i])
                {
                    q.PopBack();
                }
                q.PushBack(i);
                hhv[i] = H[q.GetFront()];
            }

            return hhv;
'''


def hhv(close, p):
    n = len(close)

    ind = core.generate(n)
    deq = deque()

    for i in range(n):
        while(len(deq) != 0 and i - deq[0] > p):
            deq.popleft()
        while(len(deq) != 0 and close[deq[-1]] < close[i]):
            deq.pop()
        deq.append(i)
        ind[i] = close[deq[0]]

    return ind


def llv(close, p):
    n = len(close)

    ind = core.generate(n)
    deq = deque()

    for i in range(n):
        while(len(deq) != 0 and i - deq[0] > p):
            deq.popleft()
        while(len(deq) != 0 and close[deq[-1]] > close[i]):
            deq.pop()
        deq.append(i)
        ind[i] = close[deq[0]]

    return ind


def price_range(high, low, close, length):
    n = len(close)
    
    _hhv = hhv(high, length)
    _llv = llv(low, length)
    pr = core.generate(n)
    diff = core.generate(n)

    for i in range(n):
        if(_hhv[i] - _llv[i] > 0):
            pr[i] = (close[i] - _llv[i]) / (_hhv[i] - _llv[i])
        else:
            pr[i] = 0
        diff[i] = _hhv[i] - _llv[i]
        
    return pr, diff
