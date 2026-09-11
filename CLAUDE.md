# Project Guidelines

## Communication Protocol (Caveman Mode)
Respond terse like smart caveman. All technical substance stay. Only fluff die.
* Drop: articles (a/an/the), filler (just/really/basically), pleasantries, hedging.
* Fragments OK. Short synonyms. Technical terms exact. Code unchanged.
* Pattern: [thing] [action] [reason]. [next step].
* Boundaries: Code blocks, commit messages, and security warnings must be written in normal English.

# 3D Boat Simulation — USV Obstacle Avoidance (RL)

Proyecto de Reinforcement Learning para evasión de obstáculos con un Vehículo de Superficie No Tripulado (USV / bote autónomo). El repositorio está dividido en dos partes independientes que se comunican entre sí durante el entrenamiento y la inferencia:

- **`frontend/`** — Simulación 3D en **Unity**. Contiene la escena del bote, el agua, los obstáculos y los sensores simulados (cámaras, LIDAR, etc.). Actúa como el "entorno visual" y, cuando corresponde, expone el estado del bote y recibe acciones vía un puente de comunicación (p. ej. sockets/ML-Agents).
- **`backend/`** — Scripts de entrenamiento RL en **Python**, usando **PyTorch** y **Stable-Baselines3**. Contiene la definición del entorno (Gym/Gymnasium), el agente, la configuración de entrenamiento y los checkpoints de modelos entrenados.

## Estructura del repositorio

```
.
├── frontend/               # Proyecto Unity (simulación 3D)
├── backend/                # Entrenamiento RL en Python
│   ├── agent.py            # Definición y configuración del agente (política, hiperparámetros)
│   ├── environment.py      # Entorno Gym/Gymnasium que envuelve la simulación del bote
│   ├── requirements.txt    # Dependencias de Python
│   └── models/             # Checkpoints y modelos entrenados (.pt, .zip)
├── CLAUDE.md
└── .gitignore
```

## Guías de estilo

### Python (backend/)
- Seguir **PEP 8**. Usar type hints en firmas de funciones públicas.
- Nombrar archivos y funciones en `snake_case`; clases en `PascalCase`.
- Un módulo por responsabilidad: `environment.py` solo define el entorno (step, reset, reward shaping, observation/action space); `agent.py` solo define la construcción y carga del modelo/política.
- No hardcodear rutas absolutas ni hiperparámetros mágicos dentro de la lógica — centralizarlos en constantes o un archivo de configuración.
- Preferir `gymnasium` como interfaz de entorno (compatible con Stable-Baselines3 >= 2.0).

### Unity / C# (frontend/)
- Seguir las convenciones estándar de C#: `PascalCase` para clases, métodos y propiedades públicas; `camelCase` para variables privadas y campos serializados.
- Un `MonoBehaviour` por responsabilidad (control del bote, generación de obstáculos, sensores, puente de comunicación con Python).
- Mantener la lógica de física del bote separada de la lógica de recompensa/entrenamiento (esa vive en el backend).

### General
- Commits en tiempo presente y descriptivos (p. ej. "Add LIDAR sensor bridge", no "Added stuff").
- No commitear modelos entrenados grandes ni artefactos de compilación de Unity (ver `.gitignore`).

## Comandos principales

### Backend (entrenamiento RL)
```bash
# Crear entorno virtual e instalar dependencias
cd backend
python -m venv .venv
.venv\Scripts\activate        # Windows
pip install -r requirements.txt

# Entrenar el agente
python agent.py --train

# Evaluar un modelo entrenado
python agent.py --eval --model models/<nombre_del_modelo>.zip
```

### Frontend (simulación Unity)
- Abrir la carpeta `frontend/` como proyecto desde **Unity Hub**.
- Ejecutar la escena principal en modo Play para validar la simulación de forma manual.
- Si se usa ML-Agents, generar el build headless para entrenamiento con:
```bash
# Ejemplo, ajustar según el build target real
Unity -batchmode -nographics -projectPath frontend -executeMethod BuildScript.PerformBuild -quit
```

## Notas para contribuciones futuras
- Cualquier cambio en el espacio de observación/acción de `environment.py` debe reflejarse también en el lado de Unity (frontend) para mantener la compatibilidad del protocolo de comunicación.
- Los modelos entrenados (`backend/models/*.pt`, `*.zip`) no se versionan en git; documentar en el PR/commit dónde se almacenan si son necesarios para reproducibilidad (p. ej. almacenamiento externo).
