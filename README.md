# Amanotes Playable Ads Case Study

## Technology References

| Technology                    | Version       |
| ----------------------------- | ------------- |
| Unity                         | `6000.0.83f1` |
| Unity Playworks / Luna Plugin | `7.2.0`       |
| Spine                         | `4.2`         |
| DOTween                       | `1.2.705`     |

---

## Project Structure

```text
Assets/
├── Scenes/
│   └── Main.unity
├── Scripts/
│   ├── View/
│   │   ├── Behaviour/
│   │   ├── Manager/
│   │   └── Menu/
│   ├── SO/
│   │   └── LevelSO.cs
│   └── Utility/
│       ├── Event/
│       │   └── GameEvent.cs
│       └── Editor/
│           └── LevelParseEditor.cs
├── Resources/
└── Plugins/
```

### Key Components

* **`Assets/Scenes/Main.unity`**
  The enabled build scene. It contains the gameplay setup, camera, UI, managers, cats, lanes, and environment.

* **`Assets/Scripts/View/Behaviour/`**
  Contains the main gameplay actors and world layout:
  `Cat`, `Spawner`, `Edible`, and `Environment`.

* **`Assets/Scripts/View/Manager/`**
  Contains the main managers:

    * `PlayableManager` — handles run state and Luna lifecycle calls.
    * `UIManager` — controls menu visibility.
    * `AudioManager` — handles music and sound effects.

* **`Assets/Scripts/View/Menu/`**
  Contains the intro, gameplay HUD, outro, and CTA screens.

* **`Assets/Scripts/Utility/Event/GameEvent.cs`**
  A static event hub used to communicate gameplay state, score, audio progress, and transitions.

* **`Assets/Scripts/SO/LevelSO.cs`**
  Defines level, note, and note-type data.

* **`Assets/Scripts/Utility/Editor/LevelParseEditor.cs`**
  Provides an Editor action that converts the assigned JSON `TextAsset` into note data.

* **`Assets/Resources/`**
  Contains level/audio assets, prefabs, and transition assets.

* **`Assets/Plugins/`**
  Contains the bundled DOTween and Spine integrations.

---

## Gameplay Flow

### 1. Start the Run

* The intro screen waits for the first touch or mouse click.
* The first input starts the gameplay.

### 2. Control the Cats

* Each cat reads input from its own half of the screen.
* Players can drag each cat horizontally.
* Each cat is clamped so it stays within its platform.

### 3. Spawn Level Items

* The spawner reads the level's note schedule.
* Notes/items are spawned according to their configured timing and lane.
* `LevelSO` maps each note variant to its prefab and score value.
* Lane `pid` `0–2` belongs to the left platform, while `pid` `3–5` belongs to the right platform.

### 4. Eat Items and Score

* When a cat overlaps an edible item, the cat plays its eating animation.
* The item is destroyed.
* Its configured score is sent through `GameEvent`.
* `PlayMenu` receives the event and updates the displayed score.

---

## Win Condition

* `AudioManager` waits for the configured delay.
* The level music starts playing.
* When the track finishes, the game signals a **win**.
* The win event stops the item spawner.
* The game performs a short transition to the outro screen.
* The CTA triggers Luna's full-game install API and playable end API.

---

## Lose Condition

* Items that pass their deadline accelerate toward the ground.
* When a missed item collides with the ground, the game signals a **loss**.
* The loss event stops the item spawner.
* The game performs a short transition to the outro screen.
* The CTA triggers Luna's full-game install API and playable end API.

---

## Architecture and Trade-offs

### Architecture

The playable uses **scene-serialized references** and a `LevelSO` asset to configure gameplay values. Persistent managers and the game environment use **singletons** for easy global access.

A small **static event hub** connects gameplay objects, the HUD, and managers without requiring direct references between them.

* **DOTween** handles UI and background animations.
* **Spine** handles cat animations.
* **ScriptableObject** is used for level and note configuration.
* **JSON** is used as the source format for the pre-authored level schedule.

### Trade-offs

This architecture keeps the single-scene playable simple to configure and iterate on. The trade-off is greater reliance on **global singleton/event state** and **scene setup**, which would be less suitable for a larger multi-level game.

The level schedule is **pre-authored as JSON** and converted in the Unity Editor instead of being loaded and validated through a runtime content pipeline.

Notes are also **instantiated and destroyed directly**, while each cat handles its own input without a separate input abstraction.

These choices are intentional for a **short, fixed playable**. They reduce setup and development overhead, but provide less **reusability, automated validation, and scalability** for larger levels or broader device support.

---

## Improvement Ideas

### High Priority

#### 1. Use a State Machine for Game Flow

Replace the current event-driven handling of the start, win, and lose conditions with a **State Machine**.
**Benefits:**

* Makes the game flow more explicit.
* Centralizes state transitions.
* Makes the gameplay lifecycle easier to debug.
* Provides a cleaner foundation for adding more states later.

---

#### 2. Improve UI, VFX, and Game Feel

Polish the UI and visual effects to make the playable feel more responsive and engaging.

Focus on:

* Stronger **score feedback** when an item is eaten.
* Clearer **input feedback** when the cat successfully interacts with an edible.
* More responsive animations and visual reactions.
* Additional VFX and feedback to reinforce successful actions.

**Benefits:**

* Makes player actions easier to understand.
* Improves responsiveness and perceived game quality.
* Makes successful interactions feel more satisfying.

---

### Medium Priority

#### 3. Improve Background Animation

Instead of using a static background with frozen lanterns, use an empty static background and a **Particle System** to spawn lanterns that move from the bottom of the screen to the top.

Add **fireworks VFX** during the win state.

**Benefits:**

* Adds more motion and visual energy to the scene.
* Creates a more dynamic background without manually animating each lantern.
* Makes the win state feel more rewarding.
* Keeps the visual effects relatively lightweight and reusable.

---

#### 4. Separate Win and Lose Outro Backgrounds

Use different background visuals for the **win** and **lose** outro states.

**Benefits:**

* Makes the outcome immediately recognizable.
* Gives each result a distinct visual identity.
* Provides clearer visual communication to the player.
* Allows the win and lose states to have different visual moods.
