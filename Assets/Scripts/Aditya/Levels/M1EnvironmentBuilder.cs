using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

/// <summary>
/// Technical Artist environment builder for Milestone 1: Critical Technical Proof.
/// Assembles high-fidelity industrial architecture, PBR materials, chasm geometry,
/// structural framing, practical illumination, and post-processing while strictly
/// preserving all existing gameplay logic, colliders, and triggers.
/// </summary>
public static class M1EnvironmentBuilder
{
    private const string RootName = "_ENVIRONMENT_ARCHITECTURE";

    [MenuItem("StealMan/Rebuild M1 Production Environment")]
    public static void BuildEnvironment()
    {
        // 1. Remove previous iteration if exists
        var existingRoot = GameObject.Find(RootName);
        if (existingRoot != null)
        {
            Object.DestroyImmediate(existingRoot);
        }

        var envRoot = new GameObject(RootName);

#if UNITY_EDITOR
        Undo.RegisterCreatedObjectUndo(envRoot, "Build M1 Production Environment");
#endif

        // 2. Load Core Materials
        var matFloor = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_FacilityFloor.mat");
        var matWall = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_FacilityWall.mat");
        var matBeam = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_FacilityBeam.mat");
        var matChasm = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_ChasmDepth.mat");
        var matClimbPBR = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_ClimbableWall_PBR.mat");
        var matDrawbridgeDeck = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Drawbridge_Deck.mat");
        var matDoorBulkhead = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_StealableDoor_Bulkhead.mat");
        var matExitGate = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_ExitPortal_Gate.mat");
        var matCyanGlow = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_AccentCyanGlow.mat");
        var matAmberGlow = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_AccentAmberGlow.mat");
        var matHazardBorder = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_HazardBorderYellow.mat");
        var matPlayerBase = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_PlayerMannequin_Base.mat");

        // Hide original flat plane renderer while keeping its collider
        var origPlane = GameObject.Find("Plane");
        if (origPlane != null)
        {
            var mr = origPlane.GetComponent<MeshRenderer>();
            if (mr != null) mr.enabled = false;
        }

        // =========================================================
        // A. FLOORS & SUBTERRANEAN CHASM PITS
        // =========================================================
        var floorsRoot = new GameObject("Floors").transform;
        floorsRoot.SetParent(envRoot.transform, false);

        // Section 1: Entrance Vestibule (Z: -6 to 2)
        CreateBox("Floor_Vestibule", new Vector3(0f, -0.05f, -2.0f), new Vector3(7.4f, 0.1f, 8.0f), matFloor, floorsRoot, true);

        // Section 2: Hazard Gallery & Drawbridge Approach (Z: 2 to 5.25)
        CreateBox("Floor_HazardPlaza_Center", new Vector3(0f, -0.05f, 3.625f), new Vector3(7.4f, 0.1f, 3.25f), matFloor, floorsRoot, true);
        CreateBox("Floor_DangerWing", new Vector3(-8.2f, -0.05f, 7.0f), new Vector3(9.0f, 0.1f, 6.0f), matFloor, floorsRoot, true);
        CreateBox("Border_DangerWing", new Vector3(-9.47f, 0.01f, 7.0f), new Vector3(5.2f, 0.02f, 4.2f), matHazardBorder, floorsRoot, false);

        CreateBox("Floor_TrapWing", new Vector3(8.2f, -0.05f, 7.0f), new Vector3(9.0f, 0.1f, 6.0f), matFloor, floorsRoot, true);
        CreateBox("Border_TrapWing", new Vector3(9.92f, 0.01f, 7.0f), new Vector3(5.2f, 0.02f, 4.2f), matHazardBorder, floorsRoot, false);

        // Section 3: Central Chasm Pit 1 (Beneath Drawbridge, Z: 5.25 to 8.75)
        CreateBox("Chasm_Drawbridge_Base", new Vector3(0f, -4.5f, 7.0f), new Vector3(7.4f, 0.2f, 3.5f), matChasm, floorsRoot, false);
        CreateBox("Chasm_Drawbridge_NorthWall", new Vector3(0f, -2.25f, 8.75f), new Vector3(7.4f, 4.5f, 0.2f), matWall, floorsRoot, false);
        CreateBox("Chasm_Drawbridge_SouthWall", new Vector3(0f, -2.25f, 5.25f), new Vector3(7.4f, 4.5f, 0.2f), matWall, floorsRoot, false);
        // Chasm floor depth exhaust vent
        CreateBox("Chasm_DepthVent", new Vector3(0f, -4.4f, 7.0f), new Vector3(3.0f, 0.1f, 1.5f), matBeam, floorsRoot, false);

        // Section 4: Mid Landing (Z: 8.75 to 10.5)
        CreateBox("Floor_MidLanding", new Vector3(0f, -0.05f, 9.625f), new Vector3(7.4f, 0.1f, 1.75f), matFloor, floorsRoot, true);

        // Section 5: Central Chasm Pit 2 (Beneath Moving Platform, Z: 10.5 to 13.5)
        CreateBox("Chasm_Platform_Base", new Vector3(0f, -4.5f, 12.0f), new Vector3(7.4f, 0.2f, 3.0f), matChasm, floorsRoot, false);
        CreateBox("Chasm_Platform_NorthWall", new Vector3(0f, -2.25f, 13.5f), new Vector3(7.4f, 4.5f, 0.2f), matWall, floorsRoot, false);
        CreateBox("Chasm_Platform_SouthWall", new Vector3(0f, -2.25f, 10.5f), new Vector3(7.4f, 4.5f, 0.2f), matWall, floorsRoot, false);

        // Section 6: Exit Chamber Runway (Z: 13.5 to 23.0)
        CreateBox("Floor_ExitRunway", new Vector3(0f, -0.05f, 18.25f), new Vector3(7.4f, 0.1f, 9.5f), matFloor, floorsRoot, true);

        // =========================================================
        // B. PERIMETER INDUSTRIAL WALLS & BULKHEADS
        // =========================================================
        var wallsRoot = new GameObject("Walls").transform;
        wallsRoot.SetParent(envRoot.transform, false);

        // South Back Wall (Z = -6.0)
        CreateBox("Wall_South", new Vector3(0f, 2.5f, -6.0f), new Vector3(7.6f, 5.0f, 0.4f), matWall, wallsRoot, true);

        // Vestibule Side Walls (Z: -6 to 2)
        CreateBox("Wall_Vestibule_West", new Vector3(-3.8f, 2.5f, -2.0f), new Vector3(0.4f, 5.0f, 8.0f), matWall, wallsRoot, true);
        CreateBox("Wall_Vestibule_East", new Vector3(3.8f, 2.5f, -2.0f), new Vector3(0.4f, 5.0f, 8.0f), matWall, wallsRoot, true);

        // Hazard Chamber Perimeter Walls
        CreateBox("Wall_Danger_West", new Vector3(-12.8f, 2.5f, 7.0f), new Vector3(0.4f, 5.0f, 6.2f), matWall, wallsRoot, true);
        CreateBox("Wall_Danger_South", new Vector3(-8.3f, 2.5f, 3.9f), new Vector3(8.6f, 5.0f, 0.4f), matWall, wallsRoot, true);
        CreateBox("Wall_Danger_North", new Vector3(-8.3f, 2.5f, 10.1f), new Vector3(8.6f, 5.0f, 0.4f), matWall, wallsRoot, true);

        CreateBox("Wall_Trap_East", new Vector3(12.8f, 2.5f, 7.0f), new Vector3(0.4f, 5.0f, 6.2f), matWall, wallsRoot, true);
        CreateBox("Wall_Trap_South", new Vector3(8.3f, 2.5f, 3.9f), new Vector3(8.6f, 5.0f, 0.4f), matWall, wallsRoot, true);
        CreateBox("Wall_Trap_North", new Vector3(8.3f, 2.5f, 10.1f), new Vector3(8.6f, 5.0f, 0.4f), matWall, wallsRoot, true);

        // Chasm & Exit Runway Side Walls (Z: 10.5 to 23.0)
        CreateBox("Wall_Chasm_West", new Vector3(-3.8f, 2.5f, 16.75f), new Vector3(0.4f, 5.0f, 12.5f), matWall, wallsRoot, true);
        CreateBox("Wall_Chasm_East", new Vector3(3.8f, 2.5f, 16.75f), new Vector3(0.4f, 5.0f, 12.5f), matWall, wallsRoot, true);

        // North Terminal Wall (Z = 23.0)
        CreateBox("Wall_North_Terminal", new Vector3(0f, 2.5f, 23.0f), new Vector3(7.6f, 5.0f, 0.4f), matWall, wallsRoot, true);

        // =========================================================
        // C. CEILING STRUCTURAL GIRDERS / TRUSSES & ROOF
        // =========================================================
        var trussesRoot = new GameObject("StructuralTrusses").transform;
        trussesRoot.SetParent(envRoot.transform, false);

        float[] trussZ = new float[] { -4.0f, -1.0f, 2.0f, 5.25f, 8.75f, 12.0f, 15.5f, 19.0f, 22.0f };
        foreach (float z in trussZ)
        {
            CreateBox($"Truss_Z{z:F0}", new Vector3(0f, 4.8f, z), new Vector3(7.6f, 0.35f, 0.35f), matBeam, trussesRoot, false);
            // Column brackets
            CreateBox($"Bracket_W_Z{z:F0}", new Vector3(-3.5f, 4.5f, z), new Vector3(0.3f, 0.8f, 0.3f), matBeam, trussesRoot, false);
            CreateBox($"Bracket_E_Z{z:F0}", new Vector3(3.5f, 4.5f, z), new Vector3(0.3f, 0.8f, 0.3f), matBeam, trussesRoot, false);
        }

        // Solid ceiling enclosure (prevents daytime skybox showing through roof)
        var roofRoot = new GameObject("CeilingRoof").transform;
        roofRoot.SetParent(envRoot.transform, false);
        CreateBox("Roof_MainCorridor", new Vector3(0f, 5.05f, 8.5f), new Vector3(8.0f, 0.2f, 30.0f), matBeam, roofRoot, false);
        CreateBox("Roof_DangerWing", new Vector3(-8.3f, 5.05f, 7.0f), new Vector3(9.2f, 0.2f, 6.6f), matBeam, roofRoot, false);
        CreateBox("Roof_TrapWing", new Vector3(8.3f, 5.05f, 7.0f), new Vector3(9.2f, 0.2f, 6.6f), matBeam, roofRoot, false);

        // =========================================================
        // D. BULKHEAD GATE AT STEALABLE DOOR (Z = 2.0)
        // =========================================================
        var bulkheadRoot = new GameObject("BulkheadGate").transform;
        bulkheadRoot.SetParent(envRoot.transform, false);

        // Heavy lintel above the stealable door
        CreateBox("Bulkhead_TopLintel", new Vector3(0f, 3.2f, 2.0f), new Vector3(6.5f, 0.8f, 0.6f), matDoorBulkhead, bulkheadRoot, false);
        CreateBox("Bulkhead_TrimCyan", new Vector3(0f, 2.75f, 1.95f), new Vector3(5.5f, 0.08f, 0.05f), matCyanGlow, bulkheadRoot, false);

        // Upgrade door visual
        var door = GameObject.Find("StealableDoor_Test");
        if (door != null)
        {
            var doorRenderer = door.GetComponent<Renderer>();
            if (doorRenderer != null)
            {
                doorRenderer.sharedMaterial = matDoorBulkhead;
            }
        }

        // Update Door Pillars
        var dlPillar = GameObject.Find("Door_LeftPillar");
        if (dlPillar != null)
        {
            var r = dlPillar.GetComponent<Renderer>();
            if (r != null) r.sharedMaterial = matBeam;
        }
        var drPillar = GameObject.Find("Door_RightPillar");
        if (drPillar != null)
        {
            var r = drPillar.GetComponent<Renderer>();
            if (r != null) r.sharedMaterial = matBeam;
        }

        // =========================================================
        // E. CLIMBABLE WALL PRESENTATION (X = -1.9, Z = 7.0)
        // =========================================================
        var climbWall = GameObject.Find("Drawbridge_ClimbableWall");
        if (climbWall != null)
        {
            var r = climbWall.GetComponent<Renderer>();
            if (r != null) r.sharedMaterial = matClimbPBR;

            // Add architectural detailing as non-colliding visual children
            var visualDetail = climbWall.transform.Find("ClimbingTowerDetails");
            if (visualDetail != null) Object.DestroyImmediate(visualDetail.gameObject);

            var towerDetails = new GameObject("ClimbingTowerDetails").transform;
            towerDetails.SetParent(climbWall.transform, false);
            towerDetails.localPosition = Vector3.zero;

            // Vertical guide rails on sides
            CreateBox("GuideRail_L", new Vector3(-0.55f, 0f, -0.62f), new Vector3(0.08f, 1.0f, 0.08f), matBeam, towerDetails, false);
            CreateBox("GuideRail_R", new Vector3(0.55f, 0f, -0.62f), new Vector3(0.08f, 1.0f, 0.08f), matBeam, towerDetails, false);

            // Horizontal climbing conduit rungs
            for (float yLocal = -0.4f; yLocal <= 0.45f; yLocal += 0.15f)
            {
                CreateBox($"Rung_{yLocal:F2}", new Vector3(0f, yLocal, -0.63f), new Vector3(0.9f, 0.04f, 0.05f), matBeam, towerDetails, false);
            }

            // Cyan edge illumination strips
            CreateBox("CyanGuide_L", new Vector3(-0.5f, 0f, -0.61f), new Vector3(0.03f, 0.95f, 0.02f), matCyanGlow, towerDetails, false);
            CreateBox("CyanGuide_R", new Vector3(0.5f, 0f, -0.61f), new Vector3(0.03f, 0.95f, 0.02f), matCyanGlow, towerDetails, false);

            // Platform cap at top (Y = 4.0m)
            CreateBox("MaintenancePlatformCap", new Vector3(0f, 0.52f, 0f), new Vector3(1.3f, 0.08f, 1.3f), matBeam, towerDetails, false);
        }

        // =========================================================
        // F. STEALABLE DRAWBRIDGE DETAIL (Z = 7.0)
        // =========================================================
        var drawbridge = GameObject.Find("StealableDrawbridge_Test");
        if (drawbridge != null)
        {
            Transform hinge = drawbridge.transform.Find("HingePivot");
            if (hinge != null)
            {
                Transform deck = hinge.Find("Deck");
                if (deck != null)
                {
                    var deckR = deck.GetComponent<Renderer>();
                    if (deckR != null) deckR.sharedMaterial = matDrawbridgeDeck;

                    var deckDetails = deck.Find("BridgeDetails");
                    if (deckDetails != null) Object.DestroyImmediate(deckDetails.gameObject);

                    var bd = new GameObject("BridgeDetails").transform;
                    bd.SetParent(deck, false);
                    bd.localPosition = Vector3.zero;

                    // Side guard rails (moves with bridge deck!)
                    CreateBox("SideRail_L", new Vector3(-1.45f, 0.35f, 0f), new Vector3(0.08f, 0.6f, 3.2f), matBeam, bd, false);
                    CreateBox("SideRail_R", new Vector3(1.45f, 0.35f, 0f), new Vector3(0.08f, 0.6f, 3.2f), matBeam, bd, false);

                    // Warning chevron borders on entry and exit edge of bridge deck
                    CreateBox("Edge_Front", new Vector3(0f, 0.06f, 1.55f), new Vector3(2.8f, 0.02f, 0.15f), matHazardBorder, bd, false);
                    CreateBox("Edge_Back", new Vector3(0f, 0.06f, -1.55f), new Vector3(2.8f, 0.02f, 0.15f), matHazardBorder, bd, false);
                }
            }
        }

        // =========================================================
        // G. STEALABLE PLATFORM DETAIL (Z = 12.0)
        // =========================================================
        var plat = GameObject.Find("StealablePlatform_Test");
        if (plat != null)
        {
            var platDetails = plat.transform.Find("PlatformDetails");
            if (platDetails != null) Object.DestroyImmediate(platDetails.gameObject);

            var pd = new GameObject("PlatformDetails").transform;
            pd.SetParent(plat.transform, false);
            pd.localPosition = Vector3.zero;

            // Heavy steel under-chassis
            CreateBox("UnderChassis", new Vector3(0f, -0.15f, 0f), new Vector3(2.4f, 0.15f, 2.4f), matBeam, pd, false);
            // Cyan mag-lev energy runners
            CreateBox("MagLev_L", new Vector3(-1.05f, -0.12f, 0f), new Vector3(0.08f, 0.08f, 2.2f), matCyanGlow, pd, false);
            CreateBox("MagLev_R", new Vector3(1.05f, -0.12f, 0f), new Vector3(0.08f, 0.08f, 2.2f), matCyanGlow, pd, false);
            // Warning hazard edges
            CreateBox("HazardStripe_N", new Vector3(0f, 0.11f, 1.05f), new Vector3(2.1f, 0.02f, 0.1f), matHazardBorder, pd, false);
            CreateBox("HazardStripe_S", new Vector3(0f, 0.11f, -1.05f), new Vector3(2.1f, 0.02f, 0.1f), matHazardBorder, pd, false);
        }

        // =========================================================
        // H. HAZARD FIXTURE FRAMING (Danger & Trap Lights)
        // =========================================================
        var dl = GameObject.Find("DangerLight_Hazard");
        if (dl != null)
        {
            var existingCage = dl.transform.Find("HazardFixture");
            if (existingCage != null) Object.DestroyImmediate(existingCage.gameObject);

            var fixture = new GameObject("HazardFixture").transform;
            fixture.SetParent(dl.transform, false);
            fixture.localPosition = Vector3.zero;

            CreateBox("StanchionBase", new Vector3(0f, -0.45f, 0f), new Vector3(1.0f, 0.1f, 1.0f), matBeam, fixture, false);
            CreateBox("GuardPost_NW", new Vector3(-0.45f, 0.2f, 0.45f), new Vector3(0.08f, 1.2f, 0.08f), matHazardBorder, fixture, false);
            CreateBox("GuardPost_NE", new Vector3(0.45f, 0.2f, 0.45f), new Vector3(0.08f, 1.2f, 0.08f), matHazardBorder, fixture, false);
            CreateBox("GuardPost_SW", new Vector3(-0.45f, 0.2f, -0.45f), new Vector3(0.08f, 1.2f, 0.08f), matHazardBorder, fixture, false);
            CreateBox("GuardPost_SE", new Vector3(0.45f, 0.2f, -0.45f), new Vector3(0.08f, 1.2f, 0.08f), matHazardBorder, fixture, false);
            CreateBox("TopCap", new Vector3(0f, 0.82f, 0f), new Vector3(1.0f, 0.06f, 1.0f), matBeam, fixture, false);
        }

        var tl = GameObject.Find("TrapLight_Hazard");
        if (tl != null)
        {
            var existingCage = tl.transform.Find("HazardFixture");
            if (existingCage != null) Object.DestroyImmediate(existingCage.gameObject);

            var fixture = new GameObject("HazardFixture").transform;
            fixture.SetParent(tl.transform, false);
            fixture.localPosition = Vector3.zero;

            CreateBox("StanchionBase", new Vector3(0f, -0.45f, 0f), new Vector3(1.0f, 0.1f, 1.0f), matBeam, fixture, false);
            CreateBox("GuardPost_NW", new Vector3(-0.45f, 0.2f, 0.45f), new Vector3(0.08f, 1.2f, 0.08f), matHazardBorder, fixture, false);
            CreateBox("GuardPost_NE", new Vector3(0.45f, 0.2f, 0.45f), new Vector3(0.08f, 1.2f, 0.08f), matHazardBorder, fixture, false);
            CreateBox("GuardPost_SW", new Vector3(-0.45f, 0.2f, -0.45f), new Vector3(0.08f, 1.2f, 0.08f), matHazardBorder, fixture, false);
            CreateBox("GuardPost_SE", new Vector3(0.45f, 0.2f, -0.45f), new Vector3(0.08f, 1.2f, 0.08f), matHazardBorder, fixture, false);
            CreateBox("TopCap", new Vector3(0f, 0.82f, 0f), new Vector3(1.0f, 0.06f, 1.0f), matBeam, fixture, false);
        }

        // =========================================================
        // I. EXIT PORTAL MONUMENT (Z = 21.5)
        // =========================================================
        var exitZone = GameObject.Find("ExitZone");
        if (exitZone != null)
        {
            var exitRenderer = exitZone.GetComponentInChildren<Renderer>();
            if (exitRenderer != null)
            {
                exitRenderer.sharedMaterial = matExitGate;
            }

            var portalMonument = new GameObject("ExitPortalMonument").transform;
            portalMonument.SetParent(envRoot.transform, false);
            portalMonument.position = new Vector3(0f, 0f, 21.8f);

            // Massive vault door arch
            CreateBox("PortalArch_Top", new Vector3(0f, 3.8f, 21.8f), new Vector3(5.0f, 0.8f, 0.8f), matExitGate, portalMonument, false);
            CreateBox("PortalArch_L", new Vector3(-2.2f, 1.8f, 21.8f), new Vector3(0.8f, 3.6f, 0.8f), matExitGate, portalMonument, false);
            CreateBox("PortalArch_R", new Vector3(2.2f, 1.8f, 21.8f), new Vector3(0.8f, 3.6f, 0.8f), matExitGate, portalMonument, false);

            // Glowing gateway threshold frame
            CreateBox("PortalThreshold_L", new Vector3(-1.75f, 1.6f, 21.75f), new Vector3(0.06f, 3.0f, 0.06f), matExitGate, portalMonument, false);
            CreateBox("PortalThreshold_R", new Vector3(1.75f, 1.6f, 21.75f), new Vector3(0.06f, 3.0f, 0.06f), matExitGate, portalMonument, false);
            CreateBox("PortalThreshold_Top", new Vector3(0f, 3.1f, 21.75f), new Vector3(3.5f, 0.06f, 0.06f), matExitGate, portalMonument, false);
        }

        // =========================================================
        // J. CINEMATIC DYNAMIC LIGHTING
        // =========================================================
        var lightsRoot = new GameObject("PracticalLighting").transform;
        lightsRoot.SetParent(envRoot.transform, false);

        // 1. Primary Directional Key/Rim Light
        var dirLightGO = GameObject.Find("Directional Light");
        if (dirLightGO != null)
        {
            var dlLight = dirLightGO.GetComponent<Light>();
            dlLight.type = LightType.Directional;
            dlLight.color = new Color(0.61f, 0.72f, 0.84f); // Cool desaturated steel-blue
            dlLight.intensity = 0.55f;
            dlLight.shadows = LightShadows.Soft;
            dlLight.shadowStrength = 0.85f;
            dirLightGO.transform.rotation = Quaternion.Euler(52f, 325f, 0f);
        }

        // 2. Practical Lights
        CreatePointLight("Light_Vestibule", new Vector3(0f, 3.6f, -2.0f), new Color(0.96f, 0.68f, 0.38f), 2.2f, 9.0f, lightsRoot);
        CreatePointLight("Light_Bulkhead_Door", new Vector3(0f, 3.0f, 2.0f), new Color(0.78f, 0.90f, 1.0f), 2.5f, 7.5f, lightsRoot);
        CreatePointLight("Light_Drawbridge_Warm", new Vector3(0f, 4.0f, 7.0f), new Color(0.92f, 0.58f, 0.22f), 3.2f, 10.0f, lightsRoot);
        CreatePointLight("Light_ClimbWall_Cyan", new Vector3(-1.9f, 2.5f, 6.0f), new Color(0.20f, 0.85f, 0.95f), 1.8f, 5.0f, lightsRoot);
        CreatePointLight("Light_Danger_Red", new Vector3(-9.47f, 2.2f, 7.0f), new Color(1.0f, 0.15f, 0.15f), 3.8f, 8.0f, lightsRoot);
        CreatePointLight("Light_Trap_Amber", new Vector3(9.92f, 2.2f, 7.0f), new Color(1.0f, 0.65f, 0.10f), 3.4f, 8.0f, lightsRoot);
        CreatePointLight("Light_Platform_Mid", new Vector3(0f, 3.8f, 12.0f), new Color(0.35f, 0.80f, 0.95f), 2.6f, 8.5f, lightsRoot);
        CreatePointLight("Light_Exit_Beacon", new Vector3(0f, 2.8f, 21.5f), new Color(0.15f, 1.0f, 0.65f), 3.5f, 8.0f, lightsRoot);

        // =========================================================
        // K. GLOBAL VOLUME POST-PROCESSING (URP)
        // =========================================================
        var volGO = GameObject.Find("Global Volume");
        if (volGO != null)
        {
            var volume = volGO.GetComponent<Volume>();
            if (volume != null)
            {
                var profile = volume.sharedProfile != null ? volume.sharedProfile : ScriptableObject.CreateInstance<VolumeProfile>();

                // Tonemapping: ACES
                if (!profile.TryGet<Tonemapping>(out var tm))
                {
                    tm = profile.Add<Tonemapping>(true);
                }
                tm.mode.value = TonemappingMode.ACES;
                tm.active = true;

                // Bloom: Restrained & Crisp
                if (!profile.TryGet<Bloom>(out var bloom))
                {
                    bloom = profile.Add<Bloom>(true);
                }
                bloom.threshold.value = 1.05f;
                bloom.intensity.value = 0.65f;
                bloom.scatter.value = 0.68f;
                bloom.tint.value = new Color(0.85f, 0.95f, 1.0f);
                bloom.active = true;

                // Color Adjustments: Industrial grading
                if (!profile.TryGet<ColorAdjustments>(out var colorAdj))
                {
                    colorAdj = profile.Add<ColorAdjustments>(true);
                }
                colorAdj.postExposure.value = 0.18f;
                colorAdj.contrast.value = 16f;
                colorAdj.saturation.value = -10f;
                colorAdj.colorFilter.value = new Color(0.90f, 0.94f, 0.98f);
                colorAdj.active = true;

                // Vignette: Focus player attention
                if (!profile.TryGet<Vignette>(out var vignette))
                {
                    vignette = profile.Add<Vignette>(true);
                }
                vignette.intensity.value = 0.28f;
                vignette.smoothness.value = 0.42f;
                vignette.active = true;

                // White Balance: Slight cold cinematic shift
                if (!profile.TryGet<WhiteBalance>(out var wb))
                {
                    wb = profile.Add<WhiteBalance>(true);
                }
                wb.temperature.value = -6f;
                wb.tint.value = -2f;
                wb.active = true;

                volume.sharedProfile = profile;
            }
        }

        // =========================================================
        // L. PLAYER CHARACTER UPGRADE & ANIMATION SETUP
        // =========================================================
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var debugVisual = player.transform.Find("DebugVisual");
            if (debugVisual != null) debugVisual.gameObject.SetActive(false);

            var oldManny = player.transform.Find("MannequinVisual");
            if (oldManny != null) Object.DestroyImmediate(oldManny.gameObject);

            var existingVisual = player.transform.Find("PlayerVisual");
            if (existingVisual == null)
            {
                var armaturePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Hung/StarterAssets/ThirdPersonController/Character/Models/Armature.fbx");
                var animController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Prefabs/Hung/StarterAssets/ThirdPersonController/Character/Animations/StarterAssetsThirdPerson.controller");
                Avatar avatar = null;
                var allAssets = AssetDatabase.LoadAllAssetsAtPath("Assets/Prefabs/Hung/StarterAssets/ThirdPersonController/Character/Models/Armature.fbx");
                foreach (var asset in allAssets)
                {
                    if (asset is Avatar av)
                    {
                        avatar = av;
                        break;
                    }
                }

                var visual = Object.Instantiate(armaturePrefab, player.transform);
                visual.name = "PlayerVisual";
                visual.transform.localPosition = new Vector3(0, -1f, 0);
                visual.transform.localRotation = Quaternion.identity;

                foreach (var c in visual.GetComponentsInChildren<Collider>(true))
                {
                    Object.DestroyImmediate(c);
                }

                var anim = visual.GetComponent<Animator>();
                if (anim == null) anim = visual.AddComponent<Animator>();
                anim.runtimeAnimatorController = animController;
                anim.avatar = avatar;

                var smr = visual.GetComponentInChildren<SkinnedMeshRenderer>();
                if (smr != null && matPlayerBase != null)
                {
                    smr.sharedMaterial = matPlayerBase;
                }
            }

            if (player.GetComponent<PlayerAnimationAdapter>() == null)
            {
                player.AddComponent<PlayerAnimationAdapter>();
            }
        }

