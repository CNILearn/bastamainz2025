namespace UnsafeAccessorGenerator.Sample;

// Book class with private fields
public class Book
{
    private string _title = string.Empty;
    private string _publisher = string.Empty;
    
    public override string ToString() => $"{_title} {_publisher}";
}
