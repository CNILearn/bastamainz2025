using System.Runtime.CompilerServices;

namespace HelloWorldGenerator.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        // Initialize Verify for source generators
        VerifySourceGenerators.Initialize();
    }
}