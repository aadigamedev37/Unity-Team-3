using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XorinSandbox.Driving
{
    /// <summary>
    /// Owns lap progress, checkpoint ordering, timing, respawn and the race HUD.
    /// Sandbox-only: unrelated to StealMan production systems.
    /// </summary>
    public class SandboxRaceManager : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private SandboxCarController car;
        [SerializeField] private SandboxChaseCamera chaseCamera;
        [SerializeField] private Transform startPose;
        [SerializeField] private List<SandboxCheckpoint> checkpoints = new List<SandboxCheckpoint>();

        [Header("Race Settings")]
        [SerializeField] private int totalLaps = 2;
        [SerializeField] private float fallResetHeight = -8f;

        [Header("UI")]
        [SerializeField] private Text speedText;
        [SerializeField] private Text lapText;
        [SerializeField] private Text checkpointText;
        [SerializeField] private Text timeText;
        [SerializeField] private Text messageText;
        [SerializeField] private GameObject finishPanel;
        [SerializeField] private Text finishText;

        private int nextCheckpointIndex;
        private int currentLap;
        private float raceTime;
        private float bestLapTime = -1f;
        private float lapStartTime;
        private bool raceFinished;
        private bool raceStarted;
        private float messageTimer;

        public int CurrentLap => currentLap;
        public int NextCheckpointIndex => nextCheckpointIndex;
        public bool RaceFinished => raceFinished;
        public float RaceTime => raceTime;

        private void Start()
        {
            checkpoints.RemoveAll(c => c == null);
            checkpoints.Sort((a, b) => a.Index.CompareTo(b.Index));

            if (finishPanel != null)
            {
                finishPanel.SetActive(false);
            }

            ResetRace();
        }

        private void Update()
        {
            if (!raceFinished && raceStarted)
            {
                raceTime += Time.deltaTime;
            }

            if (SandboxDriveInput.RespawnPressed)
            {
                RespawnAtStart("RESPAWNED");
            }

            if (car != null && car.transform.position.y < fallResetHeight)
            {
                RespawnAtStart("OFF TRACK - RESPAWNED");
            }

            if (messageTimer > 0f)
            {
                messageTimer -= Time.deltaTime;
                if (messageTimer <= 0f && messageText != null)
                {
                    messageText.text = string.Empty;
                }
            }

            RefreshHud();
        }

        public void NotifyCheckpoint(SandboxCheckpoint checkpoint, SandboxCarController enteringCar)
        {
            if (raceFinished || checkpoint == null || enteringCar == null || checkpoints.Count == 0)
            {
                return;
            }

            if (checkpoint.Index != nextCheckpointIndex)
            {
                return;
            }

            if (!raceStarted)
            {
                raceStarted = true;
                lapStartTime = raceTime;
            }

            nextCheckpointIndex++;

            if (nextCheckpointIndex >= checkpoints.Count)
            {
                nextCheckpointIndex = 0;
                CompleteLap();
            }
            else
            {
                ShowMessage($"CHECKPOINT {checkpoint.Index + 1}/{checkpoints.Count}");
            }
        }

        private void CompleteLap()
        {
            currentLap++;
            float lapTime = raceTime - lapStartTime;
            lapStartTime = raceTime;

            if (bestLapTime < 0f || lapTime < bestLapTime)
            {
                bestLapTime = lapTime;
            }

            Debug.Log($"[SandboxRace] Lap {currentLap} complete in {lapTime:0.00}s");

            if (currentLap >= totalLaps)
            {
                FinishRace();
            }
            else
            {
                ShowMessage($"LAP {currentLap} - {lapTime:0.00}s");
            }
        }

        private void FinishRace()
        {
            raceFinished = true;

            if (car != null)
            {
                car.SetInputEnabled(false);
            }

            if (finishPanel != null)
            {
                finishPanel.SetActive(true);
            }

            if (finishText != null)
            {
                finishText.text = $"RACE COMPLETE\nTOTAL {raceTime:0.00}s\nBEST LAP {bestLapTime:0.00}s";
            }

            Debug.Log($"[SandboxRace] Race finished. Total {raceTime:0.00}s, best lap {bestLapTime:0.00}s");
        }

        /// <summary>Hooked to the Restart button.</summary>
        public void ResetRace()
        {
            nextCheckpointIndex = 0;
            currentLap = 0;
            raceTime = 0f;
            lapStartTime = 0f;
            bestLapTime = -1f;
            raceFinished = false;
            raceStarted = false;

            if (finishPanel != null)
            {
                finishPanel.SetActive(false);
            }

            if (car != null)
            {
                car.SetInputEnabled(true);
            }

            RespawnAtStart("READY - W/S DRIVE, A/D STEER");
            RefreshHud();
        }

        public void RespawnAtStart(string message)
        {
            if (car != null && startPose != null)
            {
                car.TeleportTo(startPose.position, startPose.rotation);
            }

            if (chaseCamera != null)
            {
                chaseCamera.SnapToTarget();
            }

            ShowMessage(message);
        }

        private void ShowMessage(string message)
        {
            if (messageText != null)
            {
                messageText.text = message;
            }

            messageTimer = 2.5f;
        }

        private void RefreshHud()
        {
            if (speedText != null && car != null)
            {
                speedText.text = $"SPEED  {car.SpeedKph:000} KPH";
            }

            if (lapText != null)
            {
                int shown = Mathf.Clamp(currentLap + (raceFinished ? 0 : 1), 1, Mathf.Max(1, totalLaps));
                lapText.text = $"LAP  {shown} / {totalLaps}";
            }

            if (checkpointText != null)
            {
                checkpointText.text = checkpoints.Count > 0
                    ? $"NEXT CP  {nextCheckpointIndex + 1} / {checkpoints.Count}"
                    : "NEXT CP  -";
            }

            if (timeText != null)
            {
                timeText.text = $"TIME  {raceTime:00.00}s";
            }
        }

        /// <summary>Editor/diagnostic helper used by validation.</summary>
        public string DescribeState()
        {
            return $"lap={currentLap}/{totalLaps} nextCp={nextCheckpointIndex} cps={checkpoints.Count} " +
                   $"time={raceTime:0.00} finished={raceFinished} car={(car != null)} start={(startPose != null)}";
        }
    }
}
