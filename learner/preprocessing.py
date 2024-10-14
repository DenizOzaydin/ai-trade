import core
import stats
import numpy as np

def normalize_by_close(ind, close):
    norm = np.divide(ind, close)
    mean = stats.mean(norm)
    std = stats.std(norm)
    norm = np.divide(norm, std)
    norm = np.tanh(norm)
    return norm

def normalize(ind):
    mean = stats.mean(ind)
    std = stats.std(ind)
    norm = np.divide(ind, std)
    norm = np.tanh(norm)
    return norm
