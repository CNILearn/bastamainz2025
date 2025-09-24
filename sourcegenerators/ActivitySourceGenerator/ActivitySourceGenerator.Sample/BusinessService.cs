using System.Diagnostics;
using ActivitySourceGenerator.Attributes;

namespace ActivitySourceGenerator.Sample;

public class BusinessService
{
    [Activity(ActivitySourceName = "BusinessService", ActivityName = "ProcessData")]
    public static string ProcessData(string input)
    {
        // Simulate some work
        Thread.Sleep(100);
        
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentException("Input cannot be null or empty", nameof(input));
        }
        
        return $"Processed: {input}";
    }

    [Activity]
    public static async Task<int> ProcessAsync(string data)
    {
        // Simulate async work
        await Task.Delay(200);
        return data.Length;
    }
}
