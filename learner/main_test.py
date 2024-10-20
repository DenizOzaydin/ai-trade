#%%

import core
import access
import stats
import preprocessing as pre
import models
import indicator as ind
import numpy as np
import matplotlib.pyplot as plt
import analyze

#%%

interval = "1-h"
start = "2019-01-01"
end = "2024-10-20"
feature_count = 5

_data = access.get_stock_price("BNBUSDT", interval, start, end)

#%%

mult = 1

def concat(features, index, ls):
    for x in ls:
        features[index].append(x)

def create_dataset(data):
    high = np.array(data['high'])
    low = np.array(data['low'])
    close = np.array(data['close'])
    volume = np.array(data['volume'])
    openTime = np.array(data['openTime'])

    macdLine, macdHist, _ = ind.macd(close, 35 * mult, 70 * mult, 20 * mult)
    volMacd, volHist, _ = ind.macd(volume, 35 * mult, 70 * mult, 20 * mult)
    price_range, hl_diff = ind.price_range(high, low, close, 400 * mult)

    macdLine = pre.normalize_by_close(macdLine, close)
    macdHist = pre.normalize_by_close(macdHist, close)
    volMacd = pre.normalize(volMacd)
    volHist = pre.normalize(volHist)

    hl_diff = pre.normalize_by_close(hl_diff, close)
    
    features = []
    features.append(macdLine)
    features.append(macdHist)
    features.append(volMacd)
    features.append(volHist)
    features.append(price_range)
    
    features = np.transpose(features)
    
    return features, close, openTime

f1, c1, o1 = create_dataset(_data)

features = np.concatenate([f1])
closes = np.concatenate([c1])
openTimes = np.concatenate([o1])
    
#%%

action = models.test_model("./models/btc_2.pth", features, closes, openTimes)
balance, _, _ = analyze.strategy(closes, action, 0.0000)
plt.plot(balance)
plt.show()

#%%