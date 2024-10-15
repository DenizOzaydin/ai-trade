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

symbol = "XRPUSDT"
interval = "1-h"
start = "2019-01-01"
end = "2024-08-01"

access.save_stock_price(symbol, interval, start, end)