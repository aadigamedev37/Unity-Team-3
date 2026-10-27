# StealMan — Level Progression & Campaign Overview

> **Campaign Scope**: 15 Levels across 3 Progressive Phases.
> **Architecture**: Built strictly in accordance with approved Level Design Specifications.
> **Generated Date**: 2026-10-14 17:33:06

---

## 1. Master Progression Matrix

| Level | Name | Phase | Difficulty | Dimensions | Target / Limit | Max Leash | Stealables | Hazards | Primary Mechanic Introduced |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **L01** | The Corridor | Phase 1 (Teach) | `0 / 10` | 5m x 15m | 5s / 10s | 0m / 25m | 0 | 0 | Basic Movement & Exit Trigger |
| **L02** | The Sandbox | Phase 1 (Teach) | `0 / 10` | 10m x 20m | 10s / 15s | 8m / 25m | 0 | 0 | Detach & Recall State Transition |
| **L03** | Ghost Gate | Phase 1 (Teach) | `2 / 10` | 5m x 15m | 15s / 20s | 5.5m / 25m | 1 | 0 | STEAL Mechanic (Door Intangibility) |
| **L04** | The Vent | Phase 1 (Teach) | `3 / 10` | 10m x 25m | 20s / 25s | 10m / 25m | 1 | 0 | Wall Traversal & Drawbridge Obstacle |
| **L05** | The Spotlight | Phase 1 (Teach) | `4 / 10` | 10m x 20m | 25s / 30s | 12m / 25m | 1 | 1 | Danger Light Hazard & OnShadowDestroyed |
| **L06** | Double Lock | Phase 2 (Combine) | `4 / 10` | 10m x 25m | 25s / 30s | 7m / 25m | 2 | 0 | Sequential Stealables (Multi-Step Body Advance) |
| **L07** | Long Detour | Phase 2 (Combine) | `5 / 10` | 15m x 30m | 25s / 30s | 21m / 25m | 1 | 1 | High Wall Detour & Extended Leash Routing |
| **L08** | Pulsing Corridor | Phase 2 (Combine) | `6 / 10` | 5m x 30m | 28s / 30s | 24m / 25m | 1 | 3 | Pulsing Light Rhythm & Safe Island Dash |
| **L09** | Split Decision | Phase 2 (Combine) | `6 / 10` | 15m x 25m | 22s / 25s | 20m / 25m | 1 | 2 | Branching Path Decision (Floor vs Wall) |
| **L10** | The Gauntlet | Phase 2 (Combine) | `7 / 10` | 15m x 40m | 30s / 35s | 24m / 25m | 2 | 3 | Multi-Hazard Gauntlet & Expert 1-Detach Route |
| **L11** | The Snare | Phase 3 (Master) | `7 / 10` | 5m x 25m | 20s / 25s | 14m / 25m | 1 | 1 | TRAP LIGHT Hazard & Rapid RECOVER Input |
| **L12** | The Conveyor | Phase 3 (Master) | `7 / 10` | 15m x 25m | 20s / 25s | 10m / 25m | 1 | 0 | Moving Platform Stealable (Freeze In Motion) |
| **L13** | The Breaker | Phase 3 (Master) | `8 / 10` | 15m x 20m | 25s / 30s | 12m / 25m | 1 | 1 | Light Switch Stealable (Wall Power Down) |
| **L14** | Threading the Needle | Phase 3 (Master) | `9 / 10` | 5m x 25m | 30s / 35s | 24.5m / 25m | 1 | 4 | Razor Thin Wall Ribbon & 24.5m Leash Limit |
| **L15** | Final Escape | Phase 3 (Master) | `10 / 10` | 20m x 50m | 38s / 40s | 18m / 25m | 3 | 2 | Grand Finale: Switch + Platform + Trap + Door |

---

## 2. Three-Phase Campaign Structure

### Phase 1: Teach (Levels 01 – 05)
- **Objective**: Isolate core verbs in safe or strictly bounded environments.
- **L01 (The Corridor)**: Baseline player locomotion, camera angle, and exit pad trigger.
- **L02 (The Sandbox)**: Detach & Recall loop without time pressure or hazards.
- **L03 (Ghost Gate)**: Introduce `Steal()` on physical Door to create Ghost state passage.
- **L04 (The Vent)**: Introduce vertical wall traversal to steal and lower a Drawbridge over a 6m pit.
- **L05 (The Spotlight)**: Introduce Danger Light hazard and failure/restart consequence.

