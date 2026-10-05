#!/usr/bin/env dotnet
#:include helpers.cs
#:include models/customer.cs

Console.WriteLine(Helpers.FormatOutput(new Customer("Ada")));

