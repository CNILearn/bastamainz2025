string name = nameof(List<>);
string oldName = nameof(List<string>);

Console.WriteLine($"{name} - using unbound generic"); // List
Console.WriteLine($"{oldName} - not unbound: previous version with the same result, but why add a type?");