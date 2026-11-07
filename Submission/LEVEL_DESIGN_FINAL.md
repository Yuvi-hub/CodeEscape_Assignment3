# Assignment 3 — Code Escape
## Part A: Level Design

### 1. Game overview and intended audience

**Game:** Code Escape  
**Genre:** Top-down, grid-based puzzle maze  
**Intended audience:** Beginner programming students aged 12 and older, and casual puzzle players who enjoy short, progressively more challenging levels.  
**Platform considerations:** Keyboard controls on desktop and directional touch controls on mobile.

In Code Escape, the player explores a maze to gather all the **code tokens** required to unlock an exit. Each token symbolizes a missing program instruction. Completing the collection represents assembling a complete program, while reaching the exit represents a successful compilation. The coding theme is a metaphor: the game does not ask players to type or execute real code. Its main gameplay skills are navigation, planning, recognizing rules, and adjusting strategies after mistakes.

The game uses a consistent core rule across every level: **collect all tokens before reaching the exit**. New obstacles are introduced gradually so players can build on knowledge from earlier stages rather than learning an entirely different game each time.

### 2. Common mechanics and diagram legend

- **Movement:** The player moves one grid cell at a time in four directions using WASD, arrow keys, or on-screen directional buttons.
- **Walls:** Impassable cells; players must navigate around them.
- **Code tokens:** Collectible objects that disappear when picked up and increase the progress counter.
- **Exit:** Remains locked until all tokens in the current level have been collected. Entering the unlocked exit completes the level.
- **Hazards:** Introduced in later levels. Touching one costs one life and returns the player to the level's starting position. Collected tokens remain collected during a respawn within the same attempt.
- **Lives:** Levels 3 and 4 begin with three lives. Reaching zero lives triggers a Game Over screen and a level restart option.
- **Feedback:** A visible token counter, life counter where relevant, step counter, and clear messages for locked exits, successful collection, level completion, and failure.
- **Level progression:** Finishing a level unlocks the next level. Completing level 4 displays the final victory screen.

**Diagram legend:** `#` = wall, `.` = walkable floor, `P` = player starting cell, `C` = code token, `E` = exit, `H` = hazard. Each character represents one grid cell. These diagrams are top-down layouts, not screenshots.

### 3. Level 1 — Tutorial: First Command

**Difficulty:** Tutorial  
**Main objective:** Collect one code token and reach the exit.  
**New mechanic:** Basic movement, token collection, and the locked-exit rule.

The player starts in a small maze with a clearly visible code token between the starting position and the exit. The shortest route is simple, so the player can learn the controls without the pressure of a timer, enemy, or life loss. A few additional walkable cells allow safe experimentation with turning and exploring.

At the start, a short instruction explains the movement controls. The HUD shows `Tokens: 0/1`. When the player touches the token, it disappears, the counter becomes `1/1`, and the exit changes appearance and displays an unlocked message. Reaching the exit then displays a completion message and advances to level 2.

**Challenge and reason for its difficulty:** This is the least demanding level. It teaches one action at a time and ensures the player understands the win condition before more complicated layouts are introduced.

```text
#########
#P..C..E#
#.#.###.#
#.......#
#########
```

**Completion condition:** One token collected and the exit reached. **Failure condition:** None; players can explore safely.

### 4. Level 2 — Easy: Branching Paths

**Difficulty:** Easy  
**Main objective:** Find one code token and then navigate to the exit.  
**New mechanic:** Choosing routes, recognizing dead ends, and backtracking.

The maze is larger than the tutorial and introduces walls that interrupt obvious straight-line routes. Although the exit is visible near the upper-right part of the layout, the player must navigate the connected corridors and locate the code token elsewhere in the maze. This requires the player to make route choices rather than simply moving toward the exit.

The HUD continues to show the token counter and step counter. If the player reaches the exit without the token, a `Collect all code tokens first` message explains why the door remains locked. The player can safely return through the maze, collect the token, and try again. There are no hazards, so mistakes cost time rather than lives.

**Challenge and reason for its difficulty:** The basic controls and objective are unchanged from level 1, but the larger layout and branching corridors add spatial planning. The player must remember explored routes and correct unproductive decisions.

```text
###########
#P..#...E.#
#.#.#.##..#
#.#...C...#
#...#.....#
###########
```

**Completion condition:** One token collected and the exit reached. **Failure condition:** None; dead ends only require backtracking.

### 5. Level 3 — Medium: Debug the Maze

**Difficulty:** Medium  
**Main objective:** Collect two code tokens, avoid a stationary hazard, and reach the exit.  
**New mechanics:** Multiple objectives, collision damage, limited lives, respawning, and Game Over.

This level increases the size and complexity of the maze. The two code tokens are in different parts of the layout, so the player must plan a route that visits both before approaching the exit. A stationary `bug` hazard occupies a walkable cell in the lower part of the maze. The player must observe its position and choose a safe path around it.

