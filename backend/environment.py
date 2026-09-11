"""Entorno Gymnasium para el USV: evasion de obstaculos."""

import gymnasium as gym
import numpy as np
from gymnasium import spaces


class BoatObstacleAvoidanceEnv(gym.Env):
    """Envuelve la simulacion del bote (Unity) como un entorno Gymnasium."""

    metadata = {"render_modes": ["human"]}

    def __init__(self):
        super().__init__()

        # TODO: ajustar dimensiones segun los sensores reales del bote (LIDAR, IMU, GPS, etc.)
        self.observation_space = spaces.Box(low=-np.inf, high=np.inf, shape=(10,), dtype=np.float32)

        # Acciones: [empuje, timon]
        self.action_space = spaces.Box(low=-1.0, high=1.0, shape=(2,), dtype=np.float32)

    def reset(self, *, seed=None, options=None):
        super().reset(seed=seed)
        observation = np.zeros(self.observation_space.shape, dtype=np.float32)
        info = {}
        return observation, info

    def step(self, action):
        observation = np.zeros(self.observation_space.shape, dtype=np.float32)
        reward = 0.0
        terminated = False
        truncated = False
        info = {}
        return observation, reward, terminated, truncated, info

    def render(self):
        pass

    def close(self):
        pass
