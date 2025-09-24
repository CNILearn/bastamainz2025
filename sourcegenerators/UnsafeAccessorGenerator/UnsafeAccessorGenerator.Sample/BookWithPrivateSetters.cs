namespace UnsafeAccessorGenerator.Sample;

// Book class with private setters
public class BookWithPrivateSetters
{
    public string Title { get; private set; } = string.Empty;
    public string Publisher { get; private set; } = string.Empty;
    
    public override string ToString() => $"{Title} {Publisher}";
}