The HUD shows `Tokens: 0/2` and `Lives: 3` at the beginning. Collecting a token updates the counter. Touching the hazard removes one life and returns the player to the spawn cell; tokens already collected in that attempt stay collected, so the player can focus on the remaining objective. When lives reach zero, the Game Over screen offers a restart of the level with three lives and both tokens reset. The exit unlocks only when the token counter reaches `2/2`.

**Challenge and reason for its difficulty:** Unlike the first two levels, incorrect movement can now cause a meaningful setback. The player must balance collecting objectives with avoiding danger. The hazard is stationary so players can learn collision consequences without also having to predict enemy movement.

```text
#############
#P..#......E#
#.#.#.####..#
#.#...C..#..#
#...##.#....#
#..H...#C...#
#############
```

**Completion condition:** Both tokens collected and the exit reached while at least one life remains. **Failure condition:** All three lives lost.

### 6. Level 4 — Hard: Final Compilation

**Difficulty:** Hard  
**Main objective:** Collect three code tokens and reach the exit while avoiding moving hazards.  
**New mechanic:** Timing movement around patrol hazards while planning a multi-objective route.

This is the largest and most demanding level. Three code tokens are distributed across different sections of the maze, forcing the player to travel through multiple corridors and make deliberate decisions about the order of collection. The two `H` cells indicate hazards' **starting positions**. During play, each hazard patrols horizontally between available cells in its corridor, reverses direction at an obstacle, and moves at regular intervals. Their movement adds a timing challenge absent from earlier levels.

The player starts with three lives. A collision costs one life and returns the player to the starting position, preserving tokens collected during the current attempt. Players can wait on safe cells, observe the patrol rhythm, and cross a dangerous corridor when it is clear. The HUD shows `Tokens: 0/3`, remaining lives, and steps. After the third token is collected, the exit unlocks. Reaching it displays a final victory screen confirming that the game has been completed. Losing all lives displays Game Over and allows a restart of level 4.

**Challenge and reason for its difficulty:** This level combines every earlier mechanic: movement, navigation, backtracking, multiple tokens, collision avoidance, and limited lives. Moving hazards add timing and prediction, while three separated tokens increase route-planning complexity. Players must use the skills practiced in the earlier levels together.

```text
###############
#P..#.......E.#
#.#.#.#####.#.#
#.#...C...#...#
#...##.#H.#.#.#
#C.....#...#C.#
#...H......#..#
###############
```

**Completion condition:** All three tokens collected and the exit reached while at least one life remains. **Failure condition:** All three lives lost.

### 7. Difficulty progression summary

| Level | Difficulty | Tokens | Hazards | Main new challenge | Player learns |
|---|---|---:|---|---|---|
| 1 — First Command | Tutorial | 1 | None | Basic controls and unlocking the exit | Movement and core rules |
| 2 — Branching Paths | Easy | 1 | None | Branches and detours | Exploration and backtracking |
| 3 — Debug the Maze | Medium | 2 | One stationary hazard | Damage and limited lives | Safe route planning and risk management |
| 4 — Final Compilation | Hard | 3 | Two moving hazards | Timed crossings and multiple targets | Combining planning, timing, and avoidance |

The progression is intentionally incremental. Each stage preserves the familiar control scheme and token-to-exit objective while adding a new kind of difficulty: **understanding → navigation → risk → timing and combined skills**. This helps avoid overwhelming beginners with all mechanics at once.

### 8. Visual and accessibility considerations

Walls and floors should be visually distinct. Tokens should have a recognizable symbol, and hazards should use a clearly different shape or icon. The exit should visibly indicate locked and unlocked states, accompanied by text so the rule does not rely on color alone. HUD counters communicate progress at a glance. Mobile directional buttons should be large enough for comfortable tapping, and all levels should be playable without precise gestures or fast typing.

### 9. Design rationale and relationship to the target audience

The design is intended for beginners and casual players who benefit from short levels, simple controls, immediate feedback, and steadily increasing complexity. The maze format allows the player to understand the basic rules quickly, while the code-token metaphor provides a programming-themed setting. The game is not presented as a substitute for actual programming exercises; its educational value lies in practicing sequencing, goal tracking, planning, and responding to mistakes.

This audience and design direction should be compared with the market research produced for Assignment 1. No numerical survey results, player-testing outcomes, or specific competitor findings are asserted here because those findings have not been independently verified in this document.

### 10. Layout validation

The four diagrams have consistent row widths within each level. A four-direction grid reachability check from each player spawn confirms that every required code token and exit is reachable through non-wall cells. This validates the static layouts' basic connectivity; moving-hazard behavior, collision handling, and overall playability still require testing in Unity.
