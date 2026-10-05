using System.Diagnostics;
using Dotnet11Demo;

Unions.Run();
ClosedHierarchies.Run();
SmallThings.Run();
await Libraries.RunAsync();
await OuterAsync();

// Runtime Async: build with -p:Features=runtime-async=on and compare the frames
static async Task OuterAsync() => await MiddleAsync();
static async Task MiddleAsync() => await InnerAsync();
static async Task InnerAsync()
{

    Console.WriteLine(new StackTrace(fNeedFileInfo: true));
}
