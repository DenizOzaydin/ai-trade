import core
import stats
import numpy as np
import indicator as ind

def normalize_by_divisor(ind, divisor):
    norm = np.divide(ind, divisor)
    return norm

def standard_normalization(ind):
    mean = stats.mean(ind)
    std = stats.std(ind)
    norm = (ind - mean) / std
    return norm, mean, std

def normalize_by_mean(ind):
    mean = stats.mean(ind)
    norm = np.divide(ind, mean)
    return norm, mean

def normalize_by_std(ind):
    std = stats.std(ind)
    norm = np.divide(ind, std)
    return norm, std

def normalize_by_tanh(ind):
    norm = np.tanh(ind)
    return norm

def normalize_features(features):
    featuresT = features.T
    standard_deviations = []
    features_new = []
    for i in range(len(featuresT)):
        std = np.std(featuresT[i])
        x = np.divide(featuresT[i], std)
        standard_deviations.append(std)
        for j in range(len(x)):
            if(x[j] < -4.):
                x[j] = -4.
            if(x[j] > 4.):
                x[j] = 4.
        features_new.append(x)
    return np.transpose(features_new), standard_deviations

def normalize_features_std(features, std):
    featuresT = features.T
    standard_deviations = []
    features_new = []
    for i in range(len(featuresT)):
        x = np.divide(featuresT[i], std[i])
        for j in range(len(x)):
            if(x[j] < -4.):
                x[j] = -4.
            if(x[j] > 4.):
                x[j] = 4.
        features_new.append(x)
    return np.transpose(features_new)

def create_dataset(data, indicators):
    high = np.array(data['high'])
    low = np.array(data['low'])
    close = np.array(data['close'])
    volume = np.array(data['volume'])
    openTime = np.array(data['openTime'])

    features = []

    for ix in indicators:
        features.append(ind.solve(ix, high, low, close, volume))

    features = np.transpose(features)

    return features, close, openTime