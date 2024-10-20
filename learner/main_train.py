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
end = "2023-01-01"
feature_count = 5

btc = access.get_stock_price("BTCUSDT", interval, start, end)
eth = access.get_stock_price("ETHUSDT", interval, start, end)
bnb = access.get_stock_price("BNBUSDT", interval, start, end)
ltc = access.get_stock_price("LTCUSDT", interval, start, end)
ada = access.get_stock_price("BNBUSDT", interval, start, end)
sol = access.get_stock_price("LTCUSDT", interval, start, end)

#%%

mult = 1

def concat(features, index, ls):
    for x in ls:
        features[index].append(x)

indicators = []
indicators.append(("macdLine", ("close", 35 * mult, 70 * mult, 20 * mult)))
indicators.append(("macdHist", ("close", 35 * mult, 70 * mult, 20 * mult)))
indicators.append(("macdLine", ("volume", 35 * mult, 70 * mult, 20 * mult)))
indicators.append(("macdHist", ("volume", 35 * mult, 70 * mult, 20 * mult)))
indicators.append(("priceRange", (400 * mult)))

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

f1, c1, o1 = create_dataset(btc)
f2, c2, o2 = create_dataset(eth)
f3, c3, o3 = create_dataset(bnb)
f4, c4, o4 = create_dataset(ltc)
f5, c5, o5 = create_dataset(ada)
f6, c6, o6 = create_dataset(sol)

features = np.concatenate([f1, f2, f3, f4, f5, f6])
closes = np.concatenate([c1, c2, c3, c4, c5, c6])
openTimes = np.concatenate([o1, o2, o3, o4, o5, o6])
    
#%%

settings = models.PPOSettings()
settings.batch_size = len(features)
settings.n_steps = len(features)
settings.n_epochs = 10
settings.learning_rate = 0.1
settings.total_timesteps = len(features) * 400
settings.kwargs = [5]

model = models.train_model(features, closes, openTimes, settings)
models.save_model(model, "./models/", "btc_2")
models.save_model_weights("./models", "btc_2", indicators)
#%%

