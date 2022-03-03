# Reference: https://github.com/AurelianTactics/unity_ml_agents_benchmarks/blob/master/basic_env/basic_stable_ppo_single.py

from gym_unity.envs import UnityEnv
from stable_baselines.common.policies import MlpPolicy
from stable_baselines.common.vec_env import DummyVecEnv
from stable_baselines import PPO2
from stable_baselines.bench import Monitor
import os
import time

env_name = "../BuiltEnvironments/Env1Turns"
env = UnityEnv(env_name, worker_id=2, use_visual=True)

# Create log dir
time_int = int(time.time())
log_dir = "../StableBaselinesResults/Env1Turns{}/".format(time_int)
os.makedirs(log_dir, exist_ok=True)
env = Monitor(env, log_dir, allow_early_resets=True)

env = DummyVecEnv([lambda: env])  # The algorithms require a vectorized environment to run

model = PPO2(MlpPolicy, env, verbose=1)
model.learn(total_timesteps=15000)