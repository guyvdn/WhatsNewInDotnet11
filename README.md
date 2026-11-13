# What's new in .NET 11

A 25-minute tour of .NET 11 and C# 15, told as "less code to write, less code to trust": union types and closed hierarchies,
Runtime Async, file-based apps that scale, the library APIs you'll use
tomorrow, and what changes in ASP.NET Core and EF Core 11.

Status: GA (11.0.0, 10 Nov 2026). .NET 11 is an **STS** release, supported until
14 Nov 2028 — the same day as .NET 10 (LTS).

## Contents

| Path | What it is |
|------|------------|
| `deck/` | The [reveal.js](https://revealjs.com) deck: chapter rail, slash-to-jump palette, three live panels, speaker notes |
| `examples/Dotnet11Demo` | Every C# 15 and library snippet from the deck, compiled and run on GA (SDK 11.0.100) |
| `examples/RuntimeAsyncDemo` | The stack-trace comparison behind the Runtime Async slide |
| `examples/FileBasedApp` | `dotnet run app.cs` with `#:include` |

## Running the deck

```powershell
./start-deck.ps1                   # installs deps on first run, serves on http://localhost:8083
```

Keys: <kbd>?</kbd> help, <kbd>/</kbd> jump to a slide, <kbd>B</kbd> run the demo on
an interactive slide, <kbd>R</kbd> reset it, <kbd>D</kbd> draw, <kbd>S</kbd> speaker
notes.

| Interactive slide | What it does |
|---|---|
| The compiler has your back | A `switch` over a union misses a case: CS8509 (text from a real GA build). The light bulb adds the arm and the build goes green. |
| Unions on the wire | Serializes `Reading(42)`, `Reading("hello")` and a `PetUnion` with System.Text.Json: the active case only, no wrapper, no `$type`. |
| Runtime Async | The same code with `runtime-async` off and on: 13 frames vs 5, measured on GA. |

## Running the examples

Needs the .NET 11 SDK. `examples/global.json` pins 11.0.100 and rolls forward to
later feature bands.

```powershell
cd examples
dotnet run --project Dotnet11Demo

# Runtime Async: compare the two stack traces
dotnet build RuntimeAsyncDemo --no-incremental; dotnet RuntimeAsyncDemo/bin/Debug/net11.0/RuntimeAsyncDemo.dll
dotnet build RuntimeAsyncDemo --no-incremental -p:Features=runtime-async=on; dotnet RuntimeAsyncDemo/bin/Debug/net11.0/RuntimeAsyncDemo.dll

cd FileBasedApp
dotnet run app.cs -e GREETING=hi
```

`--no-incremental` matters: changing `Features` alone does not trigger a recompile.

## Sources

- [What's new in .NET 11](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-11/overview)
- [What's new in C# 15](https://learn.microsoft.com/dotnet/csharp/whats-new/csharp-15)
- [What's new in ASP.NET Core 11](https://learn.microsoft.com/aspnet/core/release-notes/aspnetcore-11)
- [What's new in EF Core 11](https://learn.microsoft.com/ef/core/what-is-new/ef-core-11.0/whatsnew)
- [Breaking changes in .NET 11](https://learn.microsoft.com/dotnet/core/compatibility/11)
- [.NET 11 release notes](https://github.com/dotnet/core/tree/main/release-notes/11.0)
