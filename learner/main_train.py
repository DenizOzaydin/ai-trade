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

model_name = "xxx"
interval = "1-h"
start = "2019-01-01"
end = "2023-01-01"
stocks = ["BTCUSDT", "ETHUSDT"]
datas = []

for stock in stocks:
    datas.append(access.get_stock_price(stock, interval, start, end))

indicators = []
indicators.append(["emaRatio", ["close"], 20, 50])
indicators.append(["spectrum", ["high"], ["low"], ["close"], 40])

fss = []
css = []
oss = []

for data in datas:
    f, c, o = pre.create_dataset(data, indicators)
    fss.append(f)
    css.append(c)
    oss.append(o)

features = np.concatenate(fss)
closes = np.concatenate(css)
openTimes = np.concatenate(oss)

features, std = pre.normalize_features(features)
    
#%%

settings = models.PPOSettings()
settings.batch_size = len(features)
settings.n_steps = len(features)
settings.n_epochs = 20
settings.learning_rate = 0.1
settings.total_timesteps = len(features) * 400
settings.kwargs = [4, 2]
settings.gamma = 1

model = models.train_model(features, closes, openTimes, settings)
models.save_model(model, "./models/", model_name)
models.save_model_weights("./models", model_name, {"interval": interval, "indicators": indicators, "std": std})
#%%

