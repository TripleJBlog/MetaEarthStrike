# Meta Earth Strike: War of Factions

This Unity project uses a singleton pattern for core managers such as EconomyManager and GameManager. Scripts should access these managers via `EconomyManager.Instance` or `GameManager.Instance` for global access.

## 🎮 Game Overview

Meta Earth Strike is a real-time strategy game where players control a faction, manage resources, summon heroes, and upgrade armies to conquer enemy base. The game is optimized for mobile sessions (~10-15 minutes per match).

## ✨ Core Features

### ⚔️ Single Lane Combat
- **One strategic lane** for focused 1v1 battles
- **Auto-spawning units** that march toward enemy base
- **Simplified waypoint system** for efficient pathfinding
- **Real-time combat** with automatic targeting

### 🏰 Faction System
- **3 unique factions**: Alliance, Horde, Scourge
- **Faction-specific units** with unique stats and abilities
- **Distinct visual themes** and upgrade paths
- **Hero units** with faction-specific abilities

### 💰 Economy & Upgrade System
- **Passive income** every second
- **5 upgrade categories**:
  - Income Boost (increases gold per second)
  - Melee Units (upgrades melee unit tiers)
  - Ranged Units (upgrades ranged unit tiers)
  - Siege Units (upgrades siege unit tiers)
  - Base Defense (increases base health)

### 🦸 Hero System
- **One controllable hero** per match
- **4 active abilities** with cooldowns and mana costs
- **Leveling system** with experience from kills
- **Faction-specific hero** with unique stats

### 🧠 AI Opponent
- **Dynamic AI strategies** that adapt to game state
- **5 AI strategies**: Aggressive, Defensive, Balanced, Economic, Rush
- **Intelligent decision making** based on match progression
- **Lane targeting** and hero prioritization

### 📱 Mobile-First Design
- **Touch-optimized controls** for mobile devices
- **Large touch-friendly buttons** for upgrades and abilities
- **Real-time status displays** (gold, health, timer)
- **Pause and speed controls** (1x/2x)
- **Responsive design** for various screen sizes
- **Touch pan and pinch zoom** for camera control

## 🛠️ Technical Architecture

### Core Systems
- **GameManager**: Central game state and coordination
- **LaneManager**: Single lane management and unit spawning
- **Unit**: Individual unit behavior, combat, and movement
- **EconomyManager**: Resource management and upgrade system
- **HeroManager**: Hero control, abilities, and leveling
- **UIManager**: Mobile-optimized user interface
- **ObjectPool**: Performance optimization for unit spawning
- **CameraController**: 2.5D isometric camera with touch controls
- **EnemyAI**: AI opponent with dynamic strategies

### Data Management
- **ScriptableObjects** for unit and faction data
- **JSON configuration** for balance adjustments
- **Modular upgrade system** with scaling costs

### Performance Features
- **Object pooling** for efficient unit management
- **Simplified pathfinding** using waypoints
- **Optimized rendering** for mobile devices
- **Memory management** for long gaming sessions

## 🚀 Getting Started

### Prerequisites
- Unity 2022.3 LTS or newer
- Universal Render Pipeline (URP)
- TextMesh Pro package

### Installation
1. Clone the repository
2. Open the project in Unity
3. Install required packages if prompted
4. Open the `SampleScene` in `Assets/Scenes/`
5. Press Play to start the game


### Mobile Controls
- **Touch**: Tap to move hero, select units
- **Touch Pan**: Drag to move camera
- **Pinch**: Zoom in/out
- **Double Tap**: Toggle hero following
- **UI Buttons**: Use hero abilities and upgrades
- **Pause Button**: Pause/unpause game
- **Speed Button**: Toggle between 1x and 2x speed

## 📁 Project Structure

```
Assets/
├── Scripts/
│   ├── Core/           # Main game systems
│   ├── Data/           # ScriptableObjects and data
│   ├── UI/             # User interface components
│   └── AI/             # Artificial intelligence
├── Scenes/             # Game scenes
├── Prefabs/            # Reusable game objects
├── Materials/          # Visual materials
└── Settings/           # Unity project settings
```

## 🎯 Game Mechanics

### Victory Conditions
- **Destroy enemy base** (reduce health to 0)
- **Survive until time limit** (15 minutes)
- **Defeat enemy hero** (optional)

### Unit Types
- **Melee**: Close combat, high health, low range
- **Ranged**: Medium range, balanced stats
- **Siege**: Long range, high damage, low health

### Upgrade System
- **5 levels per upgrade** with increasing costs
- **Scaling effects** based on upgrade level
- **Strategic choices** between economy and combat

### Hero Abilities
- **Basic Attack**: Quick damage to nearest enemy
- **Heal**: Restore health to self or allies
- **Power Strike**: Heavy damage to single target
- **Battle Cry**: Buff nearby allies

## 🔧 Development

### Adding New Units
1. Create a new `UnitData` ScriptableObject
2. Configure stats, visuals, and scaling
3. Add to faction's unit arrays
4. Update spawning logic in `LaneManager`

### Adding New Factions
1. Create a new `FactionData` ScriptableObject
2. Configure units, hero, and visual theme
3. Add to `FactionType` enum
4. Update faction selection UI

### Balancing
- Modify values in ScriptableObjects
- Adjust upgrade costs and scaling
- Fine-tune AI decision weights
- Test with different strategies

## 🎨 Art Style

- **2.5D isometric view** for strategic overview
- **Stylized fantasy aesthetic** inspired by Warcraft
- **Sprite-based units** with animations
- **Color-coded factions** for easy identification
- **Mobile-optimized** visual design

## 📱 Mobile Optimization

- **Touch-friendly UI** with large buttons
- **Optimized performance** for mobile devices
- **Battery-efficient** rendering
- **Responsive design** for various screen sizes
- **Offline play** with AI opponent

## 🔮 Future Features

### Phase 2 (Stretch Goals)
- **Real-time multiplayer** using Photon Fusion
- **Multiple maps** with different layouts
- **Ranked matchmaking** system
- **Cosmetic skins** and customization
- **Tournament mode** with brackets

### Additional Content
- **More factions** with unique mechanics
- **Special events** and seasonal content
- **Achievement system** and progression
- **Replay system** for match analysis
- **Spectator mode** for tournaments

## 📄 License

2025 copryright MetaEarth Team

## 🙏 Acknowledgments

- Inspired by Warcraft III custom maps
- Built with Unity game engine
- Uses Universal Render Pipeline
- TextMesh Pro for UI text rendering

---

**Meta Earth Strike** - Where strategy meets action in the ultimate faction war! 