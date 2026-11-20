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

// Same shape, so the structural classifier refuses them. The cases carry their own
// "Type" and ByType picks the union case with that name.
[JsonUnion(TypeClassifier = typeof(ByType))]
public union Event(Created, Deleted);
public sealed record Created(int Id) { public string Type => "Created"; }
public sealed record Deleted(int Id) { public string Type => "Deleted"; }

public sealed class ByType : JsonTypeClassifierFactory
{
    public override bool CanClassify(JsonTypeClassifierContext context) => true;

    // The classifier gets its own copy of the reader: reading ahead doesn't consume the value.
    // Returning null makes the serializer throw JsonException.
    public override JsonTypeClassifier CreateJsonClassifier(JsonTypeClassifierContext context, JsonSerializerOptions options) =>
        (ref Utf8JsonReader reader) =>
            JsonDocument.ParseValue(ref reader).RootElement.TryGetProperty("Type", out var type)
                ? context.UnionCases.FirstOrDefault(c => c.CaseType.Name == type.GetString())?.CaseType
                : null;
}

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

        Console.WriteLine(JsonSerializer.Serialize(new Event(new Created(42))));
        Console.WriteLine(JsonSerializer.Deserialize<Event>("""{"Id":42,"Type":"Deleted"}""").Value);
    }
}
