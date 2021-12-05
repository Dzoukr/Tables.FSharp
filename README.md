# Tables.FSharp [![NuGet](https://img.shields.io/nuget/v/Tables.FSharp.svg?style=flat-square)](https://www.nuget.org/packages/Tables.FSharp/)

<p align="center">
<img src="https://github.com/Dzoukr/Azure.Data.Tables.FSharp/raw/master/logo.png" width="150px"/>
</p>

Lightweight F# extension for the latest [Azure.Data.Tables SDK](https://docs.microsoft.com/en-us/dotnet/api/overview/azure/Data.Tables-readme-pre)

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

Using this library, new overloads of `Query` and `QueryAsync` are available to accept `Filter` value instead of query string. This `Filter` is transformed into valid and supported OData format, including date conversions and correct data formatting:

```f#
open Azure.Data.Tables
open Azure.Data.Tables.FSharp

let tableClient = TableClient("connString", "MyTable")

let results =
    (eq "StringValue" "Roman" * !! (ge "DateValue" DateTimeOffset.Now + lt "IntValue" 42))
    |> tableClient.Query<MyEntity>

```

To inspect transformed output of `Filter` value you can use `FilterConverter` module:

```f#
open Azure.Data.Tables.FSharp

let oDataString =
    (eq "StringValue" "Roman" * !! (ge "DateValue" DateTimeOffset.Now + lt "IntValue" 42))
    |> FilterConverter.toQuery
```

If you need to comfortably pass more arguments into `Query` / `QueryAsync` like columns selection or paging, the computation expression `tableQuery` is here for you:

```f#
tableQuery {
    filter (eq "GuidVal" Guid.Empty)
    select [ "ColumnOne"; "ColumnTwo" ]
    maxPerPage 5 // take only 5 rows per page
} |> tableClient.Query<MyEntity>
```

## CRUD Operations

`Azure.Data.Tables` is currently in beta3 and first API looks quite promising - maybe no wrapper around that won't be necessary at all. See [the examples](https://github.com/Azure/azure-sdk-for-net/tree/Azure.Data.Tables_3.0.0-beta.3/sdk/tables/Azure.Data.Tables#create-the-table-client). I'll keep monitoring progress and will add more wrappers if needed.