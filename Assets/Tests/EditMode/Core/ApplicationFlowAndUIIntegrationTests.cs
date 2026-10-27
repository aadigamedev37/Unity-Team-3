using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StealMan.Core;
using StealMan.UI;
using Mohit.MetaSystems;

public class ApplicationFlowAndUIIntegrationTests
{
    private GameObject testRoot;
    private GameManager gameManager;
    private SaveManager saveManager;

    [SetUp]
    public void SetUp()
    {
        testRoot = new GameObject("TestRoot_AppFlow");

        // Reset TimeScale
        Time.timeScale = 1f;

        // Ensure SaveManager
        if (SaveManager.Instance == null)
        {
            GameObject smObj = new GameObject("SaveManager_Test");
            smObj.transform.SetParent(testRoot.transform);
            saveManager = smObj.AddComponent<SaveManager>();
            SaveManager.Instance = saveManager;
        }
        else
        {
            saveManager = SaveManager.Instance;
        }
        saveManager.ResetProgress();

        // Ensure GameManager
        if (GameManager.Instance == null)
        {
            GameObject gmObj = new GameObject("GameManager_Test");
            gmObj.transform.SetParent(testRoot.transform);
            gameManager = gmObj.AddComponent<GameManager>();
            GameManager.Instance = gameManager;
        }
        else
        {
            gameManager = GameManager.Instance;
        }

        // Ensure LevelManager
        if (LevelManager.Instance == null)
        {
            GameObject lmObj = new GameObject("LevelManager_Test");
            lmObj.transform.SetParent(testRoot.transform);
            var lm = lmObj.AddComponent<LevelManager>();
            LevelManager.Instance = lm;
        }

        // Reset GameManager state
        gameManager.ResetSessionState();
        gameManager.Resume();
    }

    [TearDown]
    public void TearDown()
    {
        Time.timeScale = 1f;
        if (testRoot != null)
        {
            UnityEngine.Object.DestroyImmediate(testRoot);
        }
        GameManager.Instance = null;
        SaveManager.Instance = null;
        LevelManager.Instance = null;
    }

    [Test]
    public void GameManager_SingletonAndAutoInitialization_Works()
    {
        Assert.IsNotNull(GameManager.Instance, "GameManager.Instance must not be null.");
        Assert.IsFalse(GameManager.Instance.IsPaused, "GameManager should start unpaused.");
        Assert.AreEqual(1f, Time.timeScale, "Time.timeScale must be 1.0f.");
    }

    [Test]
    public void GameManager_PauseAndResume_UpdatesStateAndFiresOnPaused_MOH009()
    {
        bool receivedPause = false;
        bool lastPauseState = false;

        Action<bool> listener = (paused) =>
        {
            receivedPause = true;
            lastPauseState = paused;
        };

        gameManager.OnPaused += listener;

        // 1. Pause
        gameManager.Pause();
        Assert.IsTrue(gameManager.IsPaused, "GameManager.IsPaused should be true after Pause().");
        Assert.AreEqual(0f, Time.timeScale, "Time.timeScale must be 0f during pause.");
        Assert.IsTrue(receivedPause, "OnPaused event must fire.");
        Assert.IsTrue(lastPauseState, "OnPaused must pass true when pausing.");

        // 2. Resume
        receivedPause = false;
        gameManager.Resume();
        Assert.IsFalse(gameManager.IsPaused, "GameManager.IsPaused should be false after Resume().");
        Assert.AreEqual(1f, Time.timeScale, "Time.timeScale must be 1f after Resume().");
        Assert.IsTrue(receivedPause, "OnPaused event must fire on resume.");
        Assert.IsFalse(lastPauseState, "OnPaused must pass false when resuming.");

        gameManager.OnPaused -= listener;
    }

    [Test]
    public void GameManager_CompleteLevel_PersistsProgressAndFiresOnLevelComplete_MOH010()
    {
        int completedLvl = -1;
        Action<int> listener = (lvl) => completedLvl = lvl;

        gameManager.OnLevelComplete += listener;

        // Level 1 starts unlocked, Level 2 locked
        Assert.IsTrue(saveManager.IsLevelUnlocked(1), "Level 1 must be unlocked by default.");
        Assert.IsFalse(saveManager.IsLevelUnlocked(2), "Level 2 must be locked before Level 1 completion.");

        // Complete Level 1
        gameManager.CompleteLevel(1);

        Assert.AreEqual(1, completedLvl, "OnLevelComplete event must fire with level ID 1.");
        Assert.IsTrue(saveManager.IsLevelCompleted(1), "SaveManager must record Level 1 as completed.");
        Assert.IsTrue(saveManager.IsLevelUnlocked(2), "SaveManager must unlock Level 2 after Level 1 completion.");

        gameManager.OnLevelComplete -= listener;
    }

