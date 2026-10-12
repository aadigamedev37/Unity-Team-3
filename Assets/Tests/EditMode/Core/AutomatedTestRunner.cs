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
            typeof(StealIntegrationValidationTests)
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
}
