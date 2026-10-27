# StealMan — Level Blockout Technical Reference

> **Source of Truth**: Approved Level Design Specification PDFs (`LEVEL_SPECS/L01` through `L15`).
> **Target Engine**: Unity 6 (6000.5.2f1) | **Scene Directory**: `Assets/Scenes/LevelBlockoutReference/`
> **Generated Date**: 2026-10-14 17:33:06

---

## 1. Overview & Coordinate Standards

- **Right-Handed Standard Coordinates**: `+X` = Right, `+Y` = Up, `+Z` = Forward Progression.
- **Player Spawn Datum**: Player spawns at ground level `(0, 0, Z_start)` facing forward `+Z` (or documented offset).
- **Leash Mechanics**: Hard clamp at `25.0m` Euclidean distance from Player body to Shadow.
- **Data Sourcing Key**:
  - `DOCUMENTED`: Explicit measurement or coordinate stated in PDF specification text.
  - `DERIVED_FROM_DIAGRAM`: Scaled from top-down ASCII/schematic floorplan diagrams.
  - `IMPLEMENTATION_ASSUMPTION`: Standardized technical implementation detail (e.g. wall thickness `0.2m`, label offset).

---

## L01 — The Corridor

| Metric | Specification |
| :--- | :--- |
| **Phase** | Phase 1 (Teach) |
| **Difficulty** | 0 / 10 |
| **Play Space Dimensions** | 5m x 15m |
| **Target Time / Hard Limit** | 5s / 10s |
| **Max Required Leash / Limit** | 0m / 25m |
| **Stealable Objects** | 0 |
| **Hazard Volumes** | 0 |

### Design Objective
> Walk to the exit zone. Teaches fundamental movement without introducing Shadow.

### Traversal Routes & Metrics
- **Player Route**: Walk straight up the corridor to the exit pad.
- **Shadow Route**: N/A (Shadow mechanic not active in Level 1).

### Gameplay Sequence
1. Player moves from spawn (0, 0, 1.5) straight up the corridor.
2. Player enters Exit Zone at (0, 0, 14.0) to complete the level.

### Spatial Inventory & Placed Objects

