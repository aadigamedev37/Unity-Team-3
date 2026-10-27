using System;

class Program
{
    static int Main(string[] args)
    {
        Console.WriteLine("Executing AutomatedTestRunner via .NET CLI...");
        bool success = AutomatedTestRunner.RunAllCoreTests();
        Console.WriteLine($"Result: {(success ? "SUCCESS" : "FAILURE")}");
        return success ? 0 : 1;
    }
}