    [Test]
    public void GameManager_CompleteLevel_GuardsAgainstDuplicateCalls()
    {
        int fireCount = 0;
        Action<int> listener = (lvl) => fireCount++;

        gameManager.OnLevelComplete += listener;

        gameManager.CompleteLevel(1);
        gameManager.CompleteLevel(1); // Duplicate call

        Assert.AreEqual(1, fireCount, "OnLevelComplete must not fire multiple times for duplicate calls in same session.");

        gameManager.OnLevelComplete -= listener;
    }

    [Test]
    public void PauseMenuManager_IntegratesWithGameManagerOnPaused_MOH009()
    {
        GameObject pauseObj = new GameObject("PauseMenu_Test");
        pauseObj.transform.SetParent(testRoot.transform);

        GameObject panelObj = new GameObject("PausePanel");
        panelObj.transform.SetParent(pauseObj.transform);
        panelObj.SetActive(false);

        PauseMenuManager pauseMenu = pauseObj.AddComponent<PauseMenuManager>();
        typeof(PauseMenuManager).GetField("pausePanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(pauseMenu, panelObj);
        pauseMenu.SubscribeToGameManager(gameManager);

        // Call TogglePause via GameManager
        gameManager.Pause();
        Assert.AreEqual(0f, Time.timeScale, "Pause should set Time.timeScale to 0.");
        Assert.IsTrue(panelObj.activeSelf, "PausePanel must become active when paused.");

        gameManager.Resume();
        Assert.AreEqual(1f, Time.timeScale, "Resume should restore Time.timeScale to 1.");
        Assert.IsFalse(panelObj.activeSelf, "PausePanel must become inactive when resumed.");
    }

    [Test]
    public void SaveManager_ProgressionAndLockValidation_EnforcesCorrectUnlocks()
    {
        saveManager.ResetProgress();

        Assert.IsTrue(saveManager.IsLevelUnlocked(1));
        Assert.IsFalse(saveManager.IsLevelUnlocked(2));
        Assert.AreEqual(1, saveManager.GetHighestUnlockedLevel());

        // Completing level 1 unlocks level 2
        saveManager.CompleteLevel(1);
        Assert.IsTrue(saveManager.IsLevelCompleted(1));
        Assert.IsTrue(saveManager.IsLevelUnlocked(2));
        Assert.IsFalse(saveManager.IsLevelUnlocked(3));
        Assert.AreEqual(2, saveManager.GetHighestUnlockedLevel());

        // Out of range check
        saveManager.CompleteLevel(99);
        Assert.AreEqual(2, saveManager.GetHighestUnlockedLevel(), "Out of range levels must be safely rejected.");
    }

    [Test]
    public void ExitTrigger_PlayerDetection_RejectsShadowAndTriggersCompletion()
    {
        GameObject exitObj = new GameObject("ExitTrigger_Test");
        exitObj.transform.SetParent(testRoot.transform);
        BoxCollider col = exitObj.AddComponent<BoxCollider>();
        col.isTrigger = true;
        ExitTrigger exitTrigger = exitObj.AddComponent<ExitTrigger>();
        exitTrigger.LevelId = 1;

        // 1. Create a Shadow GameObject
        GameObject shadowObj = new GameObject("Shadow_Collider");
        shadowObj.tag = "Shadow";
        shadowObj.transform.SetParent(testRoot.transform);
        BoxCollider shadowCol = shadowObj.AddComponent<BoxCollider>();
        shadowObj.AddComponent<IntegrationTestShadow>();

        int completedLevel = -1;
        Action<int> listener = (id) => completedLevel = id;
        gameManager.OnLevelComplete += listener;

        // Invoke OnTriggerEnter with Shadow
        var method = typeof(ExitTrigger).GetMethod("OnTriggerEnter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        method.Invoke(exitTrigger, new object[] { shadowCol });

        Assert.AreEqual(-1, completedLevel, "Shadow entering ExitTrigger must NOT trigger level completion.");

        // 2. Create a Player GameObject
        GameObject playerObj = new GameObject("Player_Collider");
        playerObj.tag = "Player";
        playerObj.transform.SetParent(testRoot.transform);
        BoxCollider playerCol = playerObj.AddComponent<BoxCollider>();
        playerObj.AddComponent<IntegrationTestPlayer>();

        method.Invoke(exitTrigger, new object[] { playerCol });

        Assert.AreEqual(1, completedLevel, "Player entering ExitTrigger MUST trigger level completion for Level 1.");

        gameManager.OnLevelComplete -= listener;
    }

    [Test]
    public void LevelManager_EnsureAll15LevelsConfigured_MOH012()
    {
        Assert.IsNotNull(LevelManager.Instance, "LevelManager.Instance must not be null.");
        var levels = LevelManager.Instance.Levels;
        Assert.IsNotNull(levels, "LevelManager.Levels list must not be null.");
        Assert.GreaterOrEqual(levels.Count, 15, "LevelManager must have at least 15 configured levels.");

        for (int i = 1; i <= 15; i++)
        {
            var cfg = LevelManager.Instance.GetLevelById(i);
            Assert.IsNotNull(cfg, $"Level {i} must be configured in LevelManager.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(cfg.SceneName), $"Level {i} SceneName must not be empty.");
        }
    }

    [Test]
    public void InGameHUD_PauseButtonAndPausePanel_IntegrateCorrectly()
    {
        GameObject hudObj = new GameObject("InGameHUD_Test");
        hudObj.transform.SetParent(testRoot.transform);

        GameObject pauseBtnObj = new GameObject("PauseButton");
        pauseBtnObj.transform.SetParent(hudObj.transform);
        Button pauseBtn = pauseBtnObj.AddComponent<Button>();

        GameObject pausePanelObj = new GameObject("PausePanel");
        pausePanelObj.transform.SetParent(hudObj.transform);
        pausePanelObj.SetActive(false);

        GameObject resumeBtnObj = new GameObject("ResumeButton");
        resumeBtnObj.transform.SetParent(pausePanelObj.transform);
        Button resumeBtn = resumeBtnObj.AddComponent<Button>();

        InGameHUD hud = hudObj.AddComponent<InGameHUD>();
        var hudType = typeof(InGameHUD);
        hudType.GetField("pauseButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(hud, pauseBtn);
        hudType.GetField("pausePanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(hud, pausePanelObj);
        hudType.GetField("pauseResumeButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(hud, resumeBtn);

        // Invoke Awake & OnEnable
        hudType.GetMethod("Awake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(hud, null);
        hudType.GetMethod("OnEnable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(hud, null);
        hud.SubscribeToGameManager(gameManager);

        // Click Pause
        hud.OnPauseButtonClicked();
        Assert.IsTrue(gameManager.IsPaused, "GameManager must be paused after pause button click.");
        Assert.AreEqual(0f, Time.timeScale, "Time.timeScale must be 0 after pause button click.");
        Assert.IsTrue(pausePanelObj.activeSelf, "PausePanel must become active when paused.");

        // Click Resume
        hud.OnResumeClicked();
        Assert.IsFalse(gameManager.IsPaused, "GameManager must be unpaused after resume button click.");
        Assert.AreEqual(1f, Time.timeScale, "Time.timeScale must be 1 after resume button click.");
        Assert.IsFalse(pausePanelObj.activeSelf, "PausePanel must become inactive when resumed.");

        hudType.GetMethod("OnDisable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(hud, null);
    }

    [Test]
    public void InGameHUD_CompletionAndFailurePanels_DisplayOnEvents()
    {
        GameObject hudObj = new GameObject("InGameHUD_PanelsTest");
        hudObj.transform.SetParent(testRoot.transform);

        GameObject completionPanelObj = new GameObject("CompletionPanel");
        completionPanelObj.transform.SetParent(hudObj.transform);
        completionPanelObj.SetActive(false);

        GameObject failurePanelObj = new GameObject("FailurePanel");
        failurePanelObj.transform.SetParent(hudObj.transform);
        failurePanelObj.SetActive(false);

        InGameHUD hud = hudObj.AddComponent<InGameHUD>();
        var hudType = typeof(InGameHUD);
        hudType.GetField("completionPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(hud, completionPanelObj);
        hudType.GetField("failurePanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(hud, failurePanelObj);

        hudType.GetMethod("Awake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(hud, null);
        hudType.GetMethod("OnEnable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(hud, null);
        hud.SubscribeToGameManager(gameManager);

        // 1. Test Completion Event
        Assert.IsFalse(completionPanelObj.activeSelf);
        gameManager.CompleteLevel(1);
        Assert.IsTrue(completionPanelObj.activeSelf, "CompletionPanel must become active on level completion.");

        // 2. Test Failure Event
        gameManager.ResetSessionState();
        Assert.IsFalse(failurePanelObj.activeSelf);
        gameManager.FailLevel("Shadow destroyed");
        Assert.IsTrue(failurePanelObj.activeSelf, "FailurePanel must become active on level failure.");

        hudType.GetMethod("OnDisable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(hud, null);
    }
}