| Object Name | Category | Position (X, Y, Z) | Rotation | Scale / Size | Value Source | Notes |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `Floor_Main` | Geometry | `(0.00, 0.00, 7.50)` | `(0, 0, 0)` | `(5.00, 0.20, 15.00)` | `DOCUMENTED` | Main corridor floor (5m x 15m) |
| `Wall_Left` | Geometry | `(-2.60, 1.50, 7.50)` | `(0, 0, 0)` | `(0.20, 3.00, 15.00)` | `DOCUMENTED` | Left corridor wall (height 3m) |
| `Wall_Right` | Geometry | `(2.60, 1.50, 7.50)` | `(0, 0, 0)` | `(0.20, 3.00, 15.00)` | `DOCUMENTED` | Right corridor wall (height 3m) |
| `Wall_Back` | Geometry | `(0.00, 1.50, -0.10)` | `(0, 0, 0)` | `(5.40, 3.00, 0.20)` | `DOCUMENTED` | Back boundary wall |
| `Wall_Front_L` | Geometry | `(-1.75, 1.50, 15.10)` | `(0, 0, 0)` | `(1.50, 3.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Exit threshold left wall |
| `Wall_Front_R` | Geometry | `(1.75, 1.50, 15.10)` | `(0, 0, 0)` | `(1.50, 3.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Exit threshold right wall |
| `Player` | Player | `(-1.00, 0.10, 1.50)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Player spawn point ground level |
| `Shadow_StartMarker` | Shadow | `(-1.00, 0.10, 1.50)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DERIVED_FROM_DIAGRAM` | Attached to Player at start |
| `ExitZone` | Exit | `(0.00, 0.10, 14.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Level completion exit trigger zone |
| `Label_Title` | Label | `(0.00, 3.50, 2.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: L01: The Corridor 5m x 15m | Movement Tutorial |
| `Label_Spawn` | Label | `(-1.00, 2.00, 1.50)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Player Spawn |
| `Label_Exit` | Label | `(0.00, 2.00, 14.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Exit Zone (Walk Here) |

### Implementation Notes
Basic tutorial corridor. No hazards, no stealables.

---

## L02 — The Sandbox

| Metric | Specification |
| :--- | :--- |
| **Phase** | Phase 1 (Teach) |
| **Difficulty** | 0 / 10 |
| **Play Space Dimensions** | 10m x 20m |
| **Target Time / Hard Limit** | 10s / 15s |
| **Max Required Leash / Limit** | 8m / 25m |
| **Stealable Objects** | 0 |
| **Hazard Volumes** | 0 |

### Design Objective
> Teach Attached -> Detached -> Recall loop in a safe, unforced environment.

### Traversal Routes & Metrics
- **Player Route**: Walk to center, detach shadow to explore, recall, walk to exit.
- **Shadow Route**: Move freely around room when detached.

### Gameplay Sequence
1. Player spawns at (0, 0, 2.0).
2. Press Detach to test independent shadow movement.
3. Press Recall to snap shadow back to body.
4. Walk through open room to Exit at (0, 0, 18.5).

### Spatial Inventory & Placed Objects

| Object Name | Category | Position (X, Y, Z) | Rotation | Scale / Size | Value Source | Notes |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `Floor_Main` | Geometry | `(0.00, 0.00, 10.00)` | `(0, 0, 0)` | `(10.00, 0.20, 20.00)` | `DOCUMENTED` | Sandbox floor (10m x 20m) |
| `Wall_Left` | Geometry | `(-5.10, 1.50, 10.00)` | `(0, 0, 0)` | `(0.20, 3.00, 20.00)` | `DOCUMENTED` | Left wall (10m width) |
| `Wall_Right` | Geometry | `(5.10, 1.50, 10.00)` | `(0, 0, 0)` | `(0.20, 3.00, 20.00)` | `DOCUMENTED` | Right wall |
| `Wall_Back` | Geometry | `(0.00, 1.50, -0.10)` | `(0, 0, 0)` | `(10.40, 3.00, 0.20)` | `DOCUMENTED` | Back boundary wall |
| `Wall_Front_L` | Geometry | `(-3.00, 1.50, 20.10)` | `(0, 0, 0)` | `(4.00, 3.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Front left boundary |
| `Wall_Front_R` | Geometry | `(3.00, 1.50, 20.10)` | `(0, 0, 0)` | `(4.00, 3.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Front right boundary |
| `Player` | Player | `(0.00, 0.10, 2.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Player spawn point ground level |
| `Shadow_StartMarker` | Shadow | `(0.00, 0.10, 2.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DERIVED_FROM_DIAGRAM` | Attached to Player at start |
| `ExitZone` | Exit | `(0.00, 0.10, 18.50)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Level completion exit trigger zone |
| `Label_Title` | Label | `(0.00, 3.50, 2.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: L02: The Sandbox 10m x 20m | Detach & Recall Practice |
| `Label_Spawn` | Label | `(0.00, 2.00, 2.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Player Spawn |
| `Label_Sandbox` | Label | `(0.00, 1.00, 10.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Safe Exploration Space (Test Detach & Recall) |
| `Label_Exit` | Label | `(0.00, 2.00, 18.50)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Exit Zone |

### Implementation Notes
Safe playground proving Recall is a state transition, not a puzzle solver.

---

## L03 — Ghost Gate

| Metric | Specification |
| :--- | :--- |
| **Phase** | Phase 1 (Teach) |
| **Difficulty** | 2 / 10 |
| **Play Space Dimensions** | 5m x 15m |
| **Target Time / Hard Limit** | 15s / 20s |
| **Max Required Leash / Limit** | 5.5m / 25m |
| **Stealable Objects** | 1 |
| **Hazard Volumes** | 0 |

### Design Objective
> Introduce the STEAL mechanic: stealing an object's shadow makes it intangible (Ghost state).

### Traversal Routes & Metrics
- **Player Route**: Walk to door -> Wait -> Walk through Ghost Door -> Exit.
- **Shadow Route**: Detach -> Move to Door -> Steal -> Recall.

### Gameplay Sequence
1. Player approaches door at Z=5.0m.
2. Detach shadow.
3. Move shadow to Stealable Door at (0, 1.25, 7.5).
4. Press STEAL. Door becomes Ghost state.
5. Press Recall to reunite.
6. Player walks through intangible door to Exit at (0, 0, 13.5).

### Spatial Inventory & Placed Objects

| Object Name | Category | Position (X, Y, Z) | Rotation | Scale / Size | Value Source | Notes |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `Floor_Main` | Geometry | `(0.00, 0.00, 7.50)` | `(0, 0, 0)` | `(5.00, 0.20, 15.00)` | `DOCUMENTED` | Main floor (5m x 15m) |
| `Wall_Left` | Geometry | `(-2.60, 1.50, 7.50)` | `(0, 0, 0)` | `(0.20, 3.00, 15.00)` | `DOCUMENTED` | Left wall |
| `Wall_Right` | Geometry | `(2.60, 1.50, 7.50)` | `(0, 0, 0)` | `(0.20, 3.00, 15.00)` | `DOCUMENTED` | Right wall |
| `Wall_Back` | Geometry | `(0.00, 1.50, -0.10)` | `(0, 0, 0)` | `(5.40, 3.00, 0.20)` | `DOCUMENTED` | Back wall |
| `Pillar_Left` | Geometry | `(-1.75, 1.50, 7.50)` | `(0, 0, 0)` | `(1.50, 3.00, 0.40)` | `DERIVED_FROM_DIAGRAM` | Door frame left pillar |
| `Pillar_Right` | Geometry | `(1.75, 1.50, 7.50)` | `(0, 0, 0)` | `(1.50, 3.00, 0.40)` | `DERIVED_FROM_DIAGRAM` | Door frame right pillar |
| `Wall_Front_L` | Geometry | `(-1.75, 1.50, 15.10)` | `(0, 0, 0)` | `(1.50, 3.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Exit wall left |
| `Wall_Front_R` | Geometry | `(1.75, 1.50, 15.10)` | `(0, 0, 0)` | `(1.50, 3.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Exit wall right |
| `Player` | Player | `(0.00, 0.10, 2.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Player spawn point ground level |
| `Shadow_StartMarker` | Shadow | `(0.00, 0.10, 2.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DERIVED_FROM_DIAGRAM` | Attached to Player at start |
| `StealableDoor` | Stealable | `(0.00, 1.25, 7.50)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Stealable Door blocking hallway |
| `ExitZone` | Exit | `(0.00, 0.10, 13.50)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Level completion exit trigger zone |
| `Label_Title` | Label | `(0.00, 3.50, 2.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: L03: Ghost Gate 5m x 15m | Introduce STEAL Mechanic |
| `Label_Door` | Label | `(0.00, 3.00, 7.50)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Stealable Door (Solid -> Ghost Intangible) |
| `Label_Exit` | Label | `(0.00, 2.00, 13.50)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Exit Zone |

### Implementation Notes
First encounter with a Stealable obstacle.

---

## L04 — The Vent

| Metric | Specification |
| :--- | :--- |
| **Phase** | Phase 1 (Teach) |
| **Difficulty** | 3 / 10 |
| **Play Space Dimensions** | 10m x 25m |
| **Target Time / Hard Limit** | 20s / 25s |
| **Max Required Leash / Limit** | 10m / 25m |
| **Stealable Objects** | 1 |
| **Hazard Volumes** | 0 |

### Design Objective
> Teach Floor -> Wall Traversal and Stealable Drawbridge to cross a pit.

### Traversal Routes & Metrics
- **Player Route**: Walk to pit edge (Z=8.5m) -> Wait -> Cross lowered bridge -> Exit.
- **Shadow Route**: Detach -> Move to Right Wall -> Climb up wall around 6m pit -> Steal Drawbridge -> Recall.

### Gameplay Sequence
1. Player stops at edge of 6m pit (Z=9m to 15m).
2. Detach shadow.
3. Transition shadow onto right vertical wall (height 5m).
4. Walk shadow along wall across pit to Drawbridge mount.
5. Press STEAL. Drawbridge lowers.
6. Recall. Player crosses bridge to Exit at (0, 0, 22.0).

### Spatial Inventory & Placed Objects

| Object Name | Category | Position (X, Y, Z) | Rotation | Scale / Size | Value Source | Notes |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `Floor_Start` | Geometry | `(0.00, 0.00, 4.50)` | `(0, 0, 0)` | `(10.00, 0.20, 9.00)` | `DOCUMENTED` | Start floor section (Z: 0 to 9m) |
| `Pit_Void` | Geometry | `(0.00, -5.00, 12.00)` | `(0, 0, 0)` | `(10.00, 0.20, 6.00)` | `DOCUMENTED` | Pit gap (6m wide chasm, Z: 9 to 15m) |
| `Floor_Exit` | Geometry | `(0.00, 0.00, 20.00)` | `(0, 0, 0)` | `(10.00, 0.20, 10.00)` | `DOCUMENTED` | Exit floor section (Z: 15 to 25m) |
| `Wall_Left` | Geometry | `(-5.10, 2.50, 12.50)` | `(0, 0, 0)` | `(0.20, 5.00, 25.00)` | `DOCUMENTED` | Left wall (height 5m) |
| `Wall_Right_Climbable` | Geometry | `(5.10, 2.50, 12.50)` | `(0, 0, 0)` | `(0.20, 5.00, 25.00)` | `DOCUMENTED` | Right climbable wall (height 5m, shadow traversal route) |
| `Wall_Back` | Geometry | `(0.00, 2.50, -0.10)` | `(0, 0, 0)` | `(10.40, 5.00, 0.20)` | `DOCUMENTED` | Back wall |
| `Wall_Front_L` | Geometry | `(-3.00, 2.50, 25.10)` | `(0, 0, 0)` | `(4.00, 5.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Front exit wall left |
| `Wall_Front_R` | Geometry | `(3.00, 2.50, 25.10)` | `(0, 0, 0)` | `(4.00, 5.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Front exit wall right |
| `Player` | Player | `(0.00, 0.10, 2.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Player spawn point ground level |
| `Shadow_StartMarker` | Shadow | `(0.00, 0.10, 2.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DERIVED_FROM_DIAGRAM` | Attached to Player at start |
| `StealableDrawbridge` | Stealable | `(0.00, 0.50, 12.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Raised Drawbridge spanning 6m pit |
| `ExitZone` | Exit | `(0.00, 0.10, 22.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Level completion exit trigger zone |
| `Label_Title` | Label | `(0.00, 4.00, 2.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: L04: The Vent 10m x 25m | Wall Traversal & Drawbridge |
| `Label_Pit` | Label | `(0.00, -0.50, 12.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: 6m Pit Gap (Player Blocked) |
| `Label_WallClimb` | Label | `(4.80, 3.50, 12.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Right Wall Climbable Surface (Shadow Traversal Path) |
| `Label_Bridge` | Label | `(0.00, 2.50, 12.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Stealable Drawbridge (Raised -> Lowered) |
| `Label_Exit` | Label | `(0.00, 2.00, 22.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Exit Zone |

### Implementation Notes
First vertical wall climbing interaction. Raised drawbridge lowers when stolen.

---

## L05 — The Spotlight

| Metric | Specification |
| :--- | :--- |
| **Phase** | Phase 1 (Teach) |
| **Difficulty** | 4 / 10 |
| **Play Space Dimensions** | 10m x 20m |
| **Target Time / Hard Limit** | 25s / 30s |
| **Max Required Leash / Limit** | 12m / 25m |
| **Stealable Objects** | 1 |
| **Hazard Volumes** | 1 |

### Design Objective
> Introduce Danger Light hazard and OnShadowDestroyed failure state.

### Traversal Routes & Metrics
- **Player Route**: Wait at safe distance -> Walk through Ghost Door to Exit.
- **Shadow Route**: Detach -> Observe sweep -> Dash past light when clear -> Steal Door -> Recall.

### Gameplay Sequence
1. Player stops at Z=6.0m safe from light cone.
2. Detach shadow.
3. Time the sweeping light ping-pong cycle.
4. When light sweeps to opposite side, dash through zone.
5. Steal Door at (0, 1.25, 14.0).
6. Recall and walk to Exit at (0, 0, 18.0).

### Spatial Inventory & Placed Objects

| Object Name | Category | Position (X, Y, Z) | Rotation | Scale / Size | Value Source | Notes |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `Floor_Main` | Geometry | `(0.00, 0.00, 10.00)` | `(0, 0, 0)` | `(10.00, 0.20, 20.00)` | `DOCUMENTED` | Hall floor (10m x 20m) |
| `Wall_Left` | Geometry | `(-5.10, 1.50, 10.00)` | `(0, 0, 0)` | `(0.20, 3.00, 20.00)` | `DOCUMENTED` | Left wall |
| `Wall_Right` | Geometry | `(5.10, 1.50, 10.00)` | `(0, 0, 0)` | `(0.20, 3.00, 20.00)` | `DOCUMENTED` | Right wall |
| `Wall_Back` | Geometry | `(0.00, 1.50, -0.10)` | `(0, 0, 0)` | `(10.40, 3.00, 0.20)` | `DOCUMENTED` | Back wall |
| `Pillar_Left` | Geometry | `(-3.25, 1.50, 14.00)` | `(0, 0, 0)` | `(3.50, 3.00, 0.40)` | `DERIVED_FROM_DIAGRAM` | Door pillar left |
| `Pillar_Right` | Geometry | `(3.25, 1.50, 14.00)` | `(0, 0, 0)` | `(3.50, 3.00, 0.40)` | `DERIVED_FROM_DIAGRAM` | Door pillar right |
| `Wall_Front_L` | Geometry | `(-3.00, 1.50, 20.10)` | `(0, 0, 0)` | `(4.00, 3.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Front wall left |
| `Wall_Front_R` | Geometry | `(3.00, 1.50, 20.10)` | `(0, 0, 0)` | `(4.00, 3.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Front wall right |
| `Player` | Player | `(0.00, 0.10, 2.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Player spawn point ground level |
| `Shadow_StartMarker` | Shadow | `(0.00, 0.10, 2.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DERIVED_FROM_DIAGRAM` | Attached to Player at start |
| `StealableDoor` | Stealable | `(0.00, 1.25, 14.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Stealable Door guarded by sweeping spotlight |
| `DangerLight_Sweeping` | Hazard | `(0.00, 0.50, 10.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Sweeping Danger Light (cone sweeps +/-40 deg) |
| `ExitZone` | Exit | `(0.00, 0.10, 18.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Level completion exit trigger zone |
| `Label_Title` | Label | `(0.00, 3.50, 2.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: L05: The Spotlight 10m x 20m | Danger Light Hazard |
| `Label_Hazard` | Label | `(0.00, 3.00, 10.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Sweeping Danger Light (Lethal to Shadow - Time the Dash) |
| `Label_Door` | Label | `(0.00, 3.00, 14.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Stealable Door |
| `Label_Exit` | Label | `(0.00, 2.00, 18.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Exit Zone |

### Implementation Notes
First lethal hazard. Touching light destroys shadow and resets level.

---

## L06 — Double Lock

| Metric | Specification |
| :--- | :--- |
| **Phase** | Phase 2 (Combine) |
| **Difficulty** | 4 / 10 |
| **Play Space Dimensions** | 10m x 25m |
| **Target Time / Hard Limit** | 25s / 30s |
| **Max Required Leash / Limit** | 7m / 25m |
| **Stealable Objects** | 2 |
| **Hazard Volumes** | 0 |

### Design Objective
> Two sequential Stealable Doors proving body must reposition into intermediate safe zone.

### Traversal Routes & Metrics
- **Player Route**: Walk to Door A -> Wait -> Walk through A to Safe Zone -> Wait -> Walk through B to Exit.
- **Shadow Route**: Sub-route 1: Steal Door A -> Recall. Sub-route 2: Steal Door B -> Recall.

### Gameplay Sequence
1. Player stops at Z=4m.
2. Detach shadow -> Steal Door A at (0, 1.25, 7.0) -> Recall.
3. Player walks through Ghost Door A into Safe Zone (Z=8m to 14m).
4. Detach shadow -> Steal Door B at (0, 1.25, 16.0) -> Recall.
5. Player walks through Ghost Door B to Exit at (0, 0, 22.0).

### Spatial Inventory & Placed Objects

| Object Name | Category | Position (X, Y, Z) | Rotation | Scale / Size | Value Source | Notes |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `Floor_Main` | Geometry | `(0.00, 0.00, 12.50)` | `(0, 0, 0)` | `(10.00, 0.20, 25.00)` | `DOCUMENTED` | Hall floor (10m x 25m) |
| `Wall_Left` | Geometry | `(-5.10, 1.50, 12.50)` | `(0, 0, 0)` | `(0.20, 3.00, 25.00)` | `DOCUMENTED` | Left wall |
| `Wall_Right` | Geometry | `(5.10, 1.50, 12.50)` | `(0, 0, 0)` | `(0.20, 3.00, 25.00)` | `DOCUMENTED` | Right wall |
| `Wall_Back` | Geometry | `(0.00, 1.50, -0.10)` | `(0, 0, 0)` | `(10.40, 3.00, 0.20)` | `DOCUMENTED` | Back wall |
| `Pillar_A_L` | Geometry | `(-3.25, 1.50, 7.00)` | `(0, 0, 0)` | `(3.50, 3.00, 0.40)` | `DERIVED_FROM_DIAGRAM` | Door A left pillar |
| `Pillar_A_R` | Geometry | `(3.25, 1.50, 7.00)` | `(0, 0, 0)` | `(3.50, 3.00, 0.40)` | `DERIVED_FROM_DIAGRAM` | Door A right pillar |
| `Pillar_B_L` | Geometry | `(-3.25, 1.50, 16.00)` | `(0, 0, 0)` | `(3.50, 3.00, 0.40)` | `DERIVED_FROM_DIAGRAM` | Door B left pillar |
| `Pillar_B_R` | Geometry | `(3.25, 1.50, 16.00)` | `(0, 0, 0)` | `(3.50, 3.00, 0.40)` | `DERIVED_FROM_DIAGRAM` | Door B right pillar |
| `Wall_Front_L` | Geometry | `(-3.00, 1.50, 25.10)` | `(0, 0, 0)` | `(4.00, 3.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Front wall left |
| `Wall_Front_R` | Geometry | `(3.00, 1.50, 25.10)` | `(0, 0, 0)` | `(4.00, 3.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Front wall right |
| `Player` | Player | `(0.00, 0.10, 2.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Player spawn point ground level |
| `Shadow_StartMarker` | Shadow | `(0.00, 0.10, 2.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DERIVED_FROM_DIAGRAM` | Attached to Player at start |
| `StealableDoor_A` | Stealable | `(0.00, 1.25, 7.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Door A (First lock) |
| `StealableDoor_B` | Stealable | `(0.00, 1.25, 16.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Door B (Second lock) |
| `ExitZone` | Exit | `(0.00, 0.10, 22.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Level completion exit trigger zone |
| `Label_Title` | Label | `(0.00, 3.50, 2.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: L06: Double Lock 10m x 25m | Sequential Doors |
| `Label_DoorA` | Label | `(0.00, 3.00, 7.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Door A (First Lock) |
| `Label_SafeZone` | Label | `(0.00, 1.50, 11.50)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Intermediate Safe Zone (Reposition Body Here) |
| `Label_DoorB` | Label | `(0.00, 3.00, 16.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Door B (Second Lock) |
| `Label_Exit` | Label | `(0.00, 2.00, 22.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Exit Zone |

### Implementation Notes
Multi-step puzzle progression with intermediate body repositioning.

---

## L07 — Long Detour

| Metric | Specification |
| :--- | :--- |
| **Phase** | Phase 2 (Combine) |
| **Difficulty** | 5 / 10 |
| **Play Space Dimensions** | 15m x 30m |
| **Target Time / Hard Limit** | 25s / 30s |
| **Max Required Leash / Limit** | 21m / 25m |
| **Stealable Objects** | 1 |
| **Hazard Volumes** | 1 |

### Design Objective
> Canyon detour along left wall navigating above a static danger light (21m leash).

### Traversal Routes & Metrics
- **Player Route**: Walk to canyon edge (Z=9m) -> Wait -> Cross Drawbridge -> Exit.
- **Shadow Route**: Detach -> Left wall -> Climb high (Y=3.5-4.5m) over Danger Light -> Steal Drawbridge -> Recall.

### Gameplay Sequence
1. Player advances to canyon edge at Z=9.0m (required to stay within 21m leash).
2. Detach shadow.
3. Shadow moves left, climbs left wall.
4. Shadow climbs high to Y=4.0m to bypass static Danger Light mounted at Y=1.5m.
5. Reach Drawbridge control at (0, 0.5, 14.0).
6. Steal Drawbridge. Bridge lowers.
7. Recall. Player crosses canyon to Exit at (0, 0, 26.0).

### Spatial Inventory & Placed Objects

| Object Name | Category | Position (X, Y, Z) | Rotation | Scale / Size | Value Source | Notes |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `Floor_Start` | Geometry | `(0.00, 0.00, 5.00)` | `(0, 0, 0)` | `(15.00, 0.20, 10.00)` | `DOCUMENTED` | Start floor (Z: 0 to 10m) |
| `Canyon_Pit` | Geometry | `(0.00, -6.00, 14.00)` | `(0, 0, 0)` | `(15.00, 0.20, 8.00)` | `DOCUMENTED` | Canyon chasm (8m wide, Z: 10 to 18m) |
| `Floor_Exit` | Geometry | `(0.00, 0.00, 24.00)` | `(0, 0, 0)` | `(15.00, 0.20, 12.00)` | `DOCUMENTED` | Exit floor (Z: 18 to 30m) |
| `Wall_Left_Climbable` | Geometry | `(-7.60, 2.50, 15.00)` | `(0, 0, 0)` | `(0.20, 5.00, 30.00)` | `DOCUMENTED` | Continuous climbable left wall (height 5m) |
| `Wall_Right_Start` | Geometry | `(7.60, 2.50, 4.00)` | `(0, 0, 0)` | `(0.20, 5.00, 8.00)` | `DOCUMENTED` | Right wall start segment |
| `Wall_Right_End` | Geometry | `(7.60, 2.50, 25.00)` | `(0, 0, 0)` | `(0.20, 5.00, 10.00)` | `DOCUMENTED` | Right wall end segment (gap Z: 8 to 20m broken) |
| `Wall_Back` | Geometry | `(0.00, 2.50, -0.10)` | `(0, 0, 0)` | `(15.40, 5.00, 0.20)` | `DOCUMENTED` | Back wall |
| `Wall_Front_L` | Geometry | `(-4.50, 2.50, 30.10)` | `(0, 0, 0)` | `(6.00, 5.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Front wall left |
| `Wall_Front_R` | Geometry | `(4.50, 2.50, 30.10)` | `(0, 0, 0)` | `(6.00, 5.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Front wall right |
| `Player` | Player | `(0.00, 0.10, 2.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Player spawn point ground level |
| `Shadow_StartMarker` | Shadow | `(0.00, 0.10, 2.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DERIVED_FROM_DIAGRAM` | Attached to Player at start |
| `StealableDrawbridge` | Stealable | `(0.00, 0.50, 14.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Drawbridge spanning 8m canyon |
| `DangerLight_LeftWall` | Hazard | `(-7.00, 1.50, 14.00)` | `(0, 90, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Static Danger Light covering lower wall (Y: 0 to 2.5m) |
| `ExitZone` | Exit | `(0.00, 0.10, 26.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Level completion exit trigger zone |
| `Label_Title` | Label | `(0.00, 4.00, 2.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: L07: Long Detour 15m x 30m | High Wall Detour (21m Leash) |
| `Label_DetachPoint` | Label | `(0.00, 2.00, 9.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Player Detach Point (Z=9m) (Must stand here for 21m leash) |
| `Label_Hazard` | Label | `(-6.50, 1.50, 14.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Static Danger Light (Blocks Lower Wall - Climb High!) |
| `Label_HighRoute` | Label | `(-7.20, 4.20, 14.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: High Route (Y=3.5 - 4.5m) |
| `Label_Bridge` | Label | `(0.00, 2.50, 14.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Stealable Drawbridge |
| `Label_Exit` | Label | `(0.00, 2.00, 26.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Exit Zone |

### Implementation Notes
Right wall broken. Only path is U-shaped high wall detour. 21m leash separation.

---

## L08 — Pulsing Corridor

| Metric | Specification |
| :--- | :--- |
| **Phase** | Phase 2 (Combine) |
| **Difficulty** | 6 / 10 |
| **Play Space Dimensions** | 5m x 30m |
| **Target Time / Hard Limit** | 28s / 30s |
| **Max Required Leash / Limit** | 24m / 25m |
| **Stealable Objects** | 1 |
| **Hazard Volumes** | 3 |

### Design Objective
> Rhythm and timing: dash between 3 pulsing Danger Lights and safe islands (24m leash).

### Traversal Routes & Metrics
- **Player Route**: Wait at Start -> Walk through Ghost Door to Exit (lights do not harm Player).
- **Shadow Route**: Detach -> Dash L1 -> Safe Zone 1 -> Dash L2 -> Safe Zone 2 -> Dash L3 -> Steal Door -> Recall.

### Gameplay Sequence
1. Player detaches at start.
2. Shadow waits for L1 to cycle off -> dash to Safe Zone 1 (Z=11.5m).
3. Shadow waits for L2 off -> dash to Safe Zone 2 (Z=18.5m).
4. Shadow waits for L3 off -> dash to Stealable Door at (0, 1.25, 26.0).
5. Steal Door (at 24m leash separation).
6. Recall -> Player walks through Ghost Door to Exit.

### Spatial Inventory & Placed Objects

| Object Name | Category | Position (X, Y, Z) | Rotation | Scale / Size | Value Source | Notes |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `Floor_Main` | Geometry | `(0.00, 0.00, 15.00)` | `(0, 0, 0)` | `(5.00, 0.20, 30.00)` | `DOCUMENTED` | Hall floor (5m x 30m) |
| `Wall_Left` | Geometry | `(-2.60, 1.50, 15.00)` | `(0, 0, 0)` | `(0.20, 3.00, 30.00)` | `DOCUMENTED` | Left wall |
| `Wall_Right` | Geometry | `(2.60, 1.50, 15.00)` | `(0, 0, 0)` | `(0.20, 3.00, 30.00)` | `DOCUMENTED` | Right wall |
| `Wall_Back` | Geometry | `(0.00, 1.50, -0.10)` | `(0, 0, 0)` | `(5.40, 3.00, 0.20)` | `DOCUMENTED` | Back wall |
| `Pillar_Door_L` | Geometry | `(-1.75, 1.50, 26.00)` | `(0, 0, 0)` | `(1.50, 3.00, 0.40)` | `DERIVED_FROM_DIAGRAM` | Door pillar left |
| `Pillar_Door_R` | Geometry | `(1.75, 1.50, 26.00)` | `(0, 0, 0)` | `(1.50, 3.00, 0.40)` | `DERIVED_FROM_DIAGRAM` | Door pillar right |
| `Wall_Front_L` | Geometry | `(-1.75, 1.50, 30.10)` | `(0, 0, 0)` | `(1.50, 3.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Front wall left |
| `Wall_Front_R` | Geometry | `(1.75, 1.50, 30.10)` | `(0, 0, 0)` | `(1.50, 3.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Front wall right |
| `Player` | Player | `(0.00, 0.10, 2.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Player spawn point ground level |
| `Shadow_StartMarker` | Shadow | `(0.00, 0.10, 2.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DERIVED_FROM_DIAGRAM` | Attached to Player at start |
| `StealableDoor` | Stealable | `(0.00, 1.25, 26.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Stealable Door at end of pulsing corridor |
| `DangerLight_Pulsing_1` | Hazard | `(0.00, 0.50, 8.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Pulsing Light 1 (Z: 6 to 10m) |
| `DangerLight_Pulsing_2` | Hazard | `(0.00, 0.50, 15.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Pulsing Light 2 (Z: 13 to 17m) |
| `DangerLight_Pulsing_3` | Hazard | `(0.00, 0.50, 22.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Pulsing Light 3 (Z: 20 to 24m) |
| `ExitZone` | Exit | `(0.00, 0.10, 28.50)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Level completion exit trigger zone |
| `Label_Title` | Label | `(0.00, 3.50, 2.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: L08: Pulsing Corridor 5m x 30m | Rhythm & Safe Islands (24m Leash) |
| `Label_Light1` | Label | `(0.00, 2.00, 8.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Pulsing Light 1 |
| `Label_Safe1` | Label | `(0.00, 1.00, 11.50)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Safe Island 1 |
| `Label_Light2` | Label | `(0.00, 2.00, 15.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Pulsing Light 2 |
| `Label_Safe2` | Label | `(0.00, 1.00, 18.50)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Safe Island 2 |
| `Label_Light3` | Label | `(0.00, 2.00, 22.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Pulsing Light 3 |
| `Label_Door` | Label | `(0.00, 3.00, 26.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Stealable Door (24m Leash) |
| `Label_Exit` | Label | `(0.00, 2.00, 28.50)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Exit Zone |

### Implementation Notes
Three synchronized pulsing danger lights. 24m Euclidean distance leaves 1m margin.

---

## L09 — Split Decision

| Metric | Specification |
| :--- | :--- |
| **Phase** | Phase 2 (Combine) |
| **Difficulty** | 6 / 10 |
| **Play Space Dimensions** | 15m x 25m |
| **Target Time / Hard Limit** | 22s / 25s |
| **Max Required Leash / Limit** | 20m / 25m |
| **Stealable Objects** | 1 |
| **Hazard Volumes** | 2 |

### Design Objective
> Player agency: Choose Route A (Right Floor Ledge + Sweeping Light) or Route B (Left Wall + Pulsing Light).

### Traversal Routes & Metrics
- **Player Route**: Walk to pit edge (Z=7.0m) -> Wait -> Cross lowered Drawbridge -> Exit.
- **Shadow Route**: Choice: Route A (Floor right, dodge sweeper) or Route B (Wall left, time pulsing light). Steal bridge -> Recall.

### Gameplay Sequence
1. Player stops at pit edge at Z=7.0m.
2. Detach shadow.
3. Option A: Move along right floor ledge, time sweeping light to reach Drawbridge.
   Option B: Transition to left wall, time static pulsing light to reach Drawbridge.
4. Steal Drawbridge at (0, 0.5, 16.0).
5. Recall -> Player crosses lowered bridge to Exit.

### Spatial Inventory & Placed Objects

| Object Name | Category | Position (X, Y, Z) | Rotation | Scale / Size | Value Source | Notes |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `Floor_Start` | Geometry | `(0.00, 0.00, 4.00)` | `(0, 0, 0)` | `(15.00, 0.20, 8.00)` | `DOCUMENTED` | Start floor (Z: 0 to 8m) |
| `Central_Pit` | Geometry | `(0.00, -6.00, 12.00)` | `(0, 0, 0)` | `(10.00, 0.20, 8.00)` | `DOCUMENTED` | Central pit chasm (8m wide, Z: 8 to 16m) |
| `RouteA_FloorLedge` | Geometry | `(6.00, 0.00, 12.00)` | `(0, 0, 0)` | `(3.00, 0.20, 8.00)` | `DOCUMENTED` | Route A: Right floor ledge across pit |
| `Floor_Exit` | Geometry | `(0.00, 0.00, 20.50)` | `(0, 0, 0)` | `(15.00, 0.20, 9.00)` | `DOCUMENTED` | Exit floor (Z: 16 to 25m) |
| `Wall_Left_Climbable` | Geometry | `(-7.60, 2.50, 12.50)` | `(0, 0, 0)` | `(0.20, 5.00, 25.00)` | `DOCUMENTED` | Route B: Left climbable wall (height 5m) |
| `Wall_Right` | Geometry | `(7.60, 2.50, 12.50)` | `(0, 0, 0)` | `(0.20, 5.00, 25.00)` | `DOCUMENTED` | Right boundary wall |
| `Wall_Back` | Geometry | `(0.00, 2.50, -0.10)` | `(0, 0, 0)` | `(15.40, 5.00, 0.20)` | `DOCUMENTED` | Back wall |
| `Wall_Front_L` | Geometry | `(-4.50, 2.50, 25.10)` | `(0, 0, 0)` | `(6.00, 5.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Front wall left |
| `Wall_Front_R` | Geometry | `(4.50, 2.50, 25.10)` | `(0, 0, 0)` | `(6.00, 5.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Front wall right |
| `Player` | Player | `(0.00, 0.10, 2.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Player spawn point ground level |
| `Shadow_StartMarker` | Shadow | `(0.00, 0.10, 2.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DERIVED_FROM_DIAGRAM` | Attached to Player at start |
| `StealableDrawbridge` | Stealable | `(0.00, 0.50, 16.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Drawbridge spanning central pit |
| `DangerLight_RouteA_Sweeping` | Hazard | `(6.00, 3.00, 12.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Route A Sweeping Light covering right ledge |
| `DangerLight_RouteB_Pulsing` | Hazard | `(-7.00, 2.00, 12.00)` | `(0, 90, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Route B Pulsing Light covering left wall |
| `ExitZone` | Exit | `(0.00, 0.10, 22.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Level completion exit trigger zone |
| `Label_Title` | Label | `(0.00, 4.00, 2.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: L09: Split Decision 15m x 25m | Route A (Floor) vs Route B (Wall) |
| `Label_RouteA` | Label | `(6.00, 2.00, 9.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Route A: Floor Ledge (Sweeping Light) |
| `Label_RouteB` | Label | `(-7.00, 3.50, 9.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Route B: Wall Route (Pulsing Light) |
| `Label_Pit` | Label | `(0.00, -0.50, 12.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Central Pit |
| `Label_Bridge` | Label | `(0.00, 2.50, 16.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Stealable Drawbridge |
| `Label_Exit` | Label | `(0.00, 2.00, 22.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Exit Zone |

### Implementation Notes
Both paths mathematically viable (Route A: 18m, Route B: 19m, max separation 20m).

---

## L10 — The Gauntlet

| Metric | Specification |
| :--- | :--- |
| **Phase** | Phase 2 (Combine) |
| **Difficulty** | 7 / 10 |
| **Play Space Dimensions** | 15m x 40m |
| **Target Time / Hard Limit** | 30s / 35s |
| **Max Required Leash / Limit** | 24m / 25m |
| **Stealable Objects** | 2 |
| **Hazard Volumes** | 3 |

### Design Objective
> Section 1 Drawbridge over pit, immediately followed by Section 2 dual sweeping lights and Door.

### Traversal Routes & Metrics
- **Player Route**: Safe: Walk to pit -> Cross bridge -> Wait -> Walk through Ghost Door to Exit.
- **Shadow Route**: Safe: Detach -> Steal Bridge -> Recall. Detach -> Dodge 2 sweepers -> Steal Door -> Recall.
Expert: Detach -> Steal Bridge -> Cross bridge while detached -> Dodge sweepers -> Steal Door -> Recall (1 Detach!).

### Gameplay Sequence
1. Player advances to pit edge at Z=7.5m.
2. Detach shadow -> Climb left wall past static danger light at (-7.0, 1.5, 12.0).
3. Steal Drawbridge at (0, 0.5, 16.0). Bridge lowers.
4. (Safe route: Recall -> Player crosses -> Detach at Z=18m).
   (Expert route: Continue moving shadow across lowered bridge, max leash reaches 24m at door).
5. Shadow times Sweeping Light 1 (Z=23m) and Sweeping Light 2 (Z=28m).
6. Steal Door at (0, 1.25, 34.0).
7. Recall -> Player walks to Exit at (0, 0, 38.0).

### Spatial Inventory & Placed Objects

| Object Name | Category | Position (X, Y, Z) | Rotation | Scale / Size | Value Source | Notes |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `Floor_Section1` | Geometry | `(0.00, 0.00, 4.00)` | `(0, 0, 0)` | `(15.00, 0.20, 8.00)` | `DOCUMENTED` | Start floor (Z: 0 to 8m) |
| `Pit_Chasm` | Geometry | `(0.00, -6.00, 12.00)` | `(0, 0, 0)` | `(15.00, 0.20, 8.00)` | `DOCUMENTED` | Section 1 Pit (8m wide, Z: 8 to 16m) |
| `Floor_Section2` | Geometry | `(0.00, 0.00, 28.00)` | `(0, 0, 0)` | `(15.00, 0.20, 24.00)` | `DOCUMENTED` | Section 2 Gauntlet floor (Z: 16 to 40m) |
| `Wall_Left_Climbable` | Geometry | `(-7.60, 2.50, 20.00)` | `(0, 0, 0)` | `(0.20, 5.00, 40.00)` | `DOCUMENTED` | Left climbable wall |
| `Wall_Right` | Geometry | `(7.60, 2.50, 20.00)` | `(0, 0, 0)` | `(0.20, 5.00, 40.00)` | `DOCUMENTED` | Right wall |
| `Wall_Back` | Geometry | `(0.00, 2.50, -0.10)` | `(0, 0, 0)` | `(15.40, 5.00, 0.20)` | `DOCUMENTED` | Back wall |
| `Pillar_Door_L` | Geometry | `(-3.25, 1.50, 34.00)` | `(0, 0, 0)` | `(3.50, 3.00, 0.40)` | `DERIVED_FROM_DIAGRAM` | Door pillar left |
| `Pillar_Door_R` | Geometry | `(3.25, 1.50, 34.00)` | `(0, 0, 0)` | `(3.50, 3.00, 0.40)` | `DERIVED_FROM_DIAGRAM` | Door pillar right |
| `Wall_Front_L` | Geometry | `(-4.50, 2.50, 40.10)` | `(0, 0, 0)` | `(6.00, 5.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Front wall left |
| `Wall_Front_R` | Geometry | `(4.50, 2.50, 40.10)` | `(0, 0, 0)` | `(6.00, 5.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Front wall right |
| `Player` | Player | `(0.00, 0.10, 2.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Player spawn point ground level |
| `Shadow_StartMarker` | Shadow | `(0.00, 0.10, 2.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DERIVED_FROM_DIAGRAM` | Attached to Player at start |
| `StealableDrawbridge` | Stealable | `(0.00, 0.50, 16.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Drawbridge across Section 1 pit |
| `StealableDoor` | Stealable | `(0.00, 1.25, 34.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Stealable Door guarding exit |
| `DangerLight_PitWall` | Hazard | `(-7.00, 1.50, 12.00)` | `(0, 90, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Static Danger Light on left pit wall |
| `DangerLight_Sweeping_1` | Hazard | `(-2.50, 3.50, 23.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Sweeping Light 1 (left staggered) |
| `DangerLight_Sweeping_2` | Hazard | `(2.50, 3.50, 28.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Sweeping Light 2 (right staggered) |
| `ExitZone` | Exit | `(0.00, 0.10, 38.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Level completion exit trigger zone |
| `Label_Title` | Label | `(0.00, 4.50, 2.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: L10: The Gauntlet 15m x 40m | Pit Drawbridge + Dual Sweeper Corridor |
| `Label_Bridge` | Label | `(0.00, 2.50, 16.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Drawbridge Control |
| `Label_Sweeper1` | Label | `(-2.50, 4.50, 23.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Sweeping Light 1 |
| `Label_Sweeper2` | Label | `(2.50, 4.50, 28.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Sweeping Light 2 |
| `Label_Door` | Label | `(0.00, 3.00, 34.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Stealable Door |
| `Label_Exit` | Label | `(0.00, 2.00, 38.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Exit Zone |

### Implementation Notes
Culmination of Phase 2. Enables Expert 1-Detach route natively within 25m leash.

---

## L11 — The Snare

| Metric | Specification |
| :--- | :--- |
| **Phase** | Phase 3 (Master) |
| **Difficulty** | 7 / 10 |
| **Play Space Dimensions** | 5m x 25m |
| **Target Time / Hard Limit** | 20s / 25s |
| **Max Required Leash / Limit** | 14m / 25m |
| **Stealable Objects** | 1 |
| **Hazard Volumes** | 1 |

### Design Objective
> Introduce TRAP LIGHT hazard: snare requires 3 rapid RECOVER button presses in 2.0s to break free.

### Traversal Routes & Metrics
- **Player Route**: Walk to edge of Trap Light -> Wait -> Walk through Ghost Door to Exit.
- **Shadow Route**: Detach -> Enter Trap Light -> Trapped -> Mash RECOVER x3 -> Break free -> Steal Door -> Recall.

### Gameplay Sequence
1. Player stops at Z=8.0m.
2. Detach shadow.
3. Shadow moves into unavoidable Trap Light at (0, 1.5, 13.0).
4. Shadow is rooted in place; action button switches to RECOVER.
5. Player taps RECOVER 3 times rapidly within 2.0s.
6. Shadow breaks free from snare.
7. Shadow moves to Door at (0, 1.25, 20.0) -> Steal.
8. Recall -> Player crosses through Ghost Door to Exit.

### Spatial Inventory & Placed Objects

| Object Name | Category | Position (X, Y, Z) | Rotation | Scale / Size | Value Source | Notes |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `Floor_Main` | Geometry | `(0.00, 0.00, 12.50)` | `(0, 0, 0)` | `(5.00, 0.20, 25.00)` | `DOCUMENTED` | Hall floor (5m x 25m) |
| `Wall_Left` | Geometry | `(-2.60, 1.50, 12.50)` | `(0, 0, 0)` | `(0.20, 3.00, 25.00)` | `DOCUMENTED` | Left wall |
| `Wall_Right` | Geometry | `(2.60, 1.50, 12.50)` | `(0, 0, 0)` | `(0.20, 3.00, 25.00)` | `DOCUMENTED` | Right wall |
| `Wall_Back` | Geometry | `(0.00, 1.50, -0.10)` | `(0, 0, 0)` | `(5.40, 3.00, 0.20)` | `DOCUMENTED` | Back wall |
| `Pillar_Door_L` | Geometry | `(-1.75, 1.50, 20.00)` | `(0, 0, 0)` | `(1.50, 3.00, 0.40)` | `DERIVED_FROM_DIAGRAM` | Door pillar left |
| `Pillar_Door_R` | Geometry | `(1.75, 1.50, 20.00)` | `(0, 0, 0)` | `(1.50, 3.00, 0.40)` | `DERIVED_FROM_DIAGRAM` | Door pillar right |
| `Wall_Front_L` | Geometry | `(-1.75, 1.50, 25.10)` | `(0, 0, 0)` | `(1.50, 3.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Front wall left |
| `Wall_Front_R` | Geometry | `(1.75, 1.50, 25.10)` | `(0, 0, 0)` | `(1.50, 3.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Front wall right |
| `Player` | Player | `(0.00, 0.10, 2.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Player spawn point ground level |
| `Shadow_StartMarker` | Shadow | `(0.00, 0.10, 2.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DERIVED_FROM_DIAGRAM` | Attached to Player at start |
| `StealableDoor` | Stealable | `(0.00, 1.25, 20.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Stealable Door beyond Trap Light |
| `TrapLight_Snare` | Hazard | `(0.00, 1.50, 13.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Static Trap Light (Purple/UV hue, Z: 11 to 15m) |
| `ExitZone` | Exit | `(0.00, 0.10, 23.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Level completion exit trigger zone |
| `Label_Title` | Label | `(0.00, 3.50, 2.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: L11: The Snare 5m x 25m | Introduce TRAP LIGHT Hazard |
| `Label_Trap` | Label | `(0.00, 3.50, 13.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Static Trap Light (Purple/UV - Mash RECOVER x3) |
| `Label_Door` | Label | `(0.00, 3.00, 20.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Stealable Door |
| `Label_Exit` | Label | `(0.00, 2.00, 23.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Exit Zone |

### Implementation Notes
First encounter with Trap Light. Unlike Danger Light, does not instantly destroy shadow; requires rapid escape input.

---

## L12 — The Conveyor

| Metric | Specification |
| :--- | :--- |
| **Phase** | Phase 3 (Master) |
| **Difficulty** | 7 / 10 |
| **Play Space Dimensions** | 15m x 25m |
| **Target Time / Hard Limit** | 20s / 25s |
| **Max Required Leash / Limit** | 10m / 25m |
| **Stealable Objects** | 1 |
| **Hazard Volumes** | 0 |

### Design Objective
> Introduce PLATFORM Stealable: stealing shadow freezes rapid oscillation in place to form bridge.

### Traversal Routes & Metrics
- **Player Route**: Walk to pit edge -> Wait -> Walk across frozen Platform -> Exit.
- **Shadow Route**: Detach -> Move to pit edge -> Wait for platform to align centrally -> Steal -> Recall.

### Gameplay Sequence
1. Player stops at pit edge at Z=7.0m.
2. Detach shadow.
3. Platform slides rapidly left/right across 8m pit at 6m/s.
4. As Platform passes center alignment (X=0), press STEAL.
5. Platform enters FROZEN state (rigidly stationary solid surface).
6. Recall -> Player crosses stationary platform to Exit at (0, 0, 21.0).

### Spatial Inventory & Placed Objects

| Object Name | Category | Position (X, Y, Z) | Rotation | Scale / Size | Value Source | Notes |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `Floor_Start` | Geometry | `(0.00, 0.00, 4.00)` | `(0, 0, 0)` | `(15.00, 0.20, 8.00)` | `DOCUMENTED` | Start floor (Z: 0 to 8m) |
| `Pit_Chasm` | Geometry | `(0.00, -6.00, 12.00)` | `(0, 0, 0)` | `(15.00, 0.20, 8.00)` | `DOCUMENTED` | Pit chasm (8m wide, Z: 8 to 16m) |
| `Floor_Exit` | Geometry | `(0.00, 0.00, 20.50)` | `(0, 0, 0)` | `(15.00, 0.20, 9.00)` | `DOCUMENTED` | Exit floor (Z: 16 to 25m) |
| `Wall_Left` | Geometry | `(-7.60, 2.50, 12.50)` | `(0, 0, 0)` | `(0.20, 5.00, 25.00)` | `DOCUMENTED` | Left wall |
| `Wall_Right` | Geometry | `(7.60, 2.50, 12.50)` | `(0, 0, 0)` | `(0.20, 5.00, 25.00)` | `DOCUMENTED` | Right wall |
| `Wall_Back` | Geometry | `(0.00, 2.50, -0.10)` | `(0, 0, 0)` | `(15.40, 5.00, 0.20)` | `DOCUMENTED` | Back wall |
| `Wall_Front_L` | Geometry | `(-4.50, 2.50, 25.10)` | `(0, 0, 0)` | `(6.00, 5.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Front wall left |
| `Wall_Front_R` | Geometry | `(4.50, 2.50, 25.10)` | `(0, 0, 0)` | `(6.00, 5.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Front wall right |
| `Player` | Player | `(0.00, 0.10, 2.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Player spawn point ground level |
| `Shadow_StartMarker` | Shadow | `(0.00, 0.10, 2.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DERIVED_FROM_DIAGRAM` | Attached to Player at start |
| `StealablePlatform` | Stealable | `(0.00, 0.10, 12.00)` | `(0, 0, 0)` | `(3.00, 0.40, 4.00)` | `DOCUMENTED` | Rapid moving Stealable Platform across 8m pit |
| `ExitZone` | Exit | `(0.00, 0.10, 21.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Level completion exit trigger zone |
| `Label_Title` | Label | `(0.00, 4.00, 2.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: L12: The Conveyor 15m x 25m | Introduce PLATFORM Stealable |
| `Label_Pit` | Label | `(0.00, -0.50, 12.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: 8m Pit Gap |
| `Label_Platform` | Label | `(0.00, 2.00, 12.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Rapid Moving Platform (Steal at Center -> Freezes into Bridge) |
| `Label_Exit` | Label | `(0.00, 2.00, 21.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Exit Zone |

### Implementation Notes
Platform Stealable introduction. Stolen platform remains physically solid but stops moving.

---

## L13 — The Breaker

| Metric | Specification |
| :--- | :--- |
| **Phase** | Phase 3 (Master) |
| **Difficulty** | 8 / 10 |
| **Play Space Dimensions** | 15m x 20m |
| **Target Time / Hard Limit** | 25s / 30s |
| **Max Required Leash / Limit** | 12m / 25m |
| **Stealable Objects** | 1 |
| **Hazard Volumes** | 1 |

### Design Objective
> Introduce LIGHT SWITCH Stealable: climb wall to steal switch and permanently deactivate massive lethal light.

### Traversal Routes & Metrics
- **Player Route**: Wait at start -> Wait for light to power down -> Walk to Exit (top right).
- **Shadow Route**: Detach -> Left wall -> Climb to Light Switch at (-7.4, 3.5, 8.0) -> Steal -> Recall.

### Gameplay Sequence
1. Player enters room; path to exit on right is completely flooded by massive Danger Light.
2. Detach shadow.
3. Shadow transitions to left wall and climbs to Y=3.5m.
4. Reach Stealable Light Switch at (-7.4, 3.5, 8.0).
5. Press STEAL. Switch ghosts; connected Danger Light powers down completely.
6. Recall -> Player walks safely through deactivated light zone to Exit at (5.0, 0, 18.0).

### Spatial Inventory & Placed Objects

| Object Name | Category | Position (X, Y, Z) | Rotation | Scale / Size | Value Source | Notes |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `Floor_Main` | Geometry | `(0.00, 0.00, 10.00)` | `(0, 0, 0)` | `(15.00, 0.20, 20.00)` | `DOCUMENTED` | Hall floor (15m x 20m) |
| `Wall_Left_Climbable` | Geometry | `(-7.60, 2.50, 10.00)` | `(0, 0, 0)` | `(0.20, 5.00, 20.00)` | `DOCUMENTED` | Left climbable wall with mounted switch |
| `Wall_Right` | Geometry | `(7.60, 2.50, 10.00)` | `(0, 0, 0)` | `(0.20, 5.00, 20.00)` | `DOCUMENTED` | Right wall |
| `Wall_Back` | Geometry | `(0.00, 2.50, -0.10)` | `(0, 0, 0)` | `(15.40, 5.00, 0.20)` | `DOCUMENTED` | Back wall |
| `Wall_Front_L` | Geometry | `(-4.50, 2.50, 20.10)` | `(0, 0, 0)` | `(6.00, 5.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Front wall left |
| `Wall_Front_M` | Geometry | `(1.00, 2.50, 20.10)` | `(0, 0, 0)` | `(5.00, 5.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Front wall center (gap at X: 3.5 to 6.5m) |
| `Player` | Player | `(0.00, 0.10, 2.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Player spawn point ground level |
| `Shadow_StartMarker` | Shadow | `(0.00, 0.10, 2.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DERIVED_FROM_DIAGRAM` | Attached to Player at start |
| `StealableLightSwitch` | Stealable | `(-7.40, 3.50, 8.00)` | `(0, 90, 0)` | `(0.30, 0.80, 0.60)` | `DOCUMENTED` | Stealable Light Switch (Programmer Task: Rayaan unbuilt dependency) |
| `DangerLight_MassiveFloor` | Hazard | `(2.50, 2.00, 15.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Massive Danger Light flooding exit zone (X: -2 to 7m, Z: 11 to 19m) |
| `ExitZone` | Exit | `(5.00, 0.10, 18.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Level completion exit trigger zone |
| `Label_Title` | Label | `(0.00, 4.00, 2.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: L13: The Breaker 15m x 20m | Introduce LIGHT SWITCH Stealable |
| `Label_Switch` | Label | `(-6.80, 4.50, 8.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Stealable Light Switch (Climb Wall - Powers Down Lethal Light) [Unbuilt: Rayaan] |
| `Label_Hazard` | Label | `(2.50, 3.50, 15.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Massive Lethal Danger Light (Floods Exit Path Until Switched Off) |
| `Label_Exit` | Label | `(5.00, 2.00, 18.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Exit Zone (Top Right) |

### Implementation Notes
Programmer Dependency: StealableLightSwitch is currently unbuilt (Rayaan). Greybox representation placed with conduit visuals.

---

## L14 — Threading the Needle

| Metric | Specification |
| :--- | :--- |
| **Phase** | Phase 3 (Master) |
| **Difficulty** | 9 / 10 |
| **Play Space Dimensions** | 5m x 25m |
| **Target Time / Hard Limit** | 30s / 35s |
| **Max Required Leash / Limit** | 24.5m / 25m |
| **Stealable Objects** | 1 |
| **Hazard Volumes** | 4 |

### Design Objective
> Extreme leash precision: razor-thin safe wall ribbon past 3 Danger Lights and 1 Trap Light (24.5m / 25m leash).

### Traversal Routes & Metrics
- **Player Route**: Wait at start line (MUST detach at start) -> Walk through Ghost Door to Exit.
- **Shadow Route**: Detach -> Right wall safe ribbon -> Weave past 3 lights -> Enter Trap Light -> RECOVER x3 -> Steal Door (at 24.5m) -> Recall.

### Gameplay Sequence
1. Player MUST detach directly from start line (standing even 1m back causes leash clamp failure).
2. Shadow transitions immediately to right wall ribbon.
3. Carefully weave along right wall past floor danger light cones.
4. Shadow enters unavoidable Trap Light near top of wall at (2.4, 2.2, 19.5).
5. Player rapidly mashes RECOVER x3 within 2.0s to break free.
6. Shadow advances final 1m to Stealable Door at (0, 1.25, 22.0).
7. Euclidean distance reaches exactly 24.5m (0.5m under 25m leash limit).
8. Steal Door -> Recall -> Player walks through Ghost Door to Exit.

### Spatial Inventory & Placed Objects

| Object Name | Category | Position (X, Y, Z) | Rotation | Scale / Size | Value Source | Notes |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `Floor_Main` | Geometry | `(0.00, 0.00, 12.50)` | `(0, 0, 0)` | `(5.00, 0.20, 25.00)` | `DOCUMENTED` | Oppressive corridor floor (5m x 25m) |
| `Wall_Left` | Geometry | `(-2.60, 2.00, 12.50)` | `(0, 0, 0)` | `(0.20, 4.00, 25.00)` | `DOCUMENTED` | Left wall (height 4m) |
| `Wall_Right_Ribbon` | Geometry | `(2.60, 2.00, 12.50)` | `(0, 0, 0)` | `(0.20, 4.00, 25.00)` | `DOCUMENTED` | Right climbable wall (narrow safe ribbon path) |
| `Wall_Back` | Geometry | `(0.00, 2.00, -0.10)` | `(0, 0, 0)` | `(5.40, 4.00, 0.20)` | `DOCUMENTED` | Back wall |
| `Pillar_Door_L` | Geometry | `(-1.75, 1.50, 22.00)` | `(0, 0, 0)` | `(1.50, 3.00, 0.40)` | `DERIVED_FROM_DIAGRAM` | Door pillar left |
| `Pillar_Door_R` | Geometry | `(1.75, 1.50, 22.00)` | `(0, 0, 0)` | `(1.50, 3.00, 0.40)` | `DERIVED_FROM_DIAGRAM` | Door pillar right |
| `Wall_Front_L` | Geometry | `(-1.75, 2.00, 25.10)` | `(0, 0, 0)` | `(1.50, 4.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Front wall left |
| `Wall_Front_R` | Geometry | `(1.75, 2.00, 25.10)` | `(0, 0, 0)` | `(1.50, 4.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Front wall right |
| `Player` | Player | `(0.00, 0.10, 1.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Player spawn point ground level |
| `Shadow_StartMarker` | Shadow | `(0.00, 0.10, 1.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DERIVED_FROM_DIAGRAM` | Attached to Player at start |
| `StealableDoor` | Stealable | `(0.00, 1.25, 22.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Door at exact 24.5m Euclidean limit |
| `DangerLight_Cone_1` | Hazard | `(-0.50, 1.50, 6.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Flooding Danger Light 1 |
| `DangerLight_Cone_2` | Hazard | `(-0.50, 1.50, 12.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Flooding Danger Light 2 |
| `DangerLight_Cone_3` | Hazard | `(-0.50, 1.50, 17.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Flooding Danger Light 3 |
| `TrapLight_WallSnare` | Hazard | `(2.40, 2.20, 19.50)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Wall Trap Light snare before door |
| `ExitZone` | Exit | `(0.00, 0.10, 24.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Level completion exit trigger zone |
| `Label_Title` | Label | `(0.00, 3.50, 1.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: L14: Threading the Needle 5m x 25m | Extreme Leash Precision (24.5m / 25m) |
| `Label_FloodedFloor` | Label | `(-0.50, 3.00, 12.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Floor Flooded with Lethal Light (Right Wall Ribbon Only) |
| `Label_Ribbon` | Label | `(2.30, 3.50, 12.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Narrow Safe Ribbon (Wall) |
| `Label_WallTrap` | Label | `(2.30, 3.50, 19.50)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Wall Snare (RECOVER x3) |
| `Label_Door` | Label | `(0.00, 3.00, 22.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Stealable Door (24.5m Euclidean) |
| `Label_Exit` | Label | `(0.00, 2.00, 24.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Exit Zone |

### Implementation Notes
Ultimate leash stress test. Mathematically calibrated so backing up 1m prevents reaching door.

---

## L15 — Final Escape

| Metric | Specification |
| :--- | :--- |
| **Phase** | Phase 3 (Master) |
| **Difficulty** | 10 / 10 |
| **Play Space Dimensions** | 20m x 50m |
| **Target Time / Hard Limit** | 38s / 40s |
| **Max Required Leash / Limit** | 18m / 25m |
| **Stealable Objects** | 3 |
| **Hazard Volumes** | 2 |

### Design Objective
> Mastery exam: Stage 1 Switch, Stage 2 Platform, Stage 3 Trap + Door under 38s time pressure.

### Traversal Routes & Metrics
- **Player Route**: Wait -> Cross Pit 1 -> Wait at Pit 2 -> Cross Platform -> Wait at Stage 3 -> Walk through Ghost Door to Exit.
- **Shadow Route**: 1. Climb wall, dodge sweeper, steal Switch -> Recall.
2. Steal moving platform to freeze -> Recall.
3. Enter trap, mash RECOVER x3, steal Door -> Recall.

### Gameplay Sequence
1. Player detaches at start.
2. Shadow climbs left wall, dodges sweeping light covering Pit 1, steals Light Switch at (-9.8, 3.0, 5.0). Light dies -> Recall.
3. Player crosses Pit 1 along safe floor ledge to edge of Pit 2 (Z=18m).
4. Detach shadow -> Steal oscillating Platform across Pit 2 (Z=24m) to freeze it -> Recall.
5. Player crosses frozen platform into Stage 3 (Z=32m).
6. Detach shadow -> Enter Trap Light at Z=38m -> Mash RECOVER x3 -> Steal Door at (0, 1.25, 45.0) -> Recall.
7. Player walks through Ghost Door to Exit at (0, 0, 48.5) to complete the game!

### Spatial Inventory & Placed Objects

| Object Name | Category | Position (X, Y, Z) | Rotation | Scale / Size | Value Source | Notes |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `Floor_Stage1_Start` | Geometry | `(0.00, 0.00, 3.00)` | `(0, 0, 0)` | `(20.00, 0.20, 6.00)` | `DOCUMENTED` | Stage 1 start floor (Z: 0 to 6m) |
| `Pit1_Chasm` | Geometry | `(0.00, -6.00, 9.00)` | `(0, 0, 0)` | `(20.00, 0.20, 6.00)` | `DOCUMENTED` | Stage 1 Pit (6m chasm, Z: 6 to 12m) |
| `Floor_Stage1_Ledge` | Geometry | `(-8.00, 0.00, 9.00)` | `(0, 0, 0)` | `(4.00, 0.20, 6.00)` | `DERIVED_FROM_DIAGRAM` | Stage 1 left crossing ledge |
| `Floor_Stage2_Mid` | Geometry | `(0.00, 0.00, 16.00)` | `(0, 0, 0)` | `(20.00, 0.20, 8.00)` | `DOCUMENTED` | Stage 2 intermediate floor (Z: 12 to 20m) |
| `Pit2_Chasm` | Geometry | `(0.00, -6.00, 24.00)` | `(0, 0, 0)` | `(20.00, 0.20, 8.00)` | `DOCUMENTED` | Stage 2 Pit (8m chasm, Z: 20 to 28m) |
| `Floor_Stage3_Final` | Geometry | `(0.00, 0.00, 39.00)` | `(0, 0, 0)` | `(20.00, 0.20, 22.00)` | `DOCUMENTED` | Stage 3 final arena floor (Z: 28 to 50m) |
| `Wall_Left_Climbable` | Geometry | `(-10.10, 2.50, 25.00)` | `(0, 0, 0)` | `(0.20, 5.00, 50.00)` | `DOCUMENTED` | Left climbable wall (height 5m) |
| `Wall_Right` | Geometry | `(10.10, 2.50, 25.00)` | `(0, 0, 0)` | `(0.20, 5.00, 50.00)` | `DOCUMENTED` | Right boundary wall |
| `Wall_Back` | Geometry | `(0.00, 2.50, -0.10)` | `(0, 0, 0)` | `(20.40, 5.00, 0.20)` | `DOCUMENTED` | Back boundary wall |
| `Pillar_Door_L` | Geometry | `(-3.25, 1.50, 45.00)` | `(0, 0, 0)` | `(3.50, 3.00, 0.40)` | `DERIVED_FROM_DIAGRAM` | Final door pillar left |
| `Pillar_Door_R` | Geometry | `(3.25, 1.50, 45.00)` | `(0, 0, 0)` | `(3.50, 3.00, 0.40)` | `DERIVED_FROM_DIAGRAM` | Final door pillar right |
| `Wall_Front_L` | Geometry | `(-6.00, 2.50, 50.10)` | `(0, 0, 0)` | `(8.00, 5.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Front exit wall left |
| `Wall_Front_R` | Geometry | `(6.00, 2.50, 50.10)` | `(0, 0, 0)` | `(8.00, 5.00, 0.20)` | `DERIVED_FROM_DIAGRAM` | Front exit wall right |
| `Player` | Player | `(0.00, 0.10, 2.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Player spawn point ground level |
| `Shadow_StartMarker` | Shadow | `(0.00, 0.10, 2.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DERIVED_FROM_DIAGRAM` | Attached to Player at start |
| `StealableLightSwitch` | Stealable | `(-9.80, 3.00, 5.00)` | `(0, 90, 0)` | `(0.30, 0.80, 0.60)` | `DOCUMENTED` | Stage 1 Wall Light Switch (Unbuilt dependency: Rayaan) |
| `StealablePlatform` | Stealable | `(0.00, 0.10, 24.00)` | `(0, 0, 0)` | `(3.50, 0.40, 4.00)` | `DOCUMENTED` | Stage 2 Rapid Moving Platform |
| `StealableDoor` | Stealable | `(0.00, 1.25, 45.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Stage 3 Final Stealable Door |
| `DangerLight_Pit1_Sweeping` | Hazard | `(0.00, 3.50, 9.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Stage 1 Sweeping Danger Light over Pit 1 |
| `TrapLight_Stage3` | Hazard | `(0.00, 1.50, 38.00)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Stage 3 Trap Light Snare before Door |
| `ExitZone` | Exit | `(0.00, 0.10, 48.50)` | `(0, 0, 0)` | `(1.00, 1.00, 1.00)` | `DOCUMENTED` | Level completion exit trigger zone |
| `Label_Title` | Label | `(0.00, 4.50, 2.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: L15: Final Escape 20m x 50m | The Ultimate Exam (All Systems) |
| `Label_Stage1` | Label | `(-9.00, 4.00, 5.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Stage 1: Steal Light Switch [Unbuilt: Rayaan] |
| `Label_Sweeper` | Label | `(0.00, 4.50, 9.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Stage 1 Sweeping Light |
| `Label_Stage2` | Label | `(0.00, 2.00, 24.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Stage 2: Freeze Moving Platform Across Pit 2 |
| `Label_Stage3_Trap` | Label | `(0.00, 3.50, 38.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Stage 3: Trap Light Snare (RECOVER x3) |
| `Label_Door` | Label | `(0.00, 3.00, 45.00)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Final Stealable Door |
| `Label_Exit` | Label | `(0.00, 2.00, 48.50)` | `(50, 0, 0)` | `(1.00, 1.00, 1.00)` | `IMPLEMENTATION_ASSUMPTION` | Reference label: Final Exit Zone |

### Implementation Notes
Master integration of all game mechanics. Multi-stage room mathematically forces at least 2 Recalls.

---

