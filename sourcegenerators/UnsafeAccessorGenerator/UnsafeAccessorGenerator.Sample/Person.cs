namespace UnsafeAccessorGenerator.Sample;

public class Person
{
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;

    public override string ToString() => $"{_firstName} {_lastName}";
}
