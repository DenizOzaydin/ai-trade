import core 
import indicator as ind

def ema_ratio_strategy(close, p1, p2):
    n = len(close)
    action = core.generate(n)
    a = ind.ema_ratio(close, p1, p2)
    for i in range(n):
        if(a[i] > 0):
            action[i] = 1.
        else:
            action[i] = -1.
    return action

def spectrum_strategy(high, low, close, p):
    n = len(close)
    action = core.generate(n)
    a = ind.spectrum(high, low, close, p)
    for i in range(n):
        if(a[i] > 0.5):
            action[i] = 1.
        if(a[i] < -0.5):
            action[i] = -1.
    return action