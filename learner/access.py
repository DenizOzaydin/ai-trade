#%%

import core
import requests
import json
import pandas as pd
import os

base_url = "https://localhost:7269"

def open_json(path):
    with open(path, 'r') as fp:
        dic = json.loads(fp.read())
        return dic

def get_stock_price(symbol, interval, start, end):
    if(type(start) == type("abc")):
        start = core.convert_to_unix(start)
    if(type(end) == type("abc")):
        end = core.convert_to_unix(end)
    response = requests.get(
        f"{base_url}/api/market/get?symbol={symbol}&interval={interval}&start={start}&end={end}"
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
        f"{base_url}/api/market/get?symbol={symbol}&interval={interval}&start={start}&end={end}"
        , verify=False)
    dic = json.loads(response.content)
    st = json.dumps(dic)
    with open(f"./data/{symbol}_{interval}.json", 'w') as fp:
        fp.write(st)

def get_stock_price_from_local(symbol, interval, start, end):
    if(type(start) == type("abc")):
        start = core.convert_to_unix(start)
    if(type(end) == type("abc")):
        end = core.convert_to_unix(end)
    file_name = f"./data/{symbol}_{interval}.json"
    st = ""
    with open(file_name, 'r') as fp:
        st = fp.read()
    dic = json.loads(st)
    df = pd.DataFrame(data=dic["bars"])
    df = df.drop(["symbol", "interval"], axis=1)
    df = df[df['openTime'] > start]
    df = df[df['openTime'] < end]
    df = df.reset_index(drop=True)
    return df

#%%

def upload_model_json_to_server(path, name, desc):
    url = f"{base_url}/manage/bot/upload"
    
    with open(path, 'rb') as fp:
        files = {"File": (path, fp, 'application/json')}
        data = {"Name": name, "Description": desc}
        response = requests.post(url, files=files, data=data, verify=False)
        
# %%
