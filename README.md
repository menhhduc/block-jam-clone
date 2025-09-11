# Block Jam 2D

## Acknowledgement
This project was created by **Nguyen Le Minh Duc**\
Special thanks to the SuperNZT team for supervise me in the making of this project.

## About
- **Unity Version**: 6000.0.40f1
- **IDE**: JetBrains Rider 2025.1.5

## Descriptions
*Block Jam* is a mobile-based puzzle game that emphasizes pattern recognition and strategic decision-making. The core gameplay mechanic requires players to identify and select groups of three tiles of the same color. Each level is structured into three to four sequential stages, with a progressively increasing level of difficulty. Players interact with the game by selecting tiles to form valid groups, after which new tiles are introduced to continue play. The game features seven designated finish slots; if all seven slots are occupied and no valid color groupings remain, the game session terminates.

## Methods
**1. A-star Pathfinding for Block Navigation**
- **Purpose**: Enable precise block movement and adaptive grid updates.
- **Implementation**:
  - The A Pathfinding Project* calculates optimal paths for block repositioning after matches are cleared.
  - Dynamic node updates ensure valid movement routes, even as grid configurations change in real time.
  - A* reduces manual path logic complexity, supporting scalability and adaptive level design.

**2. Puzzle Grid and Matching Algorithm**
- **Purpose**: Deliver core gameplay functionality through efficient color-matching logic.
- **Implementation**:
  - The board is structured as a grid-based slot system managed by a central GridManager.
  - Players select and group three or more identical-color blocks to clear them from the grid.
  - A recursive matching algorithm validates and clears block groups, then triggers repositioning through A* calculations.
  - A loss condition is triggered if all seven finish slots are filled with no valid matches.

**3. Level Progression**
- **Purpose**: Provide a balanced difficulty curve across multiple stages.
- **Implementation**:
  - Levels are divided into 3–4 stages with increasing complexity via additional colors, strategic block placement, and reduced available matches.
  - Modular level data supports seamless content scaling and rapid iteration.

**4. Tween-Based Animations (DOTween Integration)**
- **Purpose**: Enhance player experience with fluid transitions and feedback.
- **Implementation**:
  - DOTween animates block movement, spawning, and removal, synced with A*-calculated paths.
  - Lightweight animation sequences maintain smooth performance on mobile devices.

**5. Game Flow Management**
- **Purpose**: Ensure consistent gameplay structure and streamlined progression.
- **Implementation**:
  - `GameManager` and `GridManager` oversee level states, scoring, stage transitions, and overall game flow.
  - Modular architecture simplifies debugging and future gameplay mechanic additions.
