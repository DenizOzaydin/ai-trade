import core
import requests
import json
import pandas as pd
import os

def get_stock_price(symbol, interval, start, end):
    if(type(start) == type("abc")):
        start = core.convert_to_unix(start)
    if(type(end) == type("abc")):
        end = core.convert_to_unix(end)
    response = requests.get(
        f"https://localhost:7245/api/market/get?symbol={symbol}&interval={interval}&start={start}&end={end}"
        , verify=False)
    dic = json.loads(response.content)
    df = pd.DataFrame(data=dic["bars"])
    df = df.drop(["symbol", "interval"], axis=1)
    return df

def save_stock_price(symbol, interval, start, end):
    if(type(start) == type("abc")):
        start = core.convert_to_unix(start)
    if(type(end) == type("abc")):
        end = core.convert_to_unix(end)
    response = requests.get(
        f"https://localhost:7245/api/market/get?symbol={symbol}&interval={interval}&start={start}&end={end}"
        , verify=False)
    dic = json.loads(response.content)
    st = json.dumps(dic)
    with open(f"./data/{symbol}_{interval}.json", 'w') as fp:
        fp.write(st)