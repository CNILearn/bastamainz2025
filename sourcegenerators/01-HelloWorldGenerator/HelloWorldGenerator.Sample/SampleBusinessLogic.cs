namespace HelloWorldGenerator.Sample;

public class SampleBusinessLogic
{
    public string ProcessData(string input)
    {
        return $"Processed: {input}";
    }

    public async Task<int> CalculateAsync(int value)
    {
        await Task.Delay(10);
        return value * 2;
    }

    public void LogMessage(string message)
    {
        Console.WriteLine($"[LOG] {message}");
    }
}
