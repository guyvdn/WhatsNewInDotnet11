namespace Dotnet11Demo;

public closed record class GateState;
public record class Closed : GateState;
public record class Open(float Percent) : GateState;

public static class ClosedHierarchies
{
    public static string Describe(GateState state) => state switch
    {
        Closed => "closed",
        Open(var percent) => $"{percent}% open",
    };

    public static void Run()
    {
        Console.WriteLine(Describe(new Open(40)));
    }
}
