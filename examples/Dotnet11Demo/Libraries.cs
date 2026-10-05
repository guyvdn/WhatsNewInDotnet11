using System.Diagnostics;

namespace Dotnet11Demo;

public record Product(int Id, string Name, string Category);
public record Category(string Name, string Description);
public record Order(int ProductId, int Qty);

public static class Libraries
{
    // What everyone wrote before .NET 11
    public static async Task BeforeAsync()
    {
        var psi = new ProcessStartInfo("git", ["status", "--porcelain"])
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        using var process = Process.Start(psi)!;
        Task<string> stdout = process.StandardOutput.ReadToEndAsync();
        Task<string> stderr = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        Console.WriteLine(await stdout);
        Console.WriteLine($"Exit code: {process.ExitCode}");
    }

    public static async Task RunAsync()
    {
        await BeforeAsync();

        ProcessTextOutput result = await Process.RunAndCaptureTextAsync(
            "git", ["status", "--porcelain"]);
        Console.WriteLine(result.StandardOutput);
        Console.WriteLine($"Exit code: {result.ExitStatus.ExitCode}");

        Product[] products = [new(1, "Keyboard", "Input"), new(2, "Cable", "Misc")];
        Category[] categories = [new("Input", "Things you type on"), new("Audio", "Things you hear")];
        Order[] orders = [new(1, 3)];

        var fullJoined = products.FullJoin(categories, p => p.Category, c => c.Name);
        foreach (var (product, category) in fullJoined)
            Console.WriteLine($"{product?.Name ?? "(none)"}: {category?.Description ?? "(none)"}");

        var joined = products.Join(orders, p => p.Id, o => o.ProductId);
        foreach (var (product, order) in joined)
            Console.WriteLine($"{product.Name}: qty={order.Qty}");
    }
}
