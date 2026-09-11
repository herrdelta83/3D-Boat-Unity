"""Punto de entrada para entrenar/evaluar el agente RL del USV."""

import argparse

from stable_baselines3 import PPO
from stable_baselines3.common.env_checker import check_env

from environment import BoatObstacleAvoidanceEnv

MODELS_DIR = "models"


def train(timesteps: int = 100_000) -> None:
    env = BoatObstacleAvoidanceEnv()
    check_env(env)

    model = PPO("MlpPolicy", env, verbose=1)
    model.learn(total_timesteps=timesteps)
    model.save(f"{MODELS_DIR}/ppo_boat_obstacle_avoidance")


def evaluate(model_path: str, episodes: int = 5) -> None:
    env = BoatObstacleAvoidanceEnv()
    model = PPO.load(model_path, env=env)

    for episode in range(episodes):
        obs, _ = env.reset()
        done = False
        total_reward = 0.0
        while not done:
            action, _ = model.predict(obs, deterministic=True)
            obs, reward, terminated, truncated, _ = env.step(action)
            total_reward += reward
            done = terminated or truncated
        print(f"Episode {episode + 1}: reward={total_reward}")


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Entrena o evalua el agente RL del USV")
    parser.add_argument("--train", action="store_true", help="Entrena un nuevo modelo")
    parser.add_argument("--eval", action="store_true", help="Evalua un modelo entrenado")
    parser.add_argument("--model", type=str, help="Ruta al modelo para evaluacion")
    parser.add_argument("--timesteps", type=int, default=100_000, help="Pasos de entrenamiento")
    args = parser.parse_args()

    if args.train:
        train(timesteps=args.timesteps)
    elif args.eval:
        if not args.model:
            parser.error("--eval requiere --model <ruta_al_modelo>")
        evaluate(args.model)
    else:
        parser.error("Especifica --train o --eval")
