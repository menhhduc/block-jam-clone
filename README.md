# Block Jam 2D

## About
This is the clone of the game **Block Jam 3D**

- **Unity Version**: 6000.0.40f1
- **IDE**: JetBrains Rider 2025.1.5

## Descriptions
_Block Jam_ is a mobile-based puzzle game that emphasizes pattern recognition and strategic decision-making. The core gameplay mechanic requires players to identify and select groups of three tiles of the same color. Each level is structured into three to four sequential stages, with a progressively increasing level of difficulty. Players interact with the game by selecting tiles to form valid groups, after which new tiles are introduced to continue play. The game features seven designated finish slots; if all seven slots are occupied and no valid color groupings remain, the game session terminates.

## Utils
- **DOTween**
- **NaughtyAttributes**

## Methods
Main method: **A-star pathfinding**
- The development of the game was primarily conducted in Unity, utilizing the A* pathfinding to manage tile selection. The game board was represented as a grid-based graph, where each tile is modeled as a node, and connections between adjacent tiles are defined as edges. This structure enabled efficient computation of valid tile groupings and movement paths.

