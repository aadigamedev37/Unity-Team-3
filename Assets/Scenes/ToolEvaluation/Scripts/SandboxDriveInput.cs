using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace XorinSandbox.Driving
{
    /// <summary>
    /// Keyboard reader that works with the legacy Input Manager and the Input System package.
    /// Isolated to the sandbox so no production input assets are touched.
    /// </summary>
    public static class SandboxDriveInput
    {
        public static float Throttle
        {
            get
            {
                float v = 0f;
#if ENABLE_INPUT_SYSTEM
                var k = Keyboard.current;
                if (k != null)
                {
                    if (k.wKey.isPressed || k.upArrowKey.isPressed) v += 1f;
                    if (k.sKey.isPressed || k.downArrowKey.isPressed) v -= 1f;
                    return v;
                }
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
                v = Input.GetAxisRaw("Vertical");
#endif
                return v;
            }
        }

        public static float Steer
        {
            get
            {
                float v = 0f;
#if ENABLE_INPUT_SYSTEM
                var k = Keyboard.current;
                if (k != null)
                {
                    if (k.dKey.isPressed || k.rightArrowKey.isPressed) v += 1f;
                    if (k.aKey.isPressed || k.leftArrowKey.isPressed) v -= 1f;
                    return v;
                }
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
                v = Input.GetAxisRaw("Horizontal");
#endif
                return v;
            }
        }

        public static bool HandbrakeHeld
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                var k = Keyboard.current;
                if (k != null) return k.spaceKey.isPressed;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
                return Input.GetKey(KeyCode.Space);
#else
                return false;
#endif
            }
        }

        public static bool RespawnPressed
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                var k = Keyboard.current;
                if (k != null) return k.rKey.wasPressedThisFrame;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
                return Input.GetKeyDown(KeyCode.R);
#else
                return false;
#endif
            }
        }
    }
}
