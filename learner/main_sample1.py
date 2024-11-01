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
import strategy

#%%

symbol = "ETHUSDT"
interval = "1-m"
start = "2019-01-01"
end = "2024-08-01"

data = access.get_stock_price(symbol, interval, start, end)

action = strategy.ema_ratio_strategy(data['close'], 500, 2000)
