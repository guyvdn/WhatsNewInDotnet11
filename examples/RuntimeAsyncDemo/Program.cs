using System.Diagnostics;

await OuterAsync();

static async Task OuterAsync() => await MiddleAsync();
static async Task MiddleAsync() => await InnerAsync();
static async Task InnerAsync()
{
    await Task.CompletedTask;
    Console.WriteLine(new StackTrace(fNeedFileInfo: true));
}
