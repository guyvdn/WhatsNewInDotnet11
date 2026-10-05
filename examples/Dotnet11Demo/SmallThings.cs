namespace Dotnet11Demo;

public static class SequenceIndexer
{
    extension(IEnumerable<int> sequence)
    {
        public int this[int index] => sequence.ElementAt(index);
    }
}

public static class SmallThings
{
    public static void Run()
    {
        string[] values = ["a", "b"];
        List<string> names = [with(capacity: values.Length * 2), .. values];
        HashSet<string> set = [with(StringComparer.OrdinalIgnoreCase), "Hello", "HELLO", "hello"];
        Console.WriteLine($"{names.Capacity} {set.Count}");

        int third = Enumerable.Range(1, 10)[2];
        Console.WriteLine(third);

        int[,] grid = { { 0, 0, 1 }, { 0, 2, 0 } };
        outer: for (int row = 0; row < grid.GetLength(0); row++)
        {
            for (int column = 0; column < grid.GetLength(1); column++)
            {
                if (grid[row, column] == 1) continue outer;
                if (grid[row, column] == 2) break outer;
            }
        }

        var byName = EqualityComparer<(string Name, int Age)>.Create(p => p.Name);
        var people = new HashSet<(string Name, int Age)>(byName)
            { ("Alice", 30), ("Bob", 25), ("Alice", 40) };
        Console.WriteLine(people.Count);

        long l = Random.Shared.NextInteger<long>(0L, 100L);
        Half h = Random.Shared.NextBinaryFloat<Half>();
        Console.WriteLine($"{l} {h}");
    }
}
