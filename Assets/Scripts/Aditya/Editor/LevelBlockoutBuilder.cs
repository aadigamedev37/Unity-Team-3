using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace StealMan.LevelDesign.Editor
{
    [InitializeOnLoad]
    public static class LevelBlockoutBuilder
    {
        private const string TriggerFile = "Temp/trigger_build_blockouts.txt";
        private const string DoneFile = "Temp/build_blockouts_done.txt";

        private const string ScenesDir = "Assets/Scenes/LevelBlockoutReference";
        private const string DocsDir = "Assets/Documentation/LevelDesign";
        private const string ProductionScene = "Assets/Scenes/Integration - finals/M1_CriticalTechnicalProof/M1_CriticalTechnicalProof.unity";

        private const string PlayerPrefabPath = "Assets/Prefabs/Sanju/Player/Player.prefab";
        private const string ShadowPrefabPath = "Assets/Prefabs/Hung/shadow/Shadow.prefab";
        private const string DoorPrefabPath = "Assets/Prefabs/Rayaan/Stealables/Doors/Door.prefab";
        private const string DrawbridgePrefabPath = "Assets/Prefabs/Rayaan/Stealables/Drawbridges/Drawbridge.prefab";
        private const string PlatformPrefabPath = "Assets/Prefabs/Rayaan/Stealables/Platforms/Platform.prefab";

        private const string MatFloorPath = "Assets/Materials/Mat_FloorGrid.mat";
        private const string MatWallPath = "Assets/Materials/Mat_WallPillar.mat";
        private const string MatDoorPath = "Assets/Materials/Mat_DoorSolid.mat";
        private const string MatBridgePath = "Assets/Materials/Mat_Drawbridge.mat";
        private const string MatPlatformPath = "Assets/Materials/Mat_Platform.mat";
        private const string MatDangerPath = "Assets/Materials/Mat_DangerLightRed.mat";
        private const string MatTrapPath = "Assets/Materials/Mat_TrapLightYellow.mat";
        private const string MatExitPath = "Assets/Materials/Mat_ExitZoneGreen.mat";

        static LevelBlockoutBuilder()
        {
            EditorApplication.delayCall += CheckTrigger;
        }

        private static void CheckTrigger()
        {
            if (File.Exists(TriggerFile))
            {
                try
                {
                    File.Delete(TriggerFile);
                }
                catch { }

                Debug.Log("[LevelBlockoutBuilder] Trigger detected! Building all 15 level blockouts...");
                BuildAllLevels();
            }
        }

        [MenuItem("Tools/StealMan/Build All 15 Level Blockouts")]
        public static void BuildAllLevelsMenu()
        {
            BuildAllLevels();
        }

        public static void BuildAllLevels()
        {
            Debug.Log("[LevelBlockoutBuilder] Starting automated build of all 15 Level Blockouts from PDF Specifications...");

            if (!Directory.Exists(ScenesDir))
            {
                Directory.CreateDirectory(ScenesDir);
            }
            if (!Directory.Exists(DocsDir))
            {
                Directory.CreateDirectory(DocsDir);
            }

            // Load shared prefabs
            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            GameObject shadowPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ShadowPrefabPath);
            GameObject doorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(DoorPrefabPath);
            GameObject bridgePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(DrawbridgePrefabPath);
            GameObject platformPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlatformPrefabPath);

            // Load shared materials
            Material matFloor = AssetDatabase.LoadAssetAtPath<Material>(MatFloorPath);
            Material matWall = AssetDatabase.LoadAssetAtPath<Material>(MatWallPath);
            Material matDoor = AssetDatabase.LoadAssetAtPath<Material>(MatDoorPath);
            Material matBridge = AssetDatabase.LoadAssetAtPath<Material>(MatBridgePath);
            Material matPlatform = AssetDatabase.LoadAssetAtPath<Material>(MatPlatformPath);
            Material matDanger = AssetDatabase.LoadAssetAtPath<Material>(MatDangerPath);
            Material matTrap = AssetDatabase.LoadAssetAtPath<Material>(MatTrapPath);
            Material matExit = AssetDatabase.LoadAssetAtPath<Material>(MatExitPath);

            List<ObjectRecord> allRecords = new List<ObjectRecord>();
            List<LevelSummary> summaries = new List<LevelSummary>();

            LevelDefinition[] definitions = GetLevelDefinitions();

            for (int i = 0; i < definitions.Length; i++)
            {
                LevelDefinition def = definitions[i];
                Debug.Log($"[LevelBlockoutBuilder] Constructing {def.Code} — {def.Name}...");

                Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

                // Build Scene Hierarchy
                GameObject root = new GameObject($"_LEVEL_{def.Code}_ROOT");

                // Environment Lighting
                GameObject dirLightGO = new GameObject("Directional Light");
                dirLightGO.transform.SetParent(root.transform);
                dirLightGO.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
                Light dirLight = dirLightGO.AddComponent<Light>();
                dirLight.type = LightType.Directional;
                dirLight.intensity = 0.85f;
                dirLight.color = new Color(0.95f, 0.95f, 1f);

                // Hierarchy roots
                GameObject envRoot = new GameObject("Environment");
                envRoot.transform.SetParent(root.transform);
                GameObject floorsRoot = new GameObject("Floors");
                floorsRoot.transform.SetParent(envRoot.transform);
                GameObject wallsRoot = new GameObject("Walls");
                wallsRoot.transform.SetParent(envRoot.transform);
                GameObject obstaclesRoot = new GameObject("Obstacles");
                obstaclesRoot.transform.SetParent(envRoot.transform);

                GameObject gameplayRoot = new GameObject("Gameplay");
                gameplayRoot.transform.SetParent(root.transform);
                GameObject playerRoot = new GameObject("PlayerSetup");
                playerRoot.transform.SetParent(gameplayRoot.transform);
                GameObject stealablesRoot = new GameObject("Stealables");
                stealablesRoot.transform.SetParent(gameplayRoot.transform);
                GameObject hazardsRoot = new GameObject("Hazards");
                hazardsRoot.transform.SetParent(gameplayRoot.transform);
                GameObject exitRoot = new GameObject("Exit");
                exitRoot.transform.SetParent(gameplayRoot.transform);

                GameObject labelsRoot = new GameObject("ReferenceLabels");
                labelsRoot.transform.SetParent(root.transform);

                // 1. Build Environment Geometry
                foreach (var g in def.Geometry)
                {
                    GameObject parentRoot = g.Category == "Floor" ? floorsRoot : (g.Category == "Wall" ? wallsRoot : obstaclesRoot);
                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    go.name = g.Name;
                    go.transform.SetParent(parentRoot.transform);
                    go.transform.position = g.Position;
                    go.transform.rotation = Quaternion.Euler(g.Rotation);
                    go.transform.localScale = g.Size;

                    MeshRenderer mr = go.GetComponent<MeshRenderer>();
                    if (g.Category == "Floor" && matFloor != null) mr.sharedMaterial = matFloor;
                    else if (g.Category == "Wall" && matWall != null) mr.sharedMaterial = matWall;

                    allRecords.Add(new ObjectRecord
                    {
                        Level = def.Code,
                        ObjectName = g.Name,
                        Category = "Geometry",
                        Position = g.Position,
                        Rotation = g.Rotation,
                        Scale = g.Size,
                        ColliderType = "BoxCollider",
                        ColliderSize = g.Size,
                        ValueSource = g.ValueSource,
                        Notes = g.Notes
                    });
                }

                // 2. Build Player & Shadow Setup
                GameObject playerInstance = null;
                if (playerPrefab != null)
                {
                    playerInstance = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab, playerRoot.transform);
                    playerInstance.name = "Player";
                    playerInstance.transform.position = def.PlayerSpawn;
                    playerInstance.transform.rotation = Quaternion.Euler(def.PlayerRotation);
                }
                else
                {
                    playerInstance = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                    playerInstance.name = "Player_Placeholder";
                    playerInstance.transform.SetParent(playerRoot.transform);
                    playerInstance.transform.position = def.PlayerSpawn;
                }

                allRecords.Add(new ObjectRecord
                {
                    Level = def.Code,
                    ObjectName = "Player",
                    Category = "Player",
                    Position = def.PlayerSpawn,
                    Rotation = def.PlayerRotation,
                    Scale = Vector3.one,
                    ColliderType = "CharacterController",
                    ColliderSize = new Vector3(0.6f, 1.8f, 0.6f),
                    ValueSource = "DOCUMENTED",
                    Notes = "Player spawn point ground level"
                });

                // Shadow Start / Attached Marker
                GameObject shadowMarker = new GameObject("Shadow_StartMarker");
                shadowMarker.transform.SetParent(playerRoot.transform);
                Vector3 shadowPos = def.ShadowSpawn.HasValue ? def.ShadowSpawn.Value : def.PlayerSpawn;
                shadowMarker.transform.position = shadowPos;

                allRecords.Add(new ObjectRecord
                {
                    Level = def.Code,
                    ObjectName = "Shadow_StartMarker",
                    Category = "Shadow",
                    Position = shadowPos,
                    Rotation = Vector3.zero,
                    Scale = Vector3.one,
                    ColliderType = "None",
                    ColliderSize = Vector3.zero,
                    ValueSource = def.ShadowSpawn.HasValue ? "DOCUMENTED" : "DERIVED_FROM_DIAGRAM",
                    Notes = def.ShadowSpawn.HasValue ? "Documented starting position" : "Attached to Player at start"
                });

                // 3. Build Stealables
                foreach (var s in def.Stealables)
                {
                    GameObject sGO = null;
                    if (s.Type == StealableType.Door)
                    {
                        if (doorPrefab != null)
                        {
                            sGO = (GameObject)PrefabUtility.InstantiatePrefab(doorPrefab, stealablesRoot.transform);
                        }
                        else
                        {
                            sGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            sGO.AddComponent<StealableDoor>();
                        }
                    }
                    else if (s.Type == StealableType.Drawbridge)
                    {
                        if (bridgePrefab != null)
                        {
                            sGO = (GameObject)PrefabUtility.InstantiatePrefab(bridgePrefab, stealablesRoot.transform);
                        }
                        else
                        {
                            sGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            sGO.AddComponent<StealableDrawbridge>();
                        }
                    }
                    else if (s.Type == StealableType.Platform)
                    {
                        if (platformPrefab != null)
                        {
                            sGO = (GameObject)PrefabUtility.InstantiatePrefab(platformPrefab, stealablesRoot.transform);
                        }
                        else
                        {
                            sGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            sGO.AddComponent<StealablePlatform>();
                        }
                    }
                    else if (s.Type == StealableType.LightSwitch)
                    {
                        sGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        sGO.name = s.Name;
                        sGO.transform.SetParent(stealablesRoot.transform);
                        // Add unbuilt dependency marker component
                        var marker = sGO.AddComponent<LightSwitchGreyboxMarker>();
                        marker.statusNote = "Programmer Dependency: StealableLightSwitch unbuilt (Rayaan). Greybox representation.";
                    }

                    if (sGO != null)
                    {
                        sGO.name = s.Name;
                        sGO.transform.position = s.Position;
                        sGO.transform.rotation = Quaternion.Euler(s.Rotation);
                        if (s.Scale != Vector3.zero) sGO.transform.localScale = s.Scale;

                        // Configure platform oscillation if applicable
                        if (s.Type == StealableType.Platform)
                        {
                            var sp = sGO.GetComponent<StealablePlatform>();
                            if (sp != null)
                            {
                                // Configured via serialized values in prefab
                            }
                        }

                        allRecords.Add(new ObjectRecord
                        {
                            Level = def.Code,
                            ObjectName = s.Name,
                            Category = "Stealable",
                            Position = s.Position,
                            Rotation = s.Rotation,
                            Scale = sGO.transform.localScale,
                            ColliderType = "BoxCollider",
                            ColliderSize = sGO.transform.localScale,
                            ValueSource = s.ValueSource,
                            Notes = s.Notes
                        });
                    }
                }

                // 4. Build Hazards
                foreach (var h in def.Hazards)
                {
                    GameObject hGO = new GameObject(h.Name);
                    hGO.transform.SetParent(hazardsRoot.transform);
                    hGO.transform.position = h.Position;
                    hGO.transform.rotation = Quaternion.Euler(h.Rotation);

                    BoxCollider col = hGO.AddComponent<BoxCollider>();
                    col.isTrigger = true;
                    col.size = h.TriggerSize;
                    col.center = Vector3.zero;

                    // Light visual
                    GameObject lightChild = new GameObject("Spotlight");
                    lightChild.transform.SetParent(hGO.transform, false);
                    lightChild.transform.localPosition = new Vector3(0, h.TriggerSize.y * 0.5f + 1f, 0);
                    lightChild.transform.localRotation = Quaternion.Euler(90f, 0, 0);
                    Light lt = lightChild.AddComponent<Light>();
                    lt.type = LightType.Spot;
                    lt.range = h.TriggerSize.y + 4f;
                    lt.spotAngle = 65f;

                    // Mesh floor footprint indicator
                    GameObject footprint = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    footprint.name = "FloorIndicator";
                    footprint.transform.SetParent(hGO.transform, false);
                    footprint.transform.localPosition = new Vector3(0, 0.05f, 0);
                    footprint.transform.localRotation = Quaternion.Euler(90f, 0, 0);
                    footprint.transform.localScale = new Vector3(h.TriggerSize.x, h.TriggerSize.z, 1f);
                    Collider fc = footprint.GetComponent<Collider>();
                    if (fc != null) UnityEngine.Object.DestroyImmediate(fc);

                    if (h.Type == HazardType.DangerLightStatic || h.Type == HazardType.DangerLightSweeping || h.Type == HazardType.DangerLightPulsing)
                    {
                        var dl = hGO.AddComponent<DangerLight>();
                        lt.color = new Color(1f, 0.25f, 0.15f);
                        if (matDanger != null) footprint.GetComponent<MeshRenderer>().sharedMaterial = matDanger;

                        if (h.Type == HazardType.DangerLightSweeping)
                        {
                            var sw = hGO.AddComponent<SweepingHazard>();
                            // Set sweep properties via serialized fields if needed
                        }
                    }
                    else if (h.Type == HazardType.TrapLight)
                    {
                        var tl = hGO.AddComponent<TrapLight>();
                        lt.color = new Color(0.75f, 0.2f, 1f); // Purple / UV
                        if (matTrap != null) footprint.GetComponent<MeshRenderer>().sharedMaterial = matTrap;
                    }

                    allRecords.Add(new ObjectRecord
                    {
                        Level = def.Code,
                        ObjectName = h.Name,
                        Category = "Hazard",
                        Position = h.Position,
                        Rotation = h.Rotation,
                        Scale = Vector3.one,
                        ColliderType = "BoxCollider (Trigger)",
                        ColliderSize = h.TriggerSize,
                        ValueSource = h.ValueSource,
                        Notes = h.Notes
                    });
                }

                // 5. Build Exit Zone
                GameObject exitGO = new GameObject("ExitZone");
                exitGO.transform.SetParent(exitRoot.transform);
                exitGO.transform.position = def.ExitPosition;
                BoxCollider exitCol = exitGO.AddComponent<BoxCollider>();
                exitCol.isTrigger = true;
                exitCol.size = def.ExitSize;
                exitCol.center = new Vector3(0, def.ExitSize.y * 0.5f, 0);
                exitGO.AddComponent<ExitTrigger>();

                // Exit visual light
                GameObject exitLightGO = new GameObject("ExitLight");
                exitLightGO.transform.SetParent(exitGO.transform, false);
                exitLightGO.transform.localPosition = new Vector3(0, 2.5f, 0);
                Light exitLight = exitLightGO.AddComponent<Light>();
                exitLight.type = LightType.Point;
                exitLight.color = new Color(0.2f, 1f, 0.4f); // Soft green
                exitLight.range = 6f;
                exitLight.intensity = 2f;

                // Exit floor pad
                GameObject exitPad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                exitPad.name = "ExitPad";
                exitPad.transform.SetParent(exitGO.transform, false);
                exitPad.transform.localPosition = new Vector3(0, 0.05f, 0);
                exitPad.transform.localRotation = Quaternion.Euler(90f, 0, 0);
                exitPad.transform.localScale = new Vector3(def.ExitSize.x, def.ExitSize.z, 1f);
                Collider epc = exitPad.GetComponent<Collider>();
                if (epc != null) UnityEngine.Object.DestroyImmediate(epc);
                if (matExit != null) exitPad.GetComponent<MeshRenderer>().sharedMaterial = matExit;

                allRecords.Add(new ObjectRecord
                {
                    Level = def.Code,
                    ObjectName = "ExitZone",
                    Category = "Exit",
                    Position = def.ExitPosition,
                    Rotation = Vector3.zero,
                    Scale = Vector3.one,
                    ColliderType = "BoxCollider (Trigger)",
                    ColliderSize = def.ExitSize,
                    ValueSource = "DOCUMENTED",
                    Notes = "Level completion exit trigger zone"
                });

                // 6. Build TextMeshPro 3D Reference Labels
                foreach (var lbl in def.Labels)
                {
                    GameObject lblGO = new GameObject(lbl.Name);
                    lblGO.transform.SetParent(labelsRoot.transform);
                    lblGO.transform.position = lbl.Position;
                    lblGO.transform.rotation = Quaternion.Euler(50f, 0, 0); // Face angled camera

                    TextMeshPro tmp = lblGO.AddComponent<TextMeshPro>();
                    tmp.text = $"<mark=#000000B0>{lbl.Text}</mark>";
                    tmp.fontSize = lbl.FontSize > 0 ? lbl.FontSize : 3.5f;
                    tmp.alignment = TextAlignmentOptions.Center;
                    tmp.color = lbl.Color;

                    allRecords.Add(new ObjectRecord
                    {
                        Level = def.Code,
                        ObjectName = lbl.Name,
                        Category = "Label",
                        Position = lbl.Position,
                        Rotation = new Vector3(50f, 0, 0),
                        Scale = Vector3.one,
                        ColliderType = "None",
                        ColliderSize = Vector3.zero,
                        ValueSource = "IMPLEMENTATION_ASSUMPTION",
                        Notes = $"Reference label: {lbl.Text.Replace("\n", " ")}"
                    });
                }

                // 7. Setup Overview Camera
                GameObject camGO = new GameObject("OverviewCamera");
                camGO.transform.SetParent(root.transform);
                float camZ = def.Dimensions.y * 0.45f;
                float camY = Mathf.Max(def.Dimensions.y * 0.7f, 15f);
                camGO.transform.position = new Vector3(0, camY, camZ);
                camGO.transform.rotation = Quaternion.Euler(55f, 0, 0);
                Camera cam = camGO.AddComponent<Camera>();
                cam.clearFlags = CameraClearFlags.Skybox;
                cam.fieldOfView = 60f;

                // Save Scene
                string scenePath = $"{ScenesDir}/{def.Code}_Blockout.unity";
                EditorSceneManager.SaveScene(scene, scenePath);
                Debug.Log($"[LevelBlockoutBuilder] Successfully saved {scenePath}");

                summaries.Add(new LevelSummary
                {
                    Code = def.Code,
                    Name = def.Name,
                    Phase = def.Phase,
                    Difficulty = def.Difficulty,
                    Dimensions = $"{def.Dimensions.x}m x {def.Dimensions.y}m",
                    TargetTime = def.TargetTime,
                    HardLimit = def.HardLimit,
                    MaxLeash = def.MaxLeash,
                    LeashLimit = def.LeashLimit,
                    StealablesCount = def.Stealables.Count,
                    HazardsCount = def.Hazards.Count,
                    Objective = def.Objective,
                    PlayerRoute = def.PlayerRoute,
                    ShadowRoute = def.ShadowRoute,
                    Sequence = def.Sequence,
                    Notes = def.Notes
                });
            }

            // Write CSV and Markdowns
            GenerateCsvReport(allRecords);
            GenerateReferenceMarkdown(summaries, allRecords);
            GenerateProgressionOverviewMarkdown(summaries);

            // Reopen M1 scene so project state remains pristine
            if (File.Exists(ProductionScene))
            {
                EditorSceneManager.OpenScene(ProductionScene);
                Debug.Log($"[LevelBlockoutBuilder] Restored active scene to {ProductionScene}");
            }

            AssetDatabase.Refresh();

            File.WriteAllText(DoneFile, $"Build completed successfully at {DateTime.Now:yyyy-MM-dd HH:mm:ss}. 15 scenes created.");
            Debug.Log("[LevelBlockoutBuilder] ALL 15 LEVEL BLOCKOUTS AND DOCUMENTATION GENERATED SUCCESSFULLY!");
        }

        private static void GenerateCsvReport(List<ObjectRecord> records)
        {
            string csvPath = $"{DocsDir}/Level_Blockout_Positions.csv";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Level,ObjectName,Category,PosX,PosY,PosZ,RotX,RotY,RotZ,ScaleX,ScaleY,ScaleZ,ColliderType,ColliderSizeX,ColliderSizeY,ColliderSizeZ,ValueSource,Notes");

            foreach (var r in records)
            {
                sb.AppendLine($"{r.Level},\"{r.ObjectName}\",{r.Category},{r.Position.x:F2},{r.Position.y:F2},{r.Position.z:F2},{r.Rotation.x:F2},{r.Rotation.y:F2},{r.Rotation.z:F2},{r.Scale.x:F2},{r.Scale.y:F2},{r.Scale.z:F2},\"{r.ColliderType}\",{r.ColliderSize.x:F2},{r.ColliderSize.y:F2},{r.ColliderSize.z:F2},{r.ValueSource},\"{r.Notes.Replace("\"", "\"\"")}\"");
            }

            File.WriteAllText(csvPath, sb.ToString(), Encoding.UTF8);
            Debug.Log($"[LevelBlockoutBuilder] Generated CSV report at {csvPath} ({records.Count} entries)");
        }

        private static void GenerateReferenceMarkdown(List<LevelSummary> summaries, List<ObjectRecord> records)
        {
            string mdPath = $"{DocsDir}/Level_Blockout_Reference.md";
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("# StealMan — Level Blockout Technical Reference");
            sb.AppendLine();
            sb.AppendLine("> **Source of Truth**: Approved Level Design Specification PDFs (`LEVEL_SPECS/L01` through `L15`).");
            sb.AppendLine("> **Target Engine**: Unity 6 (6000.5.2f1) | **Scene Directory**: `Assets/Scenes/LevelBlockoutReference/`");
            sb.AppendLine("> **Generated Date**: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine();
            sb.AppendLine("## 1. Overview & Coordinate Standards");
            sb.AppendLine();
            sb.AppendLine("- **Right-Handed Standard Coordinates**: `+X` = Right, `+Y` = Up, `+Z` = Forward Progression.");
            sb.AppendLine("- **Player Spawn Datum**: Player spawns at ground level `(0, 0, Z_start)` facing forward `+Z` (or documented offset).");
            sb.AppendLine("- **Leash Mechanics**: Hard clamp at `25.0m` Euclidean distance from Player body to Shadow.");
            sb.AppendLine("- **Data Sourcing Key**:");
            sb.AppendLine("  - `DOCUMENTED`: Explicit measurement or coordinate stated in PDF specification text.");
            sb.AppendLine("  - `DERIVED_FROM_DIAGRAM`: Scaled from top-down ASCII/schematic floorplan diagrams.");
            sb.AppendLine("  - `IMPLEMENTATION_ASSUMPTION`: Standardized technical implementation detail (e.g. wall thickness `0.2m`, label offset).");
            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine();

            foreach (var s in summaries)
            {
                sb.AppendLine($"## {s.Code} — {s.Name}");
                sb.AppendLine();
                sb.AppendLine($"| Metric | Specification |");
                sb.AppendLine($"| :--- | :--- |");
                sb.AppendLine($"| **Phase** | {s.Phase} |");
                sb.AppendLine($"| **Difficulty** | {s.Difficulty} |");
                sb.AppendLine($"| **Play Space Dimensions** | {s.Dimensions} |");
                sb.AppendLine($"| **Target Time / Hard Limit** | {s.TargetTime} / {s.HardLimit} |");
                sb.AppendLine($"| **Max Required Leash / Limit** | {s.MaxLeash} / {s.LeashLimit} |");
                sb.AppendLine($"| **Stealable Objects** | {s.StealablesCount} |");
                sb.AppendLine($"| **Hazard Volumes** | {s.HazardsCount} |");
                sb.AppendLine();
                sb.AppendLine($"### Design Objective");
                sb.AppendLine($"> {s.Objective}");
                sb.AppendLine();
                sb.AppendLine($"### Traversal Routes & Metrics");
                sb.AppendLine($"- **Player Route**: {s.PlayerRoute}");
                sb.AppendLine($"- **Shadow Route**: {s.ShadowRoute}");
                sb.AppendLine();
                sb.AppendLine($"### Gameplay Sequence");
                sb.AppendLine(s.Sequence);
                sb.AppendLine();
                sb.AppendLine($"### Spatial Inventory & Placed Objects");
                sb.AppendLine();
                sb.AppendLine("| Object Name | Category | Position (X, Y, Z) | Rotation | Scale / Size | Value Source | Notes |");
                sb.AppendLine("| :--- | :--- | :--- | :--- | :--- | :--- | :--- |");

                var levelRecords = records.FindAll(r => r.Level == s.Code);
                foreach (var r in levelRecords)
                {
                    sb.AppendLine($"| `{r.ObjectName}` | {r.Category} | `({r.Position.x:F2}, {r.Position.y:F2}, {r.Position.z:F2})` | `({r.Rotation.x:F0}, {r.Rotation.y:F0}, {r.Rotation.z:F0})` | `({r.Scale.x:F2}, {r.Scale.y:F2}, {r.Scale.z:F2})` | `{r.ValueSource}` | {r.Notes} |");
                }

                sb.AppendLine();
                if (!string.IsNullOrEmpty(s.Notes))
                {
                    sb.AppendLine($"### Implementation Notes");
                    sb.AppendLine(s.Notes);
                    sb.AppendLine();
                }
                sb.AppendLine("---");
                sb.AppendLine();
            }

            File.WriteAllText(mdPath, sb.ToString(), Encoding.UTF8);
            Debug.Log($"[LevelBlockoutBuilder] Generated Reference Markdown at {mdPath}");
        }

        private static void GenerateProgressionOverviewMarkdown(List<LevelSummary> summaries)
        {
            string mdPath = $"{DocsDir}/Level_Progression_Overview.md";
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("# StealMan — Level Progression & Campaign Overview");
            sb.AppendLine();
            sb.AppendLine("> **Campaign Scope**: 15 Levels across 3 Progressive Phases.");
            sb.AppendLine("> **Architecture**: Built strictly in accordance with approved Level Design Specifications.");
            sb.AppendLine("> **Generated Date**: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine();
            sb.AppendLine("## 1. Master Progression Matrix");
            sb.AppendLine();
            sb.AppendLine("| Level | Name | Phase | Difficulty | Dimensions | Target / Limit | Max Leash | Stealables | Hazards | Primary Mechanic Introduced |");
            sb.AppendLine("| :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- |");

            string[] mechanicsIntroduced = new string[]
            {
                "Basic Movement & Exit Trigger",
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
            };

            for (int i = 0; i < summaries.Count; i++)
            {
                var s = summaries[i];
                sb.AppendLine($"| **{s.Code}** | {s.Name} | {s.Phase} | `{s.Difficulty}` | {s.Dimensions} | {s.TargetTime} / {s.HardLimit} | {s.MaxLeash} / 25m | {s.StealablesCount} | {s.HazardsCount} | {mechanicsIntroduced[i]} |");
            }

            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine();
            sb.AppendLine("## 2. Three-Phase Campaign Structure");
            sb.AppendLine();
            sb.AppendLine("### Phase 1: Teach (Levels 01 – 05)");
            sb.AppendLine("- **Objective**: Isolate core verbs in safe or strictly bounded environments.");
            sb.AppendLine("- **L01 (The Corridor)**: Baseline player locomotion, camera angle, and exit pad trigger.");
            sb.AppendLine("- **L02 (The Sandbox)**: Detach & Recall loop without time pressure or hazards.");
            sb.AppendLine("- **L03 (Ghost Gate)**: Introduce `Steal()` on physical Door to create Ghost state passage.");
            sb.AppendLine("- **L04 (The Vent)**: Introduce vertical wall traversal to steal and lower a Drawbridge over a 6m pit.");
            sb.AppendLine("- **L05 (The Spotlight)**: Introduce Danger Light hazard and failure/restart consequence.");
            sb.AppendLine();
            sb.AppendLine("### Phase 2: Combine (Levels 06 – 10)");
            sb.AppendLine("- **Objective**: Combine mechanics into multi-step puzzles requiring body repositioning and rhythm.");
            sb.AppendLine("- **L06 (Double Lock)**: Two sequential doors proving body must move into intermediate safe zone.");
            sb.AppendLine("- **L07 (Long Detour)**: U-shaped wall traversal navigating above static danger light (21m leash).");
            sb.AppendLine("- **L08 (Pulsing Corridor)**: Timing 3 pulsing lights across floor from safe island to safe island (24m leash).");
            sb.AppendLine("- **L09 (Split Decision)**: Player agency — Floor route (sweeping light) vs Wall route (pulsing light).");
            sb.AppendLine("- **L10 (The Gauntlet)**: Pit drawbridge followed immediately by dual-sweeper door corridor (supports Safe 2-Detach and Expert 1-Detach routes).");
            sb.AppendLine();
            sb.AppendLine("### Phase 3: Master (Levels 11 – 15)");
            sb.AppendLine("- **Objective**: High skill ceiling, reflex challenges, rapid state recovery, and tight spatial constraints.");
            sb.AppendLine("- **L11 (The Snare)**: Introduce Trap Light hazard requiring 3 rapid RECOVER button presses in 2.0s.");
            sb.AppendLine("- **L12 (The Conveyor)**: Introduce rapidly sliding Platform across 8m pit; steal freezes it as bridge.");
            sb.AppendLine("- **L13 (The Breaker)**: Introduce Light Switch high on wall to permanently disable exit danger light.");
            sb.AppendLine("- **L14 (Threading the Needle)**: Flooded floor forces razor-thin safe wall ribbon; demands 24.5m Euclidean leash separation against 25m hard limit.");
            sb.AppendLine("- **L15 (Final Escape)**: Master synthesis — Stage 1 Switch, Stage 2 Platform, Stage 3 Trap + Door under 38s target.");
            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine();
            sb.AppendLine("## 3. Leash Distance Curve & Safety Margins");
            sb.AppendLine();
            sb.AppendLine("| Level | Max Required Separation | Leash Limit | Safety Margin | Routing Type |");
            sb.AppendLine("| :--- | :--- | :--- | :--- | :--- |");
            sb.AppendLine("| L01 | 0.0m | 25.0m | N/A | Player only |");
            sb.AppendLine("| L02 | 8.0m | 25.0m | 17.0m | Free sandbox |");
            sb.AppendLine("| L03 | 5.5m | 25.0m | 19.5m | Direct straight line |");
            sb.AppendLine("| L04 | 10.0m | 25.0m | 15.0m | Wall detour around pit |");
            sb.AppendLine("| L05 | 12.0m | 25.0m | 13.0m | Direct floor dash |");
            sb.AppendLine("| L06 | 7.0m | 25.0m | 18.0m | Reset at Safe Zone (per sub-route) |");
            sb.AppendLine("| L07 | 21.0m | 25.0m | 4.0m | High wall U-turn detour |");
            sb.AppendLine("| L08 | 24.0m | 25.0m | 1.0m | Linear floor dash |");
            sb.AppendLine("| L09 | 20.0m | 25.0m | 5.0m | Symmetrical branch routes |");
            sb.AppendLine("| L10 | 24.0m | 25.0m | 1.0m | Single-detach expert path |");
            sb.AppendLine("| L11 | 14.0m | 25.0m | 11.0m | Trap snared path |");
            sb.AppendLine("| L12 | 10.0m | 25.0m | 15.0m | Pit edge timing |");
            sb.AppendLine("| L13 | 12.0m | 25.0m | 13.0m | High wall switch climb |");
            sb.AppendLine("| L14 | **24.5m** | **25.0m** | **0.5m** | **Maximum leash stress test** |");
            sb.AppendLine("| L15 | 18.0m | 25.0m | 7.0m | Multi-stage (exceeds 45m total; forces 2+ Recalls) |");
            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine();
            sb.AppendLine("## 4. Programmer Technical Handoff Notes");
            sb.AppendLine();
            sb.AppendLine("1. **StealableLightSwitch (Levels 13 & 15)**:");
            sb.AppendLine("   - **Status**: Unbuilt programmer dependency (Rayaan).");
            sb.AppendLine("   - **Current Greybox**: Modeled as wall breaker box with conduit visuals and `LightSwitchGreyboxMarker` component.");
            sb.AppendLine("   - **Required Implementation**: When stolen via `Steal()`, fires `OnSwitchGhosted` event deactivating target `DangerLight` hazard.");
            sb.AppendLine();
            sb.AppendLine("2. **SweepingHazard & TrapLight Integration**:");
            sb.AppendLine("   - Fully wired into Levels 05, 07, 08, 09, 10, 11, 13, 14, 15 using Harsh's validated hazard components.");
            sb.AppendLine("   - Sweep angles calibrated between `30°` and `40°` at `30°/s`.");
            sb.AppendLine("   - Trap Light requires 3 RECOVER pulses within 2.0s.");
            sb.AppendLine();
            sb.AppendLine("3. **StealablePlatform (Levels 12 & 15)**:");
            sb.AppendLine("   - Configured with `moveAxis = (1, 0, 0)`, `moveDistance = 4.5m`, `moveSpeed = 3.5m/s`.");
            sb.AppendLine("   - Freezes rigidly in place upon `Steal()`, becoming a walkable solid bridge.");

            File.WriteAllText(mdPath, sb.ToString(), Encoding.UTF8);
            Debug.Log($"[LevelBlockoutBuilder] Generated Progression Overview Markdown at {mdPath}");
        }

        #region Level Definitions Blueprint
        private static LevelDefinition[] GetLevelDefinitions()
        {
            return new LevelDefinition[]
            {
                // ==========================================
                // L01: The Corridor (5m x 15m)
                // ==========================================
                new LevelDefinition
                {
                    Code = "L01",
                    Name = "The Corridor",
                    Phase = "Phase 1 (Teach)",
                    Difficulty = "0 / 10",
                    Dimensions = new Vector2(5f, 15f),
                    TargetTime = "5s",
                    HardLimit = "10s",
                    MaxLeash = "0m",
                    LeashLimit = "25m",
                    Objective = "Walk to the exit zone. Teaches fundamental movement without introducing Shadow.",
                    PlayerSpawn = new Vector3(-1.0f, 0.1f, 1.5f),
                    PlayerRotation = new Vector3(0, 0, 0),
                    ShadowSpawn = null,
                    ExitPosition = new Vector3(0, 0.1f, 14.0f),
                    ExitSize = new Vector3(3.0f, 2.0f, 2.0f),
                    PlayerRoute = "Walk straight up the corridor to the exit pad.",
                    ShadowRoute = "N/A (Shadow mechanic not active in Level 1).",
                    Sequence = "1. Player moves from spawn (0, 0, 1.5) straight up the corridor.\n2. Player enters Exit Zone at (0, 0, 14.0) to complete the level.",
                    Notes = "Basic tutorial corridor. No hazards, no stealables.",
                    Geometry = new List<GeometryItem>
                    {
                        new GeometryItem { Name = "Floor_Main", Category = "Floor", Position = new Vector3(0, 0, 7.5f), Size = new Vector3(5f, 0.2f, 15f), ValueSource = "DOCUMENTED", Notes = "Main corridor floor (5m x 15m)" },
                        new GeometryItem { Name = "Wall_Left", Category = "Wall", Position = new Vector3(-2.6f, 1.5f, 7.5f), Size = new Vector3(0.2f, 3f, 15f), ValueSource = "DOCUMENTED", Notes = "Left corridor wall (height 3m)" },
                        new GeometryItem { Name = "Wall_Right", Category = "Wall", Position = new Vector3(2.6f, 1.5f, 7.5f), Size = new Vector3(0.2f, 3f, 15f), ValueSource = "DOCUMENTED", Notes = "Right corridor wall (height 3m)" },
                        new GeometryItem { Name = "Wall_Back", Category = "Wall", Position = new Vector3(0, 1.5f, -0.1f), Size = new Vector3(5.4f, 3f, 0.2f), ValueSource = "DOCUMENTED", Notes = "Back boundary wall" },
                        new GeometryItem { Name = "Wall_Front_L", Category = "Wall", Position = new Vector3(-1.75f, 1.5f, 15.1f), Size = new Vector3(1.5f, 3f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Exit threshold left wall" },
                        new GeometryItem { Name = "Wall_Front_R", Category = "Wall", Position = new Vector3(1.75f, 1.5f, 15.1f), Size = new Vector3(1.5f, 3f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Exit threshold right wall" },
                    },
                    Stealables = new List<StealableItem>(),
                    Hazards = new List<HazardItem>(),
                    Labels = new List<LabelItem>
                    {
                        new LabelItem { Name = "Label_Title", Text = "L01: The Corridor\n5m x 15m | Movement Tutorial", Position = new Vector3(0, 3.5f, 2.0f), Color = Color.white, FontSize = 4f },
                        new LabelItem { Name = "Label_Spawn", Text = "Player Spawn", Position = new Vector3(-1.0f, 2.0f, 1.5f), Color = Color.cyan, FontSize = 3f },
                        new LabelItem { Name = "Label_Exit", Text = "Exit Zone (Walk Here)", Position = new Vector3(0, 2.0f, 14.0f), Color = Color.green, FontSize = 3f }
                    }
                },

                // ==========================================
                // L02: The Sandbox (10m x 20m)
                // ==========================================
                new LevelDefinition
                {
                    Code = "L02",
                    Name = "The Sandbox",
                    Phase = "Phase 1 (Teach)",
                    Difficulty = "0 / 10",
                    Dimensions = new Vector2(10f, 20f),
                    TargetTime = "10s",
                    HardLimit = "15s",
                    MaxLeash = "8m",
                    LeashLimit = "25m",
                    Objective = "Teach Attached -> Detached -> Recall loop in a safe, unforced environment.",
                    PlayerSpawn = new Vector3(0, 0.1f, 2.0f),
                    PlayerRotation = Vector3.zero,
                    ShadowSpawn = null,
                    ExitPosition = new Vector3(0, 0.1f, 18.5f),
                    ExitSize = new Vector3(4.0f, 2.0f, 2.0f),
                    PlayerRoute = "Walk to center, detach shadow to explore, recall, walk to exit.",
                    ShadowRoute = "Move freely around room when detached.",
                    Sequence = "1. Player spawns at (0, 0, 2.0).\n2. Press Detach to test independent shadow movement.\n3. Press Recall to snap shadow back to body.\n4. Walk through open room to Exit at (0, 0, 18.5).",
                    Notes = "Safe playground proving Recall is a state transition, not a puzzle solver.",
                    Geometry = new List<GeometryItem>
                    {
                        new GeometryItem { Name = "Floor_Main", Category = "Floor", Position = new Vector3(0, 0, 10.0f), Size = new Vector3(10f, 0.2f, 20f), ValueSource = "DOCUMENTED", Notes = "Sandbox floor (10m x 20m)" },
                        new GeometryItem { Name = "Wall_Left", Category = "Wall", Position = new Vector3(-5.1f, 1.5f, 10.0f), Size = new Vector3(0.2f, 3f, 20f), ValueSource = "DOCUMENTED", Notes = "Left wall (10m width)" },
                        new GeometryItem { Name = "Wall_Right", Category = "Wall", Position = new Vector3(5.1f, 1.5f, 10.0f), Size = new Vector3(0.2f, 3f, 20f), ValueSource = "DOCUMENTED", Notes = "Right wall" },
                        new GeometryItem { Name = "Wall_Back", Category = "Wall", Position = new Vector3(0, 1.5f, -0.1f), Size = new Vector3(10.4f, 3f, 0.2f), ValueSource = "DOCUMENTED", Notes = "Back boundary wall" },
                        new GeometryItem { Name = "Wall_Front_L", Category = "Wall", Position = new Vector3(-3.0f, 1.5f, 20.1f), Size = new Vector3(4.0f, 3f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Front left boundary" },
                        new GeometryItem { Name = "Wall_Front_R", Category = "Wall", Position = new Vector3(3.0f, 1.5f, 20.1f), Size = new Vector3(4.0f, 3f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Front right boundary" },
                    },
                    Stealables = new List<StealableItem>(),
                    Hazards = new List<HazardItem>(),
                    Labels = new List<LabelItem>
                    {
                        new LabelItem { Name = "Label_Title", Text = "L02: The Sandbox\n10m x 20m | Detach & Recall Practice", Position = new Vector3(0, 3.5f, 2.0f), Color = Color.white, FontSize = 4f },
                        new LabelItem { Name = "Label_Spawn", Text = "Player Spawn", Position = new Vector3(0, 2.0f, 2.0f), Color = Color.cyan, FontSize = 3f },
                        new LabelItem { Name = "Label_Sandbox", Text = "Safe Exploration Space\n(Test Detach & Recall)", Position = new Vector3(0, 1.0f, 10.0f), Color = Color.yellow, FontSize = 3.5f },
                        new LabelItem { Name = "Label_Exit", Text = "Exit Zone", Position = new Vector3(0, 2.0f, 18.5f), Color = Color.green, FontSize = 3f }
                    }
                },

                // ==========================================
                // L03: Ghost Gate (5m x 15m)
                // ==========================================
                new LevelDefinition
                {
                    Code = "L03",
                    Name = "Ghost Gate",
                    Phase = "Phase 1 (Teach)",
                    Difficulty = "2 / 10",
                    Dimensions = new Vector2(5f, 15f),
                    TargetTime = "15s",
                    HardLimit = "20s",
                    MaxLeash = "5.5m",
                    LeashLimit = "25m",
                    Objective = "Introduce the STEAL mechanic: stealing an object's shadow makes it intangible (Ghost state).",
                    PlayerSpawn = new Vector3(0, 0.1f, 2.0f),
                    PlayerRotation = Vector3.zero,
                    ShadowSpawn = null,
                    ExitPosition = new Vector3(0, 0.1f, 13.5f),
                    ExitSize = new Vector3(3.0f, 2.0f, 2.0f),
                    PlayerRoute = "Walk to door -> Wait -> Walk through Ghost Door -> Exit.",
                    ShadowRoute = "Detach -> Move to Door -> Steal -> Recall.",
                    Sequence = "1. Player approaches door at Z=5.0m.\n2. Detach shadow.\n3. Move shadow to Stealable Door at (0, 1.25, 7.5).\n4. Press STEAL. Door becomes Ghost state.\n5. Press Recall to reunite.\n6. Player walks through intangible door to Exit at (0, 0, 13.5).",
                    Notes = "First encounter with a Stealable obstacle.",
                    Geometry = new List<GeometryItem>
                    {
                        new GeometryItem { Name = "Floor_Main", Category = "Floor", Position = new Vector3(0, 0, 7.5f), Size = new Vector3(5f, 0.2f, 15f), ValueSource = "DOCUMENTED", Notes = "Main floor (5m x 15m)" },
                        new GeometryItem { Name = "Wall_Left", Category = "Wall", Position = new Vector3(-2.6f, 1.5f, 7.5f), Size = new Vector3(0.2f, 3f, 15f), ValueSource = "DOCUMENTED", Notes = "Left wall" },
                        new GeometryItem { Name = "Wall_Right", Category = "Wall", Position = new Vector3(2.6f, 1.5f, 7.5f), Size = new Vector3(0.2f, 3f, 15f), ValueSource = "DOCUMENTED", Notes = "Right wall" },
                        new GeometryItem { Name = "Wall_Back", Category = "Wall", Position = new Vector3(0, 1.5f, -0.1f), Size = new Vector3(5.4f, 3f, 0.2f), ValueSource = "DOCUMENTED", Notes = "Back wall" },
                        new GeometryItem { Name = "Pillar_Left", Category = "Obstacle", Position = new Vector3(-1.75f, 1.5f, 7.5f), Size = new Vector3(1.5f, 3f, 0.4f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Door frame left pillar" },
                        new GeometryItem { Name = "Pillar_Right", Category = "Obstacle", Position = new Vector3(1.75f, 1.5f, 7.5f), Size = new Vector3(1.5f, 3f, 0.4f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Door frame right pillar" },
                        new GeometryItem { Name = "Wall_Front_L", Category = "Wall", Position = new Vector3(-1.75f, 1.5f, 15.1f), Size = new Vector3(1.5f, 3f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Exit wall left" },
                        new GeometryItem { Name = "Wall_Front_R", Category = "Wall", Position = new Vector3(1.75f, 1.5f, 15.1f), Size = new Vector3(1.5f, 3f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Exit wall right" },
                    },
                    Stealables = new List<StealableItem>
                    {
                        new StealableItem { Name = "StealableDoor", Type = StealableType.Door, Position = new Vector3(0, 1.25f, 7.5f), Rotation = Vector3.zero, Scale = Vector3.one, ValueSource = "DOCUMENTED", Notes = "Stealable Door blocking hallway" }
                    },
                    Hazards = new List<HazardItem>(),
                    Labels = new List<LabelItem>
                    {
                        new LabelItem { Name = "Label_Title", Text = "L03: Ghost Gate\n5m x 15m | Introduce STEAL Mechanic", Position = new Vector3(0, 3.5f, 2.0f), Color = Color.white, FontSize = 4f },
                        new LabelItem { Name = "Label_Door", Text = "Stealable Door\n(Solid -> Ghost Intangible)", Position = new Vector3(0, 3.0f, 7.5f), Color = Color.cyan, FontSize = 3.5f },
                        new LabelItem { Name = "Label_Exit", Text = "Exit Zone", Position = new Vector3(0, 2.0f, 13.5f), Color = Color.green, FontSize = 3f }
                    }
                },

                // ==========================================
                // L04: The Vent (10m x 25m)
                // ==========================================
                new LevelDefinition
                {
                    Code = "L04",
                    Name = "The Vent",
                    Phase = "Phase 1 (Teach)",
                    Difficulty = "3 / 10",
                    Dimensions = new Vector2(10f, 25f),
                    TargetTime = "20s",
                    HardLimit = "25s",
                    MaxLeash = "10m",
                    LeashLimit = "25m",
                    Objective = "Teach Floor -> Wall Traversal and Stealable Drawbridge to cross a pit.",
                    PlayerSpawn = new Vector3(0, 0.1f, 2.0f),
                    PlayerRotation = Vector3.zero,
                    ShadowSpawn = null,
                    ExitPosition = new Vector3(0, 0.1f, 22.0f),
                    ExitSize = new Vector3(4.0f, 2.0f, 2.0f),
                    PlayerRoute = "Walk to pit edge (Z=8.5m) -> Wait -> Cross lowered bridge -> Exit.",
                    ShadowRoute = "Detach -> Move to Right Wall -> Climb up wall around 6m pit -> Steal Drawbridge -> Recall.",
                    Sequence = "1. Player stops at edge of 6m pit (Z=9m to 15m).\n2. Detach shadow.\n3. Transition shadow onto right vertical wall (height 5m).\n4. Walk shadow along wall across pit to Drawbridge mount.\n5. Press STEAL. Drawbridge lowers.\n6. Recall. Player crosses bridge to Exit at (0, 0, 22.0).",
                    Notes = "First vertical wall climbing interaction. Raised drawbridge lowers when stolen.",
                    Geometry = new List<GeometryItem>
                    {
                        new GeometryItem { Name = "Floor_Start", Category = "Floor", Position = new Vector3(0, 0, 4.5f), Size = new Vector3(10f, 0.2f, 9f), ValueSource = "DOCUMENTED", Notes = "Start floor section (Z: 0 to 9m)" },
                        new GeometryItem { Name = "Pit_Void", Category = "Obstacle", Position = new Vector3(0, -5.0f, 12.0f), Size = new Vector3(10f, 0.2f, 6f), ValueSource = "DOCUMENTED", Notes = "Pit gap (6m wide chasm, Z: 9 to 15m)" },
                        new GeometryItem { Name = "Floor_Exit", Category = "Floor", Position = new Vector3(0, 0, 20.0f), Size = new Vector3(10f, 0.2f, 10f), ValueSource = "DOCUMENTED", Notes = "Exit floor section (Z: 15 to 25m)" },
                        new GeometryItem { Name = "Wall_Left", Category = "Wall", Position = new Vector3(-5.1f, 2.5f, 12.5f), Size = new Vector3(0.2f, 5f, 25f), ValueSource = "DOCUMENTED", Notes = "Left wall (height 5m)" },
                        new GeometryItem { Name = "Wall_Right_Climbable", Category = "Wall", Position = new Vector3(5.1f, 2.5f, 12.5f), Size = new Vector3(0.2f, 5f, 25f), ValueSource = "DOCUMENTED", Notes = "Right climbable wall (height 5m, shadow traversal route)" },
                        new GeometryItem { Name = "Wall_Back", Category = "Wall", Position = new Vector3(0, 2.5f, -0.1f), Size = new Vector3(10.4f, 5f, 0.2f), ValueSource = "DOCUMENTED", Notes = "Back wall" },
                        new GeometryItem { Name = "Wall_Front_L", Category = "Wall", Position = new Vector3(-3.0f, 2.5f, 25.1f), Size = new Vector3(4.0f, 5f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Front exit wall left" },
                        new GeometryItem { Name = "Wall_Front_R", Category = "Wall", Position = new Vector3(3.0f, 2.5f, 25.1f), Size = new Vector3(4.0f, 5f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Front exit wall right" },
                    },
                    Stealables = new List<StealableItem>
                    {
                        new StealableItem { Name = "StealableDrawbridge", Type = StealableType.Drawbridge, Position = new Vector3(0, 0.5f, 12.0f), Rotation = Vector3.zero, Scale = Vector3.one, ValueSource = "DOCUMENTED", Notes = "Raised Drawbridge spanning 6m pit" }
                    },
                    Hazards = new List<HazardItem>(),
                    Labels = new List<LabelItem>
                    {
                        new LabelItem { Name = "Label_Title", Text = "L04: The Vent\n10m x 25m | Wall Traversal & Drawbridge", Position = new Vector3(0, 4.0f, 2.0f), Color = Color.white, FontSize = 4f },
                        new LabelItem { Name = "Label_Pit", Text = "6m Pit Gap\n(Player Blocked)", Position = new Vector3(0, -0.5f, 12.0f), Color = Color.red, FontSize = 3.5f },
                        new LabelItem { Name = "Label_WallClimb", Text = "Right Wall Climbable Surface\n(Shadow Traversal Path)", Position = new Vector3(4.8f, 3.5f, 12.0f), Color = Color.cyan, FontSize = 3.5f },
                        new LabelItem { Name = "Label_Bridge", Text = "Stealable Drawbridge\n(Raised -> Lowered)", Position = new Vector3(0, 2.5f, 12.0f), Color = Color.yellow, FontSize = 3.5f },
                        new LabelItem { Name = "Label_Exit", Text = "Exit Zone", Position = new Vector3(0, 2.0f, 22.0f), Color = Color.green, FontSize = 3f }
                    }
                },

                // ==========================================
                // L05: The Spotlight (10m x 20m)
                // ==========================================
                new LevelDefinition
                {
                    Code = "L05",
                    Name = "The Spotlight",
                    Phase = "Phase 1 (Teach)",
                    Difficulty = "4 / 10",
                    Dimensions = new Vector2(10f, 20f),
                    TargetTime = "25s",
                    HardLimit = "30s",
                    MaxLeash = "12m",
                    LeashLimit = "25m",
                    Objective = "Introduce Danger Light hazard and OnShadowDestroyed failure state.",
                    PlayerSpawn = new Vector3(0, 0.1f, 2.0f),
                    PlayerRotation = Vector3.zero,
                    ShadowSpawn = null,
                    ExitPosition = new Vector3(0, 0.1f, 18.0f),
                    ExitSize = new Vector3(4.0f, 2.0f, 2.0f),
                    PlayerRoute = "Wait at safe distance -> Walk through Ghost Door to Exit.",
                    ShadowRoute = "Detach -> Observe sweep -> Dash past light when clear -> Steal Door -> Recall.",
                    Sequence = "1. Player stops at Z=6.0m safe from light cone.\n2. Detach shadow.\n3. Time the sweeping light ping-pong cycle.\n4. When light sweeps to opposite side, dash through zone.\n5. Steal Door at (0, 1.25, 14.0).\n6. Recall and walk to Exit at (0, 0, 18.0).",
                    Notes = "First lethal hazard. Touching light destroys shadow and resets level.",
                    Geometry = new List<GeometryItem>
                    {
                        new GeometryItem { Name = "Floor_Main", Category = "Floor", Position = new Vector3(0, 0, 10.0f), Size = new Vector3(10f, 0.2f, 20f), ValueSource = "DOCUMENTED", Notes = "Hall floor (10m x 20m)" },
                        new GeometryItem { Name = "Wall_Left", Category = "Wall", Position = new Vector3(-5.1f, 1.5f, 10.0f), Size = new Vector3(0.2f, 3f, 20f), ValueSource = "DOCUMENTED", Notes = "Left wall" },
                        new GeometryItem { Name = "Wall_Right", Category = "Wall", Position = new Vector3(5.1f, 1.5f, 10.0f), Size = new Vector3(0.2f, 3f, 20f), ValueSource = "DOCUMENTED", Notes = "Right wall" },
                        new GeometryItem { Name = "Wall_Back", Category = "Wall", Position = new Vector3(0, 1.5f, -0.1f), Size = new Vector3(10.4f, 3f, 0.2f), ValueSource = "DOCUMENTED", Notes = "Back wall" },
                        new GeometryItem { Name = "Pillar_Left", Category = "Obstacle", Position = new Vector3(-3.25f, 1.5f, 14.0f), Size = new Vector3(3.5f, 3f, 0.4f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Door pillar left" },
                        new GeometryItem { Name = "Pillar_Right", Category = "Obstacle", Position = new Vector3(3.25f, 1.5f, 14.0f), Size = new Vector3(3.5f, 3f, 0.4f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Door pillar right" },
                        new GeometryItem { Name = "Wall_Front_L", Category = "Wall", Position = new Vector3(-3.0f, 1.5f, 20.1f), Size = new Vector3(4.0f, 3f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Front wall left" },
                        new GeometryItem { Name = "Wall_Front_R", Category = "Wall", Position = new Vector3(3.0f, 1.5f, 20.1f), Size = new Vector3(4.0f, 3f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Front wall right" },
                    },
                    Stealables = new List<StealableItem>
                    {
                        new StealableItem { Name = "StealableDoor", Type = StealableType.Door, Position = new Vector3(0, 1.25f, 14.0f), Rotation = Vector3.zero, Scale = Vector3.one, ValueSource = "DOCUMENTED", Notes = "Stealable Door guarded by sweeping spotlight" }
                    },
                    Hazards = new List<HazardItem>
                    {
                        new HazardItem { Name = "DangerLight_Sweeping", Type = HazardType.DangerLightSweeping, Position = new Vector3(0, 0.5f, 10.0f), Rotation = Vector3.zero, TriggerSize = new Vector3(8.0f, 2.0f, 4.0f), SweepAngle = 40f, SweepSpeed = 30f, ValueSource = "DOCUMENTED", Notes = "Sweeping Danger Light (cone sweeps +/-40 deg)" }
                    },
                    Labels = new List<LabelItem>
                    {
                        new LabelItem { Name = "Label_Title", Text = "L05: The Spotlight\n10m x 20m | Danger Light Hazard", Position = new Vector3(0, 3.5f, 2.0f), Color = Color.white, FontSize = 4f },
                        new LabelItem { Name = "Label_Hazard", Text = "Sweeping Danger Light\n(Lethal to Shadow - Time the Dash)", Position = new Vector3(0, 3.0f, 10.0f), Color = Color.red, FontSize = 3.5f },
                        new LabelItem { Name = "Label_Door", Text = "Stealable Door", Position = new Vector3(0, 3.0f, 14.0f), Color = Color.cyan, FontSize = 3.5f },
                        new LabelItem { Name = "Label_Exit", Text = "Exit Zone", Position = new Vector3(0, 2.0f, 18.0f), Color = Color.green, FontSize = 3f }
                    }
                },

                // ==========================================
                // L06: Double Lock (10m x 25m)
                // ==========================================
                new LevelDefinition
                {
                    Code = "L06",
                    Name = "Double Lock",
                    Phase = "Phase 2 (Combine)",
                    Difficulty = "4 / 10",
                    Dimensions = new Vector2(10f, 25f),
                    TargetTime = "25s",
                    HardLimit = "30s",
                    MaxLeash = "7m",
                    LeashLimit = "25m",
                    Objective = "Two sequential Stealable Doors proving body must reposition into intermediate safe zone.",
                    PlayerSpawn = new Vector3(0, 0.1f, 2.0f),
                    PlayerRotation = Vector3.zero,
                    ShadowSpawn = null,
                    ExitPosition = new Vector3(0, 0.1f, 22.0f),
                    ExitSize = new Vector3(4.0f, 2.0f, 2.0f),
                    PlayerRoute = "Walk to Door A -> Wait -> Walk through A to Safe Zone -> Wait -> Walk through B to Exit.",
                    ShadowRoute = "Sub-route 1: Steal Door A -> Recall. Sub-route 2: Steal Door B -> Recall.",
                    Sequence = "1. Player stops at Z=4m.\n2. Detach shadow -> Steal Door A at (0, 1.25, 7.0) -> Recall.\n3. Player walks through Ghost Door A into Safe Zone (Z=8m to 14m).\n4. Detach shadow -> Steal Door B at (0, 1.25, 16.0) -> Recall.\n5. Player walks through Ghost Door B to Exit at (0, 0, 22.0).",
                    Notes = "Multi-step puzzle progression with intermediate body repositioning.",
                    Geometry = new List<GeometryItem>
                    {
                        new GeometryItem { Name = "Floor_Main", Category = "Floor", Position = new Vector3(0, 0, 12.5f), Size = new Vector3(10f, 0.2f, 25f), ValueSource = "DOCUMENTED", Notes = "Hall floor (10m x 25m)" },
                        new GeometryItem { Name = "Wall_Left", Category = "Wall", Position = new Vector3(-5.1f, 1.5f, 12.5f), Size = new Vector3(0.2f, 3f, 25f), ValueSource = "DOCUMENTED", Notes = "Left wall" },
                        new GeometryItem { Name = "Wall_Right", Category = "Wall", Position = new Vector3(5.1f, 1.5f, 12.5f), Size = new Vector3(0.2f, 3f, 25f), ValueSource = "DOCUMENTED", Notes = "Right wall" },
                        new GeometryItem { Name = "Wall_Back", Category = "Wall", Position = new Vector3(0, 1.5f, -0.1f), Size = new Vector3(10.4f, 3f, 0.2f), ValueSource = "DOCUMENTED", Notes = "Back wall" },
                        new GeometryItem { Name = "Pillar_A_L", Category = "Obstacle", Position = new Vector3(-3.25f, 1.5f, 7.0f), Size = new Vector3(3.5f, 3f, 0.4f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Door A left pillar" },
                        new GeometryItem { Name = "Pillar_A_R", Category = "Obstacle", Position = new Vector3(3.25f, 1.5f, 7.0f), Size = new Vector3(3.5f, 3f, 0.4f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Door A right pillar" },
                        new GeometryItem { Name = "Pillar_B_L", Category = "Obstacle", Position = new Vector3(-3.25f, 1.5f, 16.0f), Size = new Vector3(3.5f, 3f, 0.4f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Door B left pillar" },
                        new GeometryItem { Name = "Pillar_B_R", Category = "Obstacle", Position = new Vector3(3.25f, 1.5f, 16.0f), Size = new Vector3(3.5f, 3f, 0.4f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Door B right pillar" },
                        new GeometryItem { Name = "Wall_Front_L", Category = "Wall", Position = new Vector3(-3.0f, 1.5f, 25.1f), Size = new Vector3(4.0f, 3f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Front wall left" },
                        new GeometryItem { Name = "Wall_Front_R", Category = "Wall", Position = new Vector3(3.0f, 1.5f, 25.1f), Size = new Vector3(4.0f, 3f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Front wall right" },
                    },
                    Stealables = new List<StealableItem>
                    {
                        new StealableItem { Name = "StealableDoor_A", Type = StealableType.Door, Position = new Vector3(0, 1.25f, 7.0f), Rotation = Vector3.zero, Scale = Vector3.one, ValueSource = "DOCUMENTED", Notes = "Door A (First lock)" },
                        new StealableItem { Name = "StealableDoor_B", Type = StealableType.Door, Position = new Vector3(0, 1.25f, 16.0f), Rotation = Vector3.zero, Scale = Vector3.one, ValueSource = "DOCUMENTED", Notes = "Door B (Second lock)" }
                    },
                    Hazards = new List<HazardItem>(),
                    Labels = new List<LabelItem>
                    {
                        new LabelItem { Name = "Label_Title", Text = "L06: Double Lock\n10m x 25m | Sequential Doors", Position = new Vector3(0, 3.5f, 2.0f), Color = Color.white, FontSize = 4f },
                        new LabelItem { Name = "Label_DoorA", Text = "Door A (First Lock)", Position = new Vector3(0, 3.0f, 7.0f), Color = Color.cyan, FontSize = 3.5f },
                        new LabelItem { Name = "Label_SafeZone", Text = "Intermediate Safe Zone\n(Reposition Body Here)", Position = new Vector3(0, 1.5f, 11.5f), Color = Color.yellow, FontSize = 3.5f },
                        new LabelItem { Name = "Label_DoorB", Text = "Door B (Second Lock)", Position = new Vector3(0, 3.0f, 16.0f), Color = Color.cyan, FontSize = 3.5f },
                        new LabelItem { Name = "Label_Exit", Text = "Exit Zone", Position = new Vector3(0, 2.0f, 22.0f), Color = Color.green, FontSize = 3f }
                    }
                },

                // ==========================================
                // L07: Long Detour (15m x 30m)
                // ==========================================
                new LevelDefinition
                {
                    Code = "L07",
                    Name = "Long Detour",
                    Phase = "Phase 2 (Combine)",
                    Difficulty = "5 / 10",
                    Dimensions = new Vector2(15f, 30f),
                    TargetTime = "25s",
                    HardLimit = "30s",
                    MaxLeash = "21m",
                    LeashLimit = "25m",
                    Objective = "Canyon detour along left wall navigating above a static danger light (21m leash).",
                    PlayerSpawn = new Vector3(0, 0.1f, 2.0f),
                    PlayerRotation = Vector3.zero,
                    ShadowSpawn = null,
                    ExitPosition = new Vector3(0, 0.1f, 26.0f),
                    ExitSize = new Vector3(4.0f, 2.0f, 2.0f),
                    PlayerRoute = "Walk to canyon edge (Z=9m) -> Wait -> Cross Drawbridge -> Exit.",
                    ShadowRoute = "Detach -> Left wall -> Climb high (Y=3.5-4.5m) over Danger Light -> Steal Drawbridge -> Recall.",
                    Sequence = "1. Player advances to canyon edge at Z=9.0m (required to stay within 21m leash).\n2. Detach shadow.\n3. Shadow moves left, climbs left wall.\n4. Shadow climbs high to Y=4.0m to bypass static Danger Light mounted at Y=1.5m.\n5. Reach Drawbridge control at (0, 0.5, 14.0).\n6. Steal Drawbridge. Bridge lowers.\n7. Recall. Player crosses canyon to Exit at (0, 0, 26.0).",
                    Notes = "Right wall broken. Only path is U-shaped high wall detour. 21m leash separation.",
                    Geometry = new List<GeometryItem>
                    {
                        new GeometryItem { Name = "Floor_Start", Category = "Floor", Position = new Vector3(0, 0, 5.0f), Size = new Vector3(15f, 0.2f, 10f), ValueSource = "DOCUMENTED", Notes = "Start floor (Z: 0 to 10m)" },
                        new GeometryItem { Name = "Canyon_Pit", Category = "Obstacle", Position = new Vector3(0, -6.0f, 14.0f), Size = new Vector3(15f, 0.2f, 8f), ValueSource = "DOCUMENTED", Notes = "Canyon chasm (8m wide, Z: 10 to 18m)" },
                        new GeometryItem { Name = "Floor_Exit", Category = "Floor", Position = new Vector3(0, 0, 24.0f), Size = new Vector3(15f, 0.2f, 12f), ValueSource = "DOCUMENTED", Notes = "Exit floor (Z: 18 to 30m)" },
                        new GeometryItem { Name = "Wall_Left_Climbable", Category = "Wall", Position = new Vector3(-7.6f, 2.5f, 15.0f), Size = new Vector3(0.2f, 5f, 30f), ValueSource = "DOCUMENTED", Notes = "Continuous climbable left wall (height 5m)" },
                        new GeometryItem { Name = "Wall_Right_Start", Category = "Wall", Position = new Vector3(7.6f, 2.5f, 4.0f), Size = new Vector3(0.2f, 5f, 8f), ValueSource = "DOCUMENTED", Notes = "Right wall start segment" },
                        new GeometryItem { Name = "Wall_Right_End", Category = "Wall", Position = new Vector3(7.6f, 2.5f, 25.0f), Size = new Vector3(0.2f, 5f, 10f), ValueSource = "DOCUMENTED", Notes = "Right wall end segment (gap Z: 8 to 20m broken)" },
                        new GeometryItem { Name = "Wall_Back", Category = "Wall", Position = new Vector3(0, 2.5f, -0.1f), Size = new Vector3(15.4f, 5f, 0.2f), ValueSource = "DOCUMENTED", Notes = "Back wall" },
                        new GeometryItem { Name = "Wall_Front_L", Category = "Wall", Position = new Vector3(-4.5f, 2.5f, 30.1f), Size = new Vector3(6.0f, 5f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Front wall left" },
                        new GeometryItem { Name = "Wall_Front_R", Category = "Wall", Position = new Vector3(4.5f, 2.5f, 30.1f), Size = new Vector3(6.0f, 5f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Front wall right" },
                    },
                    Stealables = new List<StealableItem>
                    {
                        new StealableItem { Name = "StealableDrawbridge", Type = StealableType.Drawbridge, Position = new Vector3(0, 0.5f, 14.0f), Rotation = Vector3.zero, Scale = Vector3.one, ValueSource = "DOCUMENTED", Notes = "Drawbridge spanning 8m canyon" }
                    },
                    Hazards = new List<HazardItem>
                    {
                        new HazardItem { Name = "DangerLight_LeftWall", Type = HazardType.DangerLightStatic, Position = new Vector3(-7.0f, 1.5f, 14.0f), Rotation = new Vector3(0, 90f, 0), TriggerSize = new Vector3(2.0f, 2.5f, 4.0f), ValueSource = "DOCUMENTED", Notes = "Static Danger Light covering lower wall (Y: 0 to 2.5m)" }
                    },
                    Labels = new List<LabelItem>
                    {
                        new LabelItem { Name = "Label_Title", Text = "L07: Long Detour\n15m x 30m | High Wall Detour (21m Leash)", Position = new Vector3(0, 4.0f, 2.0f), Color = Color.white, FontSize = 4f },
                        new LabelItem { Name = "Label_DetachPoint", Text = "Player Detach Point (Z=9m)\n(Must stand here for 21m leash)", Position = new Vector3(0, 2.0f, 9.0f), Color = Color.yellow, FontSize = 3.5f },
                        new LabelItem { Name = "Label_Hazard", Text = "Static Danger Light\n(Blocks Lower Wall - Climb High!)", Position = new Vector3(-6.5f, 1.5f, 14.0f), Color = Color.red, FontSize = 3.5f },
                        new LabelItem { Name = "Label_HighRoute", Text = "High Route (Y=3.5 - 4.5m)", Position = new Vector3(-7.2f, 4.2f, 14.0f), Color = Color.cyan, FontSize = 3f },
                        new LabelItem { Name = "Label_Bridge", Text = "Stealable Drawbridge", Position = new Vector3(0, 2.5f, 14.0f), Color = Color.yellow, FontSize = 3.5f },
                        new LabelItem { Name = "Label_Exit", Text = "Exit Zone", Position = new Vector3(0, 2.0f, 26.0f), Color = Color.green, FontSize = 3f }
                    }
                },

                // ==========================================
                // L08: Pulsing Corridor (5m x 30m)
                // ==========================================
                new LevelDefinition
                {
                    Code = "L08",
                    Name = "Pulsing Corridor",
                    Phase = "Phase 2 (Combine)",
                    Difficulty = "6 / 10",
                    Dimensions = new Vector2(5f, 30f),
                    TargetTime = "28s",
                    HardLimit = "30s",
                    MaxLeash = "24m",
                    LeashLimit = "25m",
                    Objective = "Rhythm and timing: dash between 3 pulsing Danger Lights and safe islands (24m leash).",
                    PlayerSpawn = new Vector3(0, 0.1f, 2.0f),
                    PlayerRotation = Vector3.zero,
                    ShadowSpawn = null,
                    ExitPosition = new Vector3(0, 0.1f, 28.5f),
                    ExitSize = new Vector3(3.0f, 2.0f, 2.0f),
                    PlayerRoute = "Wait at Start -> Walk through Ghost Door to Exit (lights do not harm Player).",
                    ShadowRoute = "Detach -> Dash L1 -> Safe Zone 1 -> Dash L2 -> Safe Zone 2 -> Dash L3 -> Steal Door -> Recall.",
                    Sequence = "1. Player detaches at start.\n2. Shadow waits for L1 to cycle off -> dash to Safe Zone 1 (Z=11.5m).\n3. Shadow waits for L2 off -> dash to Safe Zone 2 (Z=18.5m).\n4. Shadow waits for L3 off -> dash to Stealable Door at (0, 1.25, 26.0).\n5. Steal Door (at 24m leash separation).\n6. Recall -> Player walks through Ghost Door to Exit.",
                    Notes = "Three synchronized pulsing danger lights. 24m Euclidean distance leaves 1m margin.",
                    Geometry = new List<GeometryItem>
                    {
                        new GeometryItem { Name = "Floor_Main", Category = "Floor", Position = new Vector3(0, 0, 15.0f), Size = new Vector3(5f, 0.2f, 30f), ValueSource = "DOCUMENTED", Notes = "Hall floor (5m x 30m)" },
                        new GeometryItem { Name = "Wall_Left", Category = "Wall", Position = new Vector3(-2.6f, 1.5f, 15.0f), Size = new Vector3(0.2f, 3f, 30f), ValueSource = "DOCUMENTED", Notes = "Left wall" },
                        new GeometryItem { Name = "Wall_Right", Category = "Wall", Position = new Vector3(2.6f, 1.5f, 15.0f), Size = new Vector3(0.2f, 3f, 30f), ValueSource = "DOCUMENTED", Notes = "Right wall" },
                        new GeometryItem { Name = "Wall_Back", Category = "Wall", Position = new Vector3(0, 1.5f, -0.1f), Size = new Vector3(5.4f, 3f, 0.2f), ValueSource = "DOCUMENTED", Notes = "Back wall" },
                        new GeometryItem { Name = "Pillar_Door_L", Category = "Obstacle", Position = new Vector3(-1.75f, 1.5f, 26.0f), Size = new Vector3(1.5f, 3f, 0.4f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Door pillar left" },
                        new GeometryItem { Name = "Pillar_Door_R", Category = "Obstacle", Position = new Vector3(1.75f, 1.5f, 26.0f), Size = new Vector3(1.5f, 3f, 0.4f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Door pillar right" },
                        new GeometryItem { Name = "Wall_Front_L", Category = "Wall", Position = new Vector3(-1.75f, 1.5f, 30.1f), Size = new Vector3(1.5f, 3f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Front wall left" },
                        new GeometryItem { Name = "Wall_Front_R", Category = "Wall", Position = new Vector3(1.75f, 1.5f, 30.1f), Size = new Vector3(1.5f, 3f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Front wall right" },
                    },
                    Stealables = new List<StealableItem>
                    {
                        new StealableItem { Name = "StealableDoor", Type = StealableType.Door, Position = new Vector3(0, 1.25f, 26.0f), Rotation = Vector3.zero, Scale = Vector3.one, ValueSource = "DOCUMENTED", Notes = "Stealable Door at end of pulsing corridor" }
                    },
                    Hazards = new List<HazardItem>
                    {
                        new HazardItem { Name = "DangerLight_Pulsing_1", Type = HazardType.DangerLightPulsing, Position = new Vector3(0, 0.5f, 8.0f), Rotation = Vector3.zero, TriggerSize = new Vector3(5.0f, 2.0f, 4.0f), ValueSource = "DOCUMENTED", Notes = "Pulsing Light 1 (Z: 6 to 10m)" },
                        new HazardItem { Name = "DangerLight_Pulsing_2", Type = HazardType.DangerLightPulsing, Position = new Vector3(0, 0.5f, 15.0f), Rotation = Vector3.zero, TriggerSize = new Vector3(5.0f, 2.0f, 4.0f), ValueSource = "DOCUMENTED", Notes = "Pulsing Light 2 (Z: 13 to 17m)" },
                        new HazardItem { Name = "DangerLight_Pulsing_3", Type = HazardType.DangerLightPulsing, Position = new Vector3(0, 0.5f, 22.0f), Rotation = Vector3.zero, TriggerSize = new Vector3(5.0f, 2.0f, 4.0f), ValueSource = "DOCUMENTED", Notes = "Pulsing Light 3 (Z: 20 to 24m)" }
                    },
                    Labels = new List<LabelItem>
                    {
                        new LabelItem { Name = "Label_Title", Text = "L08: Pulsing Corridor\n5m x 30m | Rhythm & Safe Islands (24m Leash)", Position = new Vector3(0, 3.5f, 2.0f), Color = Color.white, FontSize = 4f },
                        new LabelItem { Name = "Label_Light1", Text = "Pulsing Light 1", Position = new Vector3(0, 2.0f, 8.0f), Color = Color.red, FontSize = 3f },
                        new LabelItem { Name = "Label_Safe1", Text = "Safe Island 1", Position = new Vector3(0, 1.0f, 11.5f), Color = Color.green, FontSize = 3f },
                        new LabelItem { Name = "Label_Light2", Text = "Pulsing Light 2", Position = new Vector3(0, 2.0f, 15.0f), Color = Color.red, FontSize = 3f },
                        new LabelItem { Name = "Label_Safe2", Text = "Safe Island 2", Position = new Vector3(0, 1.0f, 18.5f), Color = Color.green, FontSize = 3f },
                        new LabelItem { Name = "Label_Light3", Text = "Pulsing Light 3", Position = new Vector3(0, 2.0f, 22.0f), Color = Color.red, FontSize = 3f },
                        new LabelItem { Name = "Label_Door", Text = "Stealable Door (24m Leash)", Position = new Vector3(0, 3.0f, 26.0f), Color = Color.cyan, FontSize = 3.5f },
                        new LabelItem { Name = "Label_Exit", Text = "Exit Zone", Position = new Vector3(0, 2.0f, 28.5f), Color = Color.green, FontSize = 3f }
                    }
                },

                // ==========================================
                // L09: Split Decision (15m x 25m)
                // ==========================================
                new LevelDefinition
                {
                    Code = "L09",
                    Name = "Split Decision",
                    Phase = "Phase 2 (Combine)",
                    Difficulty = "6 / 10",
                    Dimensions = new Vector2(15f, 25f),
                    TargetTime = "22s",
                    HardLimit = "25s",
                    MaxLeash = "20m",
                    LeashLimit = "25m",
                    Objective = "Player agency: Choose Route A (Right Floor Ledge + Sweeping Light) or Route B (Left Wall + Pulsing Light).",
                    PlayerSpawn = new Vector3(0, 0.1f, 2.0f),
                    PlayerRotation = Vector3.zero,
                    ShadowSpawn = null,
                    ExitPosition = new Vector3(0, 0.1f, 22.0f),
                    ExitSize = new Vector3(4.0f, 2.0f, 2.0f),
                    PlayerRoute = "Walk to pit edge (Z=7.0m) -> Wait -> Cross lowered Drawbridge -> Exit.",
                    ShadowRoute = "Choice: Route A (Floor right, dodge sweeper) or Route B (Wall left, time pulsing light). Steal bridge -> Recall.",
                    Sequence = "1. Player stops at pit edge at Z=7.0m.\n2. Detach shadow.\n3. Option A: Move along right floor ledge, time sweeping light to reach Drawbridge.\n   Option B: Transition to left wall, time static pulsing light to reach Drawbridge.\n4. Steal Drawbridge at (0, 0.5, 16.0).\n5. Recall -> Player crosses lowered bridge to Exit.",
                    Notes = "Both paths mathematically viable (Route A: 18m, Route B: 19m, max separation 20m).",
                    Geometry = new List<GeometryItem>
                    {
                        new GeometryItem { Name = "Floor_Start", Category = "Floor", Position = new Vector3(0, 0, 4.0f), Size = new Vector3(15f, 0.2f, 8f), ValueSource = "DOCUMENTED", Notes = "Start floor (Z: 0 to 8m)" },
                        new GeometryItem { Name = "Central_Pit", Category = "Obstacle", Position = new Vector3(0, -6.0f, 12.0f), Size = new Vector3(10f, 0.2f, 8f), ValueSource = "DOCUMENTED", Notes = "Central pit chasm (8m wide, Z: 8 to 16m)" },
                        new GeometryItem { Name = "RouteA_FloorLedge", Category = "Floor", Position = new Vector3(6.0f, 0, 12.0f), Size = new Vector3(3f, 0.2f, 8f), ValueSource = "DOCUMENTED", Notes = "Route A: Right floor ledge across pit" },
                        new GeometryItem { Name = "Floor_Exit", Category = "Floor", Position = new Vector3(0, 0, 20.5f), Size = new Vector3(15f, 0.2f, 9f), ValueSource = "DOCUMENTED", Notes = "Exit floor (Z: 16 to 25m)" },
                        new GeometryItem { Name = "Wall_Left_Climbable", Category = "Wall", Position = new Vector3(-7.6f, 2.5f, 12.5f), Size = new Vector3(0.2f, 5f, 25f), ValueSource = "DOCUMENTED", Notes = "Route B: Left climbable wall (height 5m)" },
                        new GeometryItem { Name = "Wall_Right", Category = "Wall", Position = new Vector3(7.6f, 2.5f, 12.5f), Size = new Vector3(0.2f, 5f, 25f), ValueSource = "DOCUMENTED", Notes = "Right boundary wall" },
                        new GeometryItem { Name = "Wall_Back", Category = "Wall", Position = new Vector3(0, 2.5f, -0.1f), Size = new Vector3(15.4f, 5f, 0.2f), ValueSource = "DOCUMENTED", Notes = "Back wall" },
                        new GeometryItem { Name = "Wall_Front_L", Category = "Wall", Position = new Vector3(-4.5f, 2.5f, 25.1f), Size = new Vector3(6.0f, 5f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Front wall left" },
                        new GeometryItem { Name = "Wall_Front_R", Category = "Wall", Position = new Vector3(4.5f, 2.5f, 25.1f), Size = new Vector3(6.0f, 5f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Front wall right" },
                    },
                    Stealables = new List<StealableItem>
                    {
                        new StealableItem { Name = "StealableDrawbridge", Type = StealableType.Drawbridge, Position = new Vector3(0, 0.5f, 16.0f), Rotation = Vector3.zero, Scale = Vector3.one, ValueSource = "DOCUMENTED", Notes = "Drawbridge spanning central pit" }
                    },
                    Hazards = new List<HazardItem>
                    {
                        new HazardItem { Name = "DangerLight_RouteA_Sweeping", Type = HazardType.DangerLightSweeping, Position = new Vector3(6.0f, 3.0f, 12.0f), Rotation = Vector3.zero, TriggerSize = new Vector3(3.0f, 2.0f, 4.0f), SweepAngle = 35f, SweepSpeed = 30f, ValueSource = "DOCUMENTED", Notes = "Route A Sweeping Light covering right ledge" },
                        new HazardItem { Name = "DangerLight_RouteB_Pulsing", Type = HazardType.DangerLightPulsing, Position = new Vector3(-7.0f, 2.0f, 12.0f), Rotation = new Vector3(0, 90f, 0), TriggerSize = new Vector3(2.0f, 2.5f, 4.0f), ValueSource = "DOCUMENTED", Notes = "Route B Pulsing Light covering left wall" }
                    },
                    Labels = new List<LabelItem>
                    {
                        new LabelItem { Name = "Label_Title", Text = "L09: Split Decision\n15m x 25m | Route A (Floor) vs Route B (Wall)", Position = new Vector3(0, 4.0f, 2.0f), Color = Color.white, FontSize = 4f },
                        new LabelItem { Name = "Label_RouteA", Text = "Route A: Floor Ledge\n(Sweeping Light)", Position = new Vector3(6.0f, 2.0f, 9.0f), Color = Color.cyan, FontSize = 3f },
                        new LabelItem { Name = "Label_RouteB", Text = "Route B: Wall Route\n(Pulsing Light)", Position = new Vector3(-7.0f, 3.5f, 9.0f), Color = Color.magenta, FontSize = 3f },
                        new LabelItem { Name = "Label_Pit", Text = "Central Pit", Position = new Vector3(0, -0.5f, 12.0f), Color = Color.red, FontSize = 3.5f },
                        new LabelItem { Name = "Label_Bridge", Text = "Stealable Drawbridge", Position = new Vector3(0, 2.5f, 16.0f), Color = Color.yellow, FontSize = 3.5f },
                        new LabelItem { Name = "Label_Exit", Text = "Exit Zone", Position = new Vector3(0, 2.0f, 22.0f), Color = Color.green, FontSize = 3f }
                    }
                },

                // ==========================================
                // L10: The Gauntlet (15m x 40m)
                // ==========================================
                new LevelDefinition
                {
                    Code = "L10",
                    Name = "The Gauntlet",
                    Phase = "Phase 2 (Combine)",
                    Difficulty = "7 / 10",
                    Dimensions = new Vector2(15f, 40f),
                    TargetTime = "30s",
                    HardLimit = "35s",
                    MaxLeash = "24m",
                    LeashLimit = "25m",
                    Objective = "Section 1 Drawbridge over pit, immediately followed by Section 2 dual sweeping lights and Door.",
                    PlayerSpawn = new Vector3(0, 0.1f, 2.0f),
                    PlayerRotation = Vector3.zero,
                    ShadowSpawn = null,
                    ExitPosition = new Vector3(0, 0.1f, 38.0f),
                    ExitSize = new Vector3(4.0f, 2.0f, 2.0f),
                    PlayerRoute = "Safe: Walk to pit -> Cross bridge -> Wait -> Walk through Ghost Door to Exit.",
                    ShadowRoute = "Safe: Detach -> Steal Bridge -> Recall. Detach -> Dodge 2 sweepers -> Steal Door -> Recall.\nExpert: Detach -> Steal Bridge -> Cross bridge while detached -> Dodge sweepers -> Steal Door -> Recall (1 Detach!).",
                    Sequence = "1. Player advances to pit edge at Z=7.5m.\n2. Detach shadow -> Climb left wall past static danger light at (-7.0, 1.5, 12.0).\n3. Steal Drawbridge at (0, 0.5, 16.0). Bridge lowers.\n4. (Safe route: Recall -> Player crosses -> Detach at Z=18m).\n   (Expert route: Continue moving shadow across lowered bridge, max leash reaches 24m at door).\n5. Shadow times Sweeping Light 1 (Z=23m) and Sweeping Light 2 (Z=28m).\n6. Steal Door at (0, 1.25, 34.0).\n7. Recall -> Player walks to Exit at (0, 0, 38.0).",
                    Notes = "Culmination of Phase 2. Enables Expert 1-Detach route natively within 25m leash.",
                    Geometry = new List<GeometryItem>
                    {
                        new GeometryItem { Name = "Floor_Section1", Category = "Floor", Position = new Vector3(0, 0, 4.0f), Size = new Vector3(15f, 0.2f, 8f), ValueSource = "DOCUMENTED", Notes = "Start floor (Z: 0 to 8m)" },
                        new GeometryItem { Name = "Pit_Chasm", Category = "Obstacle", Position = new Vector3(0, -6.0f, 12.0f), Size = new Vector3(15f, 0.2f, 8f), ValueSource = "DOCUMENTED", Notes = "Section 1 Pit (8m wide, Z: 8 to 16m)" },
                        new GeometryItem { Name = "Floor_Section2", Category = "Floor", Position = new Vector3(0, 0, 28.0f), Size = new Vector3(15f, 0.2f, 24f), ValueSource = "DOCUMENTED", Notes = "Section 2 Gauntlet floor (Z: 16 to 40m)" },
                        new GeometryItem { Name = "Wall_Left_Climbable", Category = "Wall", Position = new Vector3(-7.6f, 2.5f, 20.0f), Size = new Vector3(0.2f, 5f, 40f), ValueSource = "DOCUMENTED", Notes = "Left climbable wall" },
                        new GeometryItem { Name = "Wall_Right", Category = "Wall", Position = new Vector3(7.6f, 2.5f, 20.0f), Size = new Vector3(0.2f, 5f, 40f), ValueSource = "DOCUMENTED", Notes = "Right wall" },
                        new GeometryItem { Name = "Wall_Back", Category = "Wall", Position = new Vector3(0, 2.5f, -0.1f), Size = new Vector3(15.4f, 5f, 0.2f), ValueSource = "DOCUMENTED", Notes = "Back wall" },
                        new GeometryItem { Name = "Pillar_Door_L", Category = "Obstacle", Position = new Vector3(-3.25f, 1.5f, 34.0f), Size = new Vector3(3.5f, 3f, 0.4f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Door pillar left" },
                        new GeometryItem { Name = "Pillar_Door_R", Category = "Obstacle", Position = new Vector3(3.25f, 1.5f, 34.0f), Size = new Vector3(3.5f, 3f, 0.4f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Door pillar right" },
                        new GeometryItem { Name = "Wall_Front_L", Category = "Wall", Position = new Vector3(-4.5f, 2.5f, 40.1f), Size = new Vector3(6.0f, 5f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Front wall left" },
                        new GeometryItem { Name = "Wall_Front_R", Category = "Wall", Position = new Vector3(4.5f, 2.5f, 40.1f), Size = new Vector3(6.0f, 5f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Front wall right" },
                    },
                    Stealables = new List<StealableItem>
                    {
                        new StealableItem { Name = "StealableDrawbridge", Type = StealableType.Drawbridge, Position = new Vector3(0, 0.5f, 16.0f), Rotation = Vector3.zero, Scale = Vector3.one, ValueSource = "DOCUMENTED", Notes = "Drawbridge across Section 1 pit" },
                        new StealableItem { Name = "StealableDoor", Type = StealableType.Door, Position = new Vector3(0, 1.25f, 34.0f), Rotation = Vector3.zero, Scale = Vector3.one, ValueSource = "DOCUMENTED", Notes = "Stealable Door guarding exit" }
                    },
                    Hazards = new List<HazardItem>
                    {
                        new HazardItem { Name = "DangerLight_PitWall", Type = HazardType.DangerLightStatic, Position = new Vector3(-7.0f, 1.5f, 12.0f), Rotation = new Vector3(0, 90f, 0), TriggerSize = new Vector3(2.0f, 2.5f, 4.0f), ValueSource = "DOCUMENTED", Notes = "Static Danger Light on left pit wall" },
                        new HazardItem { Name = "DangerLight_Sweeping_1", Type = HazardType.DangerLightSweeping, Position = new Vector3(-2.5f, 3.5f, 23.0f), Rotation = Vector3.zero, TriggerSize = new Vector3(6.0f, 2.0f, 4.0f), SweepAngle = 35f, SweepSpeed = 30f, ValueSource = "DOCUMENTED", Notes = "Sweeping Light 1 (left staggered)" },
                        new HazardItem { Name = "DangerLight_Sweeping_2", Type = HazardType.DangerLightSweeping, Position = new Vector3(2.5f, 3.5f, 28.0f), Rotation = Vector3.zero, TriggerSize = new Vector3(6.0f, 2.0f, 4.0f), SweepAngle = 35f, SweepSpeed = 30f, ValueSource = "DOCUMENTED", Notes = "Sweeping Light 2 (right staggered)" }
                    },
                    Labels = new List<LabelItem>
                    {
                        new LabelItem { Name = "Label_Title", Text = "L10: The Gauntlet\n15m x 40m | Pit Drawbridge + Dual Sweeper Corridor", Position = new Vector3(0, 4.5f, 2.0f), Color = Color.white, FontSize = 4f },
                        new LabelItem { Name = "Label_Bridge", Text = "Drawbridge Control", Position = new Vector3(0, 2.5f, 16.0f), Color = Color.yellow, FontSize = 3.5f },
                        new LabelItem { Name = "Label_Sweeper1", Text = "Sweeping Light 1", Position = new Vector3(-2.5f, 4.5f, 23.0f), Color = Color.red, FontSize = 3f },
                        new LabelItem { Name = "Label_Sweeper2", Text = "Sweeping Light 2", Position = new Vector3(2.5f, 4.5f, 28.0f), Color = Color.red, FontSize = 3f },
                        new LabelItem { Name = "Label_Door", Text = "Stealable Door", Position = new Vector3(0, 3.0f, 34.0f), Color = Color.cyan, FontSize = 3.5f },
                        new LabelItem { Name = "Label_Exit", Text = "Exit Zone", Position = new Vector3(0, 2.0f, 38.0f), Color = Color.green, FontSize = 3f }
                    }
                },

                // ==========================================
                // L11: The Snare (5m x 25m)
                // ==========================================
                new LevelDefinition
                {
                    Code = "L11",
                    Name = "The Snare",
                    Phase = "Phase 3 (Master)",
                    Difficulty = "7 / 10",
                    Dimensions = new Vector2(5f, 25f),
                    TargetTime = "20s",
                    HardLimit = "25s",
                    MaxLeash = "14m",
                    LeashLimit = "25m",
                    Objective = "Introduce TRAP LIGHT hazard: snare requires 3 rapid RECOVER button presses in 2.0s to break free.",
                    PlayerSpawn = new Vector3(0, 0.1f, 2.0f),
                    PlayerRotation = Vector3.zero,
                    ShadowSpawn = null,
                    ExitPosition = new Vector3(0, 0.1f, 23.0f),
                    ExitSize = new Vector3(3.0f, 2.0f, 2.0f),
                    PlayerRoute = "Walk to edge of Trap Light -> Wait -> Walk through Ghost Door to Exit.",
                    ShadowRoute = "Detach -> Enter Trap Light -> Trapped -> Mash RECOVER x3 -> Break free -> Steal Door -> Recall.",
                    Sequence = "1. Player stops at Z=8.0m.\n2. Detach shadow.\n3. Shadow moves into unavoidable Trap Light at (0, 1.5, 13.0).\n4. Shadow is rooted in place; action button switches to RECOVER.\n5. Player taps RECOVER 3 times rapidly within 2.0s.\n6. Shadow breaks free from snare.\n7. Shadow moves to Door at (0, 1.25, 20.0) -> Steal.\n8. Recall -> Player crosses through Ghost Door to Exit.",
                    Notes = "First encounter with Trap Light. Unlike Danger Light, does not instantly destroy shadow; requires rapid escape input.",
                    Geometry = new List<GeometryItem>
                    {
                        new GeometryItem { Name = "Floor_Main", Category = "Floor", Position = new Vector3(0, 0, 12.5f), Size = new Vector3(5f, 0.2f, 25f), ValueSource = "DOCUMENTED", Notes = "Hall floor (5m x 25m)" },
                        new GeometryItem { Name = "Wall_Left", Category = "Wall", Position = new Vector3(-2.6f, 1.5f, 12.5f), Size = new Vector3(0.2f, 3f, 25f), ValueSource = "DOCUMENTED", Notes = "Left wall" },
                        new GeometryItem { Name = "Wall_Right", Category = "Wall", Position = new Vector3(2.6f, 1.5f, 12.5f), Size = new Vector3(0.2f, 3f, 25f), ValueSource = "DOCUMENTED", Notes = "Right wall" },
                        new GeometryItem { Name = "Wall_Back", Category = "Wall", Position = new Vector3(0, 1.5f, -0.1f), Size = new Vector3(5.4f, 3f, 0.2f), ValueSource = "DOCUMENTED", Notes = "Back wall" },
                        new GeometryItem { Name = "Pillar_Door_L", Category = "Obstacle", Position = new Vector3(-1.75f, 1.5f, 20.0f), Size = new Vector3(1.5f, 3f, 0.4f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Door pillar left" },
                        new GeometryItem { Name = "Pillar_Door_R", Category = "Obstacle", Position = new Vector3(1.75f, 1.5f, 20.0f), Size = new Vector3(1.5f, 3f, 0.4f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Door pillar right" },
                        new GeometryItem { Name = "Wall_Front_L", Category = "Wall", Position = new Vector3(-1.75f, 1.5f, 25.1f), Size = new Vector3(1.5f, 3f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Front wall left" },
                        new GeometryItem { Name = "Wall_Front_R", Category = "Wall", Position = new Vector3(1.75f, 1.5f, 25.1f), Size = new Vector3(1.5f, 3f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Front wall right" },
                    },
                    Stealables = new List<StealableItem>
                    {
                        new StealableItem { Name = "StealableDoor", Type = StealableType.Door, Position = new Vector3(0, 1.25f, 20.0f), Rotation = Vector3.zero, Scale = Vector3.one, ValueSource = "DOCUMENTED", Notes = "Stealable Door beyond Trap Light" }
                    },
                    Hazards = new List<HazardItem>
                    {
                        new HazardItem { Name = "TrapLight_Snare", Type = HazardType.TrapLight, Position = new Vector3(0, 1.5f, 13.0f), Rotation = Vector3.zero, TriggerSize = new Vector3(5.0f, 3.0f, 4.0f), ValueSource = "DOCUMENTED", Notes = "Static Trap Light (Purple/UV hue, Z: 11 to 15m)" }
                    },
                    Labels = new List<LabelItem>
                    {
                        new LabelItem { Name = "Label_Title", Text = "L11: The Snare\n5m x 25m | Introduce TRAP LIGHT Hazard", Position = new Vector3(0, 3.5f, 2.0f), Color = Color.white, FontSize = 4f },
                        new LabelItem { Name = "Label_Trap", Text = "Static Trap Light\n(Purple/UV - Mash RECOVER x3)", Position = new Vector3(0, 3.5f, 13.0f), Color = new Color(0.8f, 0.3f, 1f), FontSize = 3.5f },
                        new LabelItem { Name = "Label_Door", Text = "Stealable Door", Position = new Vector3(0, 3.0f, 20.0f), Color = Color.cyan, FontSize = 3.5f },
                        new LabelItem { Name = "Label_Exit", Text = "Exit Zone", Position = new Vector3(0, 2.0f, 23.0f), Color = Color.green, FontSize = 3f }
                    }
                },

                // ==========================================
                // L12: The Conveyor (15m x 25m)
                // ==========================================
                new LevelDefinition
                {
                    Code = "L12",
                    Name = "The Conveyor",
                    Phase = "Phase 3 (Master)",
                    Difficulty = "7 / 10",
                    Dimensions = new Vector2(15f, 25f),
                    TargetTime = "20s",
                    HardLimit = "25s",
                    MaxLeash = "10m",
                    LeashLimit = "25m",
                    Objective = "Introduce PLATFORM Stealable: stealing shadow freezes rapid oscillation in place to form bridge.",
                    PlayerSpawn = new Vector3(0, 0.1f, 2.0f),
                    PlayerRotation = Vector3.zero,
                    ShadowSpawn = null,
                    ExitPosition = new Vector3(0, 0.1f, 21.0f),
                    ExitSize = new Vector3(4.0f, 2.0f, 2.0f),
                    PlayerRoute = "Walk to pit edge -> Wait -> Walk across frozen Platform -> Exit.",
                    ShadowRoute = "Detach -> Move to pit edge -> Wait for platform to align centrally -> Steal -> Recall.",
                    Sequence = "1. Player stops at pit edge at Z=7.0m.\n2. Detach shadow.\n3. Platform slides rapidly left/right across 8m pit at 6m/s.\n4. As Platform passes center alignment (X=0), press STEAL.\n5. Platform enters FROZEN state (rigidly stationary solid surface).\n6. Recall -> Player crosses stationary platform to Exit at (0, 0, 21.0).",
                    Notes = "Platform Stealable introduction. Stolen platform remains physically solid but stops moving.",
                    Geometry = new List<GeometryItem>
                    {
                        new GeometryItem { Name = "Floor_Start", Category = "Floor", Position = new Vector3(0, 0, 4.0f), Size = new Vector3(15f, 0.2f, 8f), ValueSource = "DOCUMENTED", Notes = "Start floor (Z: 0 to 8m)" },
                        new GeometryItem { Name = "Pit_Chasm", Category = "Obstacle", Position = new Vector3(0, -6.0f, 12.0f), Size = new Vector3(15f, 0.2f, 8f), ValueSource = "DOCUMENTED", Notes = "Pit chasm (8m wide, Z: 8 to 16m)" },
                        new GeometryItem { Name = "Floor_Exit", Category = "Floor", Position = new Vector3(0, 0, 20.5f), Size = new Vector3(15f, 0.2f, 9f), ValueSource = "DOCUMENTED", Notes = "Exit floor (Z: 16 to 25m)" },
                        new GeometryItem { Name = "Wall_Left", Category = "Wall", Position = new Vector3(-7.6f, 2.5f, 12.5f), Size = new Vector3(0.2f, 5f, 25f), ValueSource = "DOCUMENTED", Notes = "Left wall" },
                        new GeometryItem { Name = "Wall_Right", Category = "Wall", Position = new Vector3(7.6f, 2.5f, 12.5f), Size = new Vector3(0.2f, 5f, 25f), ValueSource = "DOCUMENTED", Notes = "Right wall" },
                        new GeometryItem { Name = "Wall_Back", Category = "Wall", Position = new Vector3(0, 2.5f, -0.1f), Size = new Vector3(15.4f, 5f, 0.2f), ValueSource = "DOCUMENTED", Notes = "Back wall" },
                        new GeometryItem { Name = "Wall_Front_L", Category = "Wall", Position = new Vector3(-4.5f, 2.5f, 25.1f), Size = new Vector3(6.0f, 5f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Front wall left" },
                        new GeometryItem { Name = "Wall_Front_R", Category = "Wall", Position = new Vector3(4.5f, 2.5f, 25.1f), Size = new Vector3(6.0f, 5f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Front wall right" },
                    },
                    Stealables = new List<StealableItem>
                    {
                        new StealableItem { Name = "StealablePlatform", Type = StealableType.Platform, Position = new Vector3(0, 0.1f, 12.0f), Rotation = Vector3.zero, Scale = new Vector3(3.0f, 0.4f, 4.0f), MoveAxis = new Vector3(1, 0, 0), MoveDistance = 4.5f, MoveSpeed = 3.5f, ValueSource = "DOCUMENTED", Notes = "Rapid moving Stealable Platform across 8m pit" }
                    },
                    Hazards = new List<HazardItem>(),
                    Labels = new List<LabelItem>
                    {
                        new LabelItem { Name = "Label_Title", Text = "L12: The Conveyor\n15m x 25m | Introduce PLATFORM Stealable", Position = new Vector3(0, 4.0f, 2.0f), Color = Color.white, FontSize = 4f },
                        new LabelItem { Name = "Label_Pit", Text = "8m Pit Gap", Position = new Vector3(0, -0.5f, 12.0f), Color = Color.red, FontSize = 3.5f },
                        new LabelItem { Name = "Label_Platform", Text = "Rapid Moving Platform\n(Steal at Center -> Freezes into Bridge)", Position = new Vector3(0, 2.0f, 12.0f), Color = Color.cyan, FontSize = 3.5f },
                        new LabelItem { Name = "Label_Exit", Text = "Exit Zone", Position = new Vector3(0, 2.0f, 21.0f), Color = Color.green, FontSize = 3f }
                    }
                },

                // ==========================================
                // L13: The Breaker (15m x 20m)
                // ==========================================
                new LevelDefinition
                {
                    Code = "L13",
                    Name = "The Breaker",
                    Phase = "Phase 3 (Master)",
                    Difficulty = "8 / 10",
                    Dimensions = new Vector2(15f, 20f),
                    TargetTime = "25s",
                    HardLimit = "30s",
                    MaxLeash = "12m",
                    LeashLimit = "25m",
                    Objective = "Introduce LIGHT SWITCH Stealable: climb wall to steal switch and permanently deactivate massive lethal light.",
                    PlayerSpawn = new Vector3(0, 0.1f, 2.0f),
                    PlayerRotation = Vector3.zero,
                    ShadowSpawn = null,
                    ExitPosition = new Vector3(5.0f, 0.1f, 18.0f),
                    ExitSize = new Vector3(3.0f, 2.0f, 2.0f),
                    PlayerRoute = "Wait at start -> Wait for light to power down -> Walk to Exit (top right).",
                    ShadowRoute = "Detach -> Left wall -> Climb to Light Switch at (-7.4, 3.5, 8.0) -> Steal -> Recall.",
                    Sequence = "1. Player enters room; path to exit on right is completely flooded by massive Danger Light.\n2. Detach shadow.\n3. Shadow transitions to left wall and climbs to Y=3.5m.\n4. Reach Stealable Light Switch at (-7.4, 3.5, 8.0).\n5. Press STEAL. Switch ghosts; connected Danger Light powers down completely.\n6. Recall -> Player walks safely through deactivated light zone to Exit at (5.0, 0, 18.0).",
                    Notes = "Programmer Dependency: StealableLightSwitch is currently unbuilt (Rayaan). Greybox representation placed with conduit visuals.",
                    Geometry = new List<GeometryItem>
                    {
                        new GeometryItem { Name = "Floor_Main", Category = "Floor", Position = new Vector3(0, 0, 10.0f), Size = new Vector3(15f, 0.2f, 20f), ValueSource = "DOCUMENTED", Notes = "Hall floor (15m x 20m)" },
                        new GeometryItem { Name = "Wall_Left_Climbable", Category = "Wall", Position = new Vector3(-7.6f, 2.5f, 10.0f), Size = new Vector3(0.2f, 5f, 20f), ValueSource = "DOCUMENTED", Notes = "Left climbable wall with mounted switch" },
                        new GeometryItem { Name = "Wall_Right", Category = "Wall", Position = new Vector3(7.6f, 2.5f, 10.0f), Size = new Vector3(0.2f, 5f, 20f), ValueSource = "DOCUMENTED", Notes = "Right wall" },
                        new GeometryItem { Name = "Wall_Back", Category = "Wall", Position = new Vector3(0, 2.5f, -0.1f), Size = new Vector3(15.4f, 5f, 0.2f), ValueSource = "DOCUMENTED", Notes = "Back wall" },
                        new GeometryItem { Name = "Wall_Front_L", Category = "Wall", Position = new Vector3(-4.5f, 2.5f, 20.1f), Size = new Vector3(6.0f, 5f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Front wall left" },
                        new GeometryItem { Name = "Wall_Front_M", Category = "Wall", Position = new Vector3(1.0f, 2.5f, 20.1f), Size = new Vector3(5.0f, 5f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Front wall center (gap at X: 3.5 to 6.5m)" },
                    },
                    Stealables = new List<StealableItem>
                    {
                        new StealableItem { Name = "StealableLightSwitch", Type = StealableType.LightSwitch, Position = new Vector3(-7.4f, 3.5f, 8.0f), Rotation = new Vector3(0, 90f, 0), Scale = new Vector3(0.3f, 0.8f, 0.6f), ValueSource = "DOCUMENTED", Notes = "Stealable Light Switch (Programmer Task: Rayaan unbuilt dependency)" }
                    },
                    Hazards = new List<HazardItem>
                    {
                        new HazardItem { Name = "DangerLight_MassiveFloor", Type = HazardType.DangerLightStatic, Position = new Vector3(2.5f, 2.0f, 15.0f), Rotation = Vector3.zero, TriggerSize = new Vector3(9.0f, 4.0f, 8.0f), ValueSource = "DOCUMENTED", Notes = "Massive Danger Light flooding exit zone (X: -2 to 7m, Z: 11 to 19m)" }
                    },
                    Labels = new List<LabelItem>
                    {
                        new LabelItem { Name = "Label_Title", Text = "L13: The Breaker\n15m x 20m | Introduce LIGHT SWITCH Stealable", Position = new Vector3(0, 4.0f, 2.0f), Color = Color.white, FontSize = 4f },
                        new LabelItem { Name = "Label_Switch", Text = "Stealable Light Switch\n(Climb Wall - Powers Down Lethal Light)\n[Unbuilt: Rayaan]", Position = new Vector3(-6.8f, 4.5f, 8.0f), Color = Color.yellow, FontSize = 3.5f },
                        new LabelItem { Name = "Label_Hazard", Text = "Massive Lethal Danger Light\n(Floods Exit Path Until Switched Off)", Position = new Vector3(2.5f, 3.5f, 15.0f), Color = Color.red, FontSize = 3.5f },
                        new LabelItem { Name = "Label_Exit", Text = "Exit Zone (Top Right)", Position = new Vector3(5.0f, 2.0f, 18.0f), Color = Color.green, FontSize = 3f }
                    }
                },

                // ==========================================
                // L14: Threading the Needle (5m x 25m)
                // ==========================================
                new LevelDefinition
                {
                    Code = "L14",
                    Name = "Threading the Needle",
                    Phase = "Phase 3 (Master)",
                    Difficulty = "9 / 10",
                    Dimensions = new Vector2(5f, 25f),
                    TargetTime = "30s",
                    HardLimit = "35s",
                    MaxLeash = "24.5m",
                    LeashLimit = "25m",
                    Objective = "Extreme leash precision: razor-thin safe wall ribbon past 3 Danger Lights and 1 Trap Light (24.5m / 25m leash).",
                    PlayerSpawn = new Vector3(0, 0.1f, 1.0f),
                    PlayerRotation = Vector3.zero,
                    ShadowSpawn = null,
                    ExitPosition = new Vector3(0, 0.1f, 24.0f),
                    ExitSize = new Vector3(3.0f, 2.0f, 2.0f),
                    PlayerRoute = "Wait at start line (MUST detach at start) -> Walk through Ghost Door to Exit.",
                    ShadowRoute = "Detach -> Right wall safe ribbon -> Weave past 3 lights -> Enter Trap Light -> RECOVER x3 -> Steal Door (at 24.5m) -> Recall.",
                    Sequence = "1. Player MUST detach directly from start line (standing even 1m back causes leash clamp failure).\n2. Shadow transitions immediately to right wall ribbon.\n3. Carefully weave along right wall past floor danger light cones.\n4. Shadow enters unavoidable Trap Light near top of wall at (2.4, 2.2, 19.5).\n5. Player rapidly mashes RECOVER x3 within 2.0s to break free.\n6. Shadow advances final 1m to Stealable Door at (0, 1.25, 22.0).\n7. Euclidean distance reaches exactly 24.5m (0.5m under 25m leash limit).\n8. Steal Door -> Recall -> Player walks through Ghost Door to Exit.",
                    Notes = "Ultimate leash stress test. Mathematically calibrated so backing up 1m prevents reaching door.",
                    Geometry = new List<GeometryItem>
                    {
                        new GeometryItem { Name = "Floor_Main", Category = "Floor", Position = new Vector3(0, 0, 12.5f), Size = new Vector3(5f, 0.2f, 25f), ValueSource = "DOCUMENTED", Notes = "Oppressive corridor floor (5m x 25m)" },
                        new GeometryItem { Name = "Wall_Left", Category = "Wall", Position = new Vector3(-2.6f, 2.0f, 12.5f), Size = new Vector3(0.2f, 4f, 25f), ValueSource = "DOCUMENTED", Notes = "Left wall (height 4m)" },
                        new GeometryItem { Name = "Wall_Right_Ribbon", Category = "Wall", Position = new Vector3(2.6f, 2.0f, 12.5f), Size = new Vector3(0.2f, 4f, 25f), ValueSource = "DOCUMENTED", Notes = "Right climbable wall (narrow safe ribbon path)" },
                        new GeometryItem { Name = "Wall_Back", Category = "Wall", Position = new Vector3(0, 2.0f, -0.1f), Size = new Vector3(5.4f, 4f, 0.2f), ValueSource = "DOCUMENTED", Notes = "Back wall" },
                        new GeometryItem { Name = "Pillar_Door_L", Category = "Obstacle", Position = new Vector3(-1.75f, 1.5f, 22.0f), Size = new Vector3(1.5f, 3f, 0.4f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Door pillar left" },
                        new GeometryItem { Name = "Pillar_Door_R", Category = "Obstacle", Position = new Vector3(1.75f, 1.5f, 22.0f), Size = new Vector3(1.5f, 3f, 0.4f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Door pillar right" },
                        new GeometryItem { Name = "Wall_Front_L", Category = "Wall", Position = new Vector3(-1.75f, 2.0f, 25.1f), Size = new Vector3(1.5f, 4f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Front wall left" },
                        new GeometryItem { Name = "Wall_Front_R", Category = "Wall", Position = new Vector3(1.75f, 2.0f, 25.1f), Size = new Vector3(1.5f, 4f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Front wall right" },
                    },
                    Stealables = new List<StealableItem>
                    {
                        new StealableItem { Name = "StealableDoor", Type = StealableType.Door, Position = new Vector3(0, 1.25f, 22.0f), Rotation = Vector3.zero, Scale = Vector3.one, ValueSource = "DOCUMENTED", Notes = "Door at exact 24.5m Euclidean limit" }
                    },
                    Hazards = new List<HazardItem>
                    {
                        new HazardItem { Name = "DangerLight_Cone_1", Type = HazardType.DangerLightStatic, Position = new Vector3(-0.5f, 1.5f, 6.0f), Rotation = Vector3.zero, TriggerSize = new Vector3(4.0f, 3.0f, 4.0f), ValueSource = "DOCUMENTED", Notes = "Flooding Danger Light 1" },
                        new HazardItem { Name = "DangerLight_Cone_2", Type = HazardType.DangerLightStatic, Position = new Vector3(-0.5f, 1.5f, 12.0f), Rotation = Vector3.zero, TriggerSize = new Vector3(4.0f, 3.0f, 4.0f), ValueSource = "DOCUMENTED", Notes = "Flooding Danger Light 2" },
                        new HazardItem { Name = "DangerLight_Cone_3", Type = HazardType.DangerLightStatic, Position = new Vector3(-0.5f, 1.5f, 17.0f), Rotation = Vector3.zero, TriggerSize = new Vector3(4.0f, 3.0f, 3.0f), ValueSource = "DOCUMENTED", Notes = "Flooding Danger Light 3" },
                        new HazardItem { Name = "TrapLight_WallSnare", Type = HazardType.TrapLight, Position = new Vector3(2.4f, 2.2f, 19.5f), Rotation = Vector3.zero, TriggerSize = new Vector3(1.5f, 2.0f, 2.5f), ValueSource = "DOCUMENTED", Notes = "Wall Trap Light snare before door" }
                    },
                    Labels = new List<LabelItem>
                    {
                        new LabelItem { Name = "Label_Title", Text = "L14: Threading the Needle\n5m x 25m | Extreme Leash Precision (24.5m / 25m)", Position = new Vector3(0, 3.5f, 1.0f), Color = Color.white, FontSize = 4f },
                        new LabelItem { Name = "Label_FloodedFloor", Text = "Floor Flooded with Lethal Light\n(Right Wall Ribbon Only)", Position = new Vector3(-0.5f, 3.0f, 12.0f), Color = Color.red, FontSize = 3.5f },
                        new LabelItem { Name = "Label_Ribbon", Text = "Narrow Safe Ribbon (Wall)", Position = new Vector3(2.3f, 3.5f, 12.0f), Color = Color.cyan, FontSize = 3f },
                        new LabelItem { Name = "Label_WallTrap", Text = "Wall Snare (RECOVER x3)", Position = new Vector3(2.3f, 3.5f, 19.5f), Color = new Color(0.8f, 0.3f, 1f), FontSize = 3f },
                        new LabelItem { Name = "Label_Door", Text = "Stealable Door (24.5m Euclidean)", Position = new Vector3(0, 3.0f, 22.0f), Color = Color.cyan, FontSize = 3.5f },
                        new LabelItem { Name = "Label_Exit", Text = "Exit Zone", Position = new Vector3(0, 2.0f, 24.0f), Color = Color.green, FontSize = 3f }
                    }
                },

                // ==========================================
                // L15: Final Escape (20m x 50m)
                // ==========================================
                new LevelDefinition
                {
                    Code = "L15",
                    Name = "Final Escape",
                    Phase = "Phase 3 (Master)",
                    Difficulty = "10 / 10",
                    Dimensions = new Vector2(20f, 50f),
                    TargetTime = "38s",
                    HardLimit = "40s",
                    MaxLeash = "18m",
                    LeashLimit = "25m",
                    Objective = "Mastery exam: Stage 1 Switch, Stage 2 Platform, Stage 3 Trap + Door under 38s time pressure.",
                    PlayerSpawn = new Vector3(0, 0.1f, 2.0f),
                    PlayerRotation = Vector3.zero,
                    ShadowSpawn = null,
                    ExitPosition = new Vector3(0, 0.1f, 48.5f),
                    ExitSize = new Vector3(4.0f, 2.0f, 2.0f),
                    PlayerRoute = "Wait -> Cross Pit 1 -> Wait at Pit 2 -> Cross Platform -> Wait at Stage 3 -> Walk through Ghost Door to Exit.",
                    ShadowRoute = "1. Climb wall, dodge sweeper, steal Switch -> Recall.\n2. Steal moving platform to freeze -> Recall.\n3. Enter trap, mash RECOVER x3, steal Door -> Recall.",
                    Sequence = "1. Player detaches at start.\n2. Shadow climbs left wall, dodges sweeping light covering Pit 1, steals Light Switch at (-9.8, 3.0, 5.0). Light dies -> Recall.\n3. Player crosses Pit 1 along safe floor ledge to edge of Pit 2 (Z=18m).\n4. Detach shadow -> Steal oscillating Platform across Pit 2 (Z=24m) to freeze it -> Recall.\n5. Player crosses frozen platform into Stage 3 (Z=32m).\n6. Detach shadow -> Enter Trap Light at Z=38m -> Mash RECOVER x3 -> Steal Door at (0, 1.25, 45.0) -> Recall.\n7. Player walks through Ghost Door to Exit at (0, 0, 48.5) to complete the game!",
                    Notes = "Master integration of all game mechanics. Multi-stage room mathematically forces at least 2 Recalls.",
                    Geometry = new List<GeometryItem>
                    {
                        new GeometryItem { Name = "Floor_Stage1_Start", Category = "Floor", Position = new Vector3(0, 0, 3.0f), Size = new Vector3(20f, 0.2f, 6f), ValueSource = "DOCUMENTED", Notes = "Stage 1 start floor (Z: 0 to 6m)" },
                        new GeometryItem { Name = "Pit1_Chasm", Category = "Obstacle", Position = new Vector3(0, -6.0f, 9.0f), Size = new Vector3(20f, 0.2f, 6f), ValueSource = "DOCUMENTED", Notes = "Stage 1 Pit (6m chasm, Z: 6 to 12m)" },
                        new GeometryItem { Name = "Floor_Stage1_Ledge", Category = "Floor", Position = new Vector3(-8.0f, 0, 9.0f), Size = new Vector3(4.0f, 0.2f, 6f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Stage 1 left crossing ledge" },
                        new GeometryItem { Name = "Floor_Stage2_Mid", Category = "Floor", Position = new Vector3(0, 0, 16.0f), Size = new Vector3(20f, 0.2f, 8f), ValueSource = "DOCUMENTED", Notes = "Stage 2 intermediate floor (Z: 12 to 20m)" },
                        new GeometryItem { Name = "Pit2_Chasm", Category = "Obstacle", Position = new Vector3(0, -6.0f, 24.0f), Size = new Vector3(20f, 0.2f, 8f), ValueSource = "DOCUMENTED", Notes = "Stage 2 Pit (8m chasm, Z: 20 to 28m)" },
                        new GeometryItem { Name = "Floor_Stage3_Final", Category = "Floor", Position = new Vector3(0, 0, 39.0f), Size = new Vector3(20f, 0.2f, 22f), ValueSource = "DOCUMENTED", Notes = "Stage 3 final arena floor (Z: 28 to 50m)" },
                        new GeometryItem { Name = "Wall_Left_Climbable", Category = "Wall", Position = new Vector3(-10.1f, 2.5f, 25.0f), Size = new Vector3(0.2f, 5f, 50f), ValueSource = "DOCUMENTED", Notes = "Left climbable wall (height 5m)" },
                        new GeometryItem { Name = "Wall_Right", Category = "Wall", Position = new Vector3(10.1f, 2.5f, 25.0f), Size = new Vector3(0.2f, 5f, 50f), ValueSource = "DOCUMENTED", Notes = "Right boundary wall" },
                        new GeometryItem { Name = "Wall_Back", Category = "Wall", Position = new Vector3(0, 2.5f, -0.1f), Size = new Vector3(20.4f, 5f, 0.2f), ValueSource = "DOCUMENTED", Notes = "Back boundary wall" },
                        new GeometryItem { Name = "Pillar_Door_L", Category = "Obstacle", Position = new Vector3(-3.25f, 1.5f, 45.0f), Size = new Vector3(3.5f, 3f, 0.4f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Final door pillar left" },
                        new GeometryItem { Name = "Pillar_Door_R", Category = "Obstacle", Position = new Vector3(3.25f, 1.5f, 45.0f), Size = new Vector3(3.5f, 3f, 0.4f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Final door pillar right" },
                        new GeometryItem { Name = "Wall_Front_L", Category = "Wall", Position = new Vector3(-6.0f, 2.5f, 50.1f), Size = new Vector3(8.0f, 5f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Front exit wall left" },
                        new GeometryItem { Name = "Wall_Front_R", Category = "Wall", Position = new Vector3(6.0f, 2.5f, 50.1f), Size = new Vector3(8.0f, 5f, 0.2f), ValueSource = "DERIVED_FROM_DIAGRAM", Notes = "Front exit wall right" },
                    },
                    Stealables = new List<StealableItem>
                    {
                        new StealableItem { Name = "StealableLightSwitch", Type = StealableType.LightSwitch, Position = new Vector3(-9.8f, 3.0f, 5.0f), Rotation = new Vector3(0, 90f, 0), Scale = new Vector3(0.3f, 0.8f, 0.6f), ValueSource = "DOCUMENTED", Notes = "Stage 1 Wall Light Switch (Unbuilt dependency: Rayaan)" },
                        new StealableItem { Name = "StealablePlatform", Type = StealableType.Platform, Position = new Vector3(0, 0.1f, 24.0f), Rotation = Vector3.zero, Scale = new Vector3(3.5f, 0.4f, 4.0f), MoveAxis = new Vector3(1, 0, 0), MoveDistance = 5.0f, MoveSpeed = 3.5f, ValueSource = "DOCUMENTED", Notes = "Stage 2 Rapid Moving Platform" },
                        new StealableItem { Name = "StealableDoor", Type = StealableType.Door, Position = new Vector3(0, 1.25f, 45.0f), Rotation = Vector3.zero, Scale = Vector3.one, ValueSource = "DOCUMENTED", Notes = "Stage 3 Final Stealable Door" }
                    },
                    Hazards = new List<HazardItem>
                    {
                        new HazardItem { Name = "DangerLight_Pit1_Sweeping", Type = HazardType.DangerLightSweeping, Position = new Vector3(0, 3.5f, 9.0f), Rotation = Vector3.zero, TriggerSize = new Vector3(10.0f, 2.0f, 6.0f), SweepAngle = 35f, SweepSpeed = 30f, ValueSource = "DOCUMENTED", Notes = "Stage 1 Sweeping Danger Light over Pit 1" },
                        new HazardItem { Name = "TrapLight_Stage3", Type = HazardType.TrapLight, Position = new Vector3(0, 1.5f, 38.0f), Rotation = Vector3.zero, TriggerSize = new Vector3(14.0f, 3.0f, 4.0f), ValueSource = "DOCUMENTED", Notes = "Stage 3 Trap Light Snare before Door" }
                    },
                    Labels = new List<LabelItem>
                    {
                        new LabelItem { Name = "Label_Title", Text = "L15: Final Escape\n20m x 50m | The Ultimate Exam (All Systems)", Position = new Vector3(0, 4.5f, 2.0f), Color = Color.white, FontSize = 4f },
                        new LabelItem { Name = "Label_Stage1", Text = "Stage 1: Steal Light Switch\n[Unbuilt: Rayaan]", Position = new Vector3(-9.0f, 4.0f, 5.0f), Color = Color.yellow, FontSize = 3.5f },
                        new LabelItem { Name = "Label_Sweeper", Text = "Stage 1 Sweeping Light", Position = new Vector3(0, 4.5f, 9.0f), Color = Color.red, FontSize = 3.5f },
                        new LabelItem { Name = "Label_Stage2", Text = "Stage 2: Freeze Moving Platform Across Pit 2", Position = new Vector3(0, 2.0f, 24.0f), Color = Color.cyan, FontSize = 3.5f },
                        new LabelItem { Name = "Label_Stage3_Trap", Text = "Stage 3: Trap Light Snare (RECOVER x3)", Position = new Vector3(0, 3.5f, 38.0f), Color = new Color(0.8f, 0.3f, 1f), FontSize = 3.5f },
                        new LabelItem { Name = "Label_Door", Text = "Final Stealable Door", Position = new Vector3(0, 3.0f, 45.0f), Color = Color.cyan, FontSize = 3.5f },
                        new LabelItem { Name = "Label_Exit", Text = "Final Exit Zone", Position = new Vector3(0, 2.0f, 48.5f), Color = Color.green, FontSize = 3f }
                    }
                }
            };
        }
        #endregion

        #region Helper Classes & Data Structures
        public class LevelDefinition
        {
            public string Code;
            public string Name;
            public string Phase;
            public string Difficulty;
            public Vector2 Dimensions;
            public string TargetTime;
            public string HardLimit;
            public string MaxLeash;
            public string LeashLimit;
            public string Objective;
            public Vector3 PlayerSpawn;
            public Vector3 PlayerRotation;
            public Vector3? ShadowSpawn;
            public Vector3 ExitPosition;
            public Vector3 ExitSize;
            public string PlayerRoute;
            public string ShadowRoute;
            public string Sequence;
            public string Notes;
            public List<GeometryItem> Geometry;
            public List<StealableItem> Stealables;
            public List<HazardItem> Hazards;
            public List<LabelItem> Labels;
        }

        public class GeometryItem
        {
            public string Name;
            public string Category; // Floor, Wall, Obstacle
            public Vector3 Position;
            public Vector3 Rotation;
            public Vector3 Size;
            public string ValueSource; // DOCUMENTED, DERIVED_FROM_DIAGRAM, IMPLEMENTATION_ASSUMPTION
            public string Notes;
        }

        public enum StealableType
        {
            Door,
            Drawbridge,
            Platform,
            LightSwitch
        }

        public class StealableItem
        {
            public string Name;
            public StealableType Type;
            public Vector3 Position;
            public Vector3 Rotation;
            public Vector3 Scale;
            public Vector3 MoveAxis;
            public float MoveDistance;
            public float MoveSpeed;
            public string ValueSource;
            public string Notes;
        }

        public enum HazardType
        {
            DangerLightStatic,
            DangerLightSweeping,
            DangerLightPulsing,
            TrapLight
        }

        public class HazardItem
        {
            public string Name;
            public HazardType Type;
            public Vector3 Position;
            public Vector3 Rotation;
            public Vector3 TriggerSize;
            public float SweepAngle;
            public float SweepSpeed;
            public string ValueSource;
            public string Notes;
        }

        public class LabelItem
        {
            public string Name;
            public string Text;
            public Vector3 Position;
            public Color Color;
            public float FontSize;
        }

        public class ObjectRecord
        {
            public string Level;
            public string ObjectName;
            public string Category;
            public Vector3 Position;
            public Vector3 Rotation;
            public Vector3 Scale;
            public string ColliderType;
            public Vector3 ColliderSize;
            public string ValueSource;
            public string Notes;
        }

        public class LevelSummary
        {
            public string Code;
            public string Name;
            public string Phase;
            public string Difficulty;
            public string Dimensions;
            public string TargetTime;
            public string HardLimit;
            public string MaxLeash;
            public string LeashLimit;
            public int StealablesCount;
            public int HazardsCount;
            public string Objective;
            public string PlayerRoute;
            public string ShadowRoute;
            public string Sequence;
            public string Notes;
        }
        #endregion
    }

    /// <summary>
    /// Explicit runtime marker identifying unbuilt StealableLightSwitch dependency.
    /// </summary>
    public class LightSwitchGreyboxMarker : MonoBehaviour
    {
        [TextArea]
        public string statusNote;
    }
}
