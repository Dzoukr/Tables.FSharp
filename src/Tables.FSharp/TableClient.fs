[<AutoOpen>]
module Azure.Data.Tables.FSharp.TableClient

open System.Threading
open Azure.Data.Tables

[<RequireQualifiedAccess>]
module private Option =
    let ofSeq seq = if seq |> Seq.isEmpty then None else Some seq

type TableClient with
    member x.QueryAsync<'a when 'a : (new : unit -> 'a) and 'a :> ITableEntity and 'a : not struct>
        (f:Filter, ?maxPerPage:int, ?select:seq<string>, ?cancellationToken:CancellationToken) =
            x.QueryAsync<'a>(FilterConverter.toQuery f,
                Option.toNullable(maxPerPage),
                Option.toObj(select),
                cancellationToken |> Option.defaultValue CancellationToken.None)

    member x.QueryAsync<'a when 'a : (new : unit -> 'a) and 'a :> ITableEntity and 'a : not struct>
        (q:TableQuery) =
            x.QueryAsync<'a>(FilterConverter.toQuery q.Filter,
                Option.toNullable(q.MaxPerPage),
                (q.Select |> Seq.ofList |> Option.ofSeq |> Option.toObj),
                q.CancellationToken)


    member x.Query<'a when 'a : (new : unit -> 'a) and 'a :> ITableEntity and 'a : not struct>
        (f:Filter, ?maxPerPage:int, ?select:seq<string>, ?cancellationToken:CancellationToken) =
            x.Query<'a>(FilterConverter.toQuery f,
                Option.toNullable(maxPerPage),
                Option.toObj(select),
                cancellationToken |> Option.defaultValue CancellationToken.None)

    member x.Query<'a when 'a : (new : unit -> 'a) and 'a :> ITableEntity and 'a : not struct>
        (q:TableQuery) =
            x.Query<'a>(FilterConverter.toQuery q.Filter,
                Option.toNullable(q.MaxPerPage),
                (q.Select |> Seq.ofList |> Option.ofSeq |> Option.toObj),
                q.CancellationToken)