using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dotnet11Demo;

public record class Cat(string Name);
public record class Dog(string Name);
public record class Bird(string Name);
public union Pet(Cat, Dog, Bird);

public record Success(string Message);
public record Failure(int ErrorCode);
public union Result(Success, Failure);

public union Reading(int, string);

[JsonUnion(TypeClassifier = typeof(JsonUnionTypeStructuralClassifier))]
public union PetUnion(JsonDog, JsonCat);
public sealed record JsonDog(string Name, string Breed);
public sealed record JsonCat(string Name, int Lives);

public static class Unions
{
    public static string NameOf(Pet pet) => pet switch
    {
        Dog d => d.Name,
        Cat c => c.Name,
        Bird b => b.Name,
    };

    public static string Describe(Result result) => result switch
    {
        Success(var message) => message,
        Failure(var errorCode) => $"Error {errorCode}"
    };

    public static void Run()
    {
        Pet pet = new Dog("Rex");
        Console.WriteLine(NameOf(pet));
        Console.WriteLine(Describe(new Failure(404)));

        Console.WriteLine(JsonSerializer.Serialize(new Reading(42)));
        Console.WriteLine(JsonSerializer.Serialize(new Reading("hello")));
        Console.WriteLine(JsonSerializer.Serialize(new PetUnion(new JsonCat("Misty", 9))));

        var back = JsonSerializer.Deserialize<PetUnion>("""{"Name":"Misty","Lives":9}""");
        Console.WriteLine(back.Value);
    }
}
