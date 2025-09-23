
int[] items = [ 1, 2, 3, 4, 5 ];
ShowInts(items);
ShowInts(3, 4, 5);
ShowInts2(6, 7, 8);

ChangeSpan(items);
ChangeSpan(items);
ShowSpan(items);
Console.WriteLine();

ChangeSpan(7, 3, 4);

Console.WriteLine();

Span<int> x = items;

UseIEnumerable(3, 4, 5);

ShowStrings("abc", "def", "ghi");

static void ShowInts(params int[] items)
{
    foreach (var item in items)
    {
        Console.WriteLine(item);
    }
}

static void ShowInts2(params List<int> items)
{
    foreach (var item in items)
    {
        Console.WriteLine(item);
    }
}

static void ShowStrings(params List<string> items)
{
    foreach (var item in items)
    {
        Console.WriteLine(item);
    }
}   

static void UseIEnumerable(params IEnumerable<int> items)
{
    foreach (var item in items)
    {
        Console.WriteLine(item);
    }
    Console.WriteLine();
}

static void ShowSpan(params Span<int> items)
{
    foreach (var item in items)
    {
        Console.WriteLine(item);
    }
    Console.WriteLine();
}

static void ChangeSpan(params Span<int> items)
{
    for (int i = 0; i < items.Length; i++)
    {
        items[i] += 2;
        Console.WriteLine(items[i]);
    }
    Console.WriteLine();
}