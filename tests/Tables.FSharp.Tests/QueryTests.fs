module Azure.Data.Tables.FSharp.Tests.QueryTests

open System
open Azure.Data.Tables
open Azure.Data.Tables.FSharp
open Expecto

let queryFilterTests (client:TableClient) = testList "Filter" [
    test "Filters by Guid" {
        let ent =
            eq "GuidVal" Guid.Empty
            |> client.Query<Data.TestEntity>
            |> Seq.toList

        Expect.equal Guid.Empty (ent.Head.GuidVal) ""
        Expect.equal 1 ent.Length ""
        Expect.equal "Val 10" ent.Head.StringVal ""
    }

    test "Filters by String" {
        let ent =
            eq "StringVal" "Val 10"
            |> client.Query<Data.TestEntity>
            |> Seq.toList

        Expect.equal Guid.Empty (ent.Head.GuidVal) ""
        Expect.equal 1 ent.Length ""
    }

    test "Filters by Bool" {
        let ent =
            eq "BoolVal" true
            |> client.Query<Data.TestEntity>
            |> Seq.toList

        Expect.equal true (ent.Head.BoolVal) ""
        Expect.equal 5 ent.Length ""
    }

    test "Filters by Date" {
        let now = DateTimeOffset.UtcNow
        let dateFrom = DateTimeOffset(now.Year, now.Month, now.Day, 3, 0, 0, TimeSpan.Zero)
        let dateTo = DateTimeOffset(now.Year, now.Month, now.Day, 6, 0, 0, TimeSpan.Zero)
        let ent =
            (gt "DateVal" dateFrom + lt "DateVal" dateTo)
            |> client.Query<Data.TestEntity>
            |> Seq.toList

        Expect.equal (dateFrom.AddHours 1.) (ent.Head.DateVal) ""
        Expect.equal 2 ent.Length ""
    }

    test "Filters by Double" {
        let ent =
            eq "DoubleVal" 7.5
            |> client.Query<Data.TestEntity>
            |> Seq.toList

        Expect.equal "Val 5" (ent.Head.StringVal) ""
        Expect.equal 1 ent.Length ""
    }

    test "Filters by Int" {
        let ent =
            eq "IntVal" 5
            |> client.Query<Data.TestEntity>
            |> Seq.toList

        Expect.equal "Val 5" (ent.Head.StringVal) ""
        Expect.equal 1 ent.Length ""
    }

    test "Filters by Int64" {
        let ent =
            eq "Int64Val" 5L
            |> client.Query<Data.TestEntity>
            |> Seq.toList

        Expect.equal "Val 5" (ent.Head.StringVal) ""
        Expect.equal 1 ent.Length ""
    }

    test "Unary filter works" {
        let ent =
            !! (eq "Int64Val" 5L)
            |> client.Query<Data.TestEntity>
            |> Seq.toList

        Expect.equal 9 ent.Length ""
        Expect.equal false (ent |> List.exists (fun x -> x.Int64Val = 5L)) ""
    }
]

let queryComputationExpressionsTests (client:TableClient) = testList "CE" [
    test "Filter works" {
        let ent =
            tableQuery {
                filter (eq "GuidVal" Guid.Empty)
            }
            |> client.Query<Data.TestEntity>
            |> Seq.toList

        Expect.equal Guid.Empty (ent.Head.GuidVal) ""
        Expect.equal 1 ent.Length ""
        Expect.equal "Val 10" ent.Head.StringVal ""
    }

    test "MaxPerPage works" {
        let pages =
            tableQuery {
                maxPerPage 5
            }
            |> client.Query<Data.TestEntity>
            |> (fun x -> x.AsPages())
            |> Seq.toList

        Expect.equal pages.Head.Values.Count 5 ""
        Expect.equal pages.Length 2 ""
    }

    test "Select works" {
        let ent =
            tableQuery {
                select [ "DoubleVal" ]
                filter (rk "row_10")
            }
            |> client.Query<Data.TestEntity>
            |> Seq.toList

        Expect.equal ent.Head.DoubleVal 15. ""
        Expect.isNull ent.Head.StringVal ""
    }
]

let tests client = testList "Query" [
    queryFilterTests client
    queryComputationExpressionsTests client
]