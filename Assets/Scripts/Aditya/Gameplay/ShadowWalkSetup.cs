using UnityEngine;

using UnityEngine.UI;

using TMPro;



#if UNITY_EDITOR

using UnityEditor;

using UnityEditor.SceneManagement;

#endif



/// <summary>

/// Technical Setup utility for configuring SHADOW WALK gameplay ability,

/// designated climbable surfaces, visual tether, particle systems, and circular HUD UI.

/// </summary>

public static class ShadowWalkSetup

{

#if UNITY_EDITOR

    [MenuItem("StealMan/Setup Shadow Walk Ability")]

    public static void ConfigureShadowWalk()

    {

        var scene = EditorSceneManager.GetActiveScene();



        // 1. Configure Explicit Climbable Surface on Drawbridge Wall

        var climbWall = GameObject.Find("Drawbridge_ClimbableWall");

        if (climbWall != null)

        {

            int shadowLayer = LayerMask.NameToLayer("ShadowSurface");

            if (shadowLayer >= 0) climbWall.layer = shadowLayer;



            var climbable = climbWall.GetComponent<ShadowClimbableSurface>();

            if (climbable == null) climbable = climbWall.AddComponent<ShadowClimbableSurface>();

            climbable.IsClimbable = true;

            Debug.Log("[ShadowWalkSetup] ShadowClimbableSurface configured on Drawbridge_ClimbableWall.");

        }



        // 2. Resolve Player and SoulOrigin

        var player = GameObject.FindWithTag("Player");

        if (player == null)

        {

            Debug.LogError("[ShadowWalkSetup] Player GameObject not found.");

            return;

        }



        Transform soulOrigin = player.transform.Find("SoulOrigin");

        if (soulOrigin == null)

        {

            var go = new GameObject("SoulOrigin");

            go.transform.SetParent(player.transform, false);

            go.transform.localPosition = new Vector3(0f, 1.2f, 0f); // Chest level

            soulOrigin = go.transform;

        }



        // 3. Configure Soul Extraction Particle System

        Transform extractObj = soulOrigin.Find("ExtractionVFX");

        if (extractObj == null)

        {

            var go = new GameObject("ExtractionVFX");

            go.transform.SetParent(soulOrigin, false);

            extractObj = go.transform;

        }

        var extractPS = extractObj.GetComponent<ParticleSystem>();

        if (extractPS == null) extractPS = extractObj.gameObject.AddComponent<ParticleSystem>();



        var extractMain = extractPS.main;

        extractMain.duration = 1.0f;

        extractMain.loop = true;

        extractMain.startLifetime = new ParticleSystem.MinMaxCurve(0.6f, 1.0f);

        extractMain.startSpeed = new ParticleSystem.MinMaxCurve(1.5f, 2.5f);

        extractMain.startSize = new ParticleSystem.MinMaxCurve(0.12f, 0.28f);

        extractMain.startColor = new Color(0.06f, 0.12f, 0.16f, 0.85f); // Dark spectral charcoal

        extractMain.simulationSpace = ParticleSystemSimulationSpace.World;

        extractMain.playOnAwake = false;



        var extractEmission = extractPS.emission;

        extractEmission.rateOverTime = 16f;

        extractEmission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 30) });



        var extractShape = extractPS.shape;

        extractShape.shapeType = ParticleSystemShapeType.Sphere;

        extractShape.radius = 0.25f;



        var matExtract = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_SoulExtractionParticle.mat");

        var extractRenderer = extractObj.GetComponent<ParticleSystemRenderer>();

        if (extractRenderer != null && matExtract != null)

        {

            extractRenderer.sharedMaterial = matExtract;

        }



        // 4. Configure Soul Recall Particle System

        Transform recallObj = soulOrigin.Find("RecallVFX");

        if (recallObj == null)

        {

            var go = new GameObject("RecallVFX");

            go.transform.SetParent(soulOrigin, false);

            recallObj = go.transform;

        }

        var recallPS = recallObj.GetComponent<ParticleSystem>();

        if (recallPS == null) recallPS = recallObj.gameObject.AddComponent<ParticleSystem>();



        var recallMain = recallPS.main;

        recallMain.duration = 0.6f;

        recallMain.loop = false;

        recallMain.startLifetime = new ParticleSystem.MinMaxCurve(0.4f, 0.7f);

        recallMain.startSpeed = new ParticleSystem.MinMaxCurve(2.5f, 4.0f);

        recallMain.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.25f);

        recallMain.startColor = new Color(0.15f, 0.95f, 1.0f, 0.95f); // Radiant Cyan

        recallMain.simulationSpace = ParticleSystemSimulationSpace.World;

        recallMain.playOnAwake = false;



        var recallEmission = recallPS.emission;

        recallEmission.rateOverTime = 0f;

        recallEmission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 45) });



        var recallShape = recallPS.shape;

        recallShape.shapeType = ParticleSystemShapeType.Sphere;

        recallShape.radius = 0.35f;



        var matRecall = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_SoulRecallParticle.mat");

        var recallRenderer = recallObj.GetComponent<ParticleSystemRenderer>();

        if (recallRenderer != null && matRecall != null)

        {

            recallRenderer.sharedMaterial = matRecall;

        }



        // 5. Clean up duplicate SurfaceTraversal on Shadow

        var shadow = GameObject.Find("Shadow");

        if (shadow != null)

        {

            var stList = shadow.GetComponents<SurfaceTraversal>();

            if (stList.Length > 1)

            {

                for (int i = 1; i < stList.Length; i++)

                {

                    Object.DestroyImmediate(stList[i]);

                }

                Debug.Log("[ShadowWalkSetup] Cleaned up duplicate SurfaceTraversal on Shadow.");

            }

        }



        // 6. Setup ShadowTether LineRenderer

        var tetherObj = GameObject.Find("ShadowTether");

        if (tetherObj == null)

        {

            tetherObj = new GameObject("ShadowTether");

        }

        var line = tetherObj.GetComponent<LineRenderer>();

        if (line == null) line = tetherObj.AddComponent<LineRenderer>();



        var matTether = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_ShadowTether.mat");

        if (matTether != null) line.sharedMaterial = matTether;

        line.startWidth = 0.05f;

        line.endWidth = 0.03f;

        line.positionCount = 20;

        line.useWorldSpace = true;

        line.enabled = false;



        // 7. Setup ShadowWalkAbility component

        var ability = player.GetComponent<ShadowWalkAbility>();

        if (ability == null) ability = player.AddComponent<ShadowWalkAbility>();



        // Wire references via SerializedObject

        var soAbility = new SerializedObject(ability);

        soAbility.FindProperty("playerAnchor").objectReferenceValue = player.transform;

        soAbility.FindProperty("soulOrigin").objectReferenceValue = soulOrigin;

        if (shadow != null) soAbility.FindProperty("shadow").objectReferenceValue = shadow.transform;

        soAbility.FindProperty("stateMachine").objectReferenceValue = Object.FindAnyObjectByType<PlayerShadowStateMachine>();

        if (shadow != null)

        {

            soAbility.FindProperty("shadowAdapter").objectReferenceValue = shadow.GetComponent<ShadowMovementAdapter>();

            soAbility.FindProperty("surfaceTraversal").objectReferenceValue = shadow.GetComponent<SurfaceTraversal>();

        }

        soAbility.FindProperty("abilityDuration").floatValue = 15f;

        soAbility.FindProperty("maxLeashLength").floatValue = 10f;

        soAbility.FindProperty("groundDetectionDistance").floatValue = 0.6f;

        soAbility.FindProperty("surfaceDetectionDistance").floatValue = 1.5f;



        int shadowSurfaceLayer = LayerMask.NameToLayer("ShadowSurface");

        if (shadowSurfaceLayer >= 0)

        {

            soAbility.FindProperty("climbableSurfaceLayer").intValue = 1 << shadowSurfaceLayer;

        }



        soAbility.FindProperty("extractionParticles").objectReferenceValue = extractPS;

        soAbility.FindProperty("recallParticles").objectReferenceValue = recallPS;

        soAbility.FindProperty("tetherLine").objectReferenceValue = line;

        soAbility.ApplyModifiedProperties();



        // 8. Configure On-Screen Circular Ability Button

        ConfigureAbilityHUD(ability);



        EditorSceneManager.MarkSceneDirty(scene);

        EditorSceneManager.SaveScene(scene);

        Debug.Log("[ShadowWalkSetup] Shadow Walk ability fully configured and scene saved!");

    }



    private static void ConfigureAbilityHUD(ShadowWalkAbility ability)

    {

        var actionBtn = GameObject.Find("Canvas/ContextualActionButton");

        if (actionBtn == null)

        {

            Debug.LogError("[ShadowWalkSetup] ContextualActionButton not found under Canvas.");

            return;

        }



        var rt = actionBtn.GetComponent<RectTransform>();

        rt.anchorMin = new Vector2(1f, 0f);

        rt.anchorMax = new Vector2(1f, 0f);

        rt.pivot = new Vector2(1f, 0f);

        rt.anchoredPosition = new Vector2(-70f, 70f);

        rt.sizeDelta = new Vector2(130f, 130f);



        // Circular background

        var bgImage = actionBtn.GetComponent<Image>();

        var circleSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Prefabs/Hung/StarterAssets/Mobile/UI/UI_Circle_Bevel_Base.png");

        var fadedCircleSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Prefabs/Hung/StarterAssets/Mobile/UI/UI_Circle_Faded.png");



        if (bgImage != null)

        {

            if (circleSprite != null) bgImage.sprite = circleSprite;

            bgImage.type = Image.Type.Simple;

            bgImage.color = new Color(0.08f, 0.10f, 0.13f, 0.95f); // Dark Charcoal

        }



        // Outer Glow Rim

        RectTransform glowRt = actionBtn.transform.Find("GlowBorder") as RectTransform;

        if (glowRt == null)

        {

            var go = new GameObject("GlowBorder", typeof(RectTransform), typeof(Image));

            glowRt = go.GetComponent<RectTransform>();

            glowRt.SetParent(actionBtn.transform, false);

        }

        var glowImage = glowRt.GetComponent<Image>();

        if (glowImage == null) glowImage = glowRt.gameObject.AddComponent<Image>();

        glowImage.raycastTarget = false;

        if (fadedCircleSprite != null) glowImage.sprite = fadedCircleSprite;

        glowImage.color = new Color(0.12f, 0.85f, 0.95f, 0.45f); // Spectral Cyan

        glowRt.anchorMin = Vector2.zero;

        glowRt.anchorMax = Vector2.one;

        glowRt.sizeDelta = new Vector2(16f, 16f);

        glowRt.anchoredPosition = Vector2.zero;

        glowRt.SetAsFirstSibling();



        // Radial Countdown Ring

        RectTransform ringRt = actionBtn.transform.Find("CountdownRing") as RectTransform;

        if (ringRt == null)

        {

            var go = new GameObject("CountdownRing", typeof(RectTransform), typeof(Image));

            ringRt = go.GetComponent<RectTransform>();

            ringRt.SetParent(actionBtn.transform, false);

        }

        var ringImage = ringRt.GetComponent<Image>();

        if (ringImage == null) ringImage = ringRt.gameObject.AddComponent<Image>();

        ringImage.raycastTarget = false;

        if (circleSprite != null) ringImage.sprite = circleSprite;

        ringImage.type = Image.Type.Filled;

        ringImage.fillMethod = Image.FillMethod.Radial360;

        ringImage.fillOrigin = (int)Image.Origin360.Top;

        ringImage.fillClockwise = true;

        ringImage.fillAmount = 1.0f;

        ringImage.color = new Color(0.15f, 0.90f, 1.0f, 0.95f); // Vibrant Cyan

        ringRt.anchorMin = Vector2.zero;

        ringRt.anchorMax = Vector2.one;

        ringRt.sizeDelta = new Vector2(4f, 4f);

        ringRt.anchoredPosition = Vector2.zero;



        // Timer Text Display

        Transform textChild = actionBtn.transform.Find("Text (TMP)");

        TextMeshProUGUI tmpText = null;

        if (textChild != null)

        {

            tmpText = textChild.GetComponent<TextMeshProUGUI>();

            tmpText.fontSize = 20;

            tmpText.fontStyle = FontStyles.Bold;

            tmpText.alignment = TextAlignmentOptions.Center;

            tmpText.color = new Color(0.95f, 0.98f, 1.0f);

            var trt = textChild.GetComponent<RectTransform>();

            trt.anchoredPosition = new Vector2(0f, 6f);

            trt.sizeDelta = new Vector2(110f, 32f);

        }



        // Subtitle Action Label

        RectTransform subRt = actionBtn.transform.Find("ActionSubtitle") as RectTransform;

        if (subRt == null)

        {

            var go = new GameObject("ActionSubtitle", typeof(RectTransform), typeof(TextMeshProUGUI));

            subRt = go.GetComponent<RectTransform>();

            subRt.SetParent(actionBtn.transform, false);

        }

        var subTmp = subRt.GetComponent<TextMeshProUGUI>();

        if (subTmp == null) subTmp = subRt.gameObject.AddComponent<TextMeshProUGUI>();

        subTmp.fontSize = 11;

        subTmp.fontStyle = FontStyles.Bold;

        subTmp.alignment = TextAlignmentOptions.Center;

        subTmp.color = new Color(0.40f, 0.85f, 0.95f, 0.90f);

        subTmp.text = "SHADOW WALK";

        // srt removed

        subRt.anchoredPosition = new Vector2(0f, -24f);

        subRt.sizeDelta = new Vector2(110f, 20f);



        // Attach and wire ShadowWalkAbilityUI

        var ui = actionBtn.GetComponent<ShadowWalkAbilityUI>();

        if (ui == null) ui = actionBtn.AddComponent<ShadowWalkAbilityUI>();



        var soUI = new SerializedObject(ui);

        soUI.FindProperty("ability").objectReferenceValue = ability;

        soUI.FindProperty("stateMachine").objectReferenceValue = Object.FindAnyObjectByType<PlayerShadowStateMachine>();

        soUI.FindProperty("contextualAction").objectReferenceValue = actionBtn.GetComponent<ContextualActionButton>();

        soUI.FindProperty("button").objectReferenceValue = actionBtn.GetComponent<Button>();

        soUI.FindProperty("buttonBackground").objectReferenceValue = bgImage;

        soUI.FindProperty("countdownRing").objectReferenceValue = ringImage;

        soUI.FindProperty("glowBorder").objectReferenceValue = glowImage;

        soUI.FindProperty("timerText").objectReferenceValue = tmpText;

        soUI.FindProperty("actionLabel").objectReferenceValue = subTmp;

        soUI.ApplyModifiedProperties();



        // Wire ability UI reference back to ability

        var soAb = new SerializedObject(ability);

        soAb.FindProperty("abilityUI").objectReferenceValue = ui;

        soAb.ApplyModifiedProperties();

    }

#endif

}

