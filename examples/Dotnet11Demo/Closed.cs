namespace Dotnet11Demo;

public closed record class Payment;
public record class Pending : Payment;
public record class Paid(decimal Amount) : Payment;
public record class Refunded(string Reason) : Payment;

public static class ClosedHierarchies
{
    public static string Describe(Payment payment) => payment switch
    {
        Pending => "waiting for the bank",
        Paid(var amount) => $"paid {amount}",
        Refunded(var reason) => $"refunded: {reason}",
    };

    public static void Run()
    {
        Console.WriteLine(Describe(new Paid(40m)));
    }
}
