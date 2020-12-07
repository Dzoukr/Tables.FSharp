# Tables.FSharp [![NuGet](https://img.shields.io/nuget/v/Tables.FSharp.svg?style=flat-square)](https://www.nuget.org/packages/Tables.FSharp/)

<p align="center">
<img src="https://github.com/Dzoukr/Azure.Data.Tables.FSharp/raw/master/logo.png" width="150px"/>
</p>

Lightweight F# extension for the latest [Azure.Data.Tables SDK](https://docs.microsoft.com/en-us/dotnet/api/overview/azure/Data.Tables-readme-pre) (currently in beta3)

## Installation
If you want to install this package manually, use usual NuGet package command

    Install-Package Tables.FSharp

or using [Paket](http://fsprojects.github.io/Paket/getting-started.html)

    paket add Tables.FSharp

 ## Querying

New SDK API provides two methods on `TableClient` for querying - `Query` and `QueryAsync`. Both methods expects filter query in [OData](https://www.odata.org) standard which can be tedious and error-prone work. Consider this:

```f#
open Azure.Data.Tables

let tableClient = TableClient("connString", "MyTable")
let manualQuery = sprintf "(StringValue eq '%s') or not ((DateValue ge datetime'%s') and IntValue lt %i)" roman (DateTimeOffset.UtcNow.ToString("o")) 42
let results = table.Query<MyEntity>(manualQuery)
```

Using this library, new overloads of `Query` and `QueryAsync` are available to accept `Filter` value instead of query string. This `Filter` is transformed into valid and supported OData format:

```f#
open Azure.Data.Tables
open Azure.Data.Tables.FSharp

let tableClient = TableClient("connString", "MyTable")
let results =
    (eq "StringValue" "Roman" * !! (ge "DateValue" now + lt "IntValue" 42))
    |> tableClient.Query<MyEntity>

```

To inspect transformed output of `Filter` value you can use `FilterConverter` module:

```f#
open Azure.Data.Tables.FSharp

let oDataString =
    (eq "StringValue" "Roman" * !! (ge "DateValue" now + lt "IntValue" 42))
    |> FilterConverter.toQuery
```

## CRUD Operations

`Azure.Data.Tables` is currently in beta3 and first API looks promising - maybe now wrapper around that won't be necessary. See [the examples](https://github.com/Azure/azure-sdk-for-net/tree/Azure.Data.Tables_3.0.0-beta.3/sdk/tables/Azure.Data.Tables#create-the-table-client).