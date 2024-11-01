import numpy as np
import core

def zero_or_one(action):
    n = len(action)
    new_action = core.generate(n)
    for i in range(n):
        new_action[i] = 1 if action[i] >= 0 else -1
    return new_action

def action_filter(action, k):
    n = len(action)
    new_action = core.generate(n)
    if n == 0:
        return new_action
    for i in range(1, n):
        new_action[i] = new_action[i-1]
        if(action[i] < -k):
            new_action[i] = -1.
        if(action[i] > k):
            new_action[i] = 1.
    return new_action

def calculate_balance(close, actions, comm):    
    balance_ = 0
    commission = 0
    lot = 0
    balance_history_ = []
    commission_history = []
    prev_action = 0
    
    action_profit = 0
    
    profits = 0
    losses = 0
    
    for i in range(1, len(close)):
        prev_lot = lot
        balance_ += lot * (close[i] - close[i-1])
        action_profit += lot * (close[i] - close[i-1])
        
        if(actions[i] != prev_action):
            lot = 1000 * actions[i] / close[i]
            prev_action = actions[i]
            
            if(action_profit > 0):
                profits += action_profit
            else:
                losses -= action_profit
            
            action_profit = 0
            
        commission += abs(lot - prev_lot) * close[i] * comm
        balance_ -= abs(lot - prev_lot) * close[i] * comm
        balance_history_.append(balance_)
        commission_history.append(commission)

    return np.array(balance_history_), np.array(commission_history), 1