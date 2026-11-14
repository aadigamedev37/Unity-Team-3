using UnityEngine;
using UnityEngine.UI;

namespace Mohit.MetaSystems
{
    // Put this on the pause button. It finds the Pause menu in the current scene, so it works in any level.
    [RequireComponent(typeof(Button))]
    public class PauseButtonHook : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                if (PauseMenuManager.Instance != null)
                    PauseMenuManager.Instance.TogglePause();
                else if (StealMan.Core.GameManager.Instance != null)
                    StealMan.Core.GameManager.Instance.TogglePause();
            });
        }
    }
}