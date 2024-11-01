import numpy as np
import gym
from stable_baselines3 import PPO
from stable_baselines3.common.env_checker import check_env
import torch
import torch.nn as nn
import os
import json

class PPOSettings:
    gamma = 1.0
    batch_size = 530000
    n_epochs = 60
    total_timesteps = 53000000
    learning_rate = 0.003
    n_steps = 530000
    clip_range = 0.1
    gae_lambda = 0.95
    ent_coef = 0.001
    kwargs = [8]
    
def open_model(path):
    return PPO.load(path)

def save_model(model, directory, name):
    model_path = os.path.join(directory, name + ".pth")
    model.save(model_path)

def get_env(states, closes, openTimes):
    class TradingEnv(gym.Env):
        def __init__(self):
            super(TradingEnv, self).__init__()
            self.action_space = gym.spaces.Box(low=-1., high=1., shape=(1,), dtype=np.float32)
            self.observation_space = gym.spaces.Box(low=-4., high=4., shape=(states.shape[1],), dtype=np.float32)
            self.current_step = 0
            self.last_lot = 0
            self.last_action = 0
    
        def reset(self):
            self.current_step = 0
            self.last_lot = 0
            self.last_action = 0
            return self._next_observation()
    
        def step(self, action):
            self.current_step += 1
            if(openTimes[self.current_step] < openTimes[self.current_step - 1]):
                self.current_step += 1
                
            done = self.current_step >= len(closes) - 2
            
            prev_close = closes[self.current_step-1]
            
            cur_close = closes[self.current_step]
            
            act = action
            
            self.last_lot = act / cur_close
             
            reward = float(self.last_lot * (cur_close - prev_close))

            info = {}
            return self._next_observation(), reward, done, info
    
        def _next_observation(self):
            return states[self.current_step].flatten().astype(np.float32)
        
    return TradingEnv()

def train_model(states, closes, openTimes, settings, pre_path=None):
    policy_kwargs = dict(
        net_arch=settings.kwargs,  
        activation_fn=nn.Tanh 
    )
    
    env = get_env(states, closes, openTimes)
    check_env(env)
    
    model = PPO("MlpPolicy", 
                env, 
                policy_kwargs=policy_kwargs, 
                gamma=settings.gamma, 
                batch_size=settings.batch_size, 
                n_epochs=settings.n_epochs, 
                verbose=1, 
                learning_rate=settings.learning_rate, 
                n_steps=settings.n_steps, 
                clip_range=settings.clip_range, 
                gae_lambda=settings.gae_lambda, 
                ent_coef=settings.ent_coef, seed=2024)
    
    if(pre_path != None):
        pre_model = open_model(pre_path)
        model.policy = pre_model.policy
    
    model.learn(total_timesteps=settings.total_timesteps)
    return model

def test_model(path, states, closes, openTimes):
    model = open_model(path)
        
    actions = []
    
    for i in range(len(states)):
        action, _ = model.predict(states[i], deterministic=True)
        actions.append(action)
        
    return np.array(actions).flatten()

def save_model_weights(dir, model_name, settings):
    path = os.path.join(dir, model_name) + ".pth"
    model = open_model(path)
    policy = model.policy

    model_params = {}

    weights = []
    biases = []

    for name, param in policy.named_parameters():
        if(name.startswith("value")):
            continue
        if(name.endswith("weight")):
            weights.append(param.data.numpy().tolist())
        if(name.endswith("bias")):
            biases.append(param.data.numpy().tolist())
        
    model_params["weights"] = weights
    model_params["biases"] = biases
    model_params["settings"] = settings

    json_path = dir + "/weights/" + model_name + ".json"
    
    with open(json_path, "w") as f:
        json.dump(model_params, f)


    
    