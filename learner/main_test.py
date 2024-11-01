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

stock = "ETHUSDT"
start = "2023-01-01"
end = "2025-01-01"

model_name = "c_2"
#settings["settings"]["interval"]
interval = "1-h"

#%%

model_path = f"./models/{model_name}.pth"
settings_path = f"./models/weights/{model_name}.json"

model = models.open_model(model_path)
settings = access.open_json(settings_path)

#%%
datas = [access.get_stock_price(stock, interval, start, end)]

#%%
indicators = settings["settings"]["indicators"]

#%%

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

features = pre.normalize_features_std(features, settings["settings"]["std"])
    
#%%

action = models.test_model(model_path, features, closes, openTimes)
balance, _, _ = analyze.calculate_balance(closes, action, 0.0000)
plt.plot(balance)
plt.show()

#%%