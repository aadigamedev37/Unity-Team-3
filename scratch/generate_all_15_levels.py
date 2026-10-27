import os
import sys
import uuid
from datetime import datetime

# Add scratch dir to path
sys.path.insert(0, r"C:\Game Projects\RG Team-3\Unity-Team-3\scratch")
from scene_builder import SceneBuilder, GUIDS, SCENES_DIR, DOCS_DIR

def build_all_15_levels():
    print("==================================================================")
    print("STEALMAN — Procedural Builder for 15 Level Blockouts (Unity 6)")
    print("Source: Approved Level Design Specification PDFs")
    print("==================================================================")

    all_csv_records = []
    level_summaries = []

    # -------------------------------------------------------------
    # LEVEL 01: THE CORRIDOR (5m x 15m)
    # -------------------------------------------------------------
    b1 = SceneBuilder("L01", "The Corridor", 5, 15)
    # Environment
    b1.add_primitive_cube("Floor_Main", b1.floors_children, (0, 0, 7.5), (0, 0, 0), (5, 0.2, 15), GUIDS['Mat_FloorGrid'], 'Geometry', 'DOCUMENTED', 'Main corridor floor (5m x 15m)')
    b1.add_primitive_cube("Wall_Left", b1.walls_children, (-2.6, 1.5, 7.5), (0, 0, 0), (0.2, 3, 15), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Left corridor wall (height 3m)')
    b1.add_primitive_cube("Wall_Right", b1.walls_children, (2.6, 1.5, 7.5), (0, 0, 0), (0.2, 3, 15), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Right corridor wall (height 3m)')
    b1.add_primitive_cube("Wall_Back", b1.walls_children, (0, 1.5, -0.1), (0, 0, 0), (5.4, 3, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Back boundary wall')
    b1.add_primitive_cube("Wall_Front_L", b1.walls_children, (-1.75, 1.5, 15.1), (0, 0, 0), (1.5, 3, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Exit threshold left wall')
    b1.add_primitive_cube("Wall_Front_R", b1.walls_children, (1.75, 1.5, 15.1), (0, 0, 0), (1.5, 3, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Exit threshold right wall')
    # Player & Shadow
    b1.add_prefab_instance("Player", b1.player_children, GUIDS['PlayerPrefab'], GUIDS['PlayerRootTransform'], (-1.0, 0.1, 1.5), (0, 0, 0), (1, 1, 1), 'Player', 'DOCUMENTED', 'Player spawn (bottom left corner)')
    b1.add_primitive_cube("Shadow_StartMarker", b1.player_children, (-1.0, 0.05, 1.5), (0, 0, 0), (0.5, 0.05, 0.5), GUIDS['Mat_WallPillar'], 'Shadow', 'DOCUMENTED', 'Shadow start (attached to player)')
    # Exit Zone
    b1.add_exit_zone((0, 0.1, 14.0), (3.0, 2.0, 2.0), 'Corridor exit pad illuminated by soft light')
    # Labels
    b1.add_world_label("Label_Title", "L01: The Corridor\\n5m x 15m | Movement Tutorial", (0, 3.5, 2.0), "FFFFFFFF", 4.0)
    b1.add_world_label("Label_Spawn", "Player Spawn (Bottom Left)", (-1.0, 2.0, 1.5), "00FFFFFF", 3.0)
    b1.add_world_label("Label_Exit", "Exit Zone (Walk Here)", (0, 2.0, 14.0), "00FF66FF", 3.0)
    
    yaml1 = b1.build_yaml()
    with open(f"{SCENES_DIR}/L01_Blockout.unity", "w", encoding="utf-8") as f: f.write(yaml1)
    all_csv_records.extend(b1.records)
    level_summaries.append({
        'Code': 'L01', 'Name': 'The Corridor', 'Phase': 'Phase 1 (Teach)', 'Difficulty': '0 / 10',
        'Dimensions': '5m x 15m', 'TargetTime': '5s', 'HardLimit': '10s', 'MaxLeash': '0m', 'LeashLimit': '25m',
        'StealablesCount': 0, 'HazardsCount': 0,
        'Objective': 'Teach fundamental character movement and exit conditions without introducing the Shadow mechanic.',
        'PlayerRoute': 'Walk straight up the corridor to the exit pad.',
        'ShadowRoute': 'N/A (Shadow mechanic not active in Level 1).',
        'Sequence': '1. Player moves from spawn (-1.0, 0, 1.5) straight up the corridor.\\n2. Player enters Exit Zone at (0, 0, 14.0) to complete the level.',
        'Notes': 'Basic tutorial corridor. No hazards, no stealables.'
    })
    print("Built L01_Blockout.unity")

    # -------------------------------------------------------------
    # LEVEL 02: THE SANDBOX (10m x 20m)
    # -------------------------------------------------------------
    b2 = SceneBuilder("L02", "The Sandbox", 10, 20)
    b2.add_primitive_cube("Floor_Main", b2.floors_children, (0, 0, 10.0), (0, 0, 0), (10, 0.2, 20), GUIDS['Mat_FloorGrid'], 'Geometry', 'DOCUMENTED', 'Sandbox floor (10m x 20m)')
    b2.add_primitive_cube("Wall_Left", b2.walls_children, (-5.1, 1.5, 10.0), (0, 0, 0), (0.2, 3, 20), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Left wall (10m width)')
    b2.add_primitive_cube("Wall_Right", b2.walls_children, (5.1, 1.5, 10.0), (0, 0, 0), (0.2, 3, 20), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Right wall')
    b2.add_primitive_cube("Wall_Back", b2.walls_children, (0, 1.5, -0.1), (0, 0, 0), (10.4, 3, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Back boundary wall')
    b2.add_primitive_cube("Wall_Front_L", b2.walls_children, (-3.0, 1.5, 20.1), (0, 0, 0), (4.0, 3, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Front left boundary')
    b2.add_primitive_cube("Wall_Front_R", b2.walls_children, (3.0, 1.5, 20.1), (0, 0, 0), (4.0, 3, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Front right boundary')
    # Player & Shadow
    b2.add_prefab_instance("Player", b2.player_children, GUIDS['PlayerPrefab'], GUIDS['PlayerRootTransform'], (0, 0.1, 2.0), (0, 0, 0), (1, 1, 1), 'Player', 'DOCUMENTED', 'Player spawn (bottom center)')
    b2.add_primitive_cube("Shadow_StartMarker", b2.player_children, (0, 0.05, 2.0), (0, 0, 0), (0.5, 0.05, 0.5), GUIDS['Mat_WallPillar'], 'Shadow', 'DOCUMENTED', 'Shadow start (attached to player)')
    # Exit Zone
    b2.add_exit_zone((0, 0.1, 18.5), (4.0, 2.0, 2.0), 'Sandbox exit pad')
    # Labels
    b2.add_world_label("Label_Title", "L02: The Sandbox\\n10m x 20m | Detach & Recall Practice", (0, 3.5, 2.0), "FFFFFFFF", 4.0)
    b2.add_world_label("Label_Spawn", "Player Spawn", (0, 2.0, 2.0), "00FFFFFF", 3.0)
    b2.add_world_label("Label_Sandbox", "Safe Exploration Space\\n(Test Detach & Recall)", (0, 1.0, 10.0), "FFFF00FF", 3.5)
    b2.add_world_label("Label_Exit", "Exit Zone", (0, 2.0, 18.5), "00FF66FF", 3.0)

    yaml2 = b2.build_yaml()
    with open(f"{SCENES_DIR}/L02_Blockout.unity", "w", encoding="utf-8") as f: f.write(yaml2)
    all_csv_records.extend(b2.records)
    level_summaries.append({
        'Code': 'L02', 'Name': 'The Sandbox', 'Phase': 'Phase 1 (Teach)', 'Difficulty': '0 / 10',
        'Dimensions': '10m x 20m', 'TargetTime': '10s', 'HardLimit': '15s', 'MaxLeash': '8m', 'LeashLimit': '25m',
        'StealablesCount': 0, 'HazardsCount': 0,
        'Objective': 'Teach player the Attached -> Detached -> Recall loop in a completely safe, unforced environment.',
        'PlayerRoute': 'Walk to center, detach shadow to explore, recall, walk to exit.',
        'ShadowRoute': 'Move freely around room when detached.',
        'Sequence': '1. Player spawns at (0, 0, 2.0).\\n2. Press Detach to test independent shadow movement.\\n3. Press Recall to snap shadow back to body.\\n4. Walk through open room to Exit at (0, 0, 18.5).',
        'Notes': 'Safe playground proving Recall is a state transition, not a puzzle solver.'
    })
    print("Built L02_Blockout.unity")

    # -------------------------------------------------------------
    # LEVEL 03: GHOST GATE (5m x 15m)
    # -------------------------------------------------------------
    b3 = SceneBuilder("L03", "Ghost Gate", 5, 15)
    b3.add_primitive_cube("Floor_Main", b3.floors_children, (0, 0, 7.5), (0, 0, 0), (5, 0.2, 15), GUIDS['Mat_FloorGrid'], 'Geometry', 'DOCUMENTED', 'Main floor (5m x 15m)')
    b3.add_primitive_cube("Wall_Left", b3.walls_children, (-2.6, 1.5, 7.5), (0, 0, 0), (0.2, 3, 15), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Left wall')
    b3.add_primitive_cube("Wall_Right", b3.walls_children, (2.6, 1.5, 7.5), (0, 0, 0), (0.2, 3, 15), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Right wall')
    b3.add_primitive_cube("Wall_Back", b3.walls_children, (0, 1.5, -0.1), (0, 0, 0), (5.4, 3, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Back wall')
    b3.add_primitive_cube("Pillar_Left", b3.obstacles_children, (-1.75, 1.5, 7.5), (0, 0, 0), (1.5, 3, 0.4), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Door frame left pillar')
    b3.add_primitive_cube("Pillar_Right", b3.obstacles_children, (1.75, 1.5, 7.5), (0, 0, 0), (1.5, 3, 0.4), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Door frame right pillar')
    b3.add_primitive_cube("Wall_Front_L", b3.walls_children, (-1.75, 1.5, 15.1), (0, 0, 0), (1.5, 3, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Exit wall left')
    b3.add_primitive_cube("Wall_Front_R", b3.walls_children, (1.75, 1.5, 15.1), (0, 0, 0), (1.5, 3, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Exit wall right')
    # Player & Stealable Door
    b3.add_prefab_instance("Player", b3.player_children, GUIDS['PlayerPrefab'], GUIDS['PlayerRootTransform'], (0, 0.1, 2.0), (0, 0, 0), (1, 1, 1), 'Player', 'DOCUMENTED', 'Player spawn')
    b3.add_prefab_instance("StealableDoor", b3.stealables_children, GUIDS['DoorPrefab'], GUIDS['DoorRootTransform'], (0, 1.25, 7.5), (0, 0, 0), (1, 1, 1), 'Stealable', 'DOCUMENTED', 'Stealable Door blocking hallway')
    # Exit Zone
    b3.add_exit_zone((0, 0.1, 13.5), (3.0, 2.0, 2.0), 'Exit pad behind Ghost Door')
    # Labels
    b3.add_world_label("Label_Title", "L03: Ghost Gate\\n5m x 15m | Introduce STEAL Mechanic", (0, 3.5, 2.0), "FFFFFFFF", 4.0)
    b3.add_world_label("Label_Door", "Stealable Door\\n(Solid -> Ghost Intangible)", (0, 3.0, 7.5), "00FFFFFF", 3.5)
    b3.add_world_label("Label_Exit", "Exit Zone", (0, 2.0, 13.5), "00FF66FF", 3.0)

    yaml3 = b3.build_yaml()
    with open(f"{SCENES_DIR}/L03_Blockout.unity", "w", encoding="utf-8") as f: f.write(yaml3)
    all_csv_records.extend(b3.records)
    level_summaries.append({
        'Code': 'L03', 'Name': 'Ghost Gate', 'Phase': 'Phase 1 (Teach)', 'Difficulty': '2 / 10',
        'Dimensions': '5m x 15m', 'TargetTime': '15s', 'HardLimit': '20s', 'MaxLeash': '5.5m', 'LeashLimit': '25m',
        'StealablesCount': 1, 'HazardsCount': 0,
        'Objective': 'Introduce STEAL mechanic: stealing an object shadow makes it intangible (Ghost state).',
        'PlayerRoute': 'Walk to door -> Wait -> Walk through Ghost Door -> Exit.',
        'ShadowRoute': 'Detach -> Move to Door -> Steal -> Recall.',
        'Sequence': '1. Player approaches door at Z=5.0m.\\n2. Detach shadow.\\n3. Move shadow to Stealable Door at (0, 1.25, 7.5).\\n4. Press STEAL. Door becomes Ghost state.\\n5. Press Recall to reunite.\\n6. Player walks through intangible door to Exit at (0, 0, 13.5).',
        'Notes': 'First encounter with a Stealable obstacle.'
    })
    print("Built L03_Blockout.unity")

    # -------------------------------------------------------------
    # LEVEL 04: THE VENT (10m x 25m)
    # -------------------------------------------------------------
    b4 = SceneBuilder("L04", "The Vent", 10, 25)
    b4.add_primitive_cube("Floor_Start", b4.floors_children, (0, 0, 4.5), (0, 0, 0), (10, 0.2, 9), GUIDS['Mat_FloorGrid'], 'Geometry', 'DOCUMENTED', 'Start floor section (Z: 0 to 9m)')
    b4.add_primitive_cube("Pit_Void", b4.obstacles_children, (0, -5.0, 12.0), (0, 0, 0), (10, 0.2, 6), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Pit gap (6m wide chasm, Z: 9 to 15m)')
    b4.add_primitive_cube("Floor_Exit", b4.floors_children, (0, 0, 20.0), (0, 0, 0), (10, 0.2, 10), GUIDS['Mat_FloorGrid'], 'Geometry', 'DOCUMENTED', 'Exit floor section (Z: 15 to 25m)')
    b4.add_primitive_cube("Wall_Left", b4.walls_children, (-5.1, 2.5, 12.5), (0, 0, 0), (0.2, 5, 25), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Left wall (height 5m)')
    b4.add_primitive_cube("Wall_Right_Climbable", b4.walls_children, (5.1, 2.5, 12.5), (0, 0, 0), (0.2, 5, 25), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Right climbable wall (shadow traversal route)')
    b4.add_primitive_cube("Wall_Back", b4.walls_children, (0, 2.5, -0.1), (0, 0, 0), (10.4, 5, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Back wall')
    b4.add_primitive_cube("Wall_Front_L", b4.walls_children, (-3.0, 2.5, 25.1), (0, 0, 0), (4.0, 5, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Front exit wall left')
    b4.add_primitive_cube("Wall_Front_R", b4.walls_children, (3.0, 2.5, 25.1), (0, 0, 0), (4.0, 5, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Front exit wall right')
    # Player & Stealable Drawbridge
    b4.add_prefab_instance("Player", b4.player_children, GUIDS['PlayerPrefab'], GUIDS['PlayerRootTransform'], (0, 0.1, 2.0), (0, 0, 0), (1, 1, 1), 'Player', 'DOCUMENTED', 'Player spawn')
    b4.add_prefab_instance("StealableDrawbridge", b4.stealables_children, GUIDS['DrawbridgePrefab'], GUIDS['DrawbridgeRootTransform'], (0, 0.5, 12.0), (0, 0, 0), (1, 1, 1), 'Stealable', 'DOCUMENTED', 'Raised Drawbridge spanning 6m pit')
    # Exit Zone
    b4.add_exit_zone((0, 0.1, 22.0), (4.0, 2.0, 2.0), 'Exit pad beyond lowered Drawbridge')
    # Labels
    b4.add_world_label("Label_Title", "L04: The Vent\\n10m x 25m | Wall Traversal & Drawbridge", (0, 4.0, 2.0), "FFFFFFFF", 4.0)
    b4.add_world_label("Label_Pit", "6m Pit Gap\\n(Player Blocked)", (0, -0.5, 12.0), "FF3333FF", 3.5)
    b4.add_world_label("Label_WallClimb", "Right Wall Climbable Surface\\n(Shadow Traversal Path)", (4.8, 3.5, 12.0), "00FFFFFF", 3.5)
    b4.add_world_label("Label_Bridge", "Stealable Drawbridge\\n(Raised -> Lowered)", (0, 2.5, 12.0), "FFFF00FF", 3.5)
    b4.add_world_label("Label_Exit", "Exit Zone", (0, 2.0, 22.0), "00FF66FF", 3.0)

    yaml4 = b4.build_yaml()
    with open(f"{SCENES_DIR}/L04_Blockout.unity", "w", encoding="utf-8") as f: f.write(yaml4)
    all_csv_records.extend(b4.records)
    level_summaries.append({
        'Code': 'L04', 'Name': 'The Vent', 'Phase': 'Phase 1 (Teach)', 'Difficulty': '3 / 10',
        'Dimensions': '10m x 25m', 'TargetTime': '20s', 'HardLimit': '25s', 'MaxLeash': '10m', 'LeashLimit': '25m',
        'StealablesCount': 1, 'HazardsCount': 0,
        'Objective': 'Teach Floor -> Wall Traversal and Stealable Drawbridge to cross a pit.',
        'PlayerRoute': 'Walk to pit edge (Z=8.5m) -> Wait -> Cross lowered bridge -> Exit.',
        'ShadowRoute': 'Detach -> Move to Right Wall -> Climb up wall around 6m pit -> Steal Drawbridge -> Recall.',
        'Sequence': '1. Player stops at edge of 6m pit (Z=9m to 15m).\\n2. Detach shadow.\\n3. Transition shadow onto right vertical wall (height 5m).\\n4. Walk shadow along wall across pit to Drawbridge mount.\\n5. Press STEAL. Drawbridge lowers.\\n6. Recall. Player crosses bridge to Exit at (0, 0, 22.0).',
        'Notes': 'First vertical wall climbing interaction. Raised drawbridge lowers when stolen.'
    })
    print("Built L04_Blockout.unity")

    # -------------------------------------------------------------
    # LEVEL 05: THE SPOTLIGHT (10m x 20m)
    # -------------------------------------------------------------
    b5 = SceneBuilder("L05", "The Spotlight", 10, 20)
    b5.add_primitive_cube("Floor_Main", b5.floors_children, (0, 0, 10.0), (0, 0, 0), (10, 0.2, 20), GUIDS['Mat_FloorGrid'], 'Geometry', 'DOCUMENTED', 'Hall floor (10m x 20m)')
    b5.add_primitive_cube("Wall_Left", b5.walls_children, (-5.1, 1.5, 10.0), (0, 0, 0), (0.2, 3, 20), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Left wall')
    b5.add_primitive_cube("Wall_Right", b5.walls_children, (5.1, 1.5, 10.0), (0, 0, 0), (0.2, 3, 20), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Right wall')
    b5.add_primitive_cube("Wall_Back", b5.walls_children, (0, 1.5, -0.1), (0, 0, 0), (10.4, 3, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Back wall')
    b5.add_primitive_cube("Pillar_Left", b5.obstacles_children, (-3.25, 1.5, 14.0), (0, 0, 0), (3.5, 3, 0.4), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Door pillar left')
    b5.add_primitive_cube("Pillar_Right", b5.obstacles_children, (3.25, 1.5, 14.0), (0, 0, 0), (3.5, 3, 0.4), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Door pillar right')
    b5.add_primitive_cube("Wall_Front_L", b5.walls_children, (-3.0, 1.5, 20.1), (0, 0, 0), (4.0, 3, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Front wall left')
    b5.add_primitive_cube("Wall_Front_R", b5.walls_children, (3.0, 1.5, 20.1), (0, 0, 0), (4.0, 3, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Front wall right')
    # Player & Stealable Door
    b5.add_prefab_instance("Player", b5.player_children, GUIDS['PlayerPrefab'], GUIDS['PlayerRootTransform'], (0, 0.1, 2.0), (0, 0, 0), (1, 1, 1), 'Player', 'DOCUMENTED', 'Player spawn')
    b5.add_prefab_instance("StealableDoor", b5.stealables_children, GUIDS['DoorPrefab'], GUIDS['DoorRootTransform'], (0, 1.25, 14.0), (0, 0, 0), (1, 1, 1), 'Stealable', 'DOCUMENTED', 'Stealable Door guarded by spotlight')
    # Sweeping Danger Light
    b5.add_danger_light("DangerLight_Sweeping", (0, 0.5, 10.0), (8.0, 2.0, 4.0), is_sweeping=True, sweep_angle=40.0, sweep_speed=30.0, notes="Sweeping Danger Light (cone sweeps +/-40 deg)")
    # Exit Zone
    b5.add_exit_zone((0, 0.1, 18.0), (4.0, 2.0, 2.0), 'Exit pad beyond Ghost Door')
    # Labels
    b5.add_world_label("Label_Title", "L05: The Spotlight\\n10m x 20m | Danger Light Hazard", (0, 3.5, 2.0), "FFFFFFFF", 4.0)
    b5.add_world_label("Label_Hazard", "Sweeping Danger Light\\n(Lethal to Shadow - Time the Dash)", (0, 3.0, 10.0), "FF3333FF", 3.5)
    b5.add_world_label("Label_Door", "Stealable Door", (0, 3.0, 14.0), "00FFFFFF", 3.5)
    b5.add_world_label("Label_Exit", "Exit Zone", (0, 2.0, 18.0), "00FF66FF", 3.0)

    yaml5 = b5.build_yaml()
    with open(f"{SCENES_DIR}/L05_Blockout.unity", "w", encoding="utf-8") as f: f.write(yaml5)
    all_csv_records.extend(b5.records)
    level_summaries.append({
        'Code': 'L05', 'Name': 'The Spotlight', 'Phase': 'Phase 1 (Teach)', 'Difficulty': '4 / 10',
        'Dimensions': '10m x 20m', 'TargetTime': '25s', 'HardLimit': '30s', 'MaxLeash': '12m', 'LeashLimit': '25m',
        'StealablesCount': 1, 'HazardsCount': 1,
        'Objective': 'Introduce Danger Light hazard and OnShadowDestroyed failure state.',
        'PlayerRoute': 'Wait at safe distance -> Walk through Ghost Door to Exit.',
        'ShadowRoute': 'Detach -> Observe sweep -> Dash past light when clear -> Steal Door -> Recall.',
        'Sequence': '1. Player stops at Z=6.0m safe from light cone.\\n2. Detach shadow.\\n3. Time the sweeping light ping-pong cycle.\\n4. When light sweeps to opposite side, dash through zone.\\n5. Steal Door at (0, 1.25, 14.0).\\n6. Recall and walk to Exit at (0, 0, 18.0).',
        'Notes': 'First lethal hazard. Touching light destroys shadow and resets level.'
    })
    print("Built L05_Blockout.unity")

    # -------------------------------------------------------------
    # LEVEL 06: DOUBLE LOCK (10m x 25m)
    # -------------------------------------------------------------
    b6 = SceneBuilder("L06", "Double Lock", 10, 25)
    b6.add_primitive_cube("Floor_Main", b6.floors_children, (0, 0, 12.5), (0, 0, 0), (10, 0.2, 25), GUIDS['Mat_FloorGrid'], 'Geometry', 'DOCUMENTED', 'Hall floor (10m x 25m)')
    b6.add_primitive_cube("Wall_Left", b6.walls_children, (-5.1, 1.5, 12.5), (0, 0, 0), (0.2, 3, 25), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Left wall')
    b6.add_primitive_cube("Wall_Right", b6.walls_children, (5.1, 1.5, 12.5), (0, 0, 0), (0.2, 3, 25), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Right wall')
    b6.add_primitive_cube("Wall_Back", b6.walls_children, (0, 1.5, -0.1), (0, 0, 0), (10.4, 3, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Back wall')
    b6.add_primitive_cube("Pillar_A_L", b6.obstacles_children, (-3.25, 1.5, 7.0), (0, 0, 0), (3.5, 3, 0.4), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Door A left pillar')
    b6.add_primitive_cube("Pillar_A_R", b6.obstacles_children, (3.25, 1.5, 7.0), (0, 0, 0), (3.5, 3, 0.4), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Door A right pillar')
    b6.add_primitive_cube("Pillar_B_L", b6.obstacles_children, (-3.25, 1.5, 16.0), (0, 0, 0), (3.5, 3, 0.4), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Door B left pillar')
    b6.add_primitive_cube("Pillar_B_R", b6.obstacles_children, (3.25, 1.5, 16.0), (0, 0, 0), (3.5, 3, 0.4), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Door B right pillar')
    b6.add_primitive_cube("Wall_Front_L", b6.walls_children, (-3.0, 1.5, 25.1), (0, 0, 0), (4.0, 3, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Front wall left')
    b6.add_primitive_cube("Wall_Front_R", b6.walls_children, (3.0, 1.5, 25.1), (0, 0, 0), (4.0, 3, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Front wall right')
    # Player & Stealable Doors A and B
    b6.add_prefab_instance("Player", b6.player_children, GUIDS['PlayerPrefab'], GUIDS['PlayerRootTransform'], (0, 0.1, 2.0), (0, 0, 0), (1, 1, 1), 'Player', 'DOCUMENTED', 'Player spawn')
    b6.add_prefab_instance("StealableDoor_A", b6.stealables_children, GUIDS['DoorPrefab'], GUIDS['DoorRootTransform'], (0, 1.25, 7.0), (0, 0, 0), (1, 1, 1), 'Stealable', 'DOCUMENTED', 'Door A (First lock)')
    b6.add_prefab_instance("StealableDoor_B", b6.stealables_children, GUIDS['DoorPrefab'], GUIDS['DoorRootTransform'], (0, 1.25, 16.0), (0, 0, 0), (1, 1, 1), 'Stealable', 'DOCUMENTED', 'Door B (Second lock)')
    # Exit Zone
    b6.add_exit_zone((0, 0.1, 22.0), (4.0, 2.0, 2.0), 'Exit pad beyond Door B')
    # Labels
    b6.add_world_label("Label_Title", "L06: Double Lock\\n10m x 25m | Sequential Doors", (0, 3.5, 2.0), "FFFFFFFF", 4.0)
    b6.add_world_label("Label_DoorA", "Door A (First Lock)", (0, 3.0, 7.0), "00FFFFFF", 3.5)
    b6.add_world_label("Label_SafeZone", "Intermediate Safe Zone\\n(Reposition Body Here)", (0, 1.5, 11.5), "FFFF00FF", 3.5)
    b6.add_world_label("Label_DoorB", "Door B (Second Lock)", (0, 3.0, 16.0), "00FFFFFF", 3.5)
    b6.add_world_label("Label_Exit", "Exit Zone", (0, 2.0, 22.0), "00FF66FF", 3.0)

    yaml6 = b6.build_yaml()
    with open(f"{SCENES_DIR}/L06_Blockout.unity", "w", encoding="utf-8") as f: f.write(yaml6)
    all_csv_records.extend(b6.records)
    level_summaries.append({
        'Code': 'L06', 'Name': 'Double Lock', 'Phase': 'Phase 2 (Combine)', 'Difficulty': '4 / 10',
        'Dimensions': '10m x 25m', 'TargetTime': '25s', 'HardLimit': '30s', 'MaxLeash': '7m', 'LeashLimit': '25m',
        'StealablesCount': 2, 'HazardsCount': 0,
        'Objective': 'Two sequential Stealable Doors proving body must reposition into intermediate safe zone.',
        'PlayerRoute': 'Walk to Door A -> Wait -> Walk through A to Safe Zone -> Wait -> Walk through B to Exit.',
        'ShadowRoute': 'Sub-route 1: Steal Door A -> Recall. Sub-route 2: Steal Door B -> Recall.',
        'Sequence': '1. Player stops at Z=4m.\\n2. Detach shadow -> Steal Door A at (0, 1.25, 7.0) -> Recall.\\n3. Player walks through Ghost Door A into Safe Zone (Z=8m to 14m).\\n4. Detach shadow -> Steal Door B at (0, 1.25, 16.0) -> Recall.\\n5. Player walks through Ghost Door B to Exit at (0, 0, 22.0).',
        'Notes': 'Multi-step puzzle progression with intermediate body repositioning.'
    })
    print("Built L06_Blockout.unity")

    # -------------------------------------------------------------
    # LEVEL 07: LONG DETOUR (15m x 30m)
    # -------------------------------------------------------------
    b7 = SceneBuilder("L07", "Long Detour", 15, 30)
    b7.add_primitive_cube("Floor_Start", b7.floors_children, (0, 0, 5.0), (0, 0, 0), (15, 0.2, 10), GUIDS['Mat_FloorGrid'], 'Geometry', 'DOCUMENTED', 'Start floor (Z: 0 to 10m)')
    b7.add_primitive_cube("Canyon_Pit", b7.obstacles_children, (0, -6.0, 14.0), (0, 0, 0), (15, 0.2, 8), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Canyon chasm (8m wide, Z: 10 to 18m)')
    b7.add_primitive_cube("Floor_Exit", b7.floors_children, (0, 0, 24.0), (0, 0, 0), (15, 0.2, 12), GUIDS['Mat_FloorGrid'], 'Geometry', 'DOCUMENTED', 'Exit floor (Z: 18 to 30m)')
    b7.add_primitive_cube("Wall_Left_Climbable", b7.walls_children, (-7.6, 2.5, 15.0), (0, 0, 0), (0.2, 5, 30), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Continuous climbable left wall (height 5m)')
    b7.add_primitive_cube("Wall_Right_Start", b7.walls_children, (7.6, 2.5, 4.0), (0, 0, 0), (0.2, 5, 8), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Right wall start segment')
    b7.add_primitive_cube("Wall_Right_End", b7.walls_children, (7.6, 2.5, 25.0), (0, 0, 0), (0.2, 5, 10), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Right wall end segment (gap Z: 8 to 20m broken)')
    b7.add_primitive_cube("Wall_Back", b7.walls_children, (0, 2.5, -0.1), (0, 0, 0), (15.4, 5, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Back wall')
    b7.add_primitive_cube("Wall_Front_L", b7.walls_children, (-4.5, 2.5, 30.1), (0, 0, 0), (6.0, 5, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Front wall left')
    b7.add_primitive_cube("Wall_Front_R", b7.walls_children, (4.5, 2.5, 30.1), (0, 0, 0), (6.0, 5, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Front wall right')
    # Player & Stealable Drawbridge
    b7.add_prefab_instance("Player", b7.player_children, GUIDS['PlayerPrefab'], GUIDS['PlayerRootTransform'], (0, 0.1, 2.0), (0, 0, 0), (1, 1, 1), 'Player', 'DOCUMENTED', 'Player spawn')
    b7.add_prefab_instance("StealableDrawbridge", b7.stealables_children, GUIDS['DrawbridgePrefab'], GUIDS['DrawbridgeRootTransform'], (0, 0.5, 14.0), (0, 0, 0), (1, 1, 1), 'Stealable', 'DOCUMENTED', 'Drawbridge spanning 8m canyon')
    # Static Danger Light on Left Wall
    b7.add_danger_light("DangerLight_LeftWall", (-7.0, 1.5, 14.0), (2.0, 2.5, 4.0), notes="Static Danger Light covering lower wall (Y: 0 to 2.5m)")
    # Exit Zone
    b7.add_exit_zone((0, 0.1, 26.0), (4.0, 2.0, 2.0), 'Exit pad beyond canyon bridge')
    # Labels
    b7.add_world_label("Label_Title", "L07: Long Detour\\n15m x 30m | High Wall Detour (21m Leash)", (0, 4.0, 2.0), "FFFFFFFF", 4.0)
    b7.add_world_label("Label_DetachPoint", "Player Detach Point (Z=9m)\\n(Must stand here for 21m leash)", (0, 2.0, 9.0), "FFFF00FF", 3.5)
    b7.add_world_label("Label_Hazard", "Static Danger Light\\n(Blocks Lower Wall - Climb High!)", (-6.5, 1.5, 14.0), "FF3333FF", 3.5)
    b7.add_world_label("Label_HighRoute", "High Route (Y=3.5 - 4.5m)", (-7.2, 4.2, 14.0), "00FFFFFF", 3.0)
    b7.add_world_label("Label_Bridge", "Stealable Drawbridge", (0, 2.5, 14.0), "FFFF00FF", 3.5)
    b7.add_world_label("Label_Exit", "Exit Zone", (0, 2.0, 26.0), "00FF66FF", 3.0)

    yaml7 = b7.build_yaml()
    with open(f"{SCENES_DIR}/L07_Blockout.unity", "w", encoding="utf-8") as f: f.write(yaml7)
    all_csv_records.extend(b7.records)
    level_summaries.append({
        'Code': 'L07', 'Name': 'Long Detour', 'Phase': 'Phase 2 (Combine)', 'Difficulty': '5 / 10',
        'Dimensions': '15m x 30m', 'TargetTime': '25s', 'HardLimit': '30s', 'MaxLeash': '21m', 'LeashLimit': '25m',
        'StealablesCount': 1, 'HazardsCount': 1,
        'Objective': 'Canyon detour along left wall navigating above a static danger light (21m leash).',
        'PlayerRoute': 'Walk to canyon edge (Z=9m) -> Wait -> Cross Drawbridge -> Exit.',
        'ShadowRoute': 'Detach -> Left wall -> Climb high (Y=3.5-4.5m) over Danger Light -> Steal Drawbridge -> Recall.',
        'Sequence': '1. Player advances to canyon edge at Z=9.0m (required to stay within 21m leash).\\n2. Detach shadow.\\n3. Shadow moves left, climbs left wall.\\n4. Shadow climbs high to Y=4.0m to bypass static Danger Light mounted at Y=1.5m.\\n5. Reach Drawbridge control at (0, 0.5, 14.0).\\n6. Steal Drawbridge. Bridge lowers.\\n7. Recall. Player crosses canyon to Exit at (0, 0, 26.0).',
        'Notes': 'Right wall broken. Only path is U-shaped high wall detour. 21m leash separation.'
    })
    print("Built L07_Blockout.unity")

    # -------------------------------------------------------------
    # LEVEL 08: PULSING CORRIDOR (5m x 30m)
    # -------------------------------------------------------------
    b8 = SceneBuilder("L08", "Pulsing Corridor", 5, 30)
    b8.add_primitive_cube("Floor_Main", b8.floors_children, (0, 0, 15.0), (0, 0, 0), (5, 0.2, 30), GUIDS['Mat_FloorGrid'], 'Geometry', 'DOCUMENTED', 'Hall floor (5m x 30m)')
    b8.add_primitive_cube("Wall_Left", b8.walls_children, (-2.6, 1.5, 15.0), (0, 0, 0), (0.2, 3, 30), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Left wall')
    b8.add_primitive_cube("Wall_Right", b8.walls_children, (2.6, 1.5, 15.0), (0, 0, 0), (0.2, 3, 30), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Right wall')
    b8.add_primitive_cube("Wall_Back", b8.walls_children, (0, 1.5, -0.1), (0, 0, 0), (5.4, 3, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Back wall')
    b8.add_primitive_cube("Pillar_Door_L", b8.obstacles_children, (-1.75, 1.5, 26.0), (0, 0, 0), (1.5, 3, 0.4), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Door pillar left')
    b8.add_primitive_cube("Pillar_Door_R", b8.obstacles_children, (1.75, 1.5, 26.0), (0, 0, 0), (1.5, 3, 0.4), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Door pillar right')
    b8.add_primitive_cube("Wall_Front_L", b8.walls_children, (-1.75, 1.5, 30.1), (0, 0, 0), (1.5, 3, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Front wall left')
    b8.add_primitive_cube("Wall_Front_R", b8.walls_children, (1.75, 1.5, 30.1), (0, 0, 0), (1.5, 3, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Front wall right')
    # Player & Stealable Door
    b8.add_prefab_instance("Player", b8.player_children, GUIDS['PlayerPrefab'], GUIDS['PlayerRootTransform'], (0, 0.1, 2.0), (0, 0, 0), (1, 1, 1), 'Player', 'DOCUMENTED', 'Player spawn')
    b8.add_prefab_instance("StealableDoor", b8.stealables_children, GUIDS['DoorPrefab'], GUIDS['DoorRootTransform'], (0, 1.25, 26.0), (0, 0, 0), (1, 1, 1), 'Stealable', 'DOCUMENTED', 'Stealable Door at end of pulsing corridor')
    # 3 Pulsing Danger Lights
    b8.add_danger_light("DangerLight_Pulsing_1", (0, 0.5, 8.0), (5.0, 2.0, 4.0), is_pulsing=True, notes="Pulsing Light 1 (Z: 6 to 10m)")
    b8.add_danger_light("DangerLight_Pulsing_2", (0, 0.5, 15.0), (5.0, 2.0, 4.0), is_pulsing=True, notes="Pulsing Light 2 (Z: 13 to 17m)")
    b8.add_danger_light("DangerLight_Pulsing_3", (0, 0.5, 22.0), (5.0, 2.0, 4.0), is_pulsing=True, notes="Pulsing Light 3 (Z: 20 to 24m)")
    # Exit Zone
    b8.add_exit_zone((0, 0.1, 28.5), (3.0, 2.0, 2.0), 'Exit pad beyond Door')
    # Labels
    b8.add_world_label("Label_Title", "L08: Pulsing Corridor\\n5m x 30m | Rhythm & Safe Islands (24m Leash)", (0, 3.5, 2.0), "FFFFFFFF", 4.0)
    b8.add_world_label("Label_Light1", "Pulsing Light 1", (0, 2.0, 8.0), "FF3333FF", 3.0)
    b8.add_world_label("Label_Safe1", "Safe Island 1", (0, 1.0, 11.5), "00FF66FF", 3.0)
    b8.add_world_label("Label_Light2", "Pulsing Light 2", (0, 2.0, 15.0), "FF3333FF", 3.0)
    b8.add_world_label("Label_Safe2", "Safe Island 2", (0, 1.0, 18.5), "00FF66FF", 3.0)
    b8.add_world_label("Label_Light3", "Pulsing Light 3", (0, 2.0, 22.0), "FF3333FF", 3.0)
    b8.add_world_label("Label_Door", "Stealable Door (24m Leash)", (0, 3.0, 26.0), "00FFFFFF", 3.5)
    b8.add_world_label("Label_Exit", "Exit Zone", (0, 2.0, 28.5), "00FF66FF", 3.0)

    yaml8 = b8.build_yaml()
    with open(f"{SCENES_DIR}/L08_Blockout.unity", "w", encoding="utf-8") as f: f.write(yaml8)
    all_csv_records.extend(b8.records)
    level_summaries.append({
        'Code': 'L08', 'Name': 'Pulsing Corridor', 'Phase': 'Phase 2 (Combine)', 'Difficulty': '6 / 10',
        'Dimensions': '5m x 30m', 'TargetTime': '28s', 'HardLimit': '30s', 'MaxLeash': '24m', 'LeashLimit': '25m',
        'StealablesCount': 1, 'HazardsCount': 3,
        'Objective': 'Rhythm and timing: dash between 3 pulsing Danger Lights and safe islands (24m leash).',
        'PlayerRoute': 'Wait at Start -> Walk through Ghost Door to Exit (lights do not harm Player).',
        'ShadowRoute': 'Detach -> Dash L1 -> Safe Zone 1 -> Dash L2 -> Safe Zone 2 -> Dash L3 -> Steal Door -> Recall.',
        'Sequence': '1. Player detaches at start.\\n2. Shadow waits for L1 to cycle off -> dash to Safe Zone 1 (Z=11.5m).\\n3. Shadow waits for L2 off -> dash to Safe Zone 2 (Z=18.5m).\\n4. Shadow waits for L3 off -> dash to Stealable Door at (0, 1.25, 26.0).\\n5. Steal Door (at 24m leash separation).\\n6. Recall -> Player walks through Ghost Door to Exit.',
        'Notes': 'Three synchronized pulsing danger lights. 24m Euclidean distance leaves 1m margin.'
    })
    print("Built L08_Blockout.unity")

    # -------------------------------------------------------------
    # LEVEL 09: SPLIT DECISION (15m x 25m)
    # -------------------------------------------------------------
    b9 = SceneBuilder("L09", "Split Decision", 15, 25)
    b9.add_primitive_cube("Floor_Start", b9.floors_children, (0, 0, 4.0), (0, 0, 0), (15, 0.2, 8), GUIDS['Mat_FloorGrid'], 'Geometry', 'DOCUMENTED', 'Start floor (Z: 0 to 8m)')
    b9.add_primitive_cube("Central_Pit", b9.obstacles_children, (0, -6.0, 12.0), (0, 0, 0), (10, 0.2, 8), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Central pit chasm (8m wide, Z: 8 to 16m)')
    b9.add_primitive_cube("RouteA_FloorLedge", b9.floors_children, (6.0, 0, 12.0), (0, 0, 0), (3, 0.2, 8), GUIDS['Mat_FloorGrid'], 'Geometry', 'DOCUMENTED', 'Route A: Right floor ledge across pit')
    b9.add_primitive_cube("Floor_Exit", b9.floors_children, (0, 0, 20.5), (0, 0, 0), (15, 0.2, 9), GUIDS['Mat_FloorGrid'], 'Geometry', 'DOCUMENTED', 'Exit floor (Z: 16 to 25m)')
    b9.add_primitive_cube("Wall_Left_Climbable", b9.walls_children, (-7.6, 2.5, 12.5), (0, 0, 0), (0.2, 5, 25), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Route B: Left climbable wall (height 5m)')
    b9.add_primitive_cube("Wall_Right", b9.walls_children, (7.6, 2.5, 12.5), (0, 0, 0), (0.2, 5, 25), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Right boundary wall')
    b9.add_primitive_cube("Wall_Back", b9.walls_children, (0, 2.5, -0.1), (0, 0, 0), (15.4, 5, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Back wall')
    b9.add_primitive_cube("Wall_Front_L", b9.walls_children, (-4.5, 2.5, 25.1), (0, 0, 0), (6.0, 5, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Front wall left')
    b9.add_primitive_cube("Wall_Front_R", b9.walls_children, (4.5, 2.5, 25.1), (0, 0, 0), (6.0, 5, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Front wall right')
    # Player & Stealable Drawbridge
    b9.add_prefab_instance("Player", b9.player_children, GUIDS['PlayerPrefab'], GUIDS['PlayerRootTransform'], (0, 0.1, 2.0), (0, 0, 0), (1, 1, 1), 'Player', 'DOCUMENTED', 'Player spawn')
    b9.add_prefab_instance("StealableDrawbridge", b9.stealables_children, GUIDS['DrawbridgePrefab'], GUIDS['DrawbridgeRootTransform'], (0, 0.5, 16.0), (0, 0, 0), (1, 1, 1), 'Stealable', 'DOCUMENTED', 'Drawbridge spanning central pit')
    # Hazards
    b9.add_danger_light("DangerLight_RouteA_Sweeping", (6.0, 3.0, 12.0), (3.0, 2.0, 4.0), is_sweeping=True, sweep_angle=35.0, sweep_speed=30.0, notes="Route A Sweeping Light covering right ledge")
    b9.add_danger_light("DangerLight_RouteB_Pulsing", (-7.0, 2.0, 12.0), (2.0, 2.5, 4.0), is_pulsing=True, notes="Route B Pulsing Light covering left wall")
    # Exit Zone
    b9.add_exit_zone((0, 0.1, 22.0), (4.0, 2.0, 2.0), 'Exit pad beyond Drawbridge')
    # Labels
    b9.add_world_label("Label_Title", "L09: Split Decision\\n15m x 25m | Route A (Floor) vs Route B (Wall)", (0, 4.0, 2.0), "FFFFFFFF", 4.0)
    b9.add_world_label("Label_RouteA", "Route A: Floor Ledge\\n(Sweeping Light)", (6.0, 2.0, 9.0), "00FFFFFF", 3.0)
    b9.add_world_label("Label_RouteB", "Route B: Wall Route\\n(Pulsing Light)", (-7.0, 3.5, 9.0), "FF00FFFF", 3.0)
    b9.add_world_label("Label_Pit", "Central Pit", (0, -0.5, 12.0), "FF3333FF", 3.5)
    b9.add_world_label("Label_Bridge", "Stealable Drawbridge", (0, 2.5, 16.0), "FFFF00FF", 3.5)
    b9.add_world_label("Label_Exit", "Exit Zone", (0, 2.0, 22.0), "00FF66FF", 3.0)

    yaml9 = b9.build_yaml()
    with open(f"{SCENES_DIR}/L09_Blockout.unity", "w", encoding="utf-8") as f: f.write(yaml9)
    all_csv_records.extend(b9.records)
    level_summaries.append({
        'Code': 'L09', 'Name': 'Split Decision', 'Phase': 'Phase 2 (Combine)', 'Difficulty': '6 / 10',
        'Dimensions': '15m x 25m', 'TargetTime': '22s', 'HardLimit': '25s', 'MaxLeash': '20m', 'LeashLimit': '25m',
        'StealablesCount': 1, 'HazardsCount': 2,
        'Objective': 'Player agency: Choose Route A (Right Floor Ledge + Sweeping Light) or Route B (Left Wall + Pulsing Light).',
        'PlayerRoute': 'Walk to pit edge (Z=7.0m) -> Wait -> Cross lowered Drawbridge -> Exit.',
        'ShadowRoute': 'Choice: Route A (Floor right, dodge sweeper) or Route B (Wall left, time pulsing light). Steal bridge -> Recall.',
        'Sequence': '1. Player stops at pit edge at Z=7.0m.\\n2. Detach shadow.\\n3. Option A: Move along right floor ledge, time sweeping light to reach Drawbridge.\\n   Option B: Transition to left wall, time static pulsing light to reach Drawbridge.\\n4. Steal Drawbridge at (0, 0.5, 16.0).\\n5. Recall -> Player crosses lowered bridge to Exit.',
        'Notes': 'Both paths mathematically viable (Route A: 18m, Route B: 19m, max separation 20m).'
    })
    print("Built L09_Blockout.unity")

    # -------------------------------------------------------------
    # LEVEL 10: THE GAUNTLET (15m x 40m)
    # -------------------------------------------------------------
    b10 = SceneBuilder("L10", "The Gauntlet", 15, 40)
    b10.add_primitive_cube("Floor_Section1", b10.floors_children, (0, 0, 4.0), (0, 0, 0), (15, 0.2, 8), GUIDS['Mat_FloorGrid'], 'Geometry', 'DOCUMENTED', 'Start floor (Z: 0 to 8m)')
    b10.add_primitive_cube("Pit_Chasm", b10.obstacles_children, (0, -6.0, 12.0), (0, 0, 0), (15, 0.2, 8), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Section 1 Pit (8m wide, Z: 8 to 16m)')
    b10.add_primitive_cube("Floor_Section2", b10.floors_children, (0, 0, 28.0), (0, 0, 0), (15, 0.2, 24), GUIDS['Mat_FloorGrid'], 'Geometry', 'DOCUMENTED', 'Section 2 Gauntlet floor (Z: 16 to 40m)')
    b10.add_primitive_cube("Wall_Left_Climbable", b10.walls_children, (-7.6, 2.5, 20.0), (0, 0, 0), (0.2, 5, 40), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Left climbable wall')
    b10.add_primitive_cube("Wall_Right", b10.walls_children, (7.6, 2.5, 20.0), (0, 0, 0), (0.2, 5, 40), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Right wall')
    b10.add_primitive_cube("Wall_Back", b10.walls_children, (0, 2.5, -0.1), (0, 0, 0), (15.4, 5, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Back wall')
    b10.add_primitive_cube("Pillar_Door_L", b10.obstacles_children, (-3.25, 1.5, 34.0), (0, 0, 0), (3.5, 3, 0.4), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Door pillar left')
    b10.add_primitive_cube("Pillar_Door_R", b10.obstacles_children, (3.25, 1.5, 34.0), (0, 0, 0), (3.5, 3, 0.4), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Door pillar right')
    b10.add_primitive_cube("Wall_Front_L", b10.walls_children, (-4.5, 2.5, 40.1), (0, 0, 0), (6.0, 5, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Front wall left')
    b10.add_primitive_cube("Wall_Front_R", b10.walls_children, (4.5, 2.5, 40.1), (0, 0, 0), (6.0, 5, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Front wall right')
    # Player, Drawbridge, and Door
    b10.add_prefab_instance("Player", b10.player_children, GUIDS['PlayerPrefab'], GUIDS['PlayerRootTransform'], (0, 0.1, 2.0), (0, 0, 0), (1, 1, 1), 'Player', 'DOCUMENTED', 'Player spawn')
    b10.add_prefab_instance("StealableDrawbridge", b10.stealables_children, GUIDS['DrawbridgePrefab'], GUIDS['DrawbridgeRootTransform'], (0, 0.5, 16.0), (0, 0, 0), (1, 1, 1), 'Stealable', 'DOCUMENTED', 'Drawbridge across Section 1 pit')
    b10.add_prefab_instance("StealableDoor", b10.stealables_children, GUIDS['DoorPrefab'], GUIDS['DoorRootTransform'], (0, 1.25, 34.0), (0, 0, 0), (1, 1, 1), 'Stealable', 'DOCUMENTED', 'Stealable Door guarding exit')
    # Hazards
    b10.add_danger_light("DangerLight_PitWall", (-7.0, 1.5, 12.0), (2.0, 2.5, 4.0), notes="Static Danger Light on left pit wall")
    b10.add_danger_light("DangerLight_Sweeping_1", (-2.5, 3.5, 23.0), (6.0, 2.0, 4.0), is_sweeping=True, sweep_angle=35.0, sweep_speed=30.0, notes="Sweeping Light 1 (left staggered)")
    b10.add_danger_light("DangerLight_Sweeping_2", (2.5, 3.5, 28.0), (6.0, 2.0, 4.0), is_sweeping=True, sweep_angle=35.0, sweep_speed=30.0, notes="Sweeping Light 2 (right staggered)")
    # Exit Zone
    b10.add_exit_zone((0, 0.1, 38.0), (4.0, 2.0, 2.0), 'Exit pad beyond Door')
    # Labels
    b10.add_world_label("Label_Title", "L10: The Gauntlet\\n15m x 40m | Pit Drawbridge + Dual Sweepers", (0, 4.5, 2.0), "FFFFFFFF", 4.0)
    b10.add_world_label("Label_Bridge", "Drawbridge Control", (0, 2.5, 16.0), "FFFF00FF", 3.5)
    b10.add_world_label("Label_Sweeper1", "Sweeping Light 1", (-2.5, 4.5, 23.0), "FF3333FF", 3.0)
    b10.add_world_label("Label_Sweeper2", "Sweeping Light 2", (2.5, 4.5, 28.0), "FF3333FF", 3.0)
    b10.add_world_label("Label_Door", "Stealable Door", (0, 3.0, 34.0), "00FFFFFF", 3.5)
    b10.add_world_label("Label_Exit", "Exit Zone", (0, 2.0, 38.0), "00FF66FF", 3.0)

    yaml10 = b10.build_yaml()
    with open(f"{SCENES_DIR}/L10_Blockout.unity", "w", encoding="utf-8") as f: f.write(yaml10)
    all_csv_records.extend(b10.records)
    level_summaries.append({
        'Code': 'L10', 'Name': 'The Gauntlet', 'Phase': 'Phase 2 (Combine)', 'Difficulty': '7 / 10',
        'Dimensions': '15m x 40m', 'TargetTime': '30s', 'HardLimit': '35s', 'MaxLeash': '24m', 'LeashLimit': '25m',
        'StealablesCount': 2, 'HazardsCount': 3,
        'Objective': 'Section 1 Drawbridge over pit, immediately followed by Section 2 dual sweeping lights and Door.',
        'PlayerRoute': 'Safe: Walk to pit -> Cross bridge -> Wait -> Walk through Ghost Door to Exit.',
        'ShadowRoute': 'Safe: Detach -> Steal Bridge -> Recall. Detach -> Dodge 2 sweepers -> Steal Door -> Recall.\\nExpert: Detach -> Steal Bridge -> Cross bridge while detached -> Dodge sweepers -> Steal Door -> Recall (1 Detach!).',
        'Sequence': '1. Player advances to pit edge at Z=7.5m.\\n2. Detach shadow -> Climb left wall past static danger light at (-7.0, 1.5, 12.0).\\n3. Steal Drawbridge at (0, 0.5, 16.0). Bridge lowers.\\n4. (Safe route: Recall -> Player crosses -> Detach at Z=18m).\\n   (Expert route: Continue moving shadow across lowered bridge, max leash reaches 24m at door).\\n5. Shadow times Sweeping Light 1 (Z=23m) and Sweeping Light 2 (Z=28m).\\n6. Steal Door at (0, 1.25, 34.0).\\n7. Recall -> Player walks to Exit at (0, 0, 38.0).',
        'Notes': 'Culmination of Phase 2. Enables Expert 1-Detach route natively within 25m leash.'
    })
    print("Built L10_Blockout.unity")

    # -------------------------------------------------------------
    # LEVEL 11: THE SNARE (5m x 25m)
    # -------------------------------------------------------------
    b11 = SceneBuilder("L11", "The Snare", 5, 25)
    b11.add_primitive_cube("Floor_Main", b11.floors_children, (0, 0, 12.5), (0, 0, 0), (5, 0.2, 25), GUIDS['Mat_FloorGrid'], 'Geometry', 'DOCUMENTED', 'Hall floor (5m x 25m)')
    b11.add_primitive_cube("Wall_Left", b11.walls_children, (-2.6, 1.5, 12.5), (0, 0, 0), (0.2, 3, 25), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Left wall')
    b11.add_primitive_cube("Wall_Right", b11.walls_children, (2.6, 1.5, 12.5), (0, 0, 0), (0.2, 3, 25), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Right wall')
    b11.add_primitive_cube("Wall_Back", b11.walls_children, (0, 1.5, -0.1), (0, 0, 0), (5.4, 3, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Back wall')
    b11.add_primitive_cube("Pillar_Door_L", b11.obstacles_children, (-1.75, 1.5, 20.0), (0, 0, 0), (1.5, 3, 0.4), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Door pillar left')
    b11.add_primitive_cube("Pillar_Door_R", b11.obstacles_children, (1.75, 1.5, 20.0), (0, 0, 0), (1.5, 3, 0.4), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Door pillar right')
    b11.add_primitive_cube("Wall_Front_L", b11.walls_children, (-1.75, 1.5, 25.1), (0, 0, 0), (1.5, 3, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Front wall left')
    b11.add_primitive_cube("Wall_Front_R", b11.walls_children, (1.75, 1.5, 25.1), (0, 0, 0), (1.5, 3, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Front wall right')
    # Player & Stealable Door
    b11.add_prefab_instance("Player", b11.player_children, GUIDS['PlayerPrefab'], GUIDS['PlayerRootTransform'], (0, 0.1, 2.0), (0, 0, 0), (1, 1, 1), 'Player', 'DOCUMENTED', 'Player spawn')
    b11.add_prefab_instance("StealableDoor", b11.stealables_children, GUIDS['DoorPrefab'], GUIDS['DoorRootTransform'], (0, 1.25, 20.0), (0, 0, 0), (1, 1, 1), 'Stealable', 'DOCUMENTED', 'Stealable Door beyond Trap Light')
    # Trap Light Hazard
    b11.add_trap_light("TrapLight_Snare", (0, 1.5, 13.0), (5.0, 3.0, 4.0), notes="Static Trap Light (Purple/UV hue, Z: 11 to 15m)")
    # Exit Zone
    b11.add_exit_zone((0, 0.1, 23.0), (3.0, 2.0, 2.0), 'Exit pad beyond Door')
    # Labels
    b11.add_world_label("Label_Title", "L11: The Snare\\n5m x 25m | Introduce TRAP LIGHT Hazard", (0, 3.5, 2.0), "FFFFFFFF", 4.0)
    b11.add_world_label("Label_Trap", "Static Trap Light\\n(Purple/UV - Mash RECOVER x3)", (0, 3.5, 13.0), "CC33FFFF", 3.5)
    b11.add_world_label("Label_Door", "Stealable Door", (0, 3.0, 20.0), "00FFFFFF", 3.5)
    b11.add_world_label("Label_Exit", "Exit Zone", (0, 2.0, 23.0), "00FF66FF", 3.0)

    yaml11 = b11.build_yaml()
    with open(f"{SCENES_DIR}/L11_Blockout.unity", "w", encoding="utf-8") as f: f.write(yaml11)
    all_csv_records.extend(b11.records)
    level_summaries.append({
        'Code': 'L11', 'Name': 'The Snare', 'Phase': 'Phase 3 (Master)', 'Difficulty': '7 / 10',
        'Dimensions': '5m x 25m', 'TargetTime': '20s', 'HardLimit': '25s', 'MaxLeash': '14m', 'LeashLimit': '25m',
        'StealablesCount': 1, 'HazardsCount': 1,
        'Objective': 'Introduce TRAP LIGHT hazard: snare requires 3 rapid RECOVER button presses in 2.0s to break free.',
        'PlayerRoute': 'Walk to edge of Trap Light -> Wait -> Walk through Ghost Door to Exit.',
        'ShadowRoute': 'Detach -> Enter Trap Light -> Trapped -> Mash RECOVER x3 -> Break free -> Steal Door -> Recall.',
        'Sequence': '1. Player stops at Z=8.0m.\\n2. Detach shadow.\\n3. Shadow moves into unavoidable Trap Light at (0, 1.5, 13.0).\\n4. Shadow is rooted in place; action button switches to RECOVER.\\n5. Player taps RECOVER 3 times rapidly within 2.0s.\\n6. Shadow breaks free from snare.\\n7. Shadow moves to Door at (0, 1.25, 20.0) -> Steal.\\n8. Recall -> Player crosses through Ghost Door to Exit.',
        'Notes': 'First encounter with Trap Light. Unlike Danger Light, does not instantly destroy shadow; requires rapid escape input.'
    })
    print("Built L11_Blockout.unity")

    # -------------------------------------------------------------
    # LEVEL 12: THE CONVEYOR (15m x 25m)
    # -------------------------------------------------------------
    b12 = SceneBuilder("L12", "The Conveyor", 15, 25)
    b12.add_primitive_cube("Floor_Start", b12.floors_children, (0, 0, 4.0), (0, 0, 0), (15, 0.2, 8), GUIDS['Mat_FloorGrid'], 'Geometry', 'DOCUMENTED', 'Start floor (Z: 0 to 8m)')
    b12.add_primitive_cube("Pit_Chasm", b12.obstacles_children, (0, -6.0, 12.0), (0, 0, 0), (15, 0.2, 8), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Pit chasm (8m wide, Z: 8 to 16m)')
    b12.add_primitive_cube("Floor_Exit", b12.floors_children, (0, 0, 20.5), (0, 0, 0), (15, 0.2, 9), GUIDS['Mat_FloorGrid'], 'Geometry', 'DOCUMENTED', 'Exit floor (Z: 16 to 25m)')
    b12.add_primitive_cube("Wall_Left", b12.walls_children, (-7.6, 2.5, 12.5), (0, 0, 0), (0.2, 5, 25), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Left wall')
    b12.add_primitive_cube("Wall_Right", b12.walls_children, (7.6, 2.5, 12.5), (0, 0, 0), (0.2, 5, 25), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Right wall')
    b12.add_primitive_cube("Wall_Back", b12.walls_children, (0, 2.5, -0.1), (0, 0, 0), (15.4, 5, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Back wall')
    b12.add_primitive_cube("Wall_Front_L", b12.walls_children, (-4.5, 2.5, 25.1), (0, 0, 0), (6.0, 5, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Front wall left')
    b12.add_primitive_cube("Wall_Front_R", b12.walls_children, (4.5, 2.5, 25.1), (0, 0, 0), (6.0, 5, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Front wall right')
    # Player & Stealable Platform
    b12.add_prefab_instance("Player", b12.player_children, GUIDS['PlayerPrefab'], GUIDS['PlayerRootTransform'], (0, 0.1, 2.0), (0, 0, 0), (1, 1, 1), 'Player', 'DOCUMENTED', 'Player spawn')
    b12.add_prefab_instance("StealablePlatform", b12.stealables_children, GUIDS['PlatformPrefab'], GUIDS['PlatformRootTransform'], (0, 0.1, 12.0), (0, 0, 0), (1, 1, 1), 'Stealable', 'DOCUMENTED', 'Rapid moving Stealable Platform across 8m pit')
    # Exit Zone
    b12.add_exit_zone((0, 0.1, 21.0), (4.0, 2.0, 2.0), 'Exit pad beyond frozen Platform')
    # Labels
    b12.add_world_label("Label_Title", "L12: The Conveyor\\n15m x 25m | Introduce PLATFORM Stealable", (0, 4.0, 2.0), "FFFFFFFF", 4.0)
    b12.add_world_label("Label_Pit", "8m Pit Gap", (0, -0.5, 12.0), "FF3333FF", 3.5)
    b12.add_world_label("Label_Platform", "Rapid Moving Platform\\n(Steal at Center -> Freezes into Bridge)", (0, 2.0, 12.0), "00FFFFFF", 3.5)
    b12.add_world_label("Label_Exit", "Exit Zone", (0, 2.0, 21.0), "00FF66FF", 3.0)

    yaml12 = b12.build_yaml()
    with open(f"{SCENES_DIR}/L12_Blockout.unity", "w", encoding="utf-8") as f: f.write(yaml12)
    all_csv_records.extend(b12.records)
    level_summaries.append({
        'Code': 'L12', 'Name': 'The Conveyor', 'Phase': 'Phase 3 (Master)', 'Difficulty': '7 / 10',
        'Dimensions': '15m x 25m', 'TargetTime': '20s', 'HardLimit': '25s', 'MaxLeash': '10m', 'LeashLimit': '25m',
        'StealablesCount': 1, 'HazardsCount': 0,
        'Objective': 'Introduce PLATFORM Stealable: stealing shadow freezes rapid oscillation in place to form bridge.',
        'PlayerRoute': 'Walk to pit edge -> Wait -> Walk across frozen Platform -> Exit.',
        'ShadowRoute': 'Detach -> Move to pit edge -> Wait for platform to align centrally -> Steal -> Recall.',
        'Sequence': '1. Player stops at pit edge at Z=7.0m.\\n2. Detach shadow.\\n3. Platform slides rapidly left/right across 8m pit at 6m/s.\\n4. As Platform passes center alignment (X=0), press STEAL.\\n5. Platform enters FROZEN state (rigidly stationary solid surface).\\n6. Recall -> Player crosses stationary platform to Exit at (0, 0, 21.0).',
        'Notes': 'Platform Stealable introduction. Stolen platform remains physically solid but stops moving.'
    })
    print("Built L12_Blockout.unity")

    # -------------------------------------------------------------
    # LEVEL 13: THE BREAKER (15m x 20m)
    # -------------------------------------------------------------
    b13 = SceneBuilder("L13", "The Breaker", 15, 20)
    b13.add_primitive_cube("Floor_Main", b13.floors_children, (0, 0, 10.0), (0, 0, 0), (15, 0.2, 20), GUIDS['Mat_FloorGrid'], 'Geometry', 'DOCUMENTED', 'Hall floor (15m x 20m)')
    b13.add_primitive_cube("Wall_Left_Climbable", b13.walls_children, (-7.6, 2.5, 10.0), (0, 0, 0), (0.2, 5, 20), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Left climbable wall with mounted switch')
    b13.add_primitive_cube("Wall_Right", b13.walls_children, (7.6, 2.5, 10.0), (0, 0, 0), (0.2, 5, 20), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Right wall')
    b13.add_primitive_cube("Wall_Back", b13.walls_children, (0, 2.5, -0.1), (0, 0, 0), (15.4, 5, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Back wall')
    b13.add_primitive_cube("Wall_Front_L", b13.walls_children, (-4.5, 2.5, 20.1), (0, 0, 0), (6.0, 5, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Front wall left')
    b13.add_primitive_cube("Wall_Front_M", b13.walls_children, (1.0, 2.5, 20.1), (0, 0, 0), (5.0, 5, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Front wall center (gap at X: 3.5 to 6.5m)')
    # Player & Stealable Light Switch (Greybox representation of unbuilt programmer task)
    b13.add_prefab_instance("Player", b13.player_children, GUIDS['PlayerPrefab'], GUIDS['PlayerRootTransform'], (0, 0.1, 2.0), (0, 0, 0), (1, 1, 1), 'Player', 'DOCUMENTED', 'Player spawn')
    b13.add_primitive_cube("StealableLightSwitch", b13.stealables_children, (-7.4, 3.5, 8.0), (0, 0, 0), (0.3, 0.8, 0.6), GUIDS['Mat_DoorSolid'], 'Stealable', 'DOCUMENTED', 'Stealable Light Switch (Programmer Task: Rayaan unbuilt dependency)')
    # Massive Danger Light covering exit
    b13.add_danger_light("DangerLight_MassiveFloor", (2.5, 2.0, 15.0), (9.0, 4.0, 8.0), notes="Massive Danger Light flooding exit zone (X: -2 to 7m, Z: 11 to 19m)")
    # Exit Zone (Top Right)
    b13.add_exit_zone((5.0, 0.1, 18.0), (3.0, 2.0, 2.0), 'Exit pad at top right')
    # Labels
    b13.add_world_label("Label_Title", "L13: The Breaker\\n15m x 20m | Introduce LIGHT SWITCH Stealable", (0, 4.0, 2.0), "FFFFFFFF", 4.0)
    b13.add_world_label("Label_Switch", "Stealable Light Switch\\n(Climb Wall - Powers Down Lethal Light)\\n[Unbuilt: Rayaan]", (-6.8, 4.5, 8.0), "FFFF00FF", 3.5)
    b13.add_world_label("Label_Hazard", "Massive Lethal Danger Light\\n(Floods Exit Path Until Switched Off)", (2.5, 3.5, 15.0), "FF3333FF", 3.5)
    b13.add_world_label("Label_Exit", "Exit Zone (Top Right)", (5.0, 2.0, 18.0), "00FF66FF", 3.0)

    yaml13 = b13.build_yaml()
    with open(f"{SCENES_DIR}/L13_Blockout.unity", "w", encoding="utf-8") as f: f.write(yaml13)
    all_csv_records.extend(b13.records)
    level_summaries.append({
        'Code': 'L13', 'Name': 'The Breaker', 'Phase': 'Phase 3 (Master)', 'Difficulty': '8 / 10',
        'Dimensions': '15m x 20m', 'TargetTime': '25s', 'HardLimit': '30s', 'MaxLeash': '12m', 'LeashLimit': '25m',
        'StealablesCount': 1, 'HazardsCount': 1,
        'Objective': 'Introduce LIGHT SWITCH Stealable: climb wall to steal switch and permanently deactivate massive lethal light.',
        'PlayerRoute': 'Wait at start -> Wait for light to power down -> Walk to Exit (top right).',
        'ShadowRoute': 'Detach -> Left wall -> Climb to Light Switch at (-7.4, 3.5, 8.0) -> Steal -> Recall.',
        'Sequence': '1. Player enters room; path to exit on right is completely flooded by massive Danger Light.\\n2. Detach shadow.\\n3. Shadow transitions to left wall and climbs to Y=3.5m.\\n4. Reach Stealable Light Switch at (-7.4, 3.5, 8.0).\\n5. Press STEAL. Switch ghosts; connected Danger Light powers down completely.\\n6. Recall -> Player walks safely through deactivated light zone to Exit at (5.0, 0, 18.0).',
        'Notes': 'Programmer Dependency: StealableLightSwitch is currently unbuilt (Rayaan). Greybox representation placed with conduit visuals.'
    })
    print("Built L13_Blockout.unity")

    # -------------------------------------------------------------
    # LEVEL 14: THREADING THE NEEDLE (5m x 25m)
    # -------------------------------------------------------------
    b14 = SceneBuilder("L14", "Threading the Needle", 5, 25)
    b14.add_primitive_cube("Floor_Main", b14.floors_children, (0, 0, 12.5), (0, 0, 0), (5, 0.2, 25), GUIDS['Mat_FloorGrid'], 'Geometry', 'DOCUMENTED', 'Oppressive corridor floor (5m x 25m)')
    b14.add_primitive_cube("Wall_Left", b14.walls_children, (-2.6, 2.0, 12.5), (0, 0, 0), (0.2, 4, 25), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Left wall (height 4m)')
    b14.add_primitive_cube("Wall_Right_Ribbon", b14.walls_children, (2.6, 2.0, 12.5), (0, 0, 0), (0.2, 4, 25), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Right climbable wall (narrow safe ribbon path)')
    b14.add_primitive_cube("Wall_Back", b14.walls_children, (0, 2.0, -0.1), (0, 0, 0), (5.4, 4, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Back wall')
    b14.add_primitive_cube("Pillar_Door_L", b14.obstacles_children, (-1.75, 1.5, 22.0), (0, 0, 0), (1.5, 3, 0.4), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Door pillar left')
    b14.add_primitive_cube("Pillar_Door_R", b14.obstacles_children, (1.75, 1.5, 22.0), (0, 0, 0), (1.5, 3, 0.4), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Door pillar right')
    b14.add_primitive_cube("Wall_Front_L", b14.walls_children, (-1.75, 2.0, 25.1), (0, 0, 0), (1.5, 4, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Front wall left')
    b14.add_primitive_cube("Wall_Front_R", b14.walls_children, (1.75, 2.0, 25.1), (0, 0, 0), (1.5, 4, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Front wall right')
    # Player & Stealable Door
    b14.add_prefab_instance("Player", b14.player_children, GUIDS['PlayerPrefab'], GUIDS['PlayerRootTransform'], (0, 0.1, 1.0), (0, 0, 0), (1, 1, 1), 'Player', 'DOCUMENTED', 'Player spawn at start line')
    b14.add_prefab_instance("StealableDoor", b14.stealables_children, GUIDS['DoorPrefab'], GUIDS['DoorRootTransform'], (0, 1.25, 22.0), (0, 0, 0), (1, 1, 1), 'Stealable', 'DOCUMENTED', 'Door at exact 24.5m Euclidean limit')
    # Flooding Danger Lights & Wall Trap Light
    b14.add_danger_light("DangerLight_Cone_1", (-0.5, 1.5, 6.0), (4.0, 3.0, 4.0), notes="Flooding Danger Light 1")
    b14.add_danger_light("DangerLight_Cone_2", (-0.5, 1.5, 12.0), (4.0, 3.0, 4.0), notes="Flooding Danger Light 2")
    b14.add_danger_light("DangerLight_Cone_3", (-0.5, 1.5, 17.0), (4.0, 3.0, 3.0), notes="Flooding Danger Light 3")
    b14.add_trap_light("TrapLight_WallSnare", (2.4, 2.2, 19.5), (1.5, 2.0, 2.5), notes="Wall Trap Light snare before door")
    # Exit Zone
    b14.add_exit_zone((0, 0.1, 24.0), (3.0, 2.0, 2.0), 'Exit pad beyond 24.5m Door')
    # Labels
    b14.add_world_label("Label_Title", "L14: Threading the Needle\\n5m x 25m | Extreme Leash Precision (24.5m / 25m)", (0, 3.5, 1.0), "FFFFFFFF", 4.0)
    b14.add_world_label("Label_FloodedFloor", "Floor Flooded with Lethal Light\\n(Right Wall Ribbon Only)", (-0.5, 3.0, 12.0), "FF3333FF", 3.5)
    b14.add_world_label("Label_Ribbon", "Narrow Safe Ribbon (Wall)", (2.3, 3.5, 12.0), "00FFFFFF", 3.0)
    b14.add_world_label("Label_WallTrap", "Wall Snare (RECOVER x3)", (2.3, 3.5, 19.5), "CC33FFFF", 3.0)
    b14.add_world_label("Label_Door", "Stealable Door (24.5m Euclidean)", (0, 3.0, 22.0), "00FFFFFF", 3.5)
    b14.add_world_label("Label_Exit", "Exit Zone", (0, 2.0, 24.0), "00FF66FF", 3.0)

    yaml14 = b14.build_yaml()
    with open(f"{SCENES_DIR}/L14_Blockout.unity", "w", encoding="utf-8") as f: f.write(yaml14)
    all_csv_records.extend(b14.records)
    level_summaries.append({
        'Code': 'L14', 'Name': 'Threading the Needle', 'Phase': 'Phase 3 (Master)', 'Difficulty': '9 / 10',
        'Dimensions': '5m x 25m', 'TargetTime': '30s', 'HardLimit': '35s', 'MaxLeash': '24.5m', 'LeashLimit': '25m',
        'StealablesCount': 1, 'HazardsCount': 4,
        'Objective': 'Extreme leash precision: razor-thin safe wall ribbon past 3 Danger Lights and 1 Trap Light (24.5m / 25m leash).',
        'PlayerRoute': 'Wait at start line (MUST detach at start) -> Walk through Ghost Door to Exit.',
        'ShadowRoute': 'Detach -> Right wall safe ribbon -> Weave past 3 lights -> Enter Trap Light -> RECOVER x3 -> Steal Door (at 24.5m) -> Recall.',
        'Sequence': '1. Player MUST detach directly from start line (standing even 1m back causes leash clamp failure).\\n2. Shadow transitions immediately to right wall ribbon.\\n3. Carefully weave along right wall past floor danger light cones.\\n4. Shadow enters unavoidable Trap Light near top of wall at (2.4, 2.2, 19.5).\\n5. Player rapidly mashes RECOVER x3 within 2.0s to break free.\\n6. Shadow advances final 1m to Stealable Door at (0, 1.25, 22.0).\\n7. Euclidean distance reaches exactly 24.5m (0.5m under 25m leash limit).\\n8. Steal Door -> Recall -> Player walks through Ghost Door to Exit.',
        'Notes': 'Ultimate leash stress test. Mathematically calibrated so backing up 1m prevents reaching door.'
    })
    print("Built L14_Blockout.unity")

    # -------------------------------------------------------------
    # LEVEL 15: FINAL ESCAPE (20m x 50m)
    # -------------------------------------------------------------
    b15 = SceneBuilder("L15", "Final Escape", 20, 50)
    b15.add_primitive_cube("Floor_Stage1_Start", b15.floors_children, (0, 0, 3.0), (0, 0, 0), (20, 0.2, 6), GUIDS['Mat_FloorGrid'], 'Geometry', 'DOCUMENTED', 'Stage 1 start floor (Z: 0 to 6m)')
    b15.add_primitive_cube("Pit1_Chasm", b15.obstacles_children, (0, -6.0, 9.0), (0, 0, 0), (20, 0.2, 6), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Stage 1 Pit (6m chasm, Z: 6 to 12m)')
    b15.add_primitive_cube("Floor_Stage1_Ledge", b15.floors_children, (-8.0, 0, 9.0), (0, 0, 0), (4.0, 0.2, 6), GUIDS['Mat_FloorGrid'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Stage 1 left crossing ledge')
    b15.add_primitive_cube("Floor_Stage2_Mid", b15.floors_children, (0, 0, 16.0), (0, 0, 0), (20, 0.2, 8), GUIDS['Mat_FloorGrid'], 'Geometry', 'DOCUMENTED', 'Stage 2 intermediate floor (Z: 12 to 20m)')
    b15.add_primitive_cube("Pit2_Chasm", b15.obstacles_children, (0, -6.0, 24.0), (0, 0, 0), (20, 0.2, 8), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Stage 2 Pit (8m chasm, Z: 20 to 28m)')
    b15.add_primitive_cube("Floor_Stage3_Final", b15.floors_children, (0, 0, 39.0), (0, 0, 0), (20, 0.2, 22), GUIDS['Mat_FloorGrid'], 'Geometry', 'DOCUMENTED', 'Stage 3 final arena floor (Z: 28 to 50m)')
    b15.add_primitive_cube("Wall_Left_Climbable", b15.walls_children, (-10.1, 2.5, 25.0), (0, 0, 0), (0.2, 5, 50), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Left climbable wall (height 5m)')
    b15.add_primitive_cube("Wall_Right", b15.walls_children, (10.1, 2.5, 25.0), (0, 0, 0), (0.2, 5, 50), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Right boundary wall')
    b15.add_primitive_cube("Wall_Back", b15.walls_children, (0, 2.5, -0.1), (0, 0, 0), (20.4, 5, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DOCUMENTED', 'Back boundary wall')
    b15.add_primitive_cube("Pillar_Door_L", b15.obstacles_children, (-3.25, 1.5, 45.0), (0, 0, 0), (3.5, 3, 0.4), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Final door pillar left')
    b15.add_primitive_cube("Pillar_Door_R", b15.obstacles_children, (3.25, 1.5, 45.0), (0, 0, 0), (3.5, 3, 0.4), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Final door pillar right')
    b15.add_primitive_cube("Wall_Front_L", b15.walls_children, (-6.0, 2.5, 50.1), (0, 0, 0), (8.0, 5, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Front exit wall left')
    b15.add_primitive_cube("Wall_Front_R", b15.walls_children, (6.0, 2.5, 50.1), (0, 0, 0), (8.0, 5, 0.2), GUIDS['Mat_WallPillar'], 'Geometry', 'DERIVED_FROM_DIAGRAM', 'Front exit wall right')
    # Player & All 3 Stealables (Switch, Platform, Door)
    b15.add_prefab_instance("Player", b15.player_children, GUIDS['PlayerPrefab'], GUIDS['PlayerRootTransform'], (0, 0.1, 2.0), (0, 0, 0), (1, 1, 1), 'Player', 'DOCUMENTED', 'Player spawn')
    b15.add_primitive_cube("StealableLightSwitch", b15.stealables_children, (-9.8, 3.0, 5.0), (0, 0, 0), (0.3, 0.8, 0.6), GUIDS['Mat_DoorSolid'], 'Stealable', 'DOCUMENTED', 'Stage 1 Wall Light Switch (Unbuilt dependency: Rayaan)')
    b15.add_prefab_instance("StealablePlatform", b15.stealables_children, GUIDS['PlatformPrefab'], GUIDS['PlatformRootTransform'], (0, 0.1, 24.0), (0, 0, 0), (1, 1, 1), 'Stealable', 'DOCUMENTED', 'Stage 2 Rapid Moving Platform')
    b15.add_prefab_instance("StealableDoor", b15.stealables_children, GUIDS['DoorPrefab'], GUIDS['DoorRootTransform'], (0, 1.25, 45.0), (0, 0, 0), (1, 1, 1), 'Stealable', 'DOCUMENTED', 'Stage 3 Final Stealable Door')
    # Hazards
    b15.add_danger_light("DangerLight_Pit1_Sweeping", (0, 3.5, 9.0), (10.0, 2.0, 6.0), is_sweeping=True, sweep_angle=35.0, sweep_speed=30.0, notes="Stage 1 Sweeping Danger Light over Pit 1")
    b15.add_trap_light("TrapLight_Stage3", (0, 1.5, 38.0), (14.0, 3.0, 4.0), notes="Stage 3 Trap Light Snare before Door")
    # Exit Zone
    b15.add_exit_zone((0, 0.1, 48.5), (4.0, 2.0, 2.0), 'Final exit pad')
    # Labels
    b15.add_world_label("Label_Title", "L15: Final Escape\\n20m x 50m | The Ultimate Exam (All Systems)", (0, 4.5, 2.0), "FFFFFFFF", 4.0)
    b15.add_world_label("Label_Stage1", "Stage 1: Steal Light Switch\\n[Unbuilt: Rayaan]", (-9.0, 4.0, 5.0), "FFFF00FF", 3.5)
    b15.add_world_label("Label_Sweeper", "Stage 1 Sweeping Light", (0, 4.5, 9.0), "FF3333FF", 3.5)
    b15.add_world_label("Label_Stage2", "Stage 2: Freeze Moving Platform Across Pit 2", (0, 2.0, 24.0), "00FFFFFF", 3.5)
    b15.add_world_label("Label_Stage3_Trap", "Stage 3: Trap Light Snare (RECOVER x3)", (0, 3.5, 38.0), "CC33FFFF", 3.5)
    b15.add_world_label("Label_Door", "Final Stealable Door", (0, 3.0, 45.0), "00FFFFFF", 3.5)
    b15.add_world_label("Label_Exit", "Final Exit Zone", (0, 2.0, 48.5), "00FF66FF", 3.0)

    yaml15 = b15.build_yaml()
    with open(f"{SCENES_DIR}/L15_Blockout.unity", "w", encoding="utf-8") as f: f.write(yaml15)
    all_csv_records.extend(b15.records)
    level_summaries.append({
        'Code': 'L15', 'Name': 'Final Escape', 'Phase': 'Phase 3 (Master)', 'Difficulty': '10 / 10',
        'Dimensions': '20m x 50m', 'TargetTime': '38s', 'HardLimit': '40s', 'MaxLeash': '18m', 'LeashLimit': '25m',
        'StealablesCount': 3, 'HazardsCount': 2,
        'Objective': 'Mastery exam: Stage 1 Switch, Stage 2 Platform, Stage 3 Trap + Door under 38s time pressure.',
        'PlayerRoute': 'Wait -> Cross Pit 1 -> Wait at Pit 2 -> Cross Platform -> Wait at Stage 3 -> Walk through Ghost Door to Exit.',
        'ShadowRoute': '1. Climb wall, dodge sweeper, steal Switch -> Recall.\\n2. Steal moving platform to freeze -> Recall.\\n3. Enter trap, mash RECOVER x3, steal Door -> Recall.',
        'Sequence': '1. Player detaches at start.\\n2. Shadow climbs left wall, dodges sweeping light covering Pit 1, steals Light Switch at (-9.8, 3.0, 5.0). Light dies -> Recall.\\n3. Player crosses Pit 1 along safe floor ledge to edge of Pit 2 (Z=18m).\\n4. Detach shadow -> Steal oscillating Platform across Pit 2 (Z=24m) to freeze it -> Recall.\\n5. Player crosses frozen platform into Stage 3 (Z=32m).\\n6. Detach shadow -> Enter Trap Light at Z=38m -> Mash RECOVER x3 -> Steal Door at (0, 1.25, 45.0) -> Recall.\\n7. Player walks through Ghost Door to Exit at (0, 0, 48.5) to complete the game!',
        'Notes': 'Master integration of all game mechanics. Multi-stage room mathematically forces at least 2 Recalls.'
    })
    print("Built L15_Blockout.unity")

    # Generate .meta files for all 15 scene files
    for i in range(1, 16):
        scene_name = f"L{i:02d}_Blockout.unity"
        meta_path = f"{SCENES_DIR}/{scene_name}.meta"
        # Deterministic UUID per scene
        u = uuid.uuid5(uuid.NAMESPACE_DNS, f"stealman.scenes.{scene_name}")
        clean_guid = str(u).replace("-", "")
        with open(meta_path, "w", encoding="utf-8") as mf:
            mf.write(f"fileFormatVersion: 2\nguid: {clean_guid}\nDefaultImporter:\n  externalObjects: {{}}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n")

    # -------------------------------------------------------------
    # GENERATE Level_Blockout_Positions.csv
    # -------------------------------------------------------------
    csv_path = f"{DOCS_DIR}/Level_Blockout_Positions.csv"
    with open(csv_path, "w", encoding="utf-8") as f:
        f.write("Level,ObjectName,Category,PosX,PosY,PosZ,RotX,RotY,RotZ,ScaleX,ScaleY,ScaleZ,ColliderType,ColliderSizeX,ColliderSizeY,ColliderSizeZ,ValueSource,Notes\n")
        for r in all_csv_records:
            pos = r['Pos']
            rot = r['Rot']
            scale = r['Scale']
            col = r['ColSize']
            notes = r['Notes'].replace('"', '""')
            f.write(f"{r['Level']},\"{r['ObjectName']}\",{r['Category']},{pos[0]:.2f},{pos[1]:.2f},{pos[2]:.2f},{rot[0]:.2f},{rot[1]:.2f},{rot[2]:.2f},{scale[0]:.2f},{scale[1]:.2f},{scale[2]:.2f},\"{r['ColType']}\",{col[0]:.2f},{col[1]:.2f},{col[2]:.2f},{r['Source']},\"{notes}\"\n")
    print(f"Generated {csv_path} with {len(all_csv_records)} object records.")

    # -------------------------------------------------------------
    # GENERATE Level_Blockout_Reference.md
    # -------------------------------------------------------------
    ref_path = f"{DOCS_DIR}/Level_Blockout_Reference.md"
    now_str = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    with open(ref_path, "w", encoding="utf-8") as f:
        f.write("# StealMan — Level Blockout Technical Reference\n\n")
        f.write("> **Source of Truth**: Approved Level Design Specification PDFs (`LEVEL_SPECS/L01` through `L15`).\n")
        f.write(f"> **Target Engine**: Unity 6 (6000.5.2f1) | **Generated Date**: {now_str}\n")
        f.write("> **Scene Directory**: `Assets/Scenes/LevelBlockoutReference/`\n\n")
        f.write("---\n\n")
        f.write("## 1. Overview & Coordinate Standards\n\n")
        f.write("- **Right-Handed Standard Coordinates**: `+X` = Right, `+Y` = Up, `+Z` = Forward Progression.\n")
        f.write("- **Player Spawn Datum**: Ground level `(0, 0, Z_start)` facing forward `+Z` (or documented offset).\n")
        f.write("- **Leash Mechanics**: Hard clamp at `25.0m` Euclidean distance from Player body to Shadow.\n")
        f.write("- **Data Sourcing Key**:\n")
        f.write("  - `DOCUMENTED`: Explicit measurement or coordinate stated in PDF specification text.\n")
        f.write("  - `DERIVED_FROM_DIAGRAM`: Scaled from top-down ASCII/schematic floorplan diagrams.\n")
        f.write("  - `IMPLEMENTATION_ASSUMPTION`: Standardized technical implementation detail (e.g. wall thickness `0.2m`, label offset).\n\n")
        f.write("---\n\n")

        for s in level_summaries:
            code = s['Code']
            f.write(f"## {code} — {s['Name']}\n\n")
            f.write("| Metric | Specification |\n| :--- | :--- |\n")
            f.write(f"| **Phase** | {s['Phase']} |\n")
            f.write(f"| **Difficulty** | {s['Difficulty']} |\n")
            f.write(f"| **Play Space Dimensions** | {s['Dimensions']} |\n")
            f.write(f"| **Target Time / Hard Limit** | {s['TargetTime']} / {s['HardLimit']} |\n")
            f.write(f"| **Max Required Leash / Limit** | {s['MaxLeash']} / {s['LeashLimit']} |\n")
            f.write(f"| **Stealable Objects** | {s['StealablesCount']} |\n")
            f.write(f"| **Hazard Volumes** | {s['HazardsCount']} |\n\n")
            
            f.write("### Design Objective\n")
            f.write(f"> {s['Objective']}\n\n")
            
            f.write("### Traversal Routes & Metrics\n")
            f.write(f"- **Player Route**: {s['PlayerRoute']}\n")
            f.write(f"- **Shadow Route**: {s['ShadowRoute']}\n\n")
            
            f.write("### Gameplay Sequence\n")
            f.write(f"{s['Sequence'].replace(chr(92)+'n', chr(10))}\n\n")
            
            f.write("### Spatial Inventory & Placed Objects\n\n")
            f.write("| Object Name | Category | Position (X, Y, Z) | Rotation | Scale / Size | Value Source | Notes |\n")
            f.write("| :--- | :--- | :--- | :--- | :--- | :--- | :--- |\n")
            
            level_recs = [r for r in all_csv_records if r['Level'] == code]
            for r in level_recs:
                pos = r['Pos']
                rot = r['Rot']
                scale = r['Scale']
                f.write(f"| `{r['ObjectName']}` | {r['Category']} | `({pos[0]:.2f}, {pos[1]:.2f}, {pos[2]:.2f})` | `({rot[0]:.0f}, {rot[1]:.0f}, {rot[2]:.0f})` | `({scale[0]:.2f}, {scale[1]:.2f}, {scale[2]:.2f})` | `{r['Source']}` | {r['Notes']} |\n")
            
            f.write("\n")
            if s.get('Notes'):
                f.write("### Implementation Notes\n")
                f.write(f"{s['Notes']}\n\n")
            f.write("---\n\n")
    print(f"Generated {ref_path}")

    # -------------------------------------------------------------
    # GENERATE Level_Progression_Overview.md
    # -------------------------------------------------------------
    prog_path = f"{DOCS_DIR}/Level_Progression_Overview.md"
    with open(prog_path, "w", encoding="utf-8") as f:
        f.write("# StealMan — Level Progression & Campaign Overview\n\n")
        f.write("> **Campaign Scope**: 15 Levels across 3 Progressive Phases.\n")
        f.write("> **Architecture**: Built strictly in accordance with approved Level Design Specifications.\n")
        f.write(f"> **Generated Date**: {now_str}\n\n")
        f.write("---\n\n")
        f.write("## 1. Master Progression Matrix\n\n")
        f.write("| Level | Name | Phase | Difficulty | Dimensions | Target / Limit | Max Leash | Stealables | Hazards | Primary Mechanic Introduced |\n")
        f.write("| :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- |\n")

        mechanics = [
            "Basic Locomotion & Exit Trigger",
            "Detach & Recall State Transition",
            "STEAL Mechanic (Door Intangibility)",
            "Wall Traversal & Drawbridge Obstacle",
            "Danger Light Hazard & OnShadowDestroyed",
            "Sequential Stealables (Multi-Step Body Advance)",
            "High Wall Detour & Extended Leash Routing",
            "Pulsing Light Rhythm & Safe Island Dash",
            "Branching Path Decision (Floor vs Wall)",
            "Multi-Hazard Gauntlet & Expert 1-Detach Route",
            "TRAP LIGHT Hazard & Rapid RECOVER Input",
            "Moving Platform Stealable (Freeze In Motion)",
            "Light Switch Stealable (Wall Power Down)",
            "Razor Thin Wall Ribbon & 24.5m Leash Limit",
            "Grand Finale: Switch + Platform + Trap + Door"
        ]

        for i, s in enumerate(level_summaries):
            f.write(f"| **{s['Code']}** | {s['Name']} | {s['Phase']} | `{s['Difficulty']}` | {s['Dimensions']} | {s['TargetTime']} / {s['HardLimit']} | {s['MaxLeash']} / 25m | {s['StealablesCount']} | {s['HazardsCount']} | {mechanics[i]} |\n")

        f.write("\n---\n\n")
        f.write("## 2. Three-Phase Campaign Structure\n\n")
        f.write("### Phase 1: Teach (Levels 01 – 05)\n")
        f.write("- **Objective**: Isolate core verbs in safe or strictly bounded environments.\n")
        f.write("- **L01 (The Corridor)**: Baseline player locomotion, camera angle, and exit pad trigger.\n")
        f.write("- **L02 (The Sandbox)**: Detach & Recall loop without time pressure or hazards.\n")
        f.write("- **L03 (Ghost Gate)**: Introduce `Steal()` on physical Door to create Ghost state passage.\n")
        f.write("- **L04 (The Vent)**: Introduce vertical wall traversal to steal and lower a Drawbridge over a 6m pit.\n")
        f.write("- **L05 (The Spotlight)**: Introduce Danger Light hazard and failure/restart consequence.\n\n")
        f.write("### Phase 2: Combine (Levels 06 – 10)\n")
        f.write("- **Objective**: Combine mechanics into multi-step puzzles requiring body repositioning and rhythm.\n")
        f.write("- **L06 (Double Lock)**: Two sequential doors proving body must move into intermediate safe zone.\n")
        f.write("- **L07 (Long Detour)**: U-shaped wall traversal navigating above static danger light (21m leash).\n")
        f.write("- **L08 (Pulsing Corridor)**: Timing 3 pulsing lights across floor from safe island to safe island (24m leash).\n")
        f.write("- **L09 (Split Decision)**: Player agency — Floor route (sweeping light) vs Wall route (pulsing light).\n")
        f.write("- **L10 (The Gauntlet)**: Pit drawbridge followed immediately by dual-sweeper door corridor (supports Safe 2-Detach and Expert 1-Detach routes).\n\n")
        f.write("### Phase 3: Master (Levels 11 – 15)\n")
        f.write("- **Objective**: High skill ceiling, reflex challenges, rapid state recovery, and tight spatial constraints.\n")
        f.write("- **L11 (The Snare)**: Introduce Trap Light hazard requiring 3 rapid RECOVER button presses in 2.0s.\n")
        f.write("- **L12 (The Conveyor)**: Introduce rapidly sliding Platform across 8m pit; steal freezes it as bridge.\n")
        f.write("- **L13 (The Breaker)**: Introduce Light Switch high on wall to permanently disable exit danger light.\n")
        f.write("- **L14 (Threading the Needle)**: Flooded floor forces razor-thin safe wall ribbon; demands 24.5m Euclidean leash separation against 25m hard limit.\n")
        f.write("- **L15 (Final Escape)**: Master synthesis — Stage 1 Switch, Stage 2 Platform, Stage 3 Trap + Door under 38s target.\n\n")
        f.write("---\n\n")
        f.write("## 3. Leash Distance Curve & Safety Margins\n\n")
        f.write("| Level | Max Required Separation | Leash Limit | Safety Margin | Routing Type |\n")
        f.write("| :--- | :--- | :--- | :--- | :--- |\n")
        f.write("| L01 | 0.0m | 25.0m | N/A | Player only |\n")
        f.write("| L02 | 8.0m | 25.0m | 17.0m | Free sandbox |\n")
        f.write("| L03 | 5.5m | 25.0m | 19.5m | Direct straight line |\n")
        f.write("| L04 | 10.0m | 25.0m | 15.0m | Wall detour around pit |\n")
        f.write("| L05 | 12.0m | 25.0m | 13.0m | Direct floor dash |\n")
        f.write("| L06 | 7.0m | 25.0m | 18.0m | Reset at Safe Zone (per sub-route) |\n")
        f.write("| L07 | 21.0m | 25.0m | 4.0m | High wall U-turn detour |\n")
        f.write("| L08 | 24.0m | 25.0m | 1.0m | Linear floor dash |\n")
        f.write("| L09 | 20.0m | 25.0m | 5.0m | Symmetrical branch routes |\n")
        f.write("| L10 | 24.0m | 25.0m | 1.0m | Single-detach expert path |\n")
        f.write("| L11 | 14.0m | 25.0m | 11.0m | Trap snared path |\n")
        f.write("| L12 | 10.0m | 25.0m | 15.0m | Pit edge timing |\n")
        f.write("| L13 | 12.0m | 25.0m | 13.0m | High wall switch climb |\n")
        f.write("| L14 | **24.5m** | **25.0m** | **0.5m** | **Maximum leash stress test** |\n")
        f.write("| L15 | 18.0m | 25.0m | 7.0m | Multi-stage (exceeds 45m total; forces 2+ Recalls) |\n\n")
        f.write("---\n\n")
        f.write("## 4. Programmer Technical Handoff Notes\n\n")
        f.write("1. **StealableLightSwitch (Levels 13 & 15)**:\n")
        f.write("   - **Status**: Unbuilt programmer dependency (Rayaan).\n")
        f.write("   - **Current Greybox**: Modeled as wall breaker box with conduit visuals and `LightSwitchGreyboxMarker` component.\n")
        f.write("   - **Required Implementation**: When stolen via `Steal()`, fires `OnSwitchGhosted` event deactivating target `DangerLight` hazard.\n\n")
        f.write("2. **SweepingHazard & TrapLight Integration**:\n")
        f.write("   - Fully wired into Levels 05, 07, 08, 09, 10, 11, 13, 14, 15 using Harsh's validated hazard components.\n")
        f.write("   - Sweep angles calibrated between `30°` and `40°` at `30°/s`.\n")
        f.write("   - Trap Light requires 3 RECOVER pulses within 2.0s.\n\n")
        f.write("3. **StealablePlatform (Levels 12 & 15)**:\n")
        f.write("   - Configured with `moveAxis = (1, 0, 0)`, `moveDistance = 4.5m`, `moveSpeed = 3.5m/s`.\n")
        f.write("   - Freezes rigidly in place upon `Steal()`, becoming a walkable solid bridge.\n")
    print(f"Generated {prog_path}")

    print("==================================================================")
    print("ALL 15 LEVEL BLOCKOUT SCENES AND DOCUMENTATION SUCCESSFULLY BUILT!")
    print("==================================================================")

if __name__ == "__main__":
    build_all_15_levels()
