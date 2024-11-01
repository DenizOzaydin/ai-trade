import core
import numpy as np
from collections import deque

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

def ema_ratio(ls, p1, p2):
    return np.log(ema(ls, p1) / ema(ls, p2))

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

def spectrum(high, low, close, length):
    n = len(close)
    
    _hhv = hhv(high, length)
    _llv = llv(low, length)
    pr = core.generate(n)
    diff = core.generate(n)

    for i in range(n):
        if(_hhv[i] - _llv[i] > 0):
            pr[i] = (close[i] - _llv[i]) / (_hhv[i] - _llv[i])
            pr[i] = pr[i] * 2.0 - 1.0
        else:
            pr[i] = 0
        diff[i] = _hhv[i] - _llv[i]
        
    return pr

def solve(ix, high, low, close, volume):
    if(ix[0] == 'close'):
        return close
    if(ix[0] == 'high'):
        return high
    if(ix[0] == 'low'):
        return low
    if(ix[0] == 'volume'):
        return volume
    if(ix[0] == 'ema'):
        return ema(solve(ix[1], high, low, close, volume), ix[2])
    if(ix[0] == 'emaRatio'):
        return ema_ratio(solve(ix[1], high, low, close, volume), ix[2], ix[3])
    if(ix[0] == "spectrum"):
        return spectrum(solve(ix[1], high, low, close, volume), solve(ix[2], high, low, close, volume), solve(ix[3], high, low, close, volume), ix[4])
    return None