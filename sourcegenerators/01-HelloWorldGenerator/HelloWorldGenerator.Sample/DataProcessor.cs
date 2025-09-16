namespace HelloWorldGenerator.Sample;

public class DataProcessor
{
    public void ProcessItems(List<string> items)
    {
        foreach (var item in items)
        {
            Console.WriteLine($"Processing: {item}");
        }
    }
}
