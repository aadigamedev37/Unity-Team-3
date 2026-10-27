using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Autonomous reflection-based test runner capable of executing NUnit test fixtures
/// inside the Unity Editor or runtime without requiring external test runners.
/// </summary>
public static class AutomatedTestRunner
{
#if UNITY_EDITOR
    [MenuItem("StealMan/Tests/Run Core Integration Tests")]
    public static void RunAllCoreTestsMenu()
    {
        RunAllCoreTests();
    }
#endif

    public static bool RunAllCoreTests()
    {
        Debug.Log("==================================================");
        Debug.Log("STARTING AUTOMATED NUNIT CORE INTEGRATION TESTS");
        Debug.Log("==================================================");

        Type[] testTypes = new Type[]
        {
            typeof(PlayerShadowStateMachineTests),
            typeof(InputSystemValidationTests),
            typeof(TrapRecoveryValidationTests),
            typeof(StealIntegrationValidationTests),
            typeof(FeedbackAndContextualActionValidationTests),
            typeof(ApplicationFlowAndUIIntegrationTests)
        };

        int totalTests = 0;
        int passedTests = 0;
        int failedTests = 0;

        foreach (Type testType in testTypes)
        {
            Debug.Log($"\n>>> Running Fixture: {testType.Name}");
            MethodInfo setUpMethod = null;
            MethodInfo tearDownMethod = null;

            foreach (var m in testType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (m.GetCustomAttribute<SetUpAttribute>() != null) setUpMethod = m;
                if (m.GetCustomAttribute<TearDownAttribute>() != null) tearDownMethod = m;
            }

            foreach (var testMethod in testType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (testMethod.GetCustomAttribute<TestAttribute>() == null)
                    continue;

                totalTests++;
                object fixtureInstance = Activator.CreateInstance(testType);

                try
                {
                    setUpMethod?.Invoke(fixtureInstance, null);
                    testMethod.Invoke(fixtureInstance, null);
                    tearDownMethod?.Invoke(fixtureInstance, null);

                    passedTests++;
                    Debug.Log($"  [PASS] {testMethod.Name}");
                }
                catch (Exception ex)
                {
                    failedTests++;
                    Exception inner = ex.InnerException ?? ex;
                    Debug.LogError($"  [FAIL] {testMethod.Name}: {inner.Message}");

                    try
                    {
                        tearDownMethod?.Invoke(fixtureInstance, null);
                    }
                    catch
                    {
                        // Ignore secondary cleanup exception
                    }
                }
            }
        }

        Debug.Log("==================================================");
        Debug.Log($"TEST SUMMARY: {passedTests}/{totalTests} PASSED, {failedTests} FAILED.");
        Debug.Log("==================================================");

        return failedTests == 0;
    }

#if UNITY_EDITOR
    [InitializeOnLoadMethod]
    private static void InitTriggerListener()
    {
        EditorApplication.update -= CheckTrigger;
        EditorApplication.update += CheckTrigger;
    }

    private static void CheckTrigger()
    {
        string triggerPath = "Temp/trigger_run_tests.txt";
        string resultPath = "Temp/test_results.txt";

        if (System.IO.File.Exists(triggerPath))
        {
            try { System.IO.File.Delete(triggerPath); } catch { }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("==================================================");
            sb.AppendLine("STARTING AUTOMATED NUNIT CORE INTEGRATION TESTS");
            sb.AppendLine("==================================================");

            Type[] testTypes = new Type[]
            {
                typeof(PlayerShadowStateMachineTests),
                typeof(InputSystemValidationTests),
                typeof(TrapRecoveryValidationTests),
                typeof(StealIntegrationValidationTests),
                typeof(FeedbackAndContextualActionValidationTests),
                typeof(ApplicationFlowAndUIIntegrationTests)
            };

            int totalTests = 0;
            int passedTests = 0;
            int failedTests = 0;

            foreach (Type testType in testTypes)
            {
                sb.AppendLine($"\n>>> Running Fixture: {testType.Name}");
                MethodInfo setUpMethod = null;
                MethodInfo tearDownMethod = null;

                foreach (var m in testType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (m.GetCustomAttribute<SetUpAttribute>() != null) setUpMethod = m;
                    if (m.GetCustomAttribute<TearDownAttribute>() != null) tearDownMethod = m;
                }

                foreach (var testMethod in testType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (testMethod.GetCustomAttribute<TestAttribute>() == null)
                        continue;

                    totalTests++;
                    object fixtureInstance = Activator.CreateInstance(testType);

                    try
                    {
                        setUpMethod?.Invoke(fixtureInstance, null);
                        testMethod.Invoke(fixtureInstance, null);
                        tearDownMethod?.Invoke(fixtureInstance, null);

                        passedTests++;
                        sb.AppendLine($"  [PASS] {testMethod.Name}");
                    }
                    catch (Exception ex)
                    {
                        failedTests++;
                        Exception inner = ex.InnerException ?? ex;
                        sb.AppendLine($"  [FAIL] {testMethod.Name}: {inner.Message}");

                        try { tearDownMethod?.Invoke(fixtureInstance, null); } catch { }
                    }
                }
            }

            sb.AppendLine("==================================================");
            sb.AppendLine($"TEST SUMMARY: {passedTests}/{totalTests} PASSED, {failedTests} FAILED.");
            sb.AppendLine("==================================================");

            System.IO.File.WriteAllText(resultPath, sb.ToString());
            Debug.Log($"[AutomatedTestRunner] Results written to {resultPath}: {passedTests}/{totalTests} passed.");
        }
    }
#endif
}