        // =========================================================
        // M. CINEMACHINE CAMERA FRAMING POLISH
        // =========================================================
        var vcam = GameObject.Find("CM_GameplayCamera");
        if (vcam != null)
        {
            var tpf = vcam.GetComponent<Unity.Cinemachine.CinemachineThirdPersonFollow>();
            if (tpf != null)
            {
                tpf.CameraDistance = 3.6f;
                tpf.ShoulderOffset = new Vector3(0f, 0.55f, 0f);
                tpf.VerticalArmLength = 0f;
            }
        }

        // =========================================================
        // N. CLEAN UP TECHNICAL DEBUG LABELS FOR CLEAN HUD
        // =========================================================
        var labels = Object.FindObjectsByType<GameplayObjectLabel>(FindObjectsSortMode.None);
        foreach (var l in labels)
        {
            var mr = l.GetComponent<MeshRenderer>();
            if (mr != null) mr.enabled = false;
        }

#if UNITY_EDITOR
        EditorSceneManager.MarkSceneDirty(envRoot.scene);
        EditorSceneManager.SaveScene(envRoot.scene);
#endif
        Debug.Log("[M1EnvironmentBuilder] Production environment architecture successfully generated and saved.");
    }

    private static GameObject CreateBox(string name, Vector3 pos, Vector3 size, Material mat, Transform parent, bool addCollider)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.position = pos;
        go.transform.localScale = size;
        if (mat != null)
        {
            go.GetComponent<Renderer>().sharedMaterial = mat;
        }
        if (!addCollider)
        {
            var c = go.GetComponent<Collider>();
            if (c != null) Object.DestroyImmediate(c);
        }
        return go;
    }

    private static Light CreatePointLight(string name, Vector3 pos, Color color, float intensity, float range, Transform parent)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.position = pos;
        var light = go.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = color;
        light.intensity = intensity;
        light.range = range;
        light.shadows = LightShadows.Soft;
        return light;
    }
}
