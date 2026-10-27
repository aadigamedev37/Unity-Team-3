using UnityEditor;
using UnityEngine;
using MCPForUnity.Editor.Services;
using System.Threading.Tasks;

[InitializeOnLoad]
public static class AutoStartMcpServer
{
    static AutoStartMcpServer()
    {
        EditorApplication.delayCall += () => _ = StartServerAsync();
    }

    public static async Task StartServerAsync()
    {
        try
        {
            if (MCPServiceLocator.Server != null)
            {
                if (!MCPServiceLocator.Server.IsLocalHttpServerReachable())
                {
                    bool started = MCPServiceLocator.Server.StartLocalHttpServer(quiet: true);
                    Debug.Log($"[AutoStartMcpServer] StartLocalHttpServer returned: {started}");
                }
            }

            if (MCPServiceLocator.Bridge != null)
            {
                bool bridgeStarted = await MCPServiceLocator.Bridge.StartAsync();
                Debug.Log($"[AutoStartMcpServer] Bridge.StartAsync returned: {bridgeStarted}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[AutoStartMcpServer] Exception: {ex.Message}");
        }
    }
}
