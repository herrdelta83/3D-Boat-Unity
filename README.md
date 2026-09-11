# Vanttec Autonomous Boat - 3D RL Simulation

Este repositorio contiene la simulación en 3D y el entorno de entrenamiento para un agente de Reinforcement Learning (RL) capaz de controlar el bote autónomo de Vanttec y evadir obstáculos en tiempo real.

## 📂 Arquitectura del Sistema

El ecosistema está desacoplado en dos capas principales para mantener la separación de responsabilidades entre el entorno gráfico y la lógica algorítmica:

* **`/frontend`**: Proyecto en Unity 3D. Contiene el modelo del bote, el entorno de obstáculos, las físicas aplicadas y los scripts en C# que exponen el estado actual de la simulación (posiciones, sensores y raycasts).
* **`/backend`**: Entorno en Python. Centraliza la configuración de hiperparámetros, la función de recompensa y el entrenamiento iterativo del agente utilizando algoritmos de RL.

## 🛠️ Stack Tecnológico

| Componente | Herramientas Principales |
| :--- | :--- |
| **Simulación y Gráficos** | Unity 3D, C# |
| **Modelos 3D** | Formato `.fbx` (convertido desde `.stl`) |
| **Backend AI** | Python, PyTorch |
| **Comunicación Bidireccional**| Unity ML-Agents Toolkit |

## 🎯 Objetivos del agente

Navegación: Alcanzar coordenadas objetivo de forma eficiente.

Evasión de Colisiones: Utilizar sensores simulados para detectar obstáculos en el agua y corregir el trayecto.

Físicas Realistas: Controlar la aceleración y el torque respetando las físicas de un vehículo marítimo (arrastre, inercia).

## 🚀 Guía de Inicio Rápido

### 1. Configuración del Entorno de Entrenamiento (Backend)
1. Navega al directorio correspondiente:
   ```bash
   cd backend