### Phase 2: Combine (Levels 06 – 10)
- **Objective**: Combine mechanics into multi-step puzzles requiring body repositioning and rhythm.
- **L06 (Double Lock)**: Two sequential doors proving body must move into intermediate safe zone.
- **L07 (Long Detour)**: U-shaped wall traversal navigating above static danger light (21m leash).
- **L08 (Pulsing Corridor)**: Timing 3 pulsing lights across floor from safe island to safe island (24m leash).
- **L09 (Split Decision)**: Player agency — Floor route (sweeping light) vs Wall route (pulsing light).
- **L10 (The Gauntlet)**: Pit drawbridge followed immediately by dual-sweeper door corridor (supports Safe 2-Detach and Expert 1-Detach routes).

### Phase 3: Master (Levels 11 – 15)
- **Objective**: High skill ceiling, reflex challenges, rapid state recovery, and tight spatial constraints.
- **L11 (The Snare)**: Introduce Trap Light hazard requiring 3 rapid RECOVER button presses in 2.0s.
- **L12 (The Conveyor)**: Introduce rapidly sliding Platform across 8m pit; steal freezes it as bridge.
- **L13 (The Breaker)**: Introduce Light Switch high on wall to permanently disable exit danger light.
- **L14 (Threading the Needle)**: Flooded floor forces razor-thin safe wall ribbon; demands 24.5m Euclidean leash separation against 25m hard limit.
- **L15 (Final Escape)**: Master synthesis — Stage 1 Switch, Stage 2 Platform, Stage 3 Trap + Door under 38s target.

---

## 3. Leash Distance Curve & Safety Margins

| Level | Max Required Separation | Leash Limit | Safety Margin | Routing Type |
| :--- | :--- | :--- | :--- | :--- |
| L01 | 0.0m | 25.0m | N/A | Player only |
| L02 | 8.0m | 25.0m | 17.0m | Free sandbox |
| L03 | 5.5m | 25.0m | 19.5m | Direct straight line |
| L04 | 10.0m | 25.0m | 15.0m | Wall detour around pit |
| L05 | 12.0m | 25.0m | 13.0m | Direct floor dash |
| L06 | 7.0m | 25.0m | 18.0m | Reset at Safe Zone (per sub-route) |
| L07 | 21.0m | 25.0m | 4.0m | High wall U-turn detour |
| L08 | 24.0m | 25.0m | 1.0m | Linear floor dash |
| L09 | 20.0m | 25.0m | 5.0m | Symmetrical branch routes |
| L10 | 24.0m | 25.0m | 1.0m | Single-detach expert path |
| L11 | 14.0m | 25.0m | 11.0m | Trap snared path |
| L12 | 10.0m | 25.0m | 15.0m | Pit edge timing |
| L13 | 12.0m | 25.0m | 13.0m | High wall switch climb |
| L14 | **24.5m** | **25.0m** | **0.5m** | **Maximum leash stress test** |
| L15 | 18.0m | 25.0m | 7.0m | Multi-stage (exceeds 45m total; forces 2+ Recalls) |

---

## 4. Programmer Technical Handoff Notes

1. **StealableLightSwitch (Levels 13 & 15)**:
   - **Status**: Unbuilt programmer dependency (Rayaan).
   - **Current Greybox**: Modeled as wall breaker box with conduit visuals and `LightSwitchGreyboxMarker` component.
   - **Required Implementation**: When stolen via `Steal()`, fires `OnSwitchGhosted` event deactivating target `DangerLight` hazard.

2. **SweepingHazard & TrapLight Integration**:
   - Fully wired into Levels 05, 07, 08, 09, 10, 11, 13, 14, 15 using Harsh's validated hazard components.
   - Sweep angles calibrated between `30°` and `40°` at `30°/s`.
   - Trap Light requires 3 RECOVER pulses within 2.0s.

3. **StealablePlatform (Levels 12 & 15)**:
   - Configured with `moveAxis = (1, 0, 0)`, `moveDistance = 4.5m`, `moveSpeed = 3.5m/s`.
   - Freezes rigidly in place upon `Steal()`, becoming a walkable solid bridge.
