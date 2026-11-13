#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:property PublishAot=false

// A union as a handler's result type (the mediator / CQRS pattern), mapped to HTTP.
// PublishAot=false: file-based apps default to NativeAOT, which needs source-generated JSON.
// dotnet run app.cs, then POST /orders {"productId":"p1","quantity":2}

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<CreateOrderHandler>();
var app = builder.Build();

app.MapPost("/orders", async (CreateOrder command, CreateOrderHandler handler) =>
    await handler.Handle(command) switch
    {
        OrderCreated created    => Results.Created($"/orders/{created.Id}", created),
        ValidationFailed failed => Results.BadRequest(failed),
        ProductNotFound missing => Results.NotFound(missing),
    });

app.Run();

public record CreateOrder(string ProductId, int Quantity);

public record OrderCreated(Guid Id);
public record ValidationFailed(string Field, string Message);
public record ProductNotFound(string ProductId);
public union CreateOrderResult(OrderCreated, ValidationFailed, ProductNotFound);

public class CreateOrderHandler
{
    private readonly HashSet<string> products = ["p1", "p2"];

    public async Task<CreateOrderResult> Handle(CreateOrder command)
    {
        await Task.Yield(); // stands in for a database call
        if (command.Quantity <= 0)
            return new ValidationFailed("Quantity", "Must be positive");
        if (!products.Contains(command.ProductId))
            return new ProductNotFound(command.ProductId);
        return new OrderCreated(Guid.NewGuid());
    }
}
