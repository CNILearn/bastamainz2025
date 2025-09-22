string name = nameof(List<>);
string oldName = nameof(List<string>);

Console.WriteLine($"{name} - using unconstrained generic"); // List
Console.WriteLine($"{oldName} - not unconstrained: previous version with the same result, but why add a type?");