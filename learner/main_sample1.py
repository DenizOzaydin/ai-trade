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

symbol = "ETHUSDT"
interval = "1-h"
start = "2019-01-01"
end = "2024-08-01"

data = access.get_stock_price_from_local(symbol, interval, start, end)
