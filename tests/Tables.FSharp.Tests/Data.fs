module Azure.Data.Tables.FSharp.Tests.Data

open System
open Azure
open Azure.Data.Tables

type TestEntity () =
    interface ITableEntity with
        member val PartitionKey = "" with get, set
        member val RowKey = "" with get, set
        member val Timestamp = Nullable() with get, set
        member val ETag = ETag.All with get, set
    member val GuidVal = Guid.Empty with get, set
    member val StringVal : string = null with get, set
    member val BoolVal = false with get, set
    member val DateVal = DateTimeOffset.MinValue with get, set
    member val DoubleVal = 0.0 with get, set
    member val IntVal = 0 with get, set
    member val Int64Val = 0L with get, set

let getEntity id (i:int) =
    let now = DateTimeOffset.UtcNow
    let e = TestEntity()
    (e :> ITableEntity).PartitionKey <- "tests"
    (e :> ITableEntity).RowKey <- sprintf "row_%i" i
    (e :> ITableEntity).Timestamp <- DateTimeOffset.UtcNow |> Nullable
    (e :> ITableEntity).ETag <- ETag.All

    e.GuidVal <- id
    e.StringVal <- sprintf "Val %i" i
    e.BoolVal <- i%2 = 0
    e.DateVal <- DateTimeOffset(now.Year, now.Month, now.Day, i, 0, 0, TimeSpan.Zero)
    e.DoubleVal <- float i * 1.5
    e.IntVal <- i
    e.Int64Val <- int64 i * 1_000_000_000_000L
    e

let prepareData (client:TableClient) =
    client.CreateIfNotExists() |> ignore
    for e in client.Query<TableEntity>() do
        client.DeleteEntity(e.PartitionKey, e.RowKey) |> ignore<Response>
    [1..9]
    |> List.map (getEntity <| Guid.NewGuid())
    |> List.iter (client.UpsertEntity >> ignore)
    let empty = getEntity Guid.Empty 10
    empty |> client.UpsertEntity |> ignore