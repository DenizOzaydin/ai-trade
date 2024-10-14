import numpy as np
import datetime
import time

def generate(n):
    return np.array([0.] * n)

def convert_to_unix_3(year, month, day):
    date = datetime.datetime(year, month, day, 0, 0)
    unix = time.mktime(date.timetuple())
    return int(unix * 1000)

def convert_to_unix(date_str):
    s = date_str.split('-')
    year = int(s[0])
    month = int(s[1])
    day = int(s[2])
    return convert_to_unix_3(year, month, day)