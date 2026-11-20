#!/usr/bin/env dotnet

// The union keyword stores its value as object?, so value-type cases box.
// A hand-written [Union] with HasValue / TryGetValue matches them without boxing.
// dotnet run -c Release app.cs

using System.Runtime.CompilerServices;

static string NameOf(Pet pet) => pet switch
{
    Dog d => d.Name,
    Cat c => c.Name,
    Bird b => b.Name,
};

static string NameOfBoxed(BoxedPet pet) => pet switch
{
    Dog d => d.Name,
    Cat c => c.Name,
    Bird b => b.Name,
};

const int N = 1_000_000;
var dog = new Dog("Rex");
NameOf(dog); NameOfBoxed(dog);   // warm up

long before = GC.GetAllocatedBytesForCurrentThread();
for (int i = 0; i < N; i++) NameOfBoxed(dog);
Console.WriteLine($"union keyword:  {(GC.GetAllocatedBytesForCurrentThread() - before) / (double)N:F1} bytes per match");

before = GC.GetAllocatedBytesForCurrentThread();
for (int i = 0; i < N; i++) NameOf(dog);
Console.WriteLine($"custom [Union]: {(GC.GetAllocatedBytesForCurrentThread() - before) / (double)N:F1} bytes per match");

public readonly record struct Cat(string Name);
public readonly record struct Dog(string Name);
public readonly record struct Bird(string Name);

public union BoxedPet(Cat, Dog, Bird);

// _case 0 is default(Pet): no pet. Value boxes, but only when someone asks for it.
[Union] public readonly struct Pet : IUnion
{
    readonly byte _case; readonly Cat _cat; readonly Dog _dog; readonly Bird _bird;

    public Pet(Cat value) => (_case, _cat) = (1, value);
    public Pet(Dog value) => (_case, _dog) = (2, value);
    public Pet(Bird value) => (_case, _bird) = (3, value);

    public bool HasValue => _case != 0;
    public bool TryGetValue(out Cat value) { value = _cat; return _case == 1; }
    public bool TryGetValue(out Dog value) { value = _dog; return _case == 2; }
    public bool TryGetValue(out Bird value) { value = _bird; return _case == 3; }
    public object? Value => _case switch { 1 => _cat, 2 => _dog, 3 => _bird, _ => null };
}
