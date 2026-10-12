# STEALMAN — CORE GAMEPLAY INTEGRATION GUIDE
**Owner:** Aditya (Unity Developer Lead — Core Gameplay, State Machine, Input, Integration, QA)  
**Engine:** Unity 6 LTS  
**Target:** Mobile (iOS / Android)

---

## 1. System Overview

The integration layer coordinates the Player body, the detachable Shadow, Mobile Input routing, Steal interactions, Hazard detection, and State Recovery into a cohesive loop:
```
Move → Detach → Steal → Avoid Light → Return → Escape
```

### Core Architecture Flow
```
Virtual Joystick / Action Button
              ↓
         InputSystem
              ↓
  Active Entity Routing (PlayerController vs IShadowController)
              ↓
   PlayerShadowStateMachine (Attached ↔ Detached ↔ Reuniting ↔ Trapped)
              ↓
       Gameplay Events & Feedback (FeedbackHooks, Hazards, Stealables)
```

---

## 2. State Machine (`PlayerShadowStateMachine`)

### State Enum (`PlayerShadowState`)
- `Attached`: Physical player in control. Shadow inactive.
- `Detached`: Shadow active and controllable. Physical player frozen.
- `Reuniting`: Shadow returning to player. Input blocked/reuniting.
- `Trapped`: Shadow caught in Trap Light hazard. Input routed to RECOVER.

### Public API
- `bool Detach()`: Valid only from `Attached`. Freezes player, activates shadow.
- `bool Recall()`: Valid only from `Detached`. Starts return sequence.
- `bool CompleteReunion()`: Valid only from `Reuniting`. Unfreezes player, returns to `Attached`.
- `bool Trap()`: Valid only from `Detached`. Enters `Trapped` state.
- `bool Recover()`: Valid only from `Trapped`. Enters `Reuniting` state.
- `bool Steal()`: Valid only from `Detached`. Validates steal attempt.
- `void ResetToAttached()`: Deterministically restores initial `Attached` state.
- `bool CanPerformAction(InputAction action)`: Verifies action legality.
- `event Action<PlayerShadowState> OnStateChanged`: Notifies all subscribers of transitions.

---

## 3. Input System (`InputSystem`)

### Routing Rules
1. **Joystick Movement:**
   - When `ActiveTarget == InputTarget.Player`: feeds `MoveInput` to `PlayerController.HandleMovement(MoveInput)`.
   - When `ActiveTarget == InputTarget.Shadow`: feeds `MoveInput` to `IShadowController.HandleMovement(MoveInput)` via `ShadowMovementAdapter`.
2. **Contextual Action Button (`ContextualActionButton`):**
   - Automatically switches its action and UI label based on state and environment:
     - `Attached` → **DETACH**
     - `Detached` (no target in range) → **RECALL**
     - `Detached` (stealable in range) → **STEAL**
     - `Trapped` → **RECOVER**
     - `Reuniting` → Disabled (**REUNITING...**)

---

## 4. Hazard & Trap Integration (T-008, T-054)

- **`DangerLight`**: When shadow enters volume, raises `OnShadowDestroyed`. Handled by `GameplayEventIntegration` to trigger level restart.
- **`TrapLight`**: When shadow enters volume, raises `OnShadowTrapped`. Handled by `GameplayEventIntegration` calling `stateMachine.Trap()`.
- **`RecoverHandler`**: Handles the trapped recovery loop (taps within time limit). When complete, calls `stateMachine.Recover()`. If timer expires, raises `OnRecoveryFailed` leading to restart.

---

## 5. Stealable Interaction (`StealInteractor`)

- Uses allocation-free `Physics.OverlapSphereNonAlloc` to detect nearby `StealableBase` objects.
- Alerts `ContextualActionButton` when a stealable target is in range.
- On `InputAction.Steal`, calls `target.Steal()` to transition objects to `Ghost` or `Frozen` states.
- Re-enables physical player passage. `RestoreTrigger` restores solid state after player exits.

---

## 6. M1 Critical Technical Proof (`M1TechnicalProofRunner`)

- Located in: `Assets/Scenes/Integration - finals/M1_CriticalTechnicalProof/M1_CriticalTechnicalProof.unity`.
- Run via Context Menu on `M1TechnicalProofRunner` ("Run M1 Technical Proof") or automatically on Start.
- Exercises all 8 proof criteria:
  1. Component validation
  2. Initial `Attached` state
  3. `Detach` transition & freeze
  4. `Steal` interaction & ghost door
  5. `Trap` hazard & trapped state
  6. `Recover` action & reunion
  7. Reunion completion & player restoration
  8. `Danger` light hazard hook verification

---

## 7. Automated Unit Tests

Located in `Assets/Tests/EditMode/Core/`:
- `PlayerShadowStateMachineTests.cs` (16 tests): Validates all state transitions, rejections, rapid input spam, and reset.
- `InputSystemValidationTests.cs` (5 tests): Validates target switching, input dispatch, and reunion routing.
- `TrapRecoveryValidationTests.cs` (3 tests): Validates trap event handling and recovery tap flow.
- `StealIntegrationValidationTests.cs` (3 tests): Validates stealable door states and colliders.